using Minesweeper.Interfaces;

namespace Minesweeper.Domain.Entities
{
    public class Cell : ICell
    {
        public bool IsMine { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public int AdjacentMines { get => CountAdjacentMines(); }
        public bool IsSecured { get => CheckIfSecured(); }
        public int Row { get; set; }
        public int Col { get; set; }
        public List<Cell> AdjacentCells { get; private set; }
        public Cell(int row, int col)
        {
            Row = row;
            Col = col;
            IsMine = false;
            IsRevealed = false;
            IsFlagged = false;
            AdjacentCells = new List<Cell>();
        }

        public void AddAdjacentCell(Cell cell)
        {
            AdjacentCells.Add(cell);
        }

        public int CountAdjacentMines()
        {
            int mines = 0;
            foreach (Cell cell in AdjacentCells)
            {
                if (cell.IsMine)
                {
                    mines++;
                }
            }
            return mines;
        }
        public bool CheckIfSecured()
        {
            int adjucentFlags = 0;
            foreach (Cell cell in AdjacentCells)
            {
                if (cell.IsFlagged)
                {
                    adjucentFlags++;
                }
            }
            return adjucentFlags == AdjacentMines;
        }
    }
}
