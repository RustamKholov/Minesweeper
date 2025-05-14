using Minesweeper.Interfaces;
using Minesweeper.Models;

namespace Minesweeper
{
    public partial class MainWindow : Form, ICellObserver, ITimerObserver
    {
        private IGameSettings _settings;
        private IGameServiceGenerator _serviceGenerator;
        private IGameService _gameService;
        private ButtonRenderer _buttonRenderer;
        private bool _mousePressed = false;
        private Button? _pressedButton = null;
        private Image _lastSmile = Properties.Resources.Smile;
        private readonly int _paddingWindth = 27;
        private readonly int _paddingHeight = 170;
        

        public MainWindow(IGameSettings settings, IGameServiceGenerator gameEngineGenerator)
        {
            _settings = settings;
            _serviceGenerator = gameEngineGenerator;
            _gameService = _serviceGenerator.CreateGameService();
            _buttonRenderer = new ButtonRenderer(tableGrid, settings);
            SubscribeComponents();
            InitializeComponent();
            
        }
        private void SubscribeComponents()
        {
            _gameService.SubscribeCellObserver(this);
            _gameService.SubscribeTimerObserver(this);
        }
        private Size GetSize()
        {
            return new Size((int)(_settings.Cols * _settings.CellSize + _paddingWindth)
                , (int)(_settings.Rows * _settings.CellSize + _paddingHeight));
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            InitializeGameGrid(_settings.Rows, _settings.Cols);

            ClientSize = GetSize();

            Mines_Label.Text = _settings.Mines.ToString("000");

            _buttonRenderer.Flat(SmileButton);
            
            SmileButton.MouseDown += Smile_Button_Pressed;
            SmileButton.MouseUp += Smile_Button_Released;

        }

        private void InitializeGameGrid(int rows, int cols)
        {

            tableGrid.Controls.Clear();
            tableGrid.ColumnStyles.Clear();
            tableGrid.RowStyles.Clear();
            tableGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableGrid.RowCount = rows;
            tableGrid.ColumnCount = cols;
            
            tableGrid.Padding = new Padding(0);
            tableGrid.Margin = new Padding(0);

            for (int i = 0; i < cols; i++)
                tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, _settings.CellSize));

