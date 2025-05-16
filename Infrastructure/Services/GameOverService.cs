using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minesweeper.Application.DTO;
using Minesweeper.Application.Interfaces;
using Minesweeper.Domain.Entities;

namespace Minesweeper.Infrastructure.Services
{
    public class GameOverService : IGameOverService
    {

        private IRecordService _sqliteDB;
        private DataBaseCSV _csvDB;

        public GameOverService()
        {
            _sqliteDB = new RecordsSQLManager();
            _csvDB = new DataBaseCSV();
        }
        public void SaveRecord(Record record)
        {
            _sqliteDB.SaveRecord(record);
            _csvDB.SaveRecord(record);
            _csvDB.SaveToCSV();
        }
    }
}
