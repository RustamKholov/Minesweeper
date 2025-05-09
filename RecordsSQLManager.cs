using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Minesweeper
{
    public class RecordsSQLManager
    {
        public List<Record> RecordsList { get; private set; } = new List<Record>();
        private readonly string _connectionString = "Data Source=Data/Minesweeper.db;Version=3;";

        public void SaveRecord(Record record)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string query = "INSERT INTO Records (Game_Seconds, Difficulty, Status, Tiles_Uncovered, Clicks_Performed, Flaggs_Set, TimeStamp) VALUES (@seconds, @difficulty, @status, @tilesUncovered, @clicksPerformed, @flaggsSet, @time_stamp)";
            using var cmd = new SQLiteCommand(query, conn);
            cmd.Parameters.AddWithValue("@seconds", record.secondsInGame);
            cmd.Parameters.AddWithValue(@"difficulty", record.difficulty.ToString());
            cmd.Parameters.AddWithValue(@"status", record.status.ToString());
            cmd.Parameters.AddWithValue("@tilesUncovered", record.tilesUncovered);
            cmd.Parameters.AddWithValue("@clicksPerformed", record.clicksPerformed);
            cmd.Parameters.AddWithValue("@flaggsSet", record.flaggsSet);
            cmd.Parameters.AddWithValue("@time_stamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (cmd.ExecuteNonQuery() != 1)
            {
                throw new Exception("Failed to save record to database.");
            }
        }
        public List<Record> GetRecords()
        {
            List<Record> records = new List<Record>();
            RecordsList.Clear();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string query = "SELECT * FROM Records";
            using var cmd = new SQLiteCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var record = new Record
                {
                    ID = reader.GetInt32(0),
                    secondsInGame = reader.GetInt32(1),
                    difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), reader.GetString(2)),
                    status = (GameStatus)Enum.Parse(typeof(GameStatus), reader.GetString(3)),
                    tilesUncovered = reader.GetInt32(4),
                    clicksPerformed = reader.GetInt32(5),
                    flaggsSet = reader.GetInt32(6),
                };
                reader.GetString(7); // TimeStamp
                records.Add(record);
            }
            return records;
        }
        private void LoadRecords()
        {
            RecordsList.Clear();
            RecordsList = GetRecords();
        }
    }
}
