namespace Minesweeper.Web.Models
{
    public record GameRecordDto(
        int ID,
        int SecondsInGame,
        Difficulty Difficulty,
        GameStatus Status,
        int TilesUncovered,
        int ClicksPerformed,
        int FlaggsSet,
        DateTime? TimeStamp
    );
}
