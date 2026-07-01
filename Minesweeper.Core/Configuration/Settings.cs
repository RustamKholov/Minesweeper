using Minesweeper.Application.Interfaces;

namespace Minesweeper.Infrastructure.Configuration
{
    public class Settings : IGameSettings
    {
        private int _rows = 9;
        private int _cols = 9;
        private int _mines = 10;
        private Difficulty _difficulty = Difficulty.Easy;
        public int Rows { get => _rows; set => _rows = value; }
        public int Cols { get => _cols; set => _cols = value; }
        public int Mines { get => _mines; set => _mines = value; }
        public Difficulty Difficulty { get => _difficulty; set => _difficulty = value; }
    }
}
