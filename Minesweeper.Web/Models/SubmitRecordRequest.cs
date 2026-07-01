namespace Minesweeper.Web.Models
{
    public record SubmitRecordRequest(
        int SecondsInGame,
        Difficulty Difficulty,
        GameStatus Status,
        int TilesUncovered,
        int ClicksPerformed,
        int FlagsSet
    );
}
