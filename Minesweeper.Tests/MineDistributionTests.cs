using Minesweeper.Domain.Logic.MineGeneration;

namespace Minesweeper.Tests
{
    // Regression guard for a real bug: SolveResult.UnresolvedFrontier used to be computed from
    // ground-truth IsMine instead of the solver's own deduced-mine set, which made repair swaps
    // one-directional and systematically funneled mines toward whatever region was hardest to
    // resolve (usually a corner/edge). A uniform-random 20.6%-density board has an expected
    // ~1.65 mine-neighbors-per-mine; this checks the generator's output stays close to that
    // instead of drifting toward dense pockets.
    public class MineDistributionTests
    {
        [Fact]
        public void HardDifficultyBoards_DoNotSystematicallyClusterMines()
        {
            var generator = new NoGuessMineGenerator(new Random(1));
            var clickPicker = new Random(2);
            int rows = 16, cols = 30, mines = 99;

            var perBoardAverages = new List<double>();

            for (int trial = 0; trial < 25; trial++)
            {
                int safeRow = clickPicker.Next(rows);
                int safeCol = clickPicker.Next(cols);
                var placement = generator.GenerateMinePositions(rows, cols, mines, safeRow, safeCol);

                var neighborMineCounts = placement.Select(mine =>
                {
                    int count = 0;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            if (dr == 0 && dc == 0) continue;
                            if (placement.Contains((mine.Row + dr, mine.Col + dc))) count++;
                        }
                    return count;
                });

                perBoardAverages.Add(neighborMineCounts.Average());
            }

            Assert.True(perBoardAverages.Average() < 2.0,
                $"Mean mine-neighbor-count across boards was {perBoardAverages.Average():F2}, expected close to the ~1.65 uniform-random baseline.");
        }
    }
}
