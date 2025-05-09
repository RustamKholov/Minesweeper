using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class StatisticForm : Form
    {
        private GameEngine _gameEngine;

        public StatisticForm(GameEngine gameEngine)
        {
            _gameEngine = gameEngine;
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            LoadFromCSV();
            LoadFromSQLite();
        }
        private void LoadFromCSV()
        {
            foreach (var record in _gameEngine.CSVDataBase.RecordsList)
            {
                csvRecordBinding.Add(record);
            }
        }
        private void LoadFromSQLite()
        {
            foreach (var record in _gameEngine.SQLiteDataBase.GetRecords())
            {
                sqlRecordBinding.Add(record);
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(sender is RadioButton radioButton)
            {
                csv_records_grid.Enabled = true;
                csv_records_grid.Visible = true;
                sql_database_grid.Enabled = false;
                sql_database_grid.Visible = false;
            }
        }

        private void sqlite_database_button_CheckedChanged(object sender, EventArgs e)
        {
            
            if (sender is RadioButton radioButton)
            {
                csv_records_grid.Enabled = false;
                csv_records_grid.Visible = false;
                sql_database_grid.Enabled = true;
                sql_database_grid.Visible = true;
            }
        }
    }
}
