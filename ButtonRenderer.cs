using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Interfaces;
using Minesweeper.Models;

namespace Minesweeper
{
    public class ButtonRenderer
    {
        private IGameSettings _gameSettings;    
        private readonly TableLayoutPanel _tableLayoutPanel;
        public ButtonRenderer(TableLayoutPanel tableLayoutPanel, IGameSettings gameSettings)
        {
            _tableLayoutPanel = tableLayoutPanel;
            _gameSettings = gameSettings;
        }
        public Button CreateCustomTile()
        {
            Button button = new Button();
            button.Dock = DockStyle.Fill;
            Flat(button);
            Unopened(button);
            button.Margin = new Padding(0);
            button.Font = _gameSettings.Font;
            button.UseVisualStyleBackColor = false;
            return button;
        }
        public void Flat(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
        }
        public void Unopened(Button button)
        {
            Flat(button);
            button.BackgroundImage = Properties.Resources.Minesweeper_unopened_square;
            button.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public void Opened(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.BorderColor = Color.Gray;
            button.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
            button.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public void OpenedMine(Button button)
        {
            button.BackgroundImage = Properties.Resources.Minesweeper_opened_square_mine;
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.Image = Properties.Resources.Mine;
            button.ImageAlign = ContentAlignment.MiddleCenter;
        }
        public void OpenedNotMine(Button button, int adjacentMines)
        {
            button.ForeColor = adjacentMines switch
            {
                1 => Color.Blue,
                2 => Color.Green,
                3 => Color.Red,
                4 => Color.DarkBlue,
                5 => Color.DarkRed,
                6 => Color.Cyan,
                7 => Color.Black,
                _ => button.ForeColor
            };
            button.Text = adjacentMines > 0 ? adjacentMines.ToString() : "";
        }
        public void Flag(Button button)
        {
            button.BackgroundImage = Properties.Resources.Minesweeper_flag;
            button.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public void FlagedWrong(Button button)
        {
            button.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.Image = Properties.Resources.Wrong_Mine;
        }
        public void Mine(Button button)
        {
            button.BackgroundImage = Properties.Resources.Minesweeper_opened_square;
            button.Image = Properties.Resources.Mine;
            button.ImageAlign = ContentAlignment.MiddleCenter;
        }
        public void MineOpened(Button button)
        {
            button.BackgroundImage = Properties.Resources.Minesweeper_opened_square_mine;
            button.Image = Properties.Resources.Mine;
            button.ImageAlign = ContentAlignment.MiddleCenter;
        }
        public void Reset(Button button)
        {
            button.BackgroundImage = Properties.Resources.Minesweeper_unopened_square;
            button.Text = "";
            button.Image = null;
        }
        public void SmilePressed(Button smile, Image previousSmile)
        {
            previousSmile = smile.BackgroundImage ?? previousSmile;
            smile.BackgroundImage = Properties.Resources.Smile_pressed;
            smile.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public void SmileReleased(Button smile, Image previousSmile)
        {
            smile.BackgroundImage = previousSmile;
        }
        public void SmileScared(Button smile)
        {
            smile.BackgroundImage = Properties.Resources.Scared;
            smile.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public void SmileDefault(Button smile)
        {
            smile.BackgroundImage = Properties.Resources.Smile;
            smile.BackgroundImageLayout = ImageLayout.Stretch;
        }
        public void SmileFail(Button smile)
        {
            smile.BackgroundImage = Properties.Resources.Fail;
        }
        public void SmileSuccess(Button smile)
        {
            smile.BackgroundImage = Properties.Resources.Success;
        }
    }
}
