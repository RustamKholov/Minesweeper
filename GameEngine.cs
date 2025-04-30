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
        private int _cellToReveal;
        public bool IsGameWon => _cellToReveal == 0;

        public GameEngine(int rows, int cols, int mines)
        {
            _rows = rows;
            _cols = cols;
            _mines = mines;
            IsGameOver = false;
            IsFirstClick = true;
            Grid = new Cell[rows, cols];
            _cellToReveal = rows * cols - mines;
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
            //// Two more random adjacent cells to the first click are safe
            //List<(int, int)> safeCells = new List<(int, int)>();
            //safeCells.Add((safeRow, safeCol));
            //safeCells.Add(GetRandomAdjacentCell(safeRow, safeCol));
            //int randomSafeCellIndex = random.Next(0, safeCells.Count);
            //(int row, int col) safeCell3 = safeCells[randomSafeCellIndex];
            //safeCells.Add(GetRandomAdjacentCell(safeCell3.row, safeCell3.col));
            
            int minesToPlace = _mines;
            while (minesToPlace > 0)
            {
                int row = random.Next(_rows);
                int col = random.Next(_cols);
                //bool inBounds = row >= 0 && row < _rows && col >= 0 && col < _cols;
                bool isNearSafeClick = Math.Abs(row - safeRow) <= 1 && Math.Abs(col - safeCol) <= 1;
                // Ensure the mine is not placed on the first click or on an already occupied cell
                if (!Grid[row, col].IsMine && !isNearSafeClick /*&& inBounds*/)
                {
                    Grid[row, col].IsMine = true;
                    minesToPlace--;
                }
            }
            CalculateAdjacentMines();
        }
        public ValueTuple<int, int> GetRandomAdjacentCell(int row, int col)
        {
            Random random = new Random();
            int r = random.Next(row - 1, row + 2);
            int c = random.Next(col - 1, col + 2);
            if (r < 0 || r >= _rows || c < 0 || c >= _cols)
            {
                return GetRandomAdjacentCell(row, col);
            }
            return (r, c);
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
                                if (r >= 0 && r < _rows && c >= 0 && c < _cols && Grid[r, c].IsMine) // border check
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
            if (!Grid[row, col].IsMine)
            {
                _cellToReveal--;
                if (_cellToReveal == 0)
                {
                    IsGameOver = true; //win
                }
            }
            if (Grid[row, col].IsMine)
            {
                IsGameOver = true;  //lose
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
    }
}

