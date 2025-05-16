using Minesweeper.Domain.Entities;
using Minesweeper.Domain.Logic;

namespace Minesweeper.Application.Interfaces
{
    public interface IGameService
    {
        Cell GetCell(int row, int col);
        void SubscribeCellObserver(ICellObserver observer);
        void UnsubscribeCellObserver(ICellObserver observer);
        void SubscribeTimerObserver(ITimerObserver observer);
        void UnsubscribeTimerObserver(ITimerObserver observer);
        void FlaggCell(Cell cell);
        void RevealCell(int row, int col);
        void RevealCell(Cell cell);
        void IncrementClick();
        bool CheckIfGameOver();
        bool CheckIfGameWon();
        void RestartGame();
        GameEngine GetNewGameEngine();
        void SetNewGameEngine(GameEngine gameEngine);
        void RebuildGameEngine(IGameSettings settings);
        void SaveGame();
        void DisposeGame();
    }

}
