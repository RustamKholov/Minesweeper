using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using LiveCharts.WinForms;
using PieChart = LiveCharts.WinForms.PieChart;

namespace Minesweeper
{
    public partial class StatisticForm : Form
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public RecordsSQLManager SQLDataBase { get; set; }
        public PieChart PieChart { get; set; }
        private List<GameStatus> gameStatuses;
        public StatisticForm()
        {

            InitializeComponent();
            SQLDataBase = new RecordsSQLManager();
            gameStatuses = SQLDataBase.GetAllStatuses();

            InitializeSeriesCollection(gameStatuses);

            PieChart = new PieChart
            {
                Dock = DockStyle.Fill,
                LegendLocation = LegendLocation.Top,
                DataTooltip = new DefaultTooltip
                {
                    SelectionMode = TooltipSelectionMode.OnlySender
                }
            };

        }

        private void StatusPie_Load(object sender, EventArgs e)
        {
            diffCheckBox.SetItemChecked(0, true);
            diffCheckBox.SetItemChecked(1, true);
            diffCheckBox.SetItemChecked(2, true);
            PieChart.Series = SeriesCollection;
            PieChart.DataContext = this;
            winRatePanel.Controls.Add(PieChart);
            BestRecordInitialize();
            ResultsInitialize();
        }
        private void UpdatePieChart()
        {
            PieChart.Update();
        }

        private void diffCheckBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            diffCheckBox.ClearSelected();
            List<Difficulty> selectedDifficulties = new List<Difficulty>();
            foreach (var item in diffCheckBox.CheckedItems)
            {
                if (Enum.TryParse(item.ToString(), out Difficulty difficulty))
                {
                    selectedDifficulties.Add(difficulty);
                }
            }
            UpdateAllElements(selectedDifficulties);
        }
        private void InitializeSeriesCollection(List<GameStatus> statuses)
        {
            SeriesCollection.Clear();
            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0}-({1:P0})", chartPoint.Y, chartPoint.Participation);
            SeriesCollection.Add(new PieSeries
            {
                Title = "Win",
                Values = new ChartValues<int> { statuses.Count(g => g == GameStatus.Win) },
                DataLabels = true,
                LabelPoint = labelPoint

            });
            SeriesCollection.Add(new PieSeries
            {
                Title = "Lose",
                Values = new ChartValues<int> { statuses.Count(g => g == GameStatus.Lose) },
                DataLabels = true,
                LabelPoint = labelPoint
            });
            SeriesCollection.Add(new PieSeries
            {
                Title = "Abandoned",
                Values = new ChartValues<int> { statuses.Count(g => g == GameStatus.Abandoned) },
                DataLabels = true,
                LabelPoint = labelPoint
            });


        }
        private void ResultsInitialize(List<Record>? records = null)
        {
            var _records = SQLDataBase.GetAllRecords();
            if (records != null)
            {
                _records = records;
            }
            if (_records.Count == 0)
            {
                wonResultTimeLabel.Text = "N/A";
                lostResultTimeLabel.Text = "N/A";
                abandonedResultTimeLabel.Text = "N/A";
                totalResultTimeLabel.Text = "N/A";
                tilesResultWonLabel.Text = "N/A";
                tilesResultLostLabel.Text = "N/A";
                tilesResultAbLabel.Text = "N/A";
                tilesResultTotalLabel.Text = "N/A";
                return;
            }
            int wonTime = _records.Sum(record => record.status == GameStatus.Win ? record.secondsInGame : 0);
            int lostTime = _records.Sum(record => record.status == GameStatus.Lose ? record.secondsInGame : 0);
            int abandonedTime = _records.Sum(record => record.status == GameStatus.Abandoned ? record.secondsInGame : 0);
            int totalTime = _records.Sum(record => record.secondsInGame);
            int wonTiles = _records.Sum(record => record.status == GameStatus.Win ? record.tilesUncovered : 0);
            int lostTiles = _records.Sum(record => record.status == GameStatus.Lose ? record.tilesUncovered : 0);
            int abandonedTiles = _records.Sum(record => record.status == GameStatus.Abandoned ? record.tilesUncovered : 0);
            int totalTiles = _records.Sum(record => record.tilesUncovered);
            wonResultTimeLabel.Text = wonTime.ToString("00:00:00");
            lostResultTimeLabel.Text = lostTime.ToString("00:00:00");
            abandonedResultTimeLabel.Text = abandonedTime.ToString("00:00:00");
            totalResultTimeLabel.Text = totalTime.ToString("00:00:00");
            tilesResultWonLabel.Text = wonTiles.ToString();
            tilesResultLostLabel.Text = lostTiles.ToString();
            tilesResultAbLabel.Text = abandonedTiles.ToString();
            tilesResultTotalLabel.Text = totalTiles.ToString();

        }
        private void BestRecordInitialize()
        {
            var bestRecord = SQLDataBase.GetBestRecord();
            if (bestRecord != null && bestRecord.timeStamp.HasValue)
            {
                bestTimeLabel.Text = bestRecord.secondsInGame.ToString("00:00:00");
                dateBestLabel.Text = bestRecord.timeStamp.ToString();
            }
        }
        private void BestRecordUpdate(List<Difficulty> selectedDifficulties)
        {
            if (selectedDifficulties.Count == 0)
            {
                BestRecordInitialize();
                return;
            }
            List<Record> bestRecords = new List<Record>();
            foreach (Difficulty difficulty in selectedDifficulties)
            {
                var record = SQLDataBase.GetBestRecord(difficulty);
                if (record != null)
                {
                    bestRecords.Add(record);
                }
            }
            if (bestRecords.Count > 0)
            {
                var bestRecord = bestRecords.OrderBy(r => r.secondsInGame).First();
                bestTimeLabel.Text = bestRecord.secondsInGame.ToString("00:00:00");
                dateBestLabel.Text = bestRecord.timeStamp.ToString();
            }
            else
            {
                bestTimeLabel.Text = "No win yet";
                dateBestLabel.Text = "N/A";
            }
        }
        private void UpdateAllElements(List<Difficulty> selectedDifficulties)
        {
            UpdateChart(selectedDifficulties);
            BestRecordUpdate(selectedDifficulties);
            UpdateResults(selectedDifficulties);
        }
        private void UpdateChart(List<Difficulty> selectedDifficulties)
        {
            if (selectedDifficulties.Count == 0)
            {
                InitializeSeriesCollection(gameStatuses);
                UpdatePieChart();
                return;
            }
            List<GameStatus> selectedStatuses = new List<GameStatus>();
            foreach (var item in selectedDifficulties)
            {
                if (Enum.TryParse(item.ToString(), out Difficulty difficulty))
                {
                    selectedStatuses = selectedStatuses.Concat(SQLDataBase.GetAllStatuses(difficulty)).ToList();
                }
            }
            InitializeSeriesCollection(selectedStatuses);
            UpdatePieChart();
        }
        private void UpdateResults(List<Difficulty> selectedDifficulties)
        {
            List<Record> records = new List<Record>();
            foreach (var item in selectedDifficulties)
            {
                if (Enum.TryParse(item.ToString(), out Difficulty difficulty))
                {
                    records = records.Concat(SQLDataBase.GetAllRecords(difficulty)).ToList();
                }
            }
            ResultsInitialize(records);
        }
    }
}
