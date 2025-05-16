namespace Minesweeper.Interfaces
{
    public interface IGameSettings
    {
        float CellSize { get; set; }
        int Cols { get; set; }
        Difficulty Difficulty { get; set; }
        Font Font { get; set; }
        int Mines { get; set; }
        int Rows { get; set; }
    }
}