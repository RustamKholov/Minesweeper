namespace Minesweeper.Domain.Logic.MineGeneration
{
    public sealed class SolveResult
    {
        public bool FullySolved { get; init; }
        public HashSet<(int Row, int Col)> UnresolvedFrontier { get; init; } = new();
        public HashSet<(int Row, int Col)> RevealedCells { get; init; } = new();
    }
}
