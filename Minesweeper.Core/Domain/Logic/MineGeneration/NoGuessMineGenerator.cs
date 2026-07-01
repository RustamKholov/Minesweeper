using Minesweeper.Application.Interfaces;

namespace Minesweeper.Domain.Logic.MineGeneration
{
    public sealed class NoGuessMineGenerator : IMineGenerator
    {
        private const int MaxRepairsPerAttempt = 60;
        private const int MaxAttempts = 500;

        private readonly NoGuessSolver _solver = new();
        private readonly Random _random;

        public NoGuessMineGenerator() : this(new Random()) { }

        public NoGuessMineGenerator(Random random)
        {
            _random = random;
        }

        public HashSet<(int Row, int Col)> GenerateMinePositions(int rows, int cols, int mines, int safeRow, int safeCol)
        {
            var bestAttempt = RandomPlacement(rows, cols, mines, safeRow, safeCol);
            int bestUnresolvedCount = int.MaxValue;

            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                var placement = attempt == 0 ? bestAttempt : RandomPlacement(rows, cols, mines, safeRow, safeCol);

                for (int repair = 0; repair < MaxRepairsPerAttempt; repair++)
                {
                    SolveResult result = _solver.Solve(placement, rows, cols, mines, safeRow, safeCol);

                    if (result.FullySolved)
                    {
                        return placement;
                    }

                    if (result.UnresolvedFrontier.Count < bestUnresolvedCount)
                    {
                        bestUnresolvedCount = result.UnresolvedFrontier.Count;
                        bestAttempt = new HashSet<(int Row, int Col)>(placement);
                    }

                    if (!TryRepair(placement, result, rows, cols, safeRow, safeCol))
                    {
                        break;
                    }
                }
            }

            return bestAttempt;
        }

        private HashSet<(int Row, int Col)> RandomPlacement(int rows, int cols, int mines, int safeRow, int safeCol)
        {
            var placement = new HashSet<(int Row, int Col)>();
            while (placement.Count < mines)
            {
                int row = _random.Next(rows);
                int col = _random.Next(cols);
                bool isNearSafeClick = Math.Abs(row - safeRow) <= 1 && Math.Abs(col - safeCol) <= 1;
                if (!isNearSafeClick)
                {
                    placement.Add((row, col));
                }
            }
            return placement;
        }

        // Swaps one ambiguous cell's mine status with a cell elsewhere on the board that (a) isn't
        // already revealed, so no clue the solver already trusts changes, and (b) has the opposite
        // mine status, so the total mine count never changes. This perturbs just the ambiguous local
        // pattern instead of discarding everything the solver already proved.
        private bool TryRepair(HashSet<(int Row, int Col)> placement, SolveResult result, int rows, int cols, int safeRow, int safeCol)
        {
            var target = result.UnresolvedFrontier.ElementAt(_random.Next(result.UnresolvedFrontier.Count));
            bool targetIsMine = placement.Contains(target);

            var candidates = new List<(int Row, int Col)>();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    var cell = (row, col);
                    if (cell == target) continue;
                    if (result.RevealedCells.Contains(cell)) continue;
                    if (placement.Contains(cell) == targetIsMine) continue;
                    candidates.Add(cell);
                }
            }

            if (candidates.Count == 0)
            {
                return false;
            }

            var partner = candidates[_random.Next(candidates.Count)];

            if (targetIsMine)
            {
                placement.Remove(target);
                placement.Add(partner);
            }
            else
            {
                placement.Remove(partner);
                placement.Add(target);
            }

            return true;
        }
    }
}
