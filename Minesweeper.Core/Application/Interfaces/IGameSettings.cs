namespace Minesweeper.Application.Interfaces
{
    public interface IGameSettings
    {
        int Cols { get; set; }
        Difficulty Difficulty { get; set; }
        int Mines { get; set; }
        int Rows { get; set; }
    }
}