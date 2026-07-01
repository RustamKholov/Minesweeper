using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Minesweeper.Domain.Entities;
using Minesweeper.Web.Services;

namespace Minesweeper.Web.Game
{
    public partial class GameBoard : ComponentBase, IDisposable
    {
        [Inject] private GameStateService State { get; set; } = default!;

        // Shape/distance-based gesture recognition only - no timers anywhere in this file.
        // Gesture category (tap vs flick) is decided purely by displacement from the touch
        // origin; duration is never part of that decision.
        private const double FlickThresholdPx = 15.0;

        private (int Row, int Col)? _activeCell;
        private double _pointerDownX, _pointerDownY;
        private long? _primaryPointerId;
        private bool _flickCommitted;
        private (int Row, int Col)? _ghostFlagCell;

        // Two-finger pan. Gesture category is decided by finger count "at gesture start" - but
        // with no timers, "start" can't mean a time window. Instead it's decided by distance:
        // if a second pointer arrives before the first has moved past the flick threshold, the
        // two are treated as arriving together (promoted to pan). If the first has already
        // committed to a flick, a second pointer arriving after that is a stray/palm-brush and
        // is ignored, letting the flick resolve normally - same distance-based logic the single-
        // finger path already uses, just applied to the "did anything else move yet" question.
        private const double PanCoherenceMinDisplacementPx = 15.0;
        private const double PanCoherenceMinCosine = 0.3;

        private long? _panPointerIdA, _panPointerIdB;
        private double _panStartAX, _panStartAY, _panStartBX, _panStartBY;
        private double _panPrevAX, _panPrevAY, _panPrevBX, _panPrevBY;
        private bool _isPanning;
        private double _panOffsetX, _panOffsetY;

        private readonly HashSet<(int Row, int Col)> _shakingCells = new();

        // Board dimensions are the fixed classic presets (see GameDifficulty) - only the pixel
        // size of each cell adapts here, purely a rendering concern, not a solver/logic one.
        private int CellPx => State.CurrentSettings.Cols switch
        {
            <= 9 => 36,
            <= 16 => 28,
            _ => 18,
        };

        private int IconPx => Math.Max(12, (int)Math.Round(CellPx * 0.62));

        private double BoardWidthPx => State.CurrentSettings.Cols * CellPx + (State.CurrentSettings.Cols - 1) * 2;
        private double BoardHeightPx => State.CurrentSettings.Rows * CellPx + (State.CurrentSettings.Rows - 1) * 2;

        // Inline (not scoped CSS) because the offset is per-instance render state, not a style
        // rule - and deliberately no transition, so the board tracks the fingers 1:1 with no lag.
        private string BoardWellStyle => $"transform: translate({_panOffsetX.ToString(System.Globalization.CultureInfo.InvariantCulture)}px, {_panOffsetY.ToString(System.Globalization.CultureInfo.InvariantCulture)}px);";

        protected override void OnInitialized()
        {
            State.Changed += HandleStateChanged;
            State.GameStarted += HandleGameStarted;
        }

        private void HandleStateChanged() => InvokeAsync(StateHasChanged);

        private void HandleGameStarted()
        {
            _panOffsetX = 0;
            _panOffsetY = 0;
            InvokeAsync(StateHasChanged);
        }

        private void OnPointerDown(PointerEventArgs e, int row, int col)
        {
            if (State.IsGameOver) return;
            long pointerId = (long)e.PointerId;

            if (_panPointerIdA is not null && _panPointerIdB is not null) return; // a 3rd+ contact - ignore

            if (_panPointerIdA is null && _primaryPointerId is null)
            {
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
                return;
            }

            if (_primaryPointerId is not null && !_flickCommitted && _panPointerIdA is null)
            {
                // Second contact arrived before the first moved anywhere - treat as two fingers
                // arriving together and promote to pan tracking. The first finger's pending tap
                // is abandoned; it will not reveal/flag when it eventually lifts.
                _panPointerIdA = _primaryPointerId;
                _panStartAX = _panPrevAX = _pointerDownX;
                _panStartAY = _panPrevAY = _pointerDownY;

                _panPointerIdB = pointerId;
                _panStartBX = _panPrevBX = e.ClientX;
                _panStartBY = _panPrevBY = e.ClientY;

                ClearPressState();
                return;
            }

            // Second contact arrived after the first already committed to a flick (or some
            // other odd ordering) - a stray/palm-brush. Ignore it; let the active gesture resolve.
        }

        private void OnPointerMove(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;

            if (pointerId == _panPointerIdA || pointerId == _panPointerIdB)
            {
                UpdatePan(pointerId, e.ClientX, e.ClientY);
                return;
            }

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

        private void UpdatePan(long pointerId, double x, double y)
        {
            double prevX, prevY;
            if (pointerId == _panPointerIdA) { prevX = _panPrevAX; prevY = _panPrevAY; _panPrevAX = x; _panPrevAY = y; }
            else { prevX = _panPrevBX; prevY = _panPrevBY; _panPrevBX = x; _panPrevBY = y; }

            if (_panPointerIdA is null || _panPointerIdB is null) return;

            if (!_isPanning)
            {
                // Coherence check: both fingers must have moved a meaningful distance from
                // where THEY individually started, in roughly the same direction (positive
                // cosine similarity). This is what naturally excludes a pinch/zoom gesture
                // (fingers moving apart = opposing vectors = negative cosine) without needing
                // a separate explicit pinch check.
                double dispAX = _panPrevAX - _panStartAX, dispAY = _panPrevAY - _panStartAY;
                double dispBX = _panPrevBX - _panStartBX, dispBY = _panPrevBY - _panStartBY;
                double magA = Math.Sqrt(dispAX * dispAX + dispAY * dispAY);
                double magB = Math.Sqrt(dispBX * dispBX + dispBY * dispBY);
                if (magA < PanCoherenceMinDisplacementPx || magB < PanCoherenceMinDisplacementPx) return;

                double cosine = (dispAX * dispBX + dispAY * dispBY) / (magA * magB);
                if (cosine < PanCoherenceMinCosine) return; // opposed/unrelated motion - not a pan

                _isPanning = true;
                return; // start clean on the next move rather than jumping by this frame's delta
            }

            // Each finger's own move event contributes half its step - a coherent two-finger
            // drag fires two move events per animation frame (one per finger) that each report
            // roughly the same delta, so halving keeps the board tracking actual finger speed
            // instead of panning twice as fast as the fingers are moving.
            double boardMaxX = BoardWidthPx / 2;
            double boardMaxY = BoardHeightPx / 2;
            _panOffsetX = Math.Clamp(_panOffsetX + (x - prevX) / 2.0, -boardMaxX, boardMaxX);
            _panOffsetY = Math.Clamp(_panOffsetY + (y - prevY) / 2.0, -boardMaxY, boardMaxY);
            StateHasChanged();
        }

        private void OnPointerUp(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;

            if (pointerId == _panPointerIdA || pointerId == _panPointerIdB)
            {
                EndPan();
                return;
            }

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
        }

        private void OnPointerLeave(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;

            if (pointerId == _panPointerIdA || pointerId == _panPointerIdB)
            {
                EndPan();
                return;
            }

            if (_primaryPointerId == pointerId && _activeCell == (row, col))
            {
                ClearPressState();
            }
        }

        private void EndPan()
        {
            _panPointerIdA = null;
            _panPointerIdB = null;
            _isPanning = false;
            // The panned view intentionally persists after fingers lift - it doesn't snap back,
            // it only resets on a fresh game (see HandleGameStarted).
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
