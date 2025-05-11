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
using CartesianChart = LiveCharts.WinForms.CartesianChart;
using System.Xml.Serialization;
using System.Reflection;

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
            InitializeGraphic();
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
                string.Format("{0} ({1:P0})", chartPoint.Y, chartPoint.Participation);
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
            var allRecords = records ?? SQLDataBase.GetAllRecords();
            if (allRecords.Count == 0)
            {
                SetLabelsToNA();
                return;
            }
            var grouper = allRecords
                .GroupBy(r => r.status)
                .ToDictionary(g => g.Key, g => new
                {
                    Time = g.Sum(r => r.secondsInGame),
                    Tiles = g.Sum(r => r.tilesUncovered),
                });
            int totalTime = allRecords.Sum(r => r.secondsInGame);
            int totalTiles = allRecords.Sum(r => r.tilesUncovered);

            wonResultTimeLabel.Text = ToTime(grouper.TryGetValue(GameStatus.Win, out var win) ? win.Time : 0);
            lostResultTimeLabel.Text = ToTime(grouper.TryGetValue(GameStatus.Lose, out var lost) ? lost.Time : 0);
            abandonedResultTimeLabel.Text = ToTime(grouper.TryGetValue(GameStatus.Abandoned, out var abandoned) ? abandoned.Time : 0);
            totalResultTimeLabel.Text = ToTime(totalTime);

            tilesResultWonLabel.Text = (win?.Tiles ?? 0).ToString();
            tilesResultLostLabel.Text = (lost?.Tiles ?? 0).ToString();
            tilesResultAbLabel.Text = (abandoned?.Tiles ?? 0).ToString();
            tilesResultTotalLabel.Text = totalTiles.ToString();


        }
        private void SetLabelsToNA()
        {
            string na = "N/A";
            wonResultTimeLabel.Text = na;
            lostResultTimeLabel.Text = na;
            abandonedResultTimeLabel.Text = na;
            totalResultTimeLabel.Text = na;
            tilesResultWonLabel.Text = na;
            tilesResultLostLabel.Text = na;
            tilesResultAbLabel.Text = na;
            tilesResultTotalLabel.Text = na;
        }

        private string ToTime(int seconds) =>
            new TimeSpan(0, 0, seconds).ToString(@"hh\:mm\:ss");
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
            UpdateGraphic(selectedDifficulties);
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
            if (records.Count == 0)
            {
                ResultsInitialize();
                return;
            }
            ResultsInitialize(records);
        }

        private void InitializeGraphic(List<Record>? selectedRecords = null)
        {
            var chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                LegendLocation = LegendLocation.Top,
                DataTooltip = new DefaultTooltip
                {
                    SelectionMode = TooltipSelectionMode.SharedXInSeries
                }
            };
            var records = selectedRecords ?? SQLDataBase.GetAllRecords();
            var grouper = records.Where(r => r.secondsInGame > 0 && r.status == GameStatus.Win)
                .OrderBy(r => r.secondsInGame)
                .GroupBy(r => r.difficulty);
            if (selectedRecords != null)
            {
                graphicPanel.Controls.Clear();
            }
            Dictionary<int, List<double>> tilesPerSecond = new Dictionary<int, List<double>>();
            foreach (var group in grouper)
            {
                var series = new LineSeries
                {
                    Title = group.Key.ToString(),
                    Values = new ChartValues<ObservablePoint>(),
                    LineSmoothness = 1,
                    Fill = System.Windows.Media.Brushes.Transparent,
                };
                foreach (var record in group)
                {
                    series.Values.Add(new ObservablePoint((double)record.tilesUncovered / record.secondsInGame, record.secondsInGame));
                }
                chart.Series.Add(series);
            }
            
            
            chart.AxisX.Add(new Axis
            {
                Title = "Tiles per Second",
                LabelFormatter = value => value.ToString("N")
            });
            chart.AxisY.Add(new Axis
            {
                Title = "Time spent",
                LabelFormatter = value => value.ToString("N")
            });

            graphicPanel.Controls.Add(chart);
        }
        private void UpdateGraphic(List<Difficulty> selectedDifficulties)
        {
            List<Record> records = new List<Record>();
            foreach (var item in selectedDifficulties)
            {
                if (Enum.TryParse(item.ToString(), out Difficulty difficulty))
                {
                    records = records.Concat(SQLDataBase.GetAllRecords(difficulty)).ToList();
                }
            }
            InitializeGraphic(records);
        }
    }
}
