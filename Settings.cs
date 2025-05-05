using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace Minesweeper
{
    public class Settings
    {
        private int _rows = 9;
        private int _cols = 9;
        private int _mines = 10;
        private  int _difficulty;
        private  int _width;
        private  int _height;
        private float _cellSize = 40f;
        private Font _font;
        public int Rows { get => _rows; set => _rows = value;  }
        public int Cols { get => _cols; set => _cols = value;  }
        public int Width { get => _width; set => _width = value;  }
        public int Height { get => _height; set => _height = value;  }
        public int Mines { get => _mines; set => _mines = value; }
        public int Difficulty { get => _difficulty; set => _difficulty = value; }
        public float CellSize { get => _cellSize; set => _cellSize = value; }
        public Font Font { get => _font; set => _font = value; }
        public Settings()
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            string fontPath = Path.Combine(Application.StartupPath,"Assets", "Fonts",  "mine-sweeper.ttf");
            pfc.AddFontFile(fontPath);
            FontFamily fontFamily = pfc.Families[0];
            _font = new Font(fontFamily, 12);
        }
    }
}
