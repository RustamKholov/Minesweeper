using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public class Record
    {
        public int ID { get; set; }
        public int secondsInGame { get; set; }
        public Difficulty difficulty { get; set; }
        public GameStatus status { get; set; }
        public int tilesUncovered { get; set; }
        public int clicksPerformed { get; set; }
        public int flaggsSet { get; set; }
        public DateTime? timeStamp { get; set; }
    }
}
