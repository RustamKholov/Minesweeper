using System.Data;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using PieChart = LiveCharts.WinForms.PieChart;
using CartesianChart = LiveCharts.WinForms.CartesianChart;
using Separator = LiveCharts.Wpf.Separator;
using Minesweeper.Application.Interfaces;
using Minesweeper.Domain.Entities;
using Minesweeper.Application.Services;
using Minesweeper.Infrastructure.Services;

namespace Minesweeper
{
    public partial class StatisticForm : Form
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public IRecordService SQLDataBase { get; set; }
        public IStatProviderService _statProviderService { get; set; }
        private List<GameStatus> _gameStatuses;
        public PieChart PieChart { get; set; }

        private List<GameStatus> _selectedStatuses = new List<GameStatus>() { GameStatus.Win };
        private List<Difficulty> _selectedDifficulties = new List<Difficulty>() { Difficulty.Easy, Difficulty.Medium, Difficulty.Hard };
        public StatisticForm()
        {
            InitializeComponent();
            SQLDataBase = new RecordsSQLManager();
            _statProviderService = new StatProviderService(SQLDataBase);
            _gameStatuses = _statProviderService.GetAllGameStatuses();

            InitializeSeriesCollection(_statProviderService.GetGameStatusCount());

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

        private void InitializeSeriesCollection(Dictionary<GameStatus, int> totals)
        {
            SeriesCollection.Clear();
            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0} ({1:P0})", chartPoint.Y, chartPoint.Participation);
            foreach (var gameStatusData in totals)
            {
                var series = new PieSeries
                {
                    Title = gameStatusData.Key.ToString(),
                    Values = new ChartValues<int> { gameStatusData.Value },
                    DataLabels = true,
                    LabelPoint = labelPoint,
                };
                SeriesCollection.Add(series);
            }
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
                InitializeSeriesCollection(_statProviderService.GetGameStatusCount());
                UpdatePieChart();
                return;
            }
            InitializeSeriesCollection(_statProviderService.GetGameStatusCount(selectedDifficulties));
            UpdatePieChart();
        }
        private void UpdateResults(List<Difficulty> selectedDifficulties)
        {
            List<Record> records = _statProviderService.GetAllRecords(selectedDifficulties);
            if (records.Count == 0)
            {
                ResultsInitialize();
                return;
            }
            ResultsInitialize(records);
        }

        private void InitializeGraphic(List<Record>? selectedRecords = null)
        {

            var records = selectedRecords ?? _statProviderService.GetAllRecords();

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
                records = _statProviderService.GetAllRecords();
            }
            FillChart(chart, records);
        }

        private void FillChart(CartesianChart chart, List<Record> records)
        {
            if (records.Count == 0) { return; }
            var chartData = _statProviderService.BuildChartData(records, _selectedStatuses);
            var seriesCollection = new SeriesCollection();
            foreach (var series in chartData.Series)
            {
                seriesCollection.Add(new ScatterSeries
                {
                    Title = series.SeriesTitle,
                    Values = new ChartValues<ScatterPoint>(series.Points),
                    MinPointShapeDiameter = 10,
                    MaxPointShapeDiameter = 40
                });
            }

            chart.Series = seriesCollection;

            double xStep = (chartData.MaxX - chartData.MinX) / 9;
            double yStep = (chartData.MaxY - chartData.MinY) / 9;

            chart.AxisX.Clear();
            chart.AxisY.Clear();

            chart.AxisX.Add(new Axis
            {
                Title = "Tiles opened per second",
                LabelFormatter = value => value.ToString("0.0"),
                MinValue = Math.Floor(chartData.MinX),
                MaxValue = Math.Ceiling(chartData.MaxX),
                Separator = new Separator { Step = xStep }
            });

            chart.AxisY.Add(new Axis
            {
                Title = "Time Spent (seconds)",
                LabelFormatter = value => TimeSpan.FromSeconds(value).ToString(@"mm\:ss"),
                MinValue = Math.Floor(chartData.MinY),
                MaxValue = Math.Ceiling(chartData.MaxY),
                Separator = new Separator { Step = yStep }
            });

        }

        private void UpdateGraphic(List<Difficulty> selectedDifficulties)
        {
            List<Record> records = _statProviderService.GetAllRecords(selectedDifficulties);
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
