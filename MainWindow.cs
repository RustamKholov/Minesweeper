namespace Minesweeper
{
    public partial class MainWindow : Form
    {
        private readonly Settings _settings;
        private GameEngine _gameEngine;
        public MainWindow(Settings settings)
        {
            _settings = settings;
            _gameEngine = new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
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
                    btn.BackColor = Color.LightGray;
                    btn.Margin = new Padding(0);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.Gray;
                    btn.Tag = (row, col);
                    btn.Click += Cell_Click;

                    tableGrid.Controls.Add(btn, col, row);
                    btn.MouseDown += (s, e) =>
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            btn.Text = btn.Text == "" ? "🚩" : "";
                        }
                    };
                    btn.MouseEnter += (s, e) =>
                    {
                        btn.BackColor = Color.Gray;
                    };
                    btn.MouseLeave += (s, e) =>
                    {
                        btn.BackColor = Color.LightGray;
                    };
                }
            }
        }
        private void Cell_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is ValueTuple<int, int> coords)
            {
                int row = coords.Item1;
                int col = coords.Item2;
                _gameEngine.RevealCell(row, col);
                if (_gameEngine.CheckIfWin())
                {
                    ShowWin();
                }
                else
                {
                    UpdateGridUI();
                }
            }
        }
        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RestartGame();
        }
        private void UpdateGridUI()
        {
            for (int row = 0; row < _settings.Rows; row++)
            {
                for (int col = 0; col < _settings.Cols; col++)
                {
                    Button? btn = tableGrid.GetControlFromPosition(col, row) as Button;
                    if (btn != null)
                    {
                        Cell cell = _gameEngine.Grid[row, col];
                        if (cell.IsRevealed)
                        {
                            btn.BackColor = Color.White;
                            btn.Enabled = false;
                            if (cell.IsMine)
                            {
                                btn.BackColor = Color.Red;
                                btn.Text = "💣";
                                ShowLose();
                                break;
                            }
                            else
                            {
                                btn.Text = cell.AdjacentMines > 0 ? cell.AdjacentMines.ToString() : "";
                            }
                            
                        }
                    }
                    
                }
            }
        }
        private void RestartGame()
        {
            _gameEngine = new GameEngine(_settings.Rows, _settings.Cols, _settings.Mines);
            InitializeGameGrid(_settings.Rows, _settings.Cols);
        }
        private void ShowLose()
        {
            MessageBox.Show("Game Over! You hit a mine.");
        }
        private void ShowWin()
        {
            MessageBox.Show("Congratulations! You cleared the field.");
        }
    }
}
