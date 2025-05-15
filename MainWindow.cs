using Minesweeper.Interfaces;
using Minesweeper.Models;

namespace Minesweeper
{
    public partial class MainWindow : Form, ICellObserver, ITimerObserver
    {
        private IGameSettings _settings;
        private readonly IGameServiceGenerator _serviceGenerator;
        private readonly IGameService _gameService;
        private readonly ButtonRenderer _buttonRenderer;
        private readonly GridBuilder _gridBuilder;
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
            SubscribeComponents();
            InitializeComponent();
            _buttonRenderer = new ButtonRenderer(tableGrid, settings);
            _gridBuilder = new GridBuilder(tableGrid, settings, _buttonRenderer);

        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            _gridBuilder.Build();
            _gridBuilder.BindEvents(AddAllEvents);

            ClientSize = GetSize();

            Mines_Label.Text = _settings.Mines.ToString("000");

            _buttonRenderer.Flat(SmileButton);

            SmileButton.MouseDown += Smile_Button_Pressed;
            SmileButton.MouseUp += Smile_Button_Released;

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
        private Cell GetRelatedCell(Button button)
        {
            if (button.Tag is ValueTuple<int, int> coords)
            {
                int col = coords.Item1;
                int row = coords.Item2;
                Cell relatedCell = _gameService.GetCell(row, col);
                return relatedCell;
            }
            else
            {
                throw new MissingMemberException();
            }
        }
        private void OpenField()
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
                                _buttonRenderer.MineBlow(btn);
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
        private void CheckGameOver()
        {
            if (_gameService.CheckIfGameOver())
            {
                DeactivateGrid();
                OpenField();
                if (_gameService.CheckIfGameWon())
                {
                    _buttonRenderer.SmileSuccess(SmileButton);
                }
                else
                {
                    _buttonRenderer.SmileFail(SmileButton);
                }
                return;
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

            _gridBuilder.Rebuild(_settings);
            _gridBuilder.BindEvents(AddAllEvents);

            CenterToScreen();
            NewGame();
            Visible = true;
        }
        private void SetDefaultLabels()
        {
            Game_Timer_Label.Text = "000";
            Mines_Label.Text = _settings.Mines.ToString("000");
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
                _buttonRenderer.ButtonOnHold(btn, GetRelatedCell(btn));
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
                    _buttonRenderer.ReleasedButton(_pressedButton, GetRelatedCell(_pressedButton));

                _pressedButton = btn;
                _buttonRenderer.ButtonOnHold(btn, GetRelatedCell(btn));
            }

        }
        private void Left_Click_Up(object? sender, MouseEventArgs e)
        {
            _mousePressed = false;
            _buttonRenderer.SmileDefault(SmileButton);
            if (_pressedButton != null)
            {

                _buttonRenderer.ReleasedButton(_pressedButton, GetRelatedCell(_pressedButton));
                _pressedButton = null;
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
        
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            _gameService.SaveGame();
            _gameService.DisposeGame();
            _gameService.UnsubscribeTimerObserver(this);
            _gameService.UnsubscribeCellObserver(this);
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

        public void UpdateTime(int time)
        {
            if (time > 999)
            {
                Game_Timer_Label.Text = "999";  // maximum time to display
            }
            else
            {
                Game_Timer_Label.Text = time.ToString("000");
            }
        }

        public void UpdateRevealed(Cell cell)
        {
            _buttonRenderer.UpdateRevealedUI(cell);
        }

        public void UpdateFlagged(Cell cell, int minesToFlagg)
        {
            _buttonRenderer.UpdateFlagedUI(cell);

            if (minesToFlagg < -99)
            {
                Mines_Label.Text = "-99";  // minimum mines/flagg to display
            }
            else if (minesToFlagg < 0)
            {
                Mines_Label.Text = minesToFlagg.ToString("00");
            }
            else
            {
                Mines_Label.Text = minesToFlagg.ToString("000");
            }
        }
        
    }
}
