using Minesweeper.Domain.Entities;

namespace Minesweeper.Interfaces
{
    public interface ICellObserver
    {
        void UpdateRevealed(Cell cell);
        void UpdateFlagged(Cell cell, int minesToFlagg);
    }
}
