using Minesweeper.Application.Interfaces;

namespace Minesweeper.Tests
{
    // Test-only IMineGenerator that always returns a caller-supplied layout, so tests can pin
    // down exact mine positions instead of going through random/no-guess generation.
    public class FixedMineGenerator : IMineGenerator
    {
        private readonly HashSet<(int Row, int Col)> _mines;

        public FixedMineGenerator(HashSet<(int Row, int Col)> mines)
        {
            _mines = mines;
        }

        public HashSet<(int Row, int Col)> GenerateMinePositions(int rows, int cols, int mines, int safeRow, int safeCol)
        {
            return _mines;
        }
    }
}
