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
        [Inject] private SettingsService Settings { get; set; } = default!;

        // Shape/distance-based gesture recognition only. Gesture category (tap vs flick vs pan) is
        // decided purely by displacement from the touch origin (plus what kind of cell it started
        // on). The one exception is the optional long-press-to-flag control, which does use a timer.
        private const double MoveThresholdPx = 15.0;
        private const int LongPressMs = 350;

        private enum Gesture { None, Flick, Pan, Cancelled }

        private (int Row, int Col)? _activeCell;
        private double _pointerDownX, _pointerDownY;
        private long? _primaryPointerId;
        private Gesture _gesture;
        private bool _originRevealed;
        private bool _longFlagged;
        private (int Row, int Col)? _ghostFlagCell;

        private readonly HashSet<(int Row, int Col)> _shakingCells = new();

        // "Grab the cleared ground to pan" navigation. A drag that STARTS on a revealed cell
        // (a number or a cleared blank) slides the whole board 1:1, like grabbing solid terrain
        // and sliding the map under you. A drag that starts on fog (unrevealed) is a flick-to-flag,
        // and a stationary press is a tap (dig fog / chord a number). No modes, no second finger,
        // no auto-follow - the meaning is decided by the cell under the finger at touch-down.
        private const double GridGapPx = 2;  // must match .board-grid gap
        private const double WellPadPx = 10; // must match .board-well padding
        private int _lastTileSize;

        private ElementReference _slotRef;
        private ElementReference _wellRef;
        private double _cameraX, _cameraY;
        private double _panStartCameraX, _panStartCameraY;
        private double _viewportW, _viewportH;

        // Per-frame pan velocity (smoothed) so a release can coast with inertia. A monotonically
        // increasing sequence number cancels an in-flight glide the instant a new gesture starts.
        private double _panVelX, _panVelY;
        private int _gestureSeq;

        private int CellPx => Settings.TileSize;
        private int IconPx => Math.Max(12, (int)Math.Round(CellPx * 0.62));

        // Inline (not scoped CSS) because the offset is per-instance render state, not a style
        // rule - and there is deliberately no CSS transition on it, so the board tracks the
        // finger 1:1 with no lag while panning.
        private string BoardWellStyle => $"transform: translate({_cameraX.ToString(CultureInfo.InvariantCulture)}px, {_cameraY.ToString(CultureInfo.InvariantCulture)}px);";

        protected override void OnInitialized()
        {
            State.Changed += HandleStateChanged;
            State.GameStarted += HandleGameStarted;
            Settings.Changed += HandleSettingsChanged;
            _lastTileSize = Settings.TileSize;
        }

        private void HandleSettingsChanged()
        {
            // A tile-size change resizes the whole board, so any existing pan offset may now be
            // out of bounds - recentre. Other settings don't affect board geometry.
            if (Settings.TileSize != _lastTileSize)
            {
                _lastTileSize = Settings.TileSize;
                _cameraX = 0;
                _cameraY = 0;
            }
            InvokeAsync(StateHasChanged);
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

            // Any new contact cancels an in-flight inertia glide (bumping the sequence the glide
            // loop watches) so a touch always immediately stops a coasting board.
            _gestureSeq++;
            _panVelX = 0;
            _panVelY = 0;

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
            _longFlagged = false;
            _ghostFlagCell = null;
            _panStartCameraX = _cameraX;
            _panStartCameraY = _cameraY;

            // Capture the pointer to this origin cell so every gesture keeps reporting here even
            // as the finger crosses cell boundaries - a flick that drifts into the next cell still
            // flags the origin, and a pan keeps tracking as the board slides under the finger.
            _ = CapturePointerAsync(e.ClientX, e.ClientY, pointerId);

            // Fog presses show dig/flag feedback; a cleared cell only shows chord feedback on a
            // number (handled in CellCssClass), so no press-active toggle needed there.
            if (!_originRevealed)
            {
                State.SetPressActive(true);
                // Optional long-press-to-flag control: schedule a flag if the finger stays put.
                if (Settings.Flag == FlagGestureMode.LongPress)
                {
                    _ = LongPressAsync(_gestureSeq, row, col);
                }
            }
        }

        // Flags the origin cell if the finger is still pressing it (no move, not lifted) after the
        // long-press delay. Only used when the flag control is set to long-press.
        private async Task LongPressAsync(int seq, int row, int col)
        {
            await Task.Delay(LongPressMs);
            if (seq != _gestureSeq || _gesture != Gesture.None || _activeCell != (row, col)) return;
            _longFlagged = true;
            State.SetPressActive(false);
            State.FlagCell(State.GetCell(row, col));
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
                else if (Settings.Flag == FlagGestureMode.LongPress)
                {
                    // In long-press mode a drag off a fog cell isn't a flick - it just cancels the
                    // pending long-press flag and does nothing.
                    _gesture = Gesture.Cancelled;
                    State.SetPressActive(false);
                    return;
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
                double newX = Math.Clamp(_panStartCameraX + dx, -maxX, maxX);
                double newY = Math.Clamp(_panStartCameraY + dy, -maxY, maxY);
                // Smoothed per-frame velocity for the release glide.
                _panVelX = _panVelX * 0.4 + (newX - _cameraX) * 0.6;
                _panVelY = _panVelY * 0.4 + (newY - _cameraY) * 0.6;
                _cameraX = newX;
                _cameraY = newY;
                // Update the transform directly - re-rendering every cell each move is what made
                // dragging feel heavy. Blazor state (_cameraX/Y) stays in sync for the next render.
                _ = SetTransformAsync();
            }
        }

        private void OnPointerUp(PointerEventArgs e, int row, int col)
        {
            long pointerId = (long)e.PointerId;
            if (_primaryPointerId != pointerId || _activeCell != (row, col)) return;

            var gesture = _gesture;
            bool longFlagged = _longFlagged;
            int seq = _gestureSeq;
            ClearGesture();

            // A pan resolves nothing; the view has already moved. On release it coasts with
            // inertia. Action always resolves on the touch-origin cell, never where it lifted.
            if (gesture == Gesture.Pan)
            {
                _ = GlideAsync(seq);
                return;
            }

            // Long-press already planted the flag during the hold, or the gesture was cancelled -
            // either way the lift does nothing further.
            if (longFlagged || gesture == Gesture.Cancelled) return;

            if (gesture == Gesture.Flick)
            {
                State.FlagCell(State.GetCell(row, col));
                return;
            }

            // A stationary press = tap. On fog it digs; on a revealed number it chords (an
            // otherwise dead gesture, and only if chording is enabled); a cleared blank does nothing.
            var cell = State.GetCell(row, col);
            if (cell.IsRevealed)
            {
                if (cell.AdjacentMines > 0 && Settings.ChordEnabled) TryChord(row, col, cell);
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

        // Inertia: after a pan release, keep sliding in the release direction with exponential
        // decay until it slows to a stop or hits an edge. A new gesture (which bumps _gestureSeq)
        // cancels it instantly. Runs off the JS transform to stay smooth, then reconciles state.
        private async Task GlideAsync(int seq)
        {
            double vx = _panVelX, vy = _panVelY;
            if (!Settings.PanInertia || Math.Sqrt(vx * vx + vy * vy) < 3)
            {
                StateHasChanged(); // inertia off, or no real fling - just reconcile the transform
                return;
            }

            (double maxX, double maxY) = PanBounds();
            while (seq == _gestureSeq && (Math.Abs(vx) > 0.35 || Math.Abs(vy) > 0.35))
            {
                double nx = Math.Clamp(_cameraX + vx, -maxX, maxX);
                double ny = Math.Clamp(_cameraY + vy, -maxY, maxY);
                if (nx == _cameraX) vx = 0; // hit an edge - kill that axis
                if (ny == _cameraY) vy = 0;
                _cameraX = nx;
                _cameraY = ny;
                await SetTransformAsync();
                vx *= 0.90;
                vy *= 0.90;
                await Task.Delay(16);
            }

            if (seq == _gestureSeq) StateHasChanged();
        }

        private async Task SetTransformAsync()
        {
            try { await JS.InvokeVoidAsync("msBoard.setTransform", _wellRef, _cameraX, _cameraY); }
            catch { /* transform will be reconciled on the next render */ }
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
            _longFlagged = false;
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
            Settings.Changed -= HandleSettingsChanged;
        }
    }
}
