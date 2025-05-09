using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace Minesweeper
{
    public partial class MainWindow : Form, ICellObserver, ITimerObserver
    {
        private Settings _settings;
        private GameEngine _gameEngine;
        private bool _mousePressed = false;
        private Button? _pressedButton = null;
        private Image _lastSmile = Properties.Resources.Smile;

        public MainWindow(Settings settings, GameEngine gameEngine)
        {
            _settings = settings;
            _gameEngine = gameEngine;
            SubscribeComponents();
            InitializeComponent();

        }
        private void SubscribeComponents()
        {
            _gameEngine.Subscribe(this);
            _gameEngine.GameTimer.Subscribe(this);
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            InitializeGameGrid(_settings.Rows, _settings.Cols);

            Mines_Label.Text = _settings.Mines.ToString("000");

            SmileButton.FlatStyle = FlatStyle.Flat;
            SmileButton.FlatAppearance.BorderSize = 0;
            SmileButton.MouseDown += Smile_Button_Pressed;
            SmileButton.MouseUp += Smile_Button_Released;

        }

        private void InitializeGameGrid(int rows, int cols)
        {

            tableGrid.Controls.Clear();
            tableGrid.ColumnStyles.Clear();
            tableGrid.RowStyles.Clear();
            tableGrid.RowCount = rows;
            tableGrid.ColumnCount = cols;
            tableGrid.AutoSize = true;
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
                    Button btn = new Button();

                    btn.Dock = DockStyle.Fill;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackgroundImage = Properties.Resources.Minesweeper_unopened_square;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
                    btn.Margin = new Padding(0);
                    btn.Font = _settings.Font;
                    btn.Tag = (col, row);
                    btn.Click += Tile_Click;
                    btn.MouseDown += Right_Click_Field;
                    btn.MouseDown += Left_Click_Down;
                    btn.MouseUp += Left_Click_Up;
                    btn.MouseMove += Mouse_Move_With_Left_Click;

                    tableGrid.Controls.Add(btn, col, row);
                }
            }
        }
        private void Smile_Button_Pressed(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                _lastSmile = btn.BackgroundImage ?? _lastSmile;
                btn.BackgroundImage = Properties.Resources.Smile_pressed;
            }
        }
        private void Smile_Button_Released(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.BackgroundImage = _lastSmile;
            }
        }
        private void Right_Click_Field(object? sender, MouseEventArgs e)
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
                SmileButton.BackgroundImage = Properties.Resources.Scared;
                DisplayButtonOnHold(btn);
            }
        }
        private void Mouse_Move_With_Left_Click(object? sender, MouseEventArgs e)
        {

            if (!_mousePressed) return;

            SmileButton.BackgroundImage = Properties.Resources.Scared;
            Point realtivePosition = Cursor.Position;
            Point clientPoint = tableGrid.PointToClient(realtivePosition);
            if(clientPoint.X < 0 || clientPoint.Y < 0)
                return;

            int col = (int)(clientPoint.X / _settings.CellSize);
            int row = (int)(clientPoint.Y/ _settings.CellSize);
            
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
            SmileButton.BackgroundImage = Properties.Resources.Smile;
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
                Cell relatedCell = _gameEngine.GetCell(row, col);
                if (relatedCell != null && !relatedCell.IsRevealed && !relatedCell.IsFlagged)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
                    btn.BackgroundImageLayout = ImageLayout.Stretch;
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
                            relatedButton.BackgroundImageLayout = ImageLayout.Stretch;
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


        private void Tile_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                _gameEngine.RevealCell(row, col);
                _gameEngine.ClicksPerformed++;
                CheckGameOver();
            }
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewGame();
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
            btn.Click -= Tile_Click;
            btn.MouseDown -= Right_Click_Field;

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
        public void CheckGameOver()
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
            _mousePressed = false;
            for (int row = 0; row < _settings.Rows; row++)
            {
                for (int col = 0; col < _settings.Cols; col++)
                {
                    if (tableGrid.GetControlFromPosition(col, row) is Button btn)
                    {
                        btn.MouseUp -= Left_Click_Up;
                        btn.Click -= Tile_Click;
                        btn.MouseDown -= Right_Click_Field;
                        btn.MouseDown -= Left_Click_Down;
                        btn.MouseMove -= Mouse_Move_With_Left_Click;
                    }
                }
            }
        }
        private void NewGame()
        {
            _gameEngine.RestartGame();
            SetDefaultLabels();
            SubscribeComponents();
            _lastSmile = Properties.Resources.Smile;
            InitializeGameGrid(_settings.Rows, _settings.Cols);
        }
        private void RebuildGame()
        {
            _gameEngine.Dispose();
            _gameEngine = new GameEngine(_settings);
            NewGame();
        }

        private void SetDefaultLabels()
        {
            Game_Timer_Label.Text = "000";
            Mines_Label.Text = _settings.Mines.ToString("000");
        }
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameEngine.CreateRecord();
            _gameEngine.Dispose();
            _gameEngine.GameTimer.UnsubscribeAll();
            _gameEngine.Unsubscribe(this);
        }
        private void ShowLose()
        {
            SmileButton.BackgroundImage = Properties.Resources.Fail;
            MessageBox.Show("Game Over! You hit a mine.", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void ShowWin()
        {
            SmileButton.BackgroundImage = Properties.Resources.Success;
            MessageBox.Show("Congratulations! You won!", "You Win", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void UpdateRevealed(Cell cell)
        {
            UpdateRevealedUI(cell);
        }

        public void UpdateFlagged(Cell cell)
        {
            UpdateFlagedUI(cell);
            Mines_Label.Text = _gameEngine.MinesToFlagg.ToString("000");
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

        private void recordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StatisticForm optionsForm = new StatisticForm(_gameEngine);
            optionsForm.ShowDialog();
        }
    }
}
