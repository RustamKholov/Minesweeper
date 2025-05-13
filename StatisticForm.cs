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
using Separator = LiveCharts.Wpf.Separator;
using System.Xml.Serialization;
using System.Reflection;
using LiveCharts.Wpf.Charts.Base;

namespace Minesweeper
{
    public partial class StatisticForm : Form
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public RecordsSQLManager SQLDataBase { get; set; }
        public PieChart PieChart { get; set; }
        private List<GameStatus> _gameStatuses;
        private List<GameStatus> _selectedStatuses = new List<GameStatus>() { GameStatus.Win };
        private List<Difficulty> _selectedDifficulties = new List<Difficulty>() { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard };
        public StatisticForm()
        {

            InitializeComponent();
            SQLDataBase = new RecordsSQLManager();
            _gameStatuses = SQLDataBase.GetAllStatuses();

            InitializeSeriesCollection(_gameStatuses);

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
            winRadioButton.Checked = true;
            winRadioButton.CheckedChanged += radioButton_Checked;
            loseRadioButton.CheckedChanged += radioButton_Checked;
            abandonedRadioButton.CheckedChanged += radioButton_Checked;
            allRadioButton.CheckedChanged += radioButton_Checked;
            PieChart.Series = SeriesCollection;
            PieChart.DataContext = this;
            winRatePanel.Controls.Add(PieChart);
            BestRecordInitialize();
            ResultsInitialize();
            InitializeGraphic();
            diffCheckBox.TabStop = false;
            DataContext = this;
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
            _selectedDifficulties = selectedDifficulties;
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
            UpdatePieChart(selectedDifficulties);
            BestRecordUpdate(selectedDifficulties);
            UpdateResults(selectedDifficulties);
            UpdateGraphic(selectedDifficulties);
        }
        private void UpdatePieChart(List<Difficulty> selectedDifficulties)
        {
            if (selectedDifficulties.Count == 0)
            {
                InitializeSeriesCollection(_gameStatuses);
                UpdatePieChart();
                return;
            }
            List<GameStatus> selectedStatuses = new List<GameStatus>();
            foreach (var difficulty in selectedDifficulties)
            {
                selectedStatuses = selectedStatuses.Concat(SQLDataBase.GetAllStatuses(difficulty)).ToList();
            }
            InitializeSeriesCollection(selectedStatuses);
            UpdatePieChart();
        }
        private void UpdateResults(List<Difficulty> selectedDifficulties)
        {
            List<Record> records = new List<Record>();
            foreach (var difficulty in selectedDifficulties)
            {
                records = records.Concat(SQLDataBase.GetAllRecords(difficulty)).ToList();
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

            var records = selectedRecords ?? SQLDataBase.GetAllRecords();

            graphicPanel.Controls.Add(CreateChart(records));

        }

        private CartesianChart CreateChart(List<Record> records)
        {

            var chart = new CartesianChart
            {
                Dock = DockStyle.Fill,
                LegendLocation = LegendLocation.Top,
                DataTooltip = new DefaultTooltip
                {
                    SelectionMode = TooltipSelectionMode.OnlySender
                }
            };

            FillChart(chart, records);
            return chart;
        }
        private void UpdateChart(CartesianChart chart, List<Record> records)
        {

            chart.Series.Clear();
            chart.AxisX.Clear();
            chart.AxisY.Clear();
            if (records.Count == 0)
            {
                records = SQLDataBase.GetAllRecords();
            }
            FillChart(chart, records);
        }

        private void FillChart(CartesianChart chart, List<Record> records)
        {
            double globalMaxX = double.MinValue;
            double globalMaxY = double.MinValue;
            double globalMinX = double.MaxValue;
            double globalMinY = double.MaxValue;
            var grouper = records.Where(r => r.secondsInGame > 0 && (_selectedStatuses.Contains(r.status)))
                .OrderBy(r => r.secondsInGame)
                .GroupBy(r => r.difficulty);

            Dictionary<int, List<double>> tilesPerSecond = new Dictionary<int, List<double>>();
            SeriesCollection bubbleSeries = new SeriesCollection();
            chart.Series = bubbleSeries;
            foreach (var group in grouper)
            {
                var series = new ScatterSeries
                {

                    Title = group.Key.ToString(),
                    Values = new ChartValues<ScatterPoint>(),
                    MinPointShapeDiameter = 10,
                    MaxPointShapeDiameter = 40,
                    
                };

                // Dictionary to group by X (tiles per second)
                var groupedPoints = group
                    .Select(r => new
                    {
                        X = Math.Round((double)r.tilesUncovered / r.secondsInGame, 2),
                        Y = (double)r.secondsInGame
                    })
                    .GroupBy(p => p.X);

                foreach (var xGroup in groupedPoints)
                {
                    double x = xGroup.Key;
                    double averageY = xGroup.Average(p => p.Y);
                    double weight = xGroup.Count(); // this for Z to show stacking

                    // Update global min and max values
                    globalMaxX = Math.Max(globalMaxX, x);
                    globalMaxY = Math.Max(globalMaxY, averageY);

                    globalMinX = Math.Min(globalMinX, x);
                    globalMinY = Math.Min(globalMinY, averageY);

                    // The Z parameter controls point size in ScatterPoint
                    series.Values.Add(new ScatterPoint()
                    {
                        X = x,
                        Y = averageY,
                        Weight = weight,
                    });
                }

                chart.Series.Add(series);
            }

            // Calculate dynamic steps and max values to show 10 ticks
            double xRange = globalMaxX - globalMinX;
            double xStep = xRange / 9; // 10 points = 9 intervals
            double xMax = globalMinX + xStep * 9;

            double yRange = globalMaxY - globalMinY;
            double yStep = yRange / 9;
            double yMax = globalMinY + yStep * 9;



            chart.AxisX.Add(new Axis
            {
                Title = "Tiles opened per second",
                LabelFormatter = value => value.ToString("0.0"),
                MinValue = Math.Floor(globalMinX),
                MaxValue = Math.Ceiling(xMax),
                Separator = new Separator
                {
                    Step = xStep,
                    IsEnabled = true
                }
            });

            chart.AxisY.Add(new Axis
            {
                Title = "Time Spent (seconds)",
                LabelFormatter = value => TimeSpan.FromSeconds(value).ToString(@"mm\:ss"),
                MinValue = Math.Floor(globalMinY),
                MaxValue = Math.Ceiling(yMax),
                Separator = new Separator
                {
                    Step = yStep,
                    IsEnabled = true
                }
            });

        }

        private void UpdateGraphic(List<Difficulty> selectedDifficulties)
        {
            List<Record> records = new List<Record>();
            foreach (var difficulty in selectedDifficulties)
            {
              records = records.Concat(SQLDataBase.GetAllRecords(difficulty)).ToList();
            }
            if (graphicPanel.GetNextControl(graphicPanel, true) is CartesianChart chart)
            {
                UpdateChart(chart, records);
            }
        }

        private void radioButton_Checked(object? sender, EventArgs e)
        {
            if(sender is System.Windows.Forms.RadioButton radioButton)
            {
                if (radioButton.Checked)
                {
                    if (radioButton.Name == "winRadioButton")
                    {
                        _selectedStatuses = new List<GameStatus>() { GameStatus.Win };
                    }
                    else if (radioButton.Name == "loseRadioButton")
                    {
                        _selectedStatuses = new List<GameStatus>() { GameStatus.Lose };
                    }
                    else if (radioButton.Name == "abandonedRadioButton")
                    {
                        _selectedStatuses = new List<GameStatus>() { GameStatus.Abandoned };
                    }
                    else if (radioButton.Name == "allRadioButton")
                    {
                        _selectedStatuses = new List<GameStatus>() { GameStatus.Win, GameStatus.Lose, GameStatus.Abandoned };
                    }
                }
                UpdateGraphic(_selectedDifficulties);
            }
        }
    }
}
