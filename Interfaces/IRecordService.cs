using Minesweeper.Models;

namespace Minesweeper.Interfaces
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
