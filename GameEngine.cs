using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class GameEngine
    {
        public Cell[,] Grid { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsFirstClick { get; set; }
        private int _rows;
        private int _cols;
        private int _mines;

        public GameEngine(int rows, int cols, int mines)
        {
            _rows = rows;
            _cols = cols;
            _mines = mines;
            IsGameOver = false;
            IsFirstClick = true;
            Grid = new Cell[rows, cols];
            InitializeEmptyGrid();
        }

        public void InitializeEmptyGrid()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    Grid[row, col] = new Cell(row, col);
                }
            }
        }
        public void GenerateMines(int safeRow, int safeCol)
        {
            Random random = new Random();
            int minesToPlace = _mines;
            while (minesToPlace > 0)
            {
                int row = random.Next(_rows);
                int col = random.Next(_cols);
                // Ensure the mine is not placed on the first click or on an already occupied cell
                if (!Grid[row, col].IsMine && (row != safeRow || col != safeCol))
                {
                    Grid[row, col].IsMine = true;
                    minesToPlace--;
                }
            }
            CalculateAdjacentMines();
        }

        private void CalculateAdjacentMines()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    if (!Grid[row, col].IsMine)
                    {
                        int mineCount = 0;
                        for (int r = row - 1; r <= row + 1; r++)
                        {
                            for (int c = col - 1; c <= col + 1; c++)
                            {
                                if (r >= 0 && r < _rows && c >= 0 && c < _cols && Grid[r, c].IsMine)
                                {
                                    mineCount++;
                                }
                            }
                        }
                        Grid[row, col].AdjacentMines = mineCount;
                    }
                }
            }
        }
        public void RevealCell(int row, int col)
        {
            if (IsGameOver || Grid[row, col].IsRevealed || Grid[row, col].IsFlagged)
                return;
            if (IsFirstClick)
            {
                GenerateMines(row, col);
                IsFirstClick = false;
            }
            Grid[row, col].IsRevealed = true;
            if (Grid[row, col].IsMine)
            {
                IsGameOver = true;
            }
            else if (Grid[row, col].AdjacentMines == 0)
            {
                for (int r = row - 1; r <= row + 1; r++)
                {
                    for (int c = col - 1; c <= col + 1; c++)
                    {
                        if (r >= 0 && r < _rows && c >= 0 && c < _cols)
                        {
                            RevealCell(r, c);
                        }
                    }
                }
            }
        }
        public bool CheckIfWin()
        {
            int revealedCells = 0;
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    if (Grid[row, col].IsRevealed && !Grid[row, col].IsMine)
                    {
                        revealedCells++;
                    }
                }
            }
            bool win = revealedCells == (_rows * _cols) - _mines;
            if (win)
            {
                IsGameOver = true;
            }
            return win;
        }
    }
}

