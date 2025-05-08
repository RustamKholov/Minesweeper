using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{

    public class DataBase
    {
        private int _nextId = 1;
        private readonly string _titles = "ID,Seconds,Difficulty,Status,Tiles_Uncovered,Clicks_Performed,Flaggs_Set";
        private string _csvPath;
        private List<Record> _recordsList = new List<Record>();
        public List<Record> RecordsList => _recordsList;

        public bool Edited { get; private set; } = false;

        public DataBase(string path = "recordsDB.csv")
        {
            _csvPath = path;
            InitializeDataBase();
        }

        public void AddRecord(Record record)
        {
            record.ID = _nextId++;
            _recordsList.Add(record);
            Edited = true;
        }

        public void DeleteUser(Record record)
        {
            _recordsList.Remove(record);
            Edited = true;
        }
        public List<Record> GetRecordsList()
        {
            return _recordsList;
        }
        private void InitializeDataBase()
        {
            if (File.Exists(_csvPath))
                LoadFromCSV();
            else
                SaveToCSV();
        }

        public void LoadFromCSV()
        {
            _recordsList.Clear();
            try
            {
                using var reader = new StreamReader(_csvPath);
                string[] headers = reader.ReadLine()?.Split(',') ?? throw new InvalidDataException("CSV missing header.");
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var record = ParseRecord(line);
                    AddRecord(record);
                }
                _nextId = _recordsList.Count > 0 ? _recordsList.Max(r => r.ID) + 1 : 1;
                Edited = false;
            }
            catch (Exception e)
            {
                throw new Exception("Error loading from CSV: " + e.Message, e);
            }
        }

        private Record ParseRecord(string line)
        {
            var parts = line.Split(',');
            if (parts.Length != 7)
                throw new FormatException($"Invalid line: {line}");
            Record record = new Record()
            {
                ID = int.Parse(parts[0]),
                secondsInGame = int.Parse(parts[1]),
                difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), parts[2]),
                status = (GameStatus)Enum.Parse(typeof(GameStatus), parts[3]),
                tilesUncovered = int.Parse(parts[4]),
                clicksPerformed = int.Parse(parts[5]),
                flaggsSet = int.Parse(parts[6])
            };
            
            return record;
        }

        public bool SaveToCSV()
        {
            try
            {
                using var sw = new StreamWriter(_csvPath);
                sw.WriteLine(_titles);
                foreach (var record in _recordsList)
                {
                    sw.WriteLine($"{record.ID},{record.secondsInGame},{record.difficulty},{record.status},{record.tilesUncovered},{record.clicksPerformed},{record.flaggsSet}");
                }
                Edited = false;
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error saving to CSV: " + e.Message, e);
            }
        }
    }
}
