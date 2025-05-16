
using Minesweeper.Infrastructure.Services;
using Minesweeper.Interfaces;

namespace Minesweeper
{
    public partial class RecordsForm : Form
    {
        private IRecordService _csvDataBase;
        private IRecordService _sqliteDataBase;

        public RecordsForm()
        {
            _csvDataBase = new DataBaseCSV();
            _sqliteDataBase = new RecordsSQLManager();
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            LoadFromCSV();
            LoadFromSQLite();
        }
        private void LoadFromCSV()
        {
            foreach (var record in _csvDataBase.GetAllRecords())
            {
                csvRecordBinding.Add(record);
            }
        }
        private void LoadFromSQLite()
        {
            foreach (var record in _sqliteDataBase.GetAllRecords())
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
