using Minesweeper.Web.Models;

namespace Minesweeper.Web.Services
{
    public record RecordsResult(
        Dictionary<Difficulty, int?> BestSecondsByDifficulty,
        Dictionary<Difficulty, int> PlayCountByDifficulty,
        List<GameRecordDto> Recent);

    public static class RecordsCalculator
    {
        // Mirrors the design prototype's computeRecords(history).
        public static RecordsResult Compute(List<GameRecordDto> history)
        {
            var ordered = history.OrderByDescending(h => h.TimeStamp ?? DateTime.MinValue).ToList();

            var best = new Dictionary<Difficulty, int?>();
            var counts = new Dictionary<Difficulty, int>();
            foreach (Difficulty difficulty in Enum.GetValues<Difficulty>())
            {
                var difficultyWins = ordered.Where(h => h.Difficulty == difficulty && h.Status == GameStatus.Win).ToList();
                best[difficulty] = difficultyWins.Count > 0 ? difficultyWins.Min(h => h.SecondsInGame) : null;
                counts[difficulty] = ordered.Count(h => h.Difficulty == difficulty);
            }

            var recent = ordered.Take(10).ToList();
            return new RecordsResult(best, counts, recent);
        }
    }
}
