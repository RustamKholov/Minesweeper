namespace Minesweeper
{
    public partial class MainWindow : Form, ICellObserver
    {
        private readonly Settings _settings;
        private GameEngine _gameEngine;
        private bool _mousePressed = false;
        private Button? _pressedButton = null;
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
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackgroundImage = Properties.Resources.Minesweeper_unopened_square;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Margin = new Padding(0);
                    btn.Font = _settings.Font;
                    btn.Tag = (col, row);
                    btn.Click += Cell_Click;
                    btn.MouseDown += Right_Click;
                    btn.MouseDown += Left_Click_Down;
                    btn.MouseUp += Left_Click_Up;
                    btn.MouseMove += Left_Clicked_Move;
                   

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
        


        private void Left_Click_Down(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && sender is Button btn)
            {
                _mousePressed = true;
                _pressedButton = btn;
                DisplayButtonOnHold(btn);
            }
        }
        private void Left_Clicked_Move(object? sender, MouseEventArgs e)
        {
            if (_mousePressed && sender is Button btn && btn != _pressedButton)
            {
                if (_pressedButton != null)
                {
                    DisplayDefaultButton(_pressedButton);
                }
                _pressedButton = btn;
                DisplayButtonOnHold(btn);
            }
        }
        private void Left_Click_Up(object? sender, MouseEventArgs e)
        {
            _mousePressed = false;
            if ( _pressedButton != null)
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
                Cell relatedCell = _gameEngine.GetCell(row, col);
                if (relatedCell != null && !relatedCell.IsRevealed && !relatedCell.IsFlagged)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
                }
                else if (relatedCell != null && relatedCell.IsRevealed && relatedCell.AdjacentMines > 0)
                {
                    foreach (Cell cell in relatedCell.AdjacentCells)
                    {
                        if (!cell.IsRevealed && !cell.IsFlagged && tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button relatedButton)
                        {
                            relatedButton.FlatStyle = FlatStyle.Flat;
                            relatedButton.FlatAppearance.BorderSize = 0;
                            relatedButton.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
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
                Cell relatedCell = _gameEngine.GetCell(row, col);
                if (relatedCell != null && !relatedCell.IsRevealed && !relatedCell.IsFlagged)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackgroundImage = Properties.Resources.Minesweeper_unopened_square;
                }
                else if (relatedCell != null && relatedCell.IsRevealed && relatedCell.AdjacentMines > 0)
                {
                    foreach (Cell cell in relatedCell.AdjacentCells)
                    {
                        if (!cell.IsRevealed && !cell.IsFlagged && tableGrid.GetControlFromPosition(cell.Col, cell.Row) is Button relatedButton)
                        {
                            relatedButton.FlatStyle = FlatStyle.Flat;
                            relatedButton.FlatAppearance.BorderSize = 0;
                            relatedButton.BackgroundImage = Properties.Resources.Minesweeper_unopened_square;
                        }
                    }
                }
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
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = Color.Gray;
            btn.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
        }
        private void RevealdButtonIsMine(Button btn)
        {
            btn.BackgroundImage = Properties.Resources.Minesweeper_opened_square_mine;
            btn.BackgroundImageLayout = ImageLayout.Stretch;
            btn.Image = Properties.Resources.Mine;
            btn.ImageAlign = ContentAlignment.MiddleCenter;
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
                    btn.BackgroundImage = Properties.Resources.Minesweeper_flag;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    btn.BackgroundImage = Properties.Resources.Minesweeper_unopened_square;
                }
            }
        }
        public void ChechGameOver()
        {
            if (_gameEngine.IsGameOver)
            {
                _gameEngine.UnsubscribeAll();
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
                            btn.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
                            btn.BackgroundImageLayout = ImageLayout.Stretch;
                            btn.Image = Properties.Resources.Wrong_Mine;
                            
                        }
                        else if (cell.IsMine)
                        {
                            if (cell.IsRevealed)
                            {
                                btn.BackgroundImage = Properties.Resources.Minesweeper_opened_square_mine;
                                btn.Image = Properties.Resources.Mine;
                                btn.ImageAlign = ContentAlignment.MiddleCenter;
                            }
                            else
                            {
                                if (_gameEngine.IsGameWon)
                                {
                                    btn.BackgroundImage = Properties.Resources.Minesweeper_flag;
                                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                                }
                                else
                                {
                                    btn.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
                                    btn.Image = Properties.Resources.Mine;
                                    btn.ImageAlign = ContentAlignment.MiddleCenter; ;
                                }
                                
                            }
                            
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
            _gameEngine.Subscribe(this);
            _gameEngine.RestartGame();
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

        
    }
}
