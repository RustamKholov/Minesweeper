using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Application.DTO;
using Minesweeper.Domain.Entities;

namespace Minesweeper.Interfaces
{
    public interface IStatProviderService
    {
        List<GameStatus> GetAllGameStatuses();
        Dictionary<GameStatus, int> GetGameStatusCount();
        Dictionary<GameStatus, int> GetGameStatusCount(List<Difficulty> selectedDifficulties);
        StatTotalResult GetStatTotalResult(List<Record> records);
        Record? GetBestRecord(Difficulty difficulty);
        Record? GetBestRecord();
        List<Record> GetAllRecords();
        List<Record> GetAllRecords(Difficulty difficulty);
        List<Record> GetAllRecords(List<Difficulty> selectedDifficulties);
        StatChartResult BuildChartData(List<Record> records, List<GameStatus> selectedStatuses);
    }
}
