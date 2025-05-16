using Minesweeper.Domain.Entities;
using Minesweeper.Domain.Logic;
using Minesweeper.Application.Interfaces;
using Minesweeper.Infrastructure.Services;

namespace Minesweeper.Application.Services
{
    public class GameService : IGameService
    {
        private GameEngine _gameEngine;

        private IGameSettings _settings;
        private IGameOverService _gameOverObserver;
        private bool _gameSaved = false;
        public GameService(IGameSettings settings)
        {
            _settings = settings;
            _gameEngine = new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
            _gameOverObserver = new GameOverService();
        }
        public bool CheckIfGameOver()
        {
            if (_gameEngine.IsGameOver)
            {
                SaveGame();
            }
            return _gameEngine.IsGameOver;
        }

        public bool CheckIfGameWon() => _gameEngine.IsGameWon;

        
        public void FlaggCell(Cell cell)
        {
            _gameEngine.FlagCell(cell);
        }

        public Cell GetCell(int row, int col) => _gameEngine.GetCell(row, col);
        

        public GameEngine GetNewGameEngine() => new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
        public void SetNewGameEngine(GameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }
        public void RebuildGameEngine(IGameSettings settings)
        {
            _settings = settings;
            _gameEngine = new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
        }

        public void IncrementClick()
        {
            _gameEngine.ClicksPerformed++;
        }

        public void RestartGame()
        {
            SaveGame();
            _gameEngine.RestartGame();
            _gameSaved = false;
        }

        public void RevealCell(int row, int col)
        {
            _gameEngine.RevealCell(row,col);
        }
        public void RevealCell(Cell cell)
        {
            RevealCell(cell.Row, cell.Col);
        }
        public void SaveGame()
        {
            if (_gameEngine.Status == GameStatus.NotStarted ||  _gameSaved) return;
            var engineRecord = _gameEngine.GetEngineRecords();
            var record = new Record()
            {
                secondsInGame = engineRecord.SecondsInGame,
                difficulty = _settings.Difficulty,
                clicksPerformed = engineRecord.ClicksPerformed,
                flaggsSet = engineRecord.FlaggsSet,
                status = engineRecord.GameStatus,
                tilesUncovered = engineRecord.TilesUncovered,
            };
            _gameOverObserver.SaveRecord(record);
            _gameSaved = true;
        }

        public void SubscribeCellObserver(ICellObserver observer)
        {
            _gameEngine.Subscribe(observer);
        }

        public void SubscribeTimerObserver(ITimerObserver observer)
        {
            _gameEngine.GameTimer.Subscribe(observer);   
        }

        public void UnsubscribeCellObserver(ICellObserver observer)
        {
            _gameEngine.Unsubscribe(observer);
        }

        public void UnsubscribeTimerObserver(ITimerObserver observer)
        {
            _gameEngine.GameTimer.Unsubscribe(observer);
        }
        public void DisposeGame()
        {
            _gameEngine.Dispose();
        }

    }
}
