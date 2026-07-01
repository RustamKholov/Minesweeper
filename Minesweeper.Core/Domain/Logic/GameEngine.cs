using Minesweeper.Application.DTO;
using Minesweeper.Application.Interfaces;
using Minesweeper.Domain.Entities;
using Minesweeper.Domain.Logic.MineGeneration;

namespace Minesweeper.Domain.Logic
{
    public class GameEngine : IDisposable
    {
        private int _rows;
        private int _cols;
        private int _mines;
        private int _cellToReveal;
        private int _flaggedCells = 0;
        private int _flaggsSet = 0;
        private int _tilesUncovered = 0;
        private int _clicksPerformed = 0;
        private IMineGenerator _mineGenerator;

        private GameStatus _status = GameStatus.NotStarted;
        public List<ICellObserver> Observers { get; set; } = new List<ICellObserver>();
        
        public Cell[,] Grid { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsFirstClick { get; set; }
        public int ClicksPerformed {get => _clicksPerformed; set { _clicksPerformed = value; }}

        private IGameTimer _gameTimer;
        public IGameTimer GameTimer => _gameTimer;
        public bool IsGameWon => _cellToReveal == 0;
        public int MinesToFlagg => _mines - _flaggedCells;
        public GameStatus Status => _status;
        public GameEngine(int rows, int cols, int mines, IMineGenerator mineGenerator)
        {
            _rows = rows;
            _cols = cols;
            _mines = mines;
            _mineGenerator = mineGenerator;

            IsGameOver = false;
            IsFirstClick = true;
            Grid = new Cell[_rows, _cols];
            _cellToReveal = _rows * _cols - _mines;
            _gameTimer = new GameTimer();
            InitializeEmptyGrid();
        }

        public void InitializeEmptyGrid()
        {
            Grid = GridFactory.CreateBoundGrid(_rows, _cols);
        }
        public void GenerateMines(int safeRow, int safeCol)
        {
            HashSet<(int Row, int Col)> minePositions = _mineGenerator.GenerateMinePositions(_rows, _cols, _mines, safeRow, safeCol);
            foreach (var (row, col) in minePositions)
            {
                Grid[row, col].IsMine = true;
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

        public void RevealCell(int row, int col)
        {
            Cell currentCell = Grid[row, col];
            if (currentCell.IsRevealed && currentCell.IsSecured && !IsFirstClick)
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
            { 
                return; 
            }
                
            if (IsFirstClick)
            {
                GenerateMines(row, col);
                IsFirstClick = false;
                _status = GameStatus.Started;
                _gameTimer.StartTimer();
            }
            currentCell.IsRevealed = true;
            _tilesUncovered++;
            NotifyReveald(currentCell);
            if (!currentCell.IsMine)
            {
                _cellToReveal--;
                if (_cellToReveal == 0)
                {
                    _status = GameStatus.Win;
                    GameOver();
                }
            }
            if (currentCell.IsMine)
            {
                _status = GameStatus.Lose;
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
                _flaggsSet++;
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
            _flaggsSet = 0;
            _tilesUncovered = 0;
            _clicksPerformed = 0;
            InitializeEmptyGrid();
            _status = GameStatus.NotStarted;
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
                observer.UpdateFlagged(cell, MinesToFlagg);
            }
        }

        public EngineRecords GetEngineRecords()
        {
            var engineRecord = new EngineRecords()
            {
                SecondsInGame = _gameTimer.SecondInLastGame,
                GameStatus = _status == GameStatus.Started ? GameStatus.Abandoned : _status,
                TilesUncovered = _tilesUncovered,
                ClicksPerformed = _clicksPerformed,
                FlaggsSet = _flaggsSet,
            };
            return engineRecord;
        }

        public void UnsubscribeAll()
        {
            Observers.Clear();
        }  

        public void Dispose()
        {
            UnsubscribeAll();
            _gameTimer.UnsubscribeAll();
            Observers.Clear();
            _gameTimer.Dispose();
        }
    }
}

