namespace Minesweeper.Domain.Entities
{
    public class Record
    {
        public int ID { get; set; }
        public int SecondsInGame { get; set; }
        public Difficulty Difficulty { get; set; }
        public GameStatus Status { get; set; }
        public int TilesUncovered { get; set; }
        public int ClicksPerformed { get; set; }
        public int FlaggsSet { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
