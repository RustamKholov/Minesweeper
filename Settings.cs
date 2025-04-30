using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public int Rows { get => _rows; set => _rows = value;  }
        public int Cols { get => _cols; set => _cols = value;  }
        public int Width { get => _width; set => _width = value;  }
        public int Height { get => _height; set => _height = value;  }
        public int Mines { get => _mines; set => _mines = value; }
        public int Difficulty { get => _difficulty; set => _difficulty = value; }
        public float CellSize { get => _cellSize; set => _cellSize = value; }
    }
}
