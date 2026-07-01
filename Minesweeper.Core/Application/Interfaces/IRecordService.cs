using Minesweeper.Domain.Entities;

namespace Minesweeper.Application.Interfaces
{
    public interface IRecordService
    {
        void SaveRecord(Record record);
        void DeleteRecord(Record record);
        List<Record> GetAllRecords(Difficulty? difficulty = null);
        List<GameStatus> GetAllGameStatuses(Difficulty? difficulty = null);
        Record? GetBestRecord(Difficulty? difficulty = null);

    }
}
