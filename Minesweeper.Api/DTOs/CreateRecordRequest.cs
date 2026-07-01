namespace Minesweeper.Api.DTOs
{
    public record CreateRecordRequest(
        int SecondsInGame,
        Difficulty Difficulty,
        GameStatus Status,
        int TilesUncovered,
        int ClicksPerformed,
        int FlagsSet
    );
}