            for (int i = 0; i < rows; i++)
                tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, _settings.CellSize));

            FillGridWithButtons(rows, cols);
        }
        private void FillGridWithButtons(int rows, int cols)
        {
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Button btn = _buttonRenderer.CreateCustomTile();
                    btn.Tag = (col, row);
                    AddAllEvents(btn);

                    tableGrid.Controls.Add(btn, col, row);
                }
            }
        }
        private void AddAllEvents(Button btn)
        {
            btn.Click += Tile_Click;
            btn.MouseDown += Right_Click_Field;
            btn.MouseDown += Left_Click_Down;
            btn.MouseUp += Left_Click_Up;
            btn.MouseMove += Mouse_Move_With_Left_Click;
        }
        private void ClearAllEvents(Button btn)
        {
            btn.Click -= Tile_Click;
            btn.MouseDown -= Right_Click_Field;
            btn.MouseDown -= Left_Click_Down;
            btn.MouseUp -= Left_Click_Up;
            btn.MouseMove -= Mouse_Move_With_Left_Click;
        }
        private void Smile_Button_Pressed(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                _buttonRenderer.SmilePressed(btn, _lastSmile);
            }
        }
        private void Smile_Button_Released(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                _buttonRenderer.SmileReleased(btn, _lastSmile);
            }
        }
        private void Right_Click_Field(object? sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                var relatedCell = _gameService.GetCell(row, col);
                if (e.Button == MouseButtons.Right)
                {
                    _gameService.FlaggCell(relatedCell);
                }
            }
        }



        private void Left_Click_Down(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is Button btn)
            {
                _mousePressed = true;
                _pressedButton = btn;
                _buttonRenderer.SmileScared(SmileButton);
                DisplayButtonOnHold(btn);
            }
        }
        private void Mouse_Move_With_Left_Click(object? sender, MouseEventArgs e)
        {

            if (!_mousePressed) return;

            _buttonRenderer.SmileScared(SmileButton);
            Point realtivePosition = Cursor.Position;
            Point clientPoint = tableGrid.PointToClient(realtivePosition);
            if (clientPoint.X < 0 || clientPoint.Y < 0)
                return;

            int col = (int)(clientPoint.X / _settings.CellSize);
            int row = (int)(clientPoint.Y / _settings.CellSize);

            if (col < 0 || col >= _settings.Cols || row < 0 || row >= _settings.Rows)
                return;

            Control? hovered = tableGrid.GetControlFromPosition(col, row);
            if (hovered is Button btn && btn != _pressedButton)
            {
                if (_pressedButton != null)
                    DisplayDefaultButton(_pressedButton);

                _pressedButton = btn;
                DisplayButtonOnHold(btn);
            }
        }
        private void Left_Click_Up(object? sender, MouseEventArgs e)
        {
            _mousePressed = false;
            _buttonRenderer.SmileDefault(SmileButton);
            if (_pressedButton != null)
            {
                DisplayDefaultButton(_pressedButton);
                _pressedButton = null;
            }
        }

        private void DisplayButtonOnHold(Button btn)
        {
            if (btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                Cell relatedCell = _gameService.GetCell(row, col);
                if (relatedCell != null && !relatedCell.IsRevealed && !relatedCell.IsFlagged)
                {
                    _buttonRenderer.Opened(btn);
                }
                else if (relatedCell != null && relatedCell.IsRevealed && relatedCell.AdjacentMines > 0)
                {
                    foreach (Cell cell in relatedCell.AdjacentCells)
                    {
                        if (!cell.IsRevealed && !cell.IsFlagged && tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button relatedButton)
                        {
                            _buttonRenderer.Opened(relatedButton);
                        }
                    }
                }
            }
        }
        private void DisplayDefaultButton(Button btn)
        {
            if (btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                Cell relatedCell = _gameService.GetCell(row, col);
                if (relatedCell != null && !relatedCell.IsRevealed && !relatedCell.IsFlagged)
                {
                    _buttonRenderer.Flat(btn);
                    _buttonRenderer.Unopened(btn);
                }
                else if (relatedCell != null && relatedCell.IsRevealed && relatedCell.AdjacentMines > 0)
                {
                    foreach (Cell cell in relatedCell.AdjacentCells)
                    {
                        if (!cell.IsRevealed && !cell.IsFlagged && tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button relatedButton)
                        {
                            _buttonRenderer.Flat(relatedButton);
                            _buttonRenderer.Unopened(relatedButton);
                        }
                    }
                }
            }
        }


        private void Tile_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                _gameService.RevealCell(row, col);
                _gameService.IncrementClick();
                CheckGameOver();
            }
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
        }
        
        private void UpdateRevealedUI(Cell cell)
        {
            if (tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button btn)
            {
                _buttonRenderer.Opened(btn);
                if (cell.AdjacentMines == 0)
                {
                    btn = _buttonRenderer.CreateCustomTile();
                    _buttonRenderer.Opened(btn);
                }
                if (cell.IsMine)
                {
                    _buttonRenderer.OpenedMine(btn);
                }
                else
                {
                    _buttonRenderer.OpenedNotMine(btn, cell.AdjacentMines);
                } 
            }
        }
        public void UpdateFlagedUI(Cell cell)
        {
            if (tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button btn)
            {
                if (cell.IsFlagged)
                {
                    _buttonRenderer.Flag(btn);
                }
                else
                {
                    _buttonRenderer.Unopened(btn);
                }
            }
        }
        public void CheckGameOver()
        {
            if (_gameService.CheckIfGameOver())
            {
                DeactivateGrid();
                ShowAllMines();
                if (_gameService.CheckIfGameWon())
                {
                    ShowWin();
                }
                else
                {
                    ShowLose();
                }
                return;
            }
        }
        private void ShowAllMines()
        {
            for (int row = 0; row < _settings.Rows; row++)
            {
                for (int col = 0; col < _settings.Cols; col++)
                {
                    Button? btn = tableGrid.GetControlFromPosition(col, row) as Button;
                    if (btn != null)
                    {
                        Cell cell = _gameService.GetCell(row, col);

                        if (cell.IsFlagged && !cell.IsMine)
                        {
                            _buttonRenderer.FlagedWrong(btn);
                        }
                        else if (cell.IsMine)
                        {
                            if (cell.IsRevealed)
                            {
                                _buttonRenderer.OpenedMine(btn);
                            }
                            else
                            {
                                if (_gameService.CheckIfGameWon())
                                {
                                    _buttonRenderer.Flag(btn);
                                }
                                else
                                {
                                    _buttonRenderer.Mine(btn); 
                                }

                            }

                        }
                    }
                }
            }
        }
        private void DeactivateGrid()
        {
            _mousePressed = false;
            for (int row = 0; row < _settings.Rows; row++)
            {
                for (int col = 0; col < _settings.Cols; col++)
                {
                    if (tableGrid.GetControlFromPosition(col, row) is Button btn)
                    {
                        ClearAllEvents(btn);
                    }
                }
            }
        }
        private void NewGame()
        {
            DeactivateGrid();
            foreach (Control control in tableGrid.Controls)
            {
                if (control is Button btn)
                {
                    _buttonRenderer.Reset(btn);
                    AddAllEvents(btn);
                }
            }
            _gameService.RestartGame();
            SetDefaultLabels();
            SubscribeComponents();
            _lastSmile = Properties.Resources.Smile;
            
        }
        private void RebuildGame()
        {
            Visible = false;
            _gameService.DisposeGame(); 
            _gameService.RebuildGameEngine(_settings);
            ClientSize = GetSize();
            InitializeGameGrid(_settings.Rows, _settings.Cols);
            CenterToScreen();
            NewGame();
            Visible = true;
        }

        private void SetDefaultLabels()
        {
            Game_Timer_Label.Text = "000";
            Mines_Label.Text = _settings.Mines.ToString("000");
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameService.SaveGame();
            _gameService.DisposeGame();
            _gameService.UnsubscribeTimerObserver(this);
            _gameService.UnsubscribeCellObserver(this);
        }
        private void ShowLose()
        {
            _buttonRenderer.SmileFail(SmileButton);
        }
        private void ShowWin()
        {
            _buttonRenderer.SmileSuccess(SmileButton);
        }

        public void UpdateRevealed(Cell cell)
        {
            UpdateRevealedUI(cell);
        }

        public void UpdateFlagged(Cell cell)
        {
            UpdateFlagedUI(cell);
            Mines_Label.Text = _gameService.GetMinesToFlag().ToString("000");
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            _settings = GameDifficulty.Hard;
            RebuildGame();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            _settings = GameDifficulty.Medium;
            RebuildGame();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _settings = GameDifficulty.Easy;
            RebuildGame();
        }

        public void UpdateTime(int time)
        {
            Game_Timer_Label.Text = time.ToString("000");
        }

        private void SmileButton_Click(object sender, EventArgs e)
        {
            NewGame();
        }

        private void statisticToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatisticForm statusPie = new StatisticForm();
            statusPie.ShowDialog();
        }

        private void recordsStripMenuItem_Click(object sender, EventArgs e)
        {
            RecordsForm optionsForm = new RecordsForm();
            optionsForm.ShowDialog();
        }
    }
}
