using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Domain.Entities;
using Minesweeper.Application.Interfaces;

namespace Minesweeper.Application.DTO
{
    public class StatTotalResult : IStatTotalResult
    {
        public Dictionary<GameStatus, List<(int time, int tiles)>> grouper;
        public int totalTime => grouper.Values.SelectMany(x => x).Sum(x => x.time);
        public int totalTiles => grouper.Values.SelectMany(x => x).Sum(x => x.tiles);
        public StatTotalResult(List<Record> records)
        {
            grouper = new Dictionary<GameStatus, List<(int time, int tiles)>>();
            foreach (var record in records)
            {
                if (!grouper.ContainsKey(record.status))
                {
                    grouper[record.status] = new List<(int time, int tiles)>();
                }
                grouper[record.status].Add((record.secondsInGame, record.tilesUncovered));
            }

        }

    }
}
