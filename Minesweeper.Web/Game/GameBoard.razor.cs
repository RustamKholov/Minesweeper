using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Minesweeper.Domain.Entities;
using Minesweeper.Web.Services;

namespace Minesweeper.Web.Game
{
    public partial class GameBoard : ComponentBase, IDisposable
    {
        [Inject] private GameStateService State { get; set; } = default!;
        [Inject] private IJSRuntime JS { get; set; } = default!;

        // Shape/distance-based gesture recognition only - no timers anywhere in this file.
        // Gesture category (tap vs flick) is decided purely by displacement from the touch
        // origin; duration is never part of that decision.
        private const double FlickThresholdPx = 15.0;

        private (int Row, int Col)? _activeCell;
        private double _pointerDownX, _pointerDownY;
        private long? _primaryPointerId;
        private bool _flickCommitted;
        private (int Row, int Col)? _ghostFlagCell;

        private readonly HashSet<(int Row, int Col)> _shakingCells = new();

        // Camera-follow navigation. There is no manual panning: the board never responds to
        // drags with a pan (a single-finger drag is already "flick to flag"). Instead the camera
        // glides to keep the cell the player just acted on centred in the viewport, so bigger
        // boards are navigated simply by playing across them. The offset is a translate applied
        // to the board-well; the CSS transition on .board-well makes the move a glide, not a jump.
        private const int CellSizePx = 36;   // fixed - identical for every difficulty (only board extent grows)
        private const double GridGapPx = 2;  // must match .board-grid gap
        private const double WellPadPx = 10; // must match .board-well padding

        private ElementReference _slotRef;
        private double _cameraX, _cameraY;

        private int CellPx => CellSizePx;
        private int IconPx => Math.Max(12, (int)Math.Round(CellPx * 0.62));

        // Inline (not scoped CSS) because the offset is per-instance render state, not a style rule.
        private string BoardWellStyle => $"transform: translate({_cameraX.ToString(CultureInfo.InvariantCulture)}px, {_cameraY.ToString(CultureInfo.InvariantCulture)}px);";

        protected override void OnInitialized()
        {
            State.Changed += HandleStateChanged;
            State.GameStarted += HandleGameStarted;
        }

        private void HandleStateChanged() => InvokeAsync(StateHasChanged);

        private void HandleGameStarted()
        {
            _cameraX = 0;
            _cameraY = 0;
            InvokeAsync(StateHasChanged);
        }

        private void OnPointerDown(PointerEventArgs e, int row, int col)
        {
            if (State.IsGameOver) return;

            // Only the primary button starts a gesture. A mouse right-click reports Button == 2
            // and is handled solely by OnContextMenu (flag) - letting it start the primary tap
            // path here meant the right-button pointerup fired RevealCell, racing the contextmenu
            // flag. Touch and pen contacts report Button == 0, so this never affects them.
            if (e.Button != 0) return;

            long pointerId = (long)e.PointerId;

            // One gesture at a time. There is no manual pan any more, so any extra contact while
            // a gesture is in flight is a stray/palm-brush and is simply ignored.
            if (_primaryPointerId is not null) return;

            var cell = State.GetCell(row, col);
            if (cell.IsFlagged)
            {
                _ = TriggerShakeAsync(row, col);
                return;
            }

            _activeCell = (row, col);
            _primaryPointerId = pointerId;
            _pointerDownX = e.ClientX;
            _pointerDownY = e.ClientY;
            _flickCommitted = false;
            State.SetPressActive(true);
        }

        private void OnPointerMove(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;

            if (_primaryPointerId != pointerId || _activeCell != (row, col)) return;

            // Displacement from the touch-down point only - path length/velocity/duration
            // never factor into this, matching the spec's "distance, not time" principle.
            double dx = e.ClientX - _pointerDownX;
            double dy = e.ClientY - _pointerDownY;
            if (!_flickCommitted && Math.Sqrt(dx * dx + dy * dy) > FlickThresholdPx)
            {
                _flickCommitted = true;
                _ghostFlagCell = (row, col);
                StateHasChanged();
            }
        }

