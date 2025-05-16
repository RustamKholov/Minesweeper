using System.Drawing.Text;
using System.IO;
using Minesweeper.Interfaces;

namespace Minesweeper.Domain.Logic
{
    public class Settings : IGameSettings
    {
        private int _rows = 9;
        private int _cols = 9;
        private int _mines = 10;
        private Difficulty _difficulty = Difficulty.Easy;
        private float _cellSize = 35f;
        private Font _font;
        public int Rows { get => _rows; set => _rows = value; }
        public int Cols { get => _cols; set => _cols = value; }
        public int Mines { get => _mines; set => _mines = value; }
        public Difficulty Difficulty { get => _difficulty; set => _difficulty = value; }
        public float CellSize { get => _cellSize; set => _cellSize = value; }
        public Font Font { get => _font; set => _font = value; }
        public Settings()
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            string fontPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Assets", "Fonts", "mine-sweeper.ttf");
            pfc.AddFontFile(fontPath);
            FontFamily fontFamily = pfc.Families[0];
            //_font = new Font(fontFamily, 11);
            _font = new Font("Sagoe", 13, FontStyle.Bold);
        }
    }
}
