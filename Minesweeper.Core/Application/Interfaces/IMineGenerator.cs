namespace Minesweeper.Application.Interfaces
{
    public interface IMineGenerator
    {
        HashSet<(int Row, int Col)> GenerateMinePositions(int rows, int cols, int mines, int safeRow, int safeCol);
    }
}
