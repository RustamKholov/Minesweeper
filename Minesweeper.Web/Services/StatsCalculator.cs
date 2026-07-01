using Minesweeper.Web.Models;

namespace Minesweeper.Web.Services
{
    public record StatsResult(
        int Total,
        int Wins,
        int Losses,
        int WinRate,
        int Streak,
        Dictionary<Difficulty, int?> AverageSecondsByDifficulty);

    public static class StatsCalculator
    {
        // Mirrors the design prototype's computeStats(history) - history there is ordered
        // most-recent-first (new entries are prepended), so we sort explicitly here since the
        // API has no ordering guarantee.
        public static StatsResult Compute(List<GameRecordDto> history)
        {
            var ordered = history.OrderByDescending(h => h.TimeStamp ?? DateTime.MinValue).ToList();

            int total = ordered.Count;
            int wins = ordered.Count(h => h.Status == GameStatus.Win);
            int losses = total - wins;
            int winRate = total > 0 ? (int)Math.Round(wins * 100.0 / total) : 0;

            int streak = 0;
            foreach (var h in ordered)
            {
                if (h.Status == GameStatus.Win) streak++;
                else break;
            }

            var average = new Dictionary<Difficulty, int?>();
            foreach (Difficulty difficulty in Enum.GetValues<Difficulty>())
            {
                var difficultyWins = ordered.Where(h => h.Difficulty == difficulty && h.Status == GameStatus.Win).ToList();
                average[difficulty] = difficultyWins.Count > 0
                    ? (int)Math.Round(difficultyWins.Average(h => h.SecondsInGame))
                    : null;
            }

            return new StatsResult(total, wins, losses, winRate, streak, average);
        }
    }
}
