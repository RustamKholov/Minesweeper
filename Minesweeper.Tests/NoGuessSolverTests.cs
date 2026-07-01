using Minesweeper.Domain.Logic.MineGeneration;

namespace Minesweeper.Tests
{
    public class NoGuessSolverTests
    {
        // Row0 is fully open and cascades into row1. Row1's clue (1) sees exactly as many
        // unrevealed neighbors as its count, so single-point deduction alone marks both
        // row2 mines directly - no subset or global reasoning needed.
        [Fact]
        public void SinglePointDeduction_ResolvesBoard()
        {
            var mines = new HashSet<(int Row, int Col)> { (2, 0), (2, 1) };
            var result = new NoGuessSolver().Solve(mines, rows: 3, cols: 3, totalMines: 2, safeRow: 0, safeCol: 1);

            Assert.True(result.FullySolved);
            var expectedRevealed = new HashSet<(int Row, int Col)>
            {
                (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 2),
            };
            Assert.Equal(expectedRevealed, result.RevealedCells);
        }

        // The classic 1-2-1: {1}={(2,0),(2,1)}, {2}={(2,0),(2,1),(2,2)}, {1}={(2,1),(2,2)}.
        // No single clue's own count matches its unrevealed-neighbor count, so single-point
        // alone deduces nothing here - only subset elimination against the middle "2" clue
        // (subtracting the "1" clues from it) can prove the two ends are mines and the
        // center is safe.
        [Fact]
        public void SubsetElimination_Solves121Pattern()
        {
            var mines = new HashSet<(int Row, int Col)> { (2, 0), (2, 2) };
            var result = new NoGuessSolver().Solve(mines, rows: 3, cols: 3, totalMines: 2, safeRow: 0, safeCol: 1);

            Assert.True(result.FullySolved);
            var expectedRevealed = new HashSet<(int Row, int Col)>
            {
                (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (2, 1),
            };
            Assert.Equal(expectedRevealed, result.RevealedCells);
        }

        // "1-1-1-1" over a 4-wide unknown row with mines only at the two ends. The first
        // pass can only prove the two middle cells safe (each is the subset-diff between a
        // "1" and its size-3 neighbor). Only once those reveals shrink the end clues' own
        // constraints does a second pass resolve the end cells as mines - this exercises the
        // outer fixed-point loop re-running after progress, not just a single pass.
        [Fact]
        public void ChainedSubsetElimination_ResolvesAcrossMultiplePasses()
        {
            var mines = new HashSet<(int Row, int Col)> { (2, 0), (2, 3) };
            var result = new NoGuessSolver().Solve(mines, rows: 3, cols: 4, totalMines: 2, safeRow: 0, safeCol: 0);

            Assert.True(result.FullySolved);
            var expectedRevealed = new HashSet<(int Row, int Col)>
            {
                (0, 0), (0, 1), (0, 2), (0, 3),
                (1, 0), (1, 1), (1, 2), (1, 3),
                (2, 1), (2, 2),
            };
            Assert.Equal(expectedRevealed, result.RevealedCells);
        }

        // Row2 is a solid wall of mines; row1's clues pin all four down via single-point
        // (each row1 cell's unrevealed-neighbor count exactly matches its number). That
        // exhausts the mine budget, so rows 3-4 - which touch no revealed clue at all - are
        // only provably safe because the remaining mine count has hit zero. No local number
        // ever points at them directly.
        [Fact]
        public void GlobalMineCount_ResolvesCellsWithNoAdjacentClue()
        {
            var mines = new HashSet<(int Row, int Col)> { (2, 0), (2, 1), (2, 2), (2, 3) };
            var result = new NoGuessSolver().Solve(mines, rows: 5, cols: 4, totalMines: 4, safeRow: 0, safeCol: 0);

            Assert.True(result.FullySolved);
            for (int col = 0; col < 4; col++)
            {
                Assert.Contains((3, col), result.RevealedCells);
                Assert.Contains((4, col), result.RevealedCells);
            }
        }

        // Both row1 clues reduce to the exact same constraint {(2,0),(2,1)} = 1 mine, with
        // nothing else distinguishing the two cells - a genuine 50/50. The solver must
        // recognize it cannot decide either cell rather than guessing one.
        [Fact]
        public void GenuineAmbiguity_IsReportedUnresolved_NotGuessed()
        {
            var mines = new HashSet<(int Row, int Col)> { (2, 0) };
            var result = new NoGuessSolver().Solve(mines, rows: 3, cols: 2, totalMines: 1, safeRow: 0, safeCol: 0);

            Assert.False(result.FullySolved);
            Assert.Contains((2, 1), result.UnresolvedFrontier);
            Assert.DoesNotContain((2, 1), result.RevealedCells);
        }

        // A single corner mine on a 3x3 grid: pure flood-fill from the opposite corner already
        // reveals every non-mine cell, leaving no frontier for any deduction pass to act on.
        // That makes this the rare case where the solver's full output must exactly equal plain
        // cascade output, so comparing it against the real GameEngine's cascade-only reveal is a
        // tight cross-check of SimulateReveal's port of RevealCell's cascade - any discrepancy in
        // either direction (over- or under-revealing) would break the equality.
        [Fact]
        public void SimulatedCascade_MatchesRealGameEngineCascade()
        {
            var mines = new HashSet<(int Row, int Col)> { (0, 0) };
            var engine = new Domain.Logic.GameEngine(rows: 3, cols: 3, mines: 1, new FixedMineGenerator(mines));
            engine.RevealCell(2, 2);

            var engineRevealed = new HashSet<(int Row, int Col)>();
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (engine.GetCell(row, col).IsRevealed)
                    {
                        engineRevealed.Add((row, col));
                    }
                }
            }

            var solverResult = new NoGuessSolver().Solve(mines, rows: 3, cols: 3, totalMines: 1, safeRow: 2, safeCol: 2);

            Assert.True(solverResult.FullySolved);
            Assert.Equal(engineRevealed, solverResult.RevealedCells);
        }
    }
}
