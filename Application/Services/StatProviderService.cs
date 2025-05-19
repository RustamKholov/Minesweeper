using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Defaults;
using Minesweeper.Application.DTO;
using Minesweeper.Domain.Entities;
using Minesweeper.Application.Interfaces;

namespace Minesweeper.Application.Services
{
    public class StatProviderService : IStatProviderService
    {
        private readonly IRecordService _recordService;
        public StatProviderService(IRecordService recordService)
        {
            _recordService = recordService;
        }
        public StatChartResult BuildChartData(List<Record> records, List<GameStatus> selectedStatuses)
        {

            double globalMaxX = double.MinValue, globalMaxY = double.MinValue;
            double globalMinX = double.MaxValue, globalMinY = double.MaxValue;

            var grouped = records
                .Where(r => r.secondsInGame > 0 && selectedStatuses.Contains(r.status))
                .OrderBy(r => r.secondsInGame)
                .GroupBy(r => r.difficulty);

            var result = new StatChartResult();

            foreach (var group in grouped)
            {
                var series = new ChartSeriesData
                {
                    SeriesTitle = group.Key.ToString()
                };

                var groupedPoints = group
                    .Select(r => new
                    {
                        X = Math.Round((double)r.tilesUncovered / r.secondsInGame, 2),
                        Y = (double)r.secondsInGame
                    })
                    .GroupBy(p => p.X);

                foreach (var xGroup in groupedPoints)
                {
                    double x = xGroup.Key;
                    double y = xGroup.Average(p => p.Y);
                    double weight = xGroup.Count();

                    globalMaxX = Math.Max(globalMaxX, x);
                    globalMaxY = Math.Max(globalMaxY, y);
                    globalMinX = Math.Min(globalMinX, x);
                    globalMinY = Math.Min(globalMinY, y);

                    series.Points.Add(new ScatterPoint(x, y, weight));
                }

                result.Series.Add(series);
            }

            result.MinX = globalMinX;
            result.MaxX = globalMinX + (globalMaxX - globalMinX) / 9 * 9;
            result.MinY = globalMinY;
            result.MaxY = globalMinY + (globalMaxY - globalMinY) / 9 * 9;

            return result;
        }
        

        public List<GameStatus> GetAllGameStatuses()
        {
             return _recordService.GetAllGameStatuses();
        }

        public List<Record> GetAllRecords()
        {
            return _recordService.GetAllRecords();
        }

        public List<Record> GetAllRecords(Difficulty difficulty)
        {
            return _recordService.GetAllRecords(difficulty);
        }
        public List<Record> GetAllRecords(List<Difficulty> selectedDifficulties)
        {
            var records = new List<Record>();
            foreach (var difficulty in selectedDifficulties)
            {
                records.AddRange(_recordService.GetAllRecords(difficulty));
            }
            return records;
        }
        public Record? GetBestRecord(Difficulty difficulty)
        {
            return _recordService.GetBestRecord(difficulty);
        }

        public Record? GetBestRecord()
        {
            return _recordService.GetBestRecord();
        }

        public Dictionary<GameStatus, int> GetGameStatusCount()
        {
            var records = _recordService.GetAllRecords();
            var grouped = records.GroupBy(r => r.status);
            var result = new Dictionary<GameStatus, int>();
            foreach (var group in grouped)
            {
                result[group.Key] = group.Count();
            }
            return result;
        }

        public Dictionary<GameStatus, int> GetGameStatusCount(List<Difficulty> selectedDifficulties)
        {
            var records = new List<Record>();
            foreach (var difficulty in selectedDifficulties)
            {
                records.AddRange(_recordService.GetAllRecords(difficulty));
            }
            var grouped = records.GroupBy(r => r.status);
            var result = new Dictionary<GameStatus, int>();
            foreach (var group in grouped)
            {
                result[group.Key] = group.Count();
            }
            return result;
        }
        public StatTotalResult GetStatTotalResult(List<Record> records)
        {
            return new StatTotalResult(records);
        }
    }
}
