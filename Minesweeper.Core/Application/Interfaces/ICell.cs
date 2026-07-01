using Minesweeper.Domain.Entities;

namespace Minesweeper.Application.Interfaces
{
    public interface ICell
    {
        List<Cell> AdjacentCells { get; }
        int AdjacentMines { get; }
        int Col { get; set; }
        bool IsFlagged { get; set; }
        bool IsMine { get; set; }
        bool IsRevealed { get; set; }
        bool IsSecured { get; }
        int Row { get; set; }

        void AddAdjacentCell(Cell cell);
        bool CheckIfSecured();
        int CountAdjacentMines();
    }
}