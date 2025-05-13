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
        public List<Record> GetAllRecords(Difficulty? difficulty = null)
        {
            List<Record> records = new List<Record>();
            RecordsList.Clear();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string query;
            using var cmd = conn.CreateCommand();
            if(difficulty != null)
            {
                query = "SELECT * FROM Records WHERE Difficulty = @difficulty";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@difficulty", difficulty.ToString());
            }
            else
            {
                query = "SELECT * FROM Records";
                cmd.CommandText = query;
            }
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
                    timeStamp = DateTime.TryParse(reader.GetString(7), out DateTime timeStamp) ? timeStamp : null
                };
                records.Add(record);
            }
            return records;
        }
        private void LoadRecords()
        {
            RecordsList.Clear();
            RecordsList = GetAllRecords();
        }
        public void DeleteRecord(Record record)
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string query = "DELETE FROM Records WHERE ID = @id";
            using var cmd = new SQLiteCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", record.ID);
            if (cmd.ExecuteNonQuery() != 1)
            {
                throw new Exception("Failed to delete record from database.");
            }
        }
        public List<GameStatus> GetAllStatuses(Difficulty? difficulty = null) {
            List<GameStatus> statuses = new List<GameStatus>();
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            string query;
            using var cmd = conn.CreateCommand();
            if (difficulty != null)
            {
                query = "SELECT Status FROM Records WHERE Difficulty = @difficulty";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@difficulty", difficulty.ToString());
            }
            else
            {
                query = "SELECT Status FROM Records";
                cmd.CommandText = query;
            }            
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var status = (GameStatus)Enum.Parse(typeof(GameStatus), reader.GetString(0));
                statuses.Add(status);
            }
            return statuses;
        }
        public Record? GetBestRecord(Difficulty? difficulty = null) //if difficulty is null, get the best record from all difficulties
        {
            using var conn = new SQLiteConnection(_connectionString);
            conn.Open();
            
            string query;
            using var cmd = conn.CreateCommand();
            if (difficulty != null)
            {
                query = "SELECT * FROM Records" +
                    " WHERE Difficulty = @difficulty AND Status = 'Win'" +
                    " ORDER BY Game_Seconds ASC" +
                    ",Clicks_Performed ASC" +
                    ",Flaggs_Set ASC" +
                    ",Tiles_Uncovered ASC" +
                    ",TimeStamp ASC" +
                    " LIMIT 1";
                cmd.CommandText = query;
                cmd.Parameters.AddWithValue("@difficulty", difficulty.ToString());
            }
            else
            {
                query = "SELECT * FROM Records" +
                    " WHERE Status = 'Win'" +
                    " ORDER BY Game_Seconds ASC" +
                    ",Clicks_Performed ASC" +
                    ",Flaggs_Set ASC" +
                    ",Tiles_Uncovered ASC" +
                    ",TimeStamp ASC" +
                    " LIMIT 1 ";
                cmd.CommandText = query;
            }
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Record
                {
                    ID = reader.GetInt32(0),
                    secondsInGame = reader.GetInt32(1),
                    difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), reader.GetString(2)),
                    status = (GameStatus)Enum.Parse(typeof(GameStatus), reader.GetString(3)),
                    tilesUncovered = reader.GetInt32(4),
                    clicksPerformed = reader.GetInt32(5),
                    flaggsSet = reader.GetInt32(6),
                    timeStamp = DateTime.TryParse(reader.GetString(7), out DateTime timeStamp) ? timeStamp : null
                };
            }
            return null;
        }
    }
}
