using Minesweeper.Controllers;
using Minesweeper.Interfaces;

namespace Minesweeper.Models
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
        private DataBaseCSV _dataBase;
        private RecordsSQLManager _sqlManager;
        private bool _gameSaved = false;
        private GameStatus _status = GameStatus.NotStarted;
        public List<ICellObserver> Observers { get; set; } = new List<ICellObserver>();
        
        public Cell[,] Grid { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsFirstClick { get; set; }
        public int ClicksPerformed {get => _clicksPerformed; set { _clicksPerformed = value; }}

        private IGameTimer _gameTimer;
        public DataBaseCSV CSVDataBase => _dataBase;
        public RecordsSQLManager SQLiteDataBase => _sqlManager;
        public IGameTimer GameTimer => _gameTimer;
        public bool IsGameWon => _cellToReveal == 0;
        public int MinesToFlagg => _mines - _flaggedCells;
        public GameEngine(int rows, int cols, int mines)
        {
            _rows = rows;
            _cols = cols;
            _mines = mines;
            _dataBase = new DataBaseCSV();
            _sqlManager = new RecordsSQLManager();
            IsGameOver = false;
            IsFirstClick = true;
            Grid = new Cell[_rows, _cols];
            _cellToReveal = _rows * _cols - _mines;
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
            CreateRecord();
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
            Grid = new Cell[_rows, _cols];
            InitializeEmptyGrid();
            _status = GameStatus.NotStarted;
            _gameSaved = false;
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
        public void CreateRecord()
        {
            if (_status == GameStatus.NotStarted || _gameSaved)
            {
                return;
            }
            Record record = new Record()
            {
                secondsInGame = _gameTimer.SecondInLastGame,
                difficulty = Difficulty.Easy,
                status = _status == GameStatus.Started ? GameStatus.Abandoned : _status,
                tilesUncovered = _tilesUncovered,
                clicksPerformed = _clicksPerformed,
                flaggsSet = _flaggsSet,
            };
            _sqlManager.SaveRecord(record);
            _dataBase.SaveRecord(record);
            _dataBase.SaveToCSV();
            _gameSaved = true;
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

