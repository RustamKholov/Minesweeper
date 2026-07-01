using Minesweeper.Domain.Entities;

namespace Minesweeper.Domain.Logic.MineGeneration
{
    public static class GridFactory
    {
        public static Cell[,] CreateBoundGrid(int rows, int cols)
        {
            Cell[,] grid = new Cell[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    grid[row, col] = new Cell(row, col);
                }
            }
            BindAdjacentCells(grid, rows, cols);
            return grid;
        }

        private static void BindAdjacentCells(Cell[,] grid, int rows, int cols)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    for (int r = row - 1; r <= row + 1; r++)
                    {
                        for (int c = col - 1; c <= col + 1; c++)
                        {
                            if (r >= 0 && r < rows && c >= 0 && c < cols)
                            {
                                grid[row, col].AddAdjacentCell(grid[r, c]);
                            }
                        }
                    }
                }
            }
        }
    }
}