        private void OnPointerUp(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;

            if (_primaryPointerId != pointerId || _activeCell != (row, col))
            {
                return; // a stray/non-primary pointer lifting - not the active gesture, ignore
            }

            bool wasFlick = _flickCommitted;
            ClearPressState();

            // Action always resolves on the touch-origin cell (row/col captured at pointerdown
            // and threaded through every handler), never wherever the pointer happened to lift.
            if (wasFlick)
            {
                State.FlagCell(State.GetCell(row, col));
                return;
            }

            var cell = State.GetCell(row, col);

            // A tap/click on an already-revealed number can't mean "reveal" - it's otherwise a
            // dead gesture, so it doubles as the chord trigger instead. Identical for mouse and
            // touch; no double-click/double-tap window needed.
            if (cell.IsRevealed)
            {
                if (cell.AdjacentMines > 0)
                {
                    TryChord(row, col, cell);
                }
                return;
            }

            State.RevealCell(row, col);
            _ = CenterCameraOnAsync(row, col);
        }

        private void OnPointerLeave(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;

            if (_primaryPointerId == pointerId && _activeCell == (row, col))
            {
                ClearPressState();
            }
        }

        private void TryChord(int row, int col, Cell cell)
        {
            if (!cell.IsSecured)
            {
                _ = TriggerShakeAsync(row, col);
                return;
            }
            State.RevealCell(row, col);
            _ = CenterCameraOnAsync(row, col);
        }

        // Relaxed, edge-triggered camera. Rather than re-centring on every tap (which fought the
        // player's own tracking of where they'd just aimed), the camera holds still while the
        // acted-on cell stays within a generous central dead-zone, and only nudges - by the
        // minimum amount - when that cell drifts close to a viewport edge. Boards that fit
        // entirely never move (clamp range collapses to 0). Measures the live viewport each call
        // so rotation/resize are handled for free; any interop hiccup is swallowed harmlessly.
        private async Task CenterCameraOnAsync(int row, int col)
        {
            double[]? size;
            try
            {
                size = await JS.InvokeAsync<double[]>("msBoard.measure", _slotRef);
            }
            catch
            {
                return;
            }
            if (size is null || size.Length < 2) return;

            double viewportW = size[0];
            double viewportH = size[1];

            int cols = State.CurrentSettings.Cols;
            int rows = State.CurrentSettings.Rows;
            double gridW = cols * CellPx + (cols - 1) * GridGapPx;
            double gridH = rows * CellPx + (rows - 1) * GridGapPx;
            double wellW = gridW + WellPadPx * 2;
            double wellH = gridH + WellPadPx * 2;

            // Camera value that would put this cell dead-centre. Padding is symmetric so it cancels.
            double cellCenterX = col * (CellPx + GridGapPx) + CellPx / 2.0;
            double cellCenterY = row * (CellPx + GridGapPx) + CellPx / 2.0;
            double centerX = gridW / 2.0 - cellCenterX;
            double centerY = gridH / 2.0 - cellCenterY;

            // Where the cell currently sits relative to the viewport centre, given the live camera.
            double screenOffX = _cameraX - centerX;
            double screenOffY = _cameraY - centerY;

            // Dead-zone half-extent: the cell may roam freely until it comes within `margin` of an
            // edge. Small margin => large dead-zone => the camera stays put most of the time.
            double marginX = Math.Min(viewportW * 0.18, CellPx * 1.5);
            double marginY = Math.Min(viewportH * 0.18, CellPx * 1.5);
            double deadHalfX = Math.Max(0, viewportW / 2.0 - marginX);
            double deadHalfY = Math.Max(0, viewportH / 2.0 - marginY);

            double newX = _cameraX, newY = _cameraY;
            if (screenOffX > deadHalfX) newX = centerX + deadHalfX;
            else if (screenOffX < -deadHalfX) newX = centerX - deadHalfX;
            if (screenOffY > deadHalfY) newY = centerY + deadHalfY;
            else if (screenOffY < -deadHalfY) newY = centerY - deadHalfY;

            double maxX = Math.Max(0, (wellW - viewportW) / 2.0);
            double maxY = Math.Max(0, (wellH - viewportH) / 2.0);
            newX = Math.Clamp(newX, -maxX, maxX);
            newY = Math.Clamp(newY, -maxY, maxY);

            // No meaningful change - leave the view (and the render) alone.
            if (Math.Abs(newX - _cameraX) < 0.5 && Math.Abs(newY - _cameraY) < 0.5) return;

            _cameraX = newX;
            _cameraY = newY;
            StateHasChanged();
        }

