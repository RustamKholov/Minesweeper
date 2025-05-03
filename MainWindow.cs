namespace Minesweeper
{
    public partial class MainWindow : Form, ICellObserver
    {
        private readonly Settings _settings;
        private GameEngine _gameEngine;
        public MainWindow(Settings settings)
        {
            _settings = settings;
            _gameEngine = new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
            _gameEngine.Subscribe(this);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeGameGrid(_settings.Rows, _settings.Cols);
        }

        private void InitializeGameGrid(int rows, int cols)
        {

            tableGrid.Controls.Clear();
            tableGrid.ColumnStyles.Clear();
            tableGrid.RowStyles.Clear();
            tableGrid.RowCount = rows;
            tableGrid.ColumnCount = cols;

            for (int i = 0; i < cols; i++)
                tableGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, _settings.CellSize));

            for (int i = 0; i < rows; i++)
                tableGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, _settings.CellSize));

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Button btn = new Button();
                    btn.Dock = DockStyle.Fill;
                    btn.FlatStyle = FlatStyle.Popup;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.Gray;
                    btn.BackColor = Color.Silver;
                    btn.Margin = new Padding(0);
                    btn.Font = new Font("Arial", 14, FontStyle.Bold);

                    btn.Tag = (col, row);
                    btn.Click += Cell_Click;
                    btn.MouseDown += Right_Click;
                    btn.MouseEnter += Cell_Hover;

                    tableGrid.Controls.Add(btn, col, row);


                }
            }
        }
        private void Right_Click(object? sender, MouseEventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                var relatedCell = _gameEngine.GetCell(row, col);
                if (e.Button == MouseButtons.Right)
                {
                    _gameEngine.FlagCell(relatedCell);
                }
            }
        }

        private void Cell_Hover(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                var relatedCell = _gameEngine.Grid[row, col];
                btn.FlatAppearance.MouseOverBackColor = btn.BackColor;
            }
        }
        private void Cell_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                var currentCell = _gameEngine.Grid[row, col];
                _gameEngine.RevealCell(row, col);
                ChechGameOver();
                //UpdateGridUI();
            }
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestartGame();
        }
        #region ButtonsRevealingUI
        private void ButtonRevealed(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.Gray;
            btn.BackColor = Color.LightGray;
        }
        private void RevealdButtonIsMine(Button btn)
        {
            btn.BackColor = Color.Red;
            btn.Text = "💣";
        }
        private void RevealdButtonIsNotMine(Button btn, Cell cell)
        {
            btn.ForeColor = cell.AdjacentMines switch
            {
                1 => Color.Blue,
                2 => Color.Green,
                3 => Color.Red,
                4 => Color.DarkBlue,
                5 => Color.DarkRed,
                6 => Color.Cyan,
                7 => Color.Black,
                _ => btn.ForeColor
            };
            btn.Text = cell.AdjacentMines > 0 ? cell.AdjacentMines.ToString() : "";
        }
        private void RevealdButtonZeroMines(Button btn)
        {
            btn.Click -= Cell_Click;
            btn.MouseDown -= Right_Click;
        }
        #endregion
        private void UpdateRevealedUI(Cell cell)
        {
            if (tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button btn)
            {
                ButtonRevealed(btn);
                if (cell.AdjacentMines == 0)
                {
                    RevealdButtonZeroMines(btn);
                }
                if (cell.IsMine)
                {
                    RevealdButtonIsMine(btn);
                }
                else
                {
                    RevealdButtonIsNotMine(btn, cell);
                }
                btn.Refresh();
            }
        }
        public void UpdateFlagedUI(Cell cell)
        {
            if (tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button btn)
            {
                if (cell.IsFlagged)
                {
                    btn.ForeColor = Color.Red;
                    btn.Text = "🚩";
                }
                else
                {
                    btn.ForeColor = Color.Black;
                    btn.Text = "";
                }
            }
        }
        public void ChechGameOver()
        {
            if (_gameEngine.IsGameOver)
            {
                DeactivateGrid();
                ShowAllMines();
                if (_gameEngine.IsGameWon)
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
                        Cell cell = _gameEngine.Grid[row, col];
                        if (cell.IsFlagged && !cell.IsMine)
                        {
                            btn.ForeColor = Color.Red;
                            btn.Text = "❌";
                        }
                        else if (cell.IsFlagged && cell.IsMine)
                        {
                            btn.ForeColor = Color.Green;
                            btn.Text = "🚩";
                        }
                        else
                        if (cell.IsMine)
                        {
                            btn.Text = "💣";
                        }
                    }
                }
            }
        }
        private void DeactivateGrid()
        {
            for (int row = 0; row < _settings.Rows; row++)
            {
                for (int col = 0; col < _settings.Cols; col++)
                {
                    Button? btn = tableGrid.GetControlFromPosition(col, row) as Button;
                    if (btn != null)
                    {
                        btn.Click -= Cell_Click;
                        btn.MouseDown -= Right_Click;

                    }
                }
            }
        }
        private void RestartGame()
        {
            _gameEngine.UnsubscribeAll();
            _gameEngine = new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
            _gameEngine.Subscribe(this);
            InitializeGameGrid(_settings.Rows, _settings.Cols);
        }
        private void ShowLose()
        {
            MessageBox.Show("Game Over! You hit a mine.");
        }
        private void ShowWin()
        {
            MessageBox.Show("Win! You cleared the field.");
        }

        public void UpdateRevealed(Cell cell)
        {
            UpdateRevealedUI(cell);
        }

        public void UpdateFlagged(Cell cell)
        {
            UpdateFlagedUI(cell);
        }

        private void tableGrid_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
