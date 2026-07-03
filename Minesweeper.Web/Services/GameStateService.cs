using Minesweeper.Application.Interfaces;
using Minesweeper.Domain.Entities;
using Minesweeper.Infrastructure.Configuration;
using Minesweeper.Web.Models;

namespace Minesweeper.Web.Services
{
    public enum GameMood { Smile, Worried, Cool, Dead }

    public sealed class GameStateService : ICellObserver, ITimerObserver, IDisposable
    {
        private readonly IGameService _gameService;
        private readonly RecordsApiClient _recordsApi;
        private bool _anyPressActive;

        public IGameSettings CurrentSettings { get; private set; }
        public int MinesLeft { get; private set; }
        public int ElapsedSeconds { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsGameWon { get; private set; }

        // Custom-sized games aren't one of the classic presets, so they're excluded from records
        // and stats (which are grouped by the fixed Easy/Medium/Hard difficulties).
        public bool IsCustom { get; private set; }

        public event Action? Changed;

        // Separate from Changed, which also fires on every reveal/flag - GameBoard uses this
        // one specifically to know when to reset its pan offset, since a per-cell-mutation
        // signal would reset the pan mid-game every time the player touches a cell.
        public event Action? GameStarted;

        public GameMood CurrentMood =>
            IsGameOver switch
            {
                true when IsGameWon => GameMood.Cool,
                true => GameMood.Dead,
                _ => _anyPressActive ? GameMood.Worried : GameMood.Smile
            };

        public GameStateService(IGameServiceGenerator gameServiceGenerator, IGameSettings initialSettings, RecordsApiClient recordsApi)
        {
            _recordsApi = recordsApi;
            CurrentSettings = initialSettings;
            _gameService = gameServiceGenerator.CreateGameService();
            MinesLeft = CurrentSettings.Mines;
            _gameService.SubscribeCellObserver(this);
            _gameService.SubscribeTimerObserver(this);
        }

        public Cell GetCell(int row, int col) => _gameService.GetCell(row, col);

        public void RevealCell(int row, int col)
        {
            if (IsGameOver) return;
            _gameService.RevealCell(row, col);
            _gameService.IncrementClick();
            AfterMutation();
        }

        public void FlagCell(Cell cell)
        {
            if (IsGameOver) return;
            _gameService.FlaggCell(cell);
            AfterMutation();
        }

        public void SetPressActive(bool active)
        {
            if (_anyPressActive == active) return;
            _anyPressActive = active;
            Changed?.Invoke();
        }

        public void SwitchDifficulty(IGameSettings newSettings)
        {
            IsCustom = false;
            ApplySettings(newSettings);
        }

        // A one-off board of arbitrary size. Clamped to sane bounds so no-guess generation stays
        // feasible/fast; not recorded (see IsCustom).
        public void SwitchToCustom(int rows, int cols, int mines)
        {
            rows = Math.Clamp(rows, 5, 30);
            cols = Math.Clamp(cols, 5, 30);
            mines = Math.Clamp(mines, 1, rows * cols - 9);
            IsCustom = true;
            ApplySettings(new Settings { Rows = rows, Cols = cols, Mines = mines, Difficulty = Difficulty.Medium });
        }

        private void ApplySettings(IGameSettings newSettings)
        {
            CurrentSettings = newSettings;
            _gameService.RebuildGameEngine(newSettings);
            _gameService.SubscribeCellObserver(this);
            _gameService.SubscribeTimerObserver(this);
            MinesLeft = newSettings.Mines;
            ElapsedSeconds = 0;
            IsGameOver = false;
            IsGameWon = false;
            _anyPressActive = false;
            Changed?.Invoke();
            GameStarted?.Invoke();
        }

        public void RestartGame()
        {
            _gameService.RestartGame();
            // GameEngine.RestartGame() runs GameOver(), which unsubscribes ALL cell and timer
            // observers. Re-subscribe (as SwitchDifficulty does) or the restarted game has no
            // timer observer at all - and unlike cell reveals (which also refresh the UI via
            // Changed/AfterMutation), the timer's only update path is UpdateTime, so the clock
            // would silently stay frozen at 0 for every game after the first.
            _gameService.SubscribeCellObserver(this);
            _gameService.SubscribeTimerObserver(this);
            MinesLeft = CurrentSettings.Mines;
            ElapsedSeconds = 0;
            IsGameOver = false;
            IsGameWon = false;
            _anyPressActive = false;
            Changed?.Invoke();
            GameStarted?.Invoke();
        }

        private void AfterMutation()
        {
            if (_gameService.CheckIfGameOver())
            {
                IsGameOver = true;
                IsGameWon = _gameService.CheckIfGameWon();
                if (!IsCustom) _ = SubmitRecordAsync();
            }
            Changed?.Invoke();
        }

        private async Task SubmitRecordAsync()
        {
            var engineRecords = _gameService.GetEngineRecords();
            var request = new SubmitRecordRequest(
                engineRecords.SecondsInGame,
                CurrentSettings.Difficulty,
                engineRecords.GameStatus,
                engineRecords.TilesUncovered,
                engineRecords.ClicksPerformed,
                engineRecords.FlaggsSet);
            await _recordsApi.SubmitAsync(request);
        }

        public void UpdateRevealed(Cell cell) => Changed?.Invoke();

        public void UpdateFlagged(Cell cell, int minesLeftToFlag)
        {
            MinesLeft = minesLeftToFlag;
            Changed?.Invoke();
        }

        public void UpdateTime(int time)
        {
            ElapsedSeconds = time;
            Changed?.Invoke();
        }

        public void Dispose() => _gameService.DisposeGame();
    }
}
