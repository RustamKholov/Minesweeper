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
        public List<ICellObserver> Observers { get; set; } = new List<ICellObserver>();
        
        public Cell[,] Grid { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsFirstClick { get; set; }
        private int _rows;
        private int _cols;
        private int _mines;
        private int _cellToReveal;
        private int _flaggedCells = 0;
        private GameTimer _gameTimer;
        public GameTimer GameTimer => _gameTimer;
        public bool IsGameWon => _cellToReveal == 0;
        public int MinesToFlagg => _mines - _flaggedCells;
        
        public GameEngine(int rows, int cols, int mines)
        {
            _rows = rows;
            _cols = cols;
            _mines = mines;
            IsGameOver = false;
            IsFirstClick = true;
            Grid = new Cell[rows, cols];
            _cellToReveal = rows * cols - mines;
            _gameTimer = new GameTimer();
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
            BindAdjacentCells();
        }
        public void GenerateMines(int safeRow, int safeCol)
        {
            Random random = new Random();
            
            int minesToPlace = _mines;
            while (minesToPlace > 0)
            {
                int row = random.Next(_rows);
                int col = random.Next(_cols);
                //bool inBounds = row >= 0 && row < _rows && col >= 0 && col < _cols;
                bool isNearSafeClick = Math.Abs(row - safeRow) <= 1 && Math.Abs(col - safeCol) <= 1;
                // first click alsways has a 0 adjucent mine
                if (!Grid[row, col].IsMine && !isNearSafeClick /*&& inBounds*/)
                {
                    Grid[row, col].IsMine = true;
                    minesToPlace--;
                }
            }
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

        private void BindAdjacentCells()
        {
            for (int row = 0; row < _rows; row++) //
            {                                       // foreach cell       
                for (int col = 0; col < _cols; col++) //
                {
                    for (int r = row - 1; r <= row + 1; r++) //
                    {                                           // foreach neighbour
                        for (int c = col - 1; c <= col + 1; c++) //
                        {
                            if (r >= 0 && r < _rows && c >= 0 && c < _cols) // border check
                            {
                                Grid[row, col].AddAdjacentCell(Grid[r, c]);    //bind
                            }
                        }
                    }
                }
            }
        }
        public void RevealCell(int row, int col)
        {
            Cell currentCell = Grid[row, col];
            if (currentCell.IsRevealed && currentCell.IsSecured)
            {
                foreach (Cell cell in currentCell.AdjacentCells)
                {
                    if (!cell.IsRevealed)
                    {
                        RevealCell(cell.Row, cell.Col);
                    }
                }
            }
            if (IsGameOver || currentCell.IsRevealed || currentCell.IsFlagged)
                return;
            if (IsFirstClick)
            {
                GenerateMines(row, col);
                IsFirstClick = false;
                _gameTimer.StartTimer();
            }
            currentCell.IsRevealed = true;
            NotifyReveald(currentCell);
            if (!currentCell.IsMine)
            {
                _cellToReveal--;
                if (_cellToReveal == 0)
                {
                    GameOver();
                }
            }
            if (currentCell.IsMine)
            {
                GameOver();
            }
            else if (currentCell.AdjacentMines == 0)
            {
                foreach (Cell cell in currentCell.AdjacentCells)
                {
                    if (!cell.IsRevealed && !cell.IsFlagged)
                    {
                        RevealCell(cell.Row, cell.Col);
                    }
                }
            }
        }
        public Cell GetCell(int row, int col)
        {
            if (row < 0 || row >= _rows || col < 0 || col >= _cols)
            {
                throw new ArgumentOutOfRangeException("Cell is out of bounds");
            }
            return Grid[row, col];
        }
        public void FlagCell(int row, int col)
        {
            Cell currentCell = Grid[row, col];
            if (currentCell.IsRevealed)
                return;
            if (!currentCell.IsFlagged)
            {
                _flaggedCells++;
                currentCell.IsFlagged = true;
            }
            else
            {
                _flaggedCells--;
                currentCell.IsFlagged = false;
            }
                NotifyFlagged(currentCell);
        }
        public void FlagCell(Cell cell)
        {
            FlagCell(cell.Row, cell.Col);
        }
        public void GameOver()
        {
            _gameTimer.StopTimer();
            IsGameOver = true;
            UnsubscribeAll();
            _gameTimer.UnsubscribeAll();
        }
        public void RestartGame()
        {
            GameOver();
            IsGameOver = false;
            IsFirstClick = true;
            _cellToReveal = _rows * _cols - _mines;
            _flaggedCells = 0;
            InitializeEmptyGrid();
        }
        public void Subscribe(ICellObserver observer)
        {
            Observers.Add(observer);
        }
        public void Unsubscribe(ICellObserver observer)
        {
            Observers.Remove(observer);
        }
        public void NotifyReveald(Cell cell)
        {
            foreach (var observer in Observers)
            {
                observer.UpdateRevealed(cell);
            }
        }
        public void NotifyFlagged(Cell cell)
        {
            foreach (var observer in Observers)
            {
                observer.UpdateFlagged(cell);
            }
        }
        public void UnsubscribeAll()
        {
            Observers.Clear();
        }
    }
}