        private async Task TriggerShakeAsync(int row, int col)
        {
            _shakingCells.Add((row, col));
            StateHasChanged();
            await Task.Delay(300);
            _shakingCells.Remove((row, col));
            StateHasChanged();
        }

        private void ClearPressState()
        {
            _activeCell = null;
            _primaryPointerId = null;
            _ghostFlagCell = null;
            _flickCommitted = false;
            State.SetPressActive(false);
        }

        private void OnContextMenu(MouseEventArgs e, int row, int col)
        {
            if (State.IsGameOver) return;
            var cell = State.GetCell(row, col);
            State.FlagCell(cell);
        }

        public enum CellVisual { Plain, Number, WrongFlag, MineExploded, MinePlain, Flag, GhostFlag }

        // GameEngine never mutates cell state to "reveal everything" on game-over - WinForms'
        // MainWindow.OpenField() computes the same end-of-game presentation itself, purely at
        // render time, without touching Cell. Mirrored here (same branch order/semantics)
        // rather than changing shared Core behavior that WinForms also depends on. Cells the
        // player never touched stay looking hidden even after a loss, unless they're a mine.
        private CellVisual GetVisual(Cell cell, int row, int col)
        {
            if (State.IsGameOver)
            {
                if (cell.IsFlagged && !cell.IsMine) return CellVisual.WrongFlag;
                if (cell.IsMine)
                {
                    if (cell.IsRevealed) return CellVisual.MineExploded;
                    return State.IsGameWon ? CellVisual.Flag : CellVisual.MinePlain;
                }
            }

            if (cell.IsRevealed)
            {
                return cell.AdjacentMines > 0 ? CellVisual.Number : CellVisual.Plain;
            }

            if (cell.IsFlagged) return CellVisual.Flag;
            if (_ghostFlagCell == (row, col)) return CellVisual.GhostFlag;
            return CellVisual.Plain;
        }

        private bool IsPresentedAsRevealed(Cell cell, CellVisual visual) =>
            cell.IsRevealed || visual is CellVisual.MineExploded or CellVisual.MinePlain;

        private string CellCssClass(Cell cell, int row, int col, CellVisual visual)
        {
            var classes = new List<string> { "cell" };
            bool isRevealed = IsPresentedAsRevealed(cell, visual);
            classes.Add(isRevealed ? "revealed" : "unrevealed");
            if (visual == CellVisual.MineExploded) classes.Add("mine-exploded");

            // Unrevealed cells press for dig/flag; revealed numbers now press for the chord a
            // tap triggers on them. A revealed blank has no tap action, so no press feedback -
            // showing "pressable" there would be misleading.
            bool isPressable = !isRevealed || visual == CellVisual.Number;
            if (_activeCell == (row, col) && isPressable) classes.Add("active");

            if (_shakingCells.Contains((row, col))) classes.Add("shake");
            if (visual == CellVisual.Number) classes.Add($"num-{cell.AdjacentMines}");
            return string.Join(' ', classes);
        }

        public void Dispose()
        {
            State.Changed -= HandleStateChanged;
            State.GameStarted -= HandleGameStarted;
        }
    }
}
