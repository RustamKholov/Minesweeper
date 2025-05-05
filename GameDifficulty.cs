using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class GameDifficulty
    {
        public static Settings Easy => new Settings { Rows = 9, Cols = 9, Mines = 10 };
        public static Settings Medium => new Settings { Rows = 16, Cols = 16, Mines = 40};
        public static Settings Hard => new Settings { Rows = 16, Cols = 30, Mines = 99 };
    }
}
