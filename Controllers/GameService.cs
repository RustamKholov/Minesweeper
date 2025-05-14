using Minesweeper.Interfaces;
using Minesweeper.Models;

namespace Minesweeper.Controllers
{
    public class GameService : IGameService
    {
        private GameEngine _gameEngine;

        private IGameSettings _settings;

        public GameService(IGameSettings settings)
        {
            _settings = settings;
            _gameEngine = new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
        }
        public bool CheckIfGameOver() => _gameEngine.IsGameOver;

        public bool CheckIfGameWon() => _gameEngine.IsGameWon;

        
        public void FlaggCell(Cell cell)
        {
            _gameEngine.FlagCell(cell);
        }

        public Cell GetCell(int row, int col) => _gameEngine.GetCell(row, col);
        

        public GameEngine GetNewGameEngine() => new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);

        public int GetMinesToFlag() => _gameEngine.MinesToFlagg;

        public void IncrementClick()
        {
            _gameEngine.ClicksPerformed++;
        }

        public void RestartGame()
        {
            _gameEngine.RestartGame();
        }

        public void RevealCell(int row, int col)
        {
            _gameEngine.RevealCell(row,col);
        }

        public void SaveGame()
        {
            _gameEngine.CreateRecord();
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
