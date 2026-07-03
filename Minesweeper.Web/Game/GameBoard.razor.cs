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
        // Gesture category (tap vs flick vs pan) is decided purely by displacement from the
        // touch origin (plus what kind of cell it started on); duration is never part of it.
        private const double MoveThresholdPx = 15.0;

        private enum Gesture { None, Flick, Pan }

        private (int Row, int Col)? _activeCell;
        private double _pointerDownX, _pointerDownY;
        private long? _primaryPointerId;
        private Gesture _gesture;
        private bool _originRevealed;
        private (int Row, int Col)? _ghostFlagCell;

        private readonly HashSet<(int Row, int Col)> _shakingCells = new();

        // "Grab the cleared ground to pan" navigation. A drag that STARTS on a revealed cell
        // (a number or a cleared blank) slides the whole board 1:1, like grabbing solid terrain
        // and sliding the map under you. A drag that starts on fog (unrevealed) is a flick-to-flag,
        // and a stationary press is a tap (dig fog / chord a number). No modes, no second finger,
        // no auto-follow - the meaning is decided by the cell under the finger at touch-down.
        private const int CellSizePx = 36;   // fixed - identical for every difficulty (only board extent grows)
        private const double GridGapPx = 2;  // must match .board-grid gap
        private const double WellPadPx = 10; // must match .board-well padding

        private ElementReference _slotRef;
        private double _cameraX, _cameraY;
        private double _panStartCameraX, _panStartCameraY;
        private double _viewportW, _viewportH;

        private int CellPx => CellSizePx;
        private int IconPx => Math.Max(12, (int)Math.Round(CellPx * 0.62));

        // Inline (not scoped CSS) because the offset is per-instance render state, not a style
        // rule - and there is deliberately no CSS transition on it, so the board tracks the
        // finger 1:1 with no lag while panning.
        private string BoardWellStyle => $"transform: translate({_cameraX.ToString(CultureInfo.InvariantCulture)}px, {_cameraY.ToString(CultureInfo.InvariantCulture)}px);";

        protected override void OnInitialized()
        {
            State.Changed += HandleStateChanged;
            State.GameStarted += HandleGameStarted;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // Viewport (the board-slot) size only changes on resize/rotate, not per difficulty,
            // so measuring once after the first render is enough to clamp panning; a pan also
            // refreshes it defensively. Board extent is derived from settings, no measure needed.
            if (firstRender) await MeasureViewportAsync();
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
            // and is handled solely by OnContextMenu (flag) - letting it start the primary path
            // here meant the right-button pointerup fired RevealCell, racing the contextmenu flag.
            // Touch and pen contacts report Button == 0, so this never affects them.
            if (e.Button != 0) return;

            long pointerId = (long)e.PointerId;

            // One gesture at a time; any extra contact mid-gesture is a stray/palm-brush - ignore.
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
            _gesture = Gesture.None;
            _originRevealed = cell.IsRevealed;
            _ghostFlagCell = null;
            _panStartCameraX = _cameraX;
            _panStartCameraY = _cameraY;

            // Capture the pointer to this origin cell so every gesture keeps reporting here even
            // as the finger crosses cell boundaries - a flick that drifts into the next cell still
            // flags the origin, and a pan keeps tracking as the board slides under the finger.
            _ = CapturePointerAsync(e.ClientX, e.ClientY, pointerId);

            // Fog presses show dig/flag feedback; a cleared cell only shows chord feedback on a
            // number (handled in CellCssClass), so no press-active toggle needed there.
            if (!_originRevealed) State.SetPressActive(true);
        }

        private void OnPointerMove(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;
            if (_primaryPointerId != pointerId || _activeCell != (row, col)) return;

            double dx = e.ClientX - _pointerDownX;
            double dy = e.ClientY - _pointerDownY;

            if (_gesture == Gesture.None)
            {
                // Displacement from the touch-down point only - path length/velocity/duration
                // never factor in, matching the "distance, not time" principle.
                if (Math.Sqrt(dx * dx + dy * dy) <= MoveThresholdPx) return;

                if (_originRevealed)
                {
                    _gesture = Gesture.Pan;
                    _ = MeasureViewportAsync(); // refresh clamp bounds (handles rotate) for this drag
                }
                else
                {
                    _gesture = Gesture.Flick;
                    _ghostFlagCell = (row, col);
                    StateHasChanged();
                    return;
                }
            }

            if (_gesture == Gesture.Pan)
            {
                (double maxX, double maxY) = PanBounds();
                _cameraX = Math.Clamp(_panStartCameraX + dx, -maxX, maxX);
                _cameraY = Math.Clamp(_panStartCameraY + dy, -maxY, maxY);
                StateHasChanged();
            }
        }

        private void OnPointerUp(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;
            if (_primaryPointerId != pointerId || _activeCell != (row, col)) return;

            var gesture = _gesture;
            ClearGesture();

            // A pan resolves nothing; the view has already moved. Action always resolves on the
            // touch-origin cell, never wherever the pointer happened to lift.
            if (gesture == Gesture.Pan) return;

            if (gesture == Gesture.Flick)
            {
                State.FlagCell(State.GetCell(row, col));
                return;
            }

            // A stationary press = tap. On fog it digs; on a revealed number it chords (an
            // otherwise dead gesture); on a cleared blank it does nothing.
            var cell = State.GetCell(row, col);
            if (cell.IsRevealed)
            {
                if (cell.AdjacentMines > 0) TryChord(row, col, cell);
                return;
            }

            State.RevealCell(row, col);
        }

        // Note: no pointerleave handler. The pointer is captured to the origin cell on pointerdown,
        // so pointerup/pointercancel always resolve there - and setPointerCapture itself fires a
        // spurious pointerleave that would otherwise abort a legitimate pending tap.
        private void OnPointerCancel(PointerEventArgs e, int row, int col)
        {
            if (_primaryPointerId == (long)e.PointerId) ClearGesture();
        }

        private void TryChord(int row, int col, Cell cell)
        {
            if (!cell.IsSecured)
            {
                _ = TriggerShakeAsync(row, col);
                return;
            }
            State.RevealCell(row, col);
        }

        // Clamp so a pan can't pull the board frame past the viewport edge. When the board fits a
        // given axis the range collapses to 0 (that axis simply can't be panned).
        private (double MaxX, double MaxY) PanBounds()
        {
            int cols = State.CurrentSettings.Cols;
            int rows = State.CurrentSettings.Rows;
            double wellW = cols * CellPx + (cols - 1) * GridGapPx + WellPadPx * 2;
            double wellH = rows * CellPx + (rows - 1) * GridGapPx + WellPadPx * 2;
            double vw = _viewportW > 0 ? _viewportW : wellW;
            double vh = _viewportH > 0 ? _viewportH : wellH;
            return (Math.Max(0, (wellW - vw) / 2.0), Math.Max(0, (wellH - vh) / 2.0));
        }

        private async Task MeasureViewportAsync()
        {
            try
            {
                var size = await JS.InvokeAsync<double[]>("msBoard.measure", _slotRef);
                if (size is { Length: >= 2 }) { _viewportW = size[0]; _viewportH = size[1]; }
            }
            catch { /* a missed measure just means no clamp this frame - harmless */ }
        }

        private async Task CapturePointerAsync(double x, double y, long pointerId)
        {
            try { await JS.InvokeVoidAsync("msBoard.capture", x, y, pointerId); }
            catch { /* capture is best-effort */ }
        }

        private async Task TriggerShakeAsync(int row, int col)
        {
            _shakingCells.Add((row, col));
            StateHasChanged();
            await Task.Delay(180);
            _shakingCells.Remove((row, col));
            StateHasChanged();
        }

        private void ClearGesture()
        {
            _activeCell = null;
            _primaryPointerId = null;
            _ghostFlagCell = null;
            _gesture = Gesture.None;
            _originRevealed = false;
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

            // Press feedback only for an in-progress tap (no drag committed yet): unrevealed cells
            // press for dig/flag, a revealed number presses for its chord. Once a flick or pan
            // commits, drop the pressed look. A revealed blank has no tap action, so never presses.
            bool isPressable = !isRevealed || visual == CellVisual.Number;
            if (_activeCell == (row, col) && isPressable && _gesture == Gesture.None) classes.Add("active");

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
