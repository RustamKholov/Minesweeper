using System.Diagnostics;
using Minesweeper.Domain.Logic.MineGeneration;

namespace Minesweeper.Tests
{
    public class NoGuessMineGeneratorTests
    {
        private const int EasyRows = 9, EasyCols = 9, EasyMines = 10;
        private const int MediumRows = 16, MediumCols = 16, MediumMines = 40;
        private const int HardRows = 16, HardCols = 30, HardMines = 99;

        [Fact]
        public void GenerateMinePositions_ReturnsExactlyRequestedMineCount()
        {
            var generator = new NoGuessMineGenerator(new Random(1));
            var placement = generator.GenerateMinePositions(EasyRows, EasyCols, EasyMines, safeRow: 4, safeCol: 4);

            Assert.Equal(EasyMines, placement.Count);
        }

        [Fact]
        public void GenerateMinePositions_NeverPlacesAMineInTheSafeZone()
        {
            var generator = new NoGuessMineGenerator(new Random(2));
            int safeRow = 4, safeCol = 4;
            var placement = generator.GenerateMinePositions(EasyRows, EasyCols, EasyMines, safeRow, safeCol);

            foreach (var (row, col) in placement)
            {
                bool inSafeZone = Math.Abs(row - safeRow) <= 1 && Math.Abs(col - safeCol) <= 1;
                Assert.False(inSafeZone, $"({row},{col}) is inside the safe zone around ({safeRow},{safeCol})");
            }
        }

        [Fact]
        public void GenerateMinePositions_AllPositionsAreInBounds()
        {
            var generator = new NoGuessMineGenerator(new Random(3));
            var placement = generator.GenerateMinePositions(EasyRows, EasyCols, EasyMines, safeRow: 0, safeCol: 0);

            foreach (var (row, col) in placement)
            {
                Assert.InRange(row, 0, EasyRows - 1);
                Assert.InRange(col, 0, EasyCols - 1);
            }
        }

        // The real end-to-end guarantee: every board the generator hands back must be provably
        // solvable by a *fresh* solver instance, not just the one the generator used internally.
        [Theory]
        [InlineData(EasyRows, EasyCols, EasyMines, 15)]
        [InlineData(MediumRows, MediumCols, MediumMines, 10)]
        [InlineData(HardRows, HardCols, HardMines, 5)]
        public void GeneratedBoards_AreAlwaysFullySolvableByAnIndependentSolver(int rows, int cols, int mines, int iterations)
        {
            var generator = new NoGuessMineGenerator(new Random(12345));
            var random = new Random(67890);

            for (int i = 0; i < iterations; i++)
            {
                int safeRow = random.Next(rows);
                int safeCol = random.Next(cols);

                var placement = generator.GenerateMinePositions(rows, cols, mines, safeRow, safeCol);
                Assert.Equal(mines, placement.Count);

                var verification = new NoGuessSolver().Solve(placement, rows, cols, mines, safeRow, safeCol);
                Assert.True(verification.FullySolved,
                    $"Board from iteration {i} (safe click {safeRow},{safeCol}) was not fully solvable.");
            }
        }

        // Regression guard: if the repair/backtracking caps ever stop being enforced, Hard-difficulty
        // generation should fail loudly here (as a slow test) rather than silently hang in the app.
        [Fact]
        public void GenerateMinePositions_OnHardDifficulty_CompletesWithinAGenerousTimeBound()
        {
            var generator = new NoGuessMineGenerator(new Random(999));
            var stopwatch = Stopwatch.StartNew();

            generator.GenerateMinePositions(HardRows, HardCols, HardMines, safeRow: 8, safeCol: 15);

            stopwatch.Stop();
            Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(5),
                $"Hard-difficulty generation took {stopwatch.Elapsed}, expected under 5 seconds.");
        }
    }
}
