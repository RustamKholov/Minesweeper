using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper.Application.DTO
{
    public class EngineRecords
    {
        public int SecondsInGame { get; set; }
        public GameStatus GameStatus { get; set; }
        public int TilesUncovered { get; set; }
        public int ClicksPerformed { get; set; }
        public int FlaggsSet { get; set; }
    }
}
