using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Minesweeper.Domain.Entities;
using Minesweeper.Web.Services;

namespace Minesweeper.Web.Game
{
    public partial class GameBoard : ComponentBase, IDisposable
    {
        [Inject] private GameStateService State { get; set; } = default!;

        private const double FlickThresholdPx = 10.0;
        private const int DoubleTapWindowMs = 350;

        private (int Row, int Col)? _activeCell;
        private double _pointerDownX, _pointerDownY;
        private long? _activePointerId;
        private bool _flickCommitted;
        private (int Row, int Col)? _ghostFlagCell;

        private (int Row, int Col)? _lastTapCell;
        private DateTime _lastTapAt;

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

        protected override void OnInitialized()
        {
            State.Changed += HandleStateChanged;
        }

        private void HandleStateChanged() => InvokeAsync(StateHasChanged);

        private void OnPointerDown(PointerEventArgs e, int row, int col)
        {
            if (State.IsGameOver) return;
            if (_activeCell is not null && _activeCell != (row, col)) return;

            var cell = State.GetCell(row, col);
            if (cell.IsFlagged)
            {
                _ = TriggerShakeAsync(row, col);
                return;
            }

            _activeCell = (row, col);
            _activePointerId = (long)e.PointerId;
            _pointerDownX = e.ClientX;
            _pointerDownY = e.ClientY;
            _flickCommitted = false;
            State.SetPressActive(true);
        }

        private void OnPointerMove(PointerEventArgs e, int row, int col)
        {
            if (_activeCell != (row, col) || _activePointerId != (long)e.PointerId) return;

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
            if (_activeCell != (row, col) || _activePointerId != (long)e.PointerId)
            {
                return;
            }

            bool wasFlick = _flickCommitted;
            ClearPressState();

            if (wasFlick)
            {
                State.FlagCell(State.GetCell(row, col));
                return;
            }

            var cell = State.GetCell(row, col);
            DateTime now = DateTime.UtcNow;
            bool isDoubleTap = _lastTapCell == (row, col) && (now - _lastTapAt).TotalMilliseconds <= DoubleTapWindowMs;
            _lastTapCell = (row, col);
            _lastTapAt = now;

            if (cell.IsRevealed && cell.AdjacentMines > 0 && isDoubleTap)
            {
                TryChord(row, col, cell);
            }
            else if (!cell.IsRevealed)
            {
                State.RevealCell(row, col);
            }
        }

        private void OnPointerLeave(PointerEventArgs e, int row, int col)
        {
            if (_activeCell == (row, col) && _activePointerId == (long)e.PointerId)
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
            _activePointerId = null;
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
            if (IsPresentedAsRevealed(cell, visual))
            {
                classes.Add("revealed");
                if (visual == CellVisual.MineExploded) classes.Add("mine-exploded");
            }
            else
            {
                classes.Add("unrevealed");
                if (_activeCell == (row, col)) classes.Add("active");
            }
            if (_shakingCells.Contains((row, col))) classes.Add("shake");
            if (visual == CellVisual.Number) classes.Add($"num-{cell.AdjacentMines}");
            return string.Join(' ', classes);
        }

        public void Dispose() => State.Changed -= HandleStateChanged;
    }
}
