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
    public partial class StatusPie : Form
    {
        public SeriesCollection SeriesCollection { get; set; } = new SeriesCollection();
        public RecordsSQLManager SQLDataBase { get; set; }
        public PieChart PieChart { get; set; }
        private List<GameStatus> gameStatuses;
        public StatusPie()
        {

            InitializeComponent();
            SQLDataBase = new RecordsSQLManager();
            gameStatuses = SQLDataBase.GetAllStatuses();

            InitializeSeriesCollection(gameStatuses);
            PieChart = new PieChart
            {
                Dock = DockStyle.Fill,
                LegendLocation = LegendLocation.Left,
                DataTooltip = new DefaultTooltip
                {
                    SelectionMode = TooltipSelectionMode.SharedXValues
                }
            };
        }

        private void StatusPie_Load(object sender, EventArgs e)
        {

            PieChart.Series = SeriesCollection;
            PieChart.DataContext = this;
            winRatePanel.Controls.Add(PieChart);
        }
        private void UpdatePieChart()
        {
            PieChart.Update();
        }

        private void diffCheckBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(diffCheckBox.CheckedItems.Count == 0)
            {           
                InitializeSeriesCollection(gameStatuses);
                UpdatePieChart();
                return;
            }
            List<GameStatus> selectedStatuses = new List<GameStatus>();
            foreach (var item in diffCheckBox.CheckedItems)
            {
                if (Enum.TryParse(item.ToString(), out Difficulty difficulty))
                {
                    selectedStatuses = selectedStatuses.Concat(SQLDataBase.GetAllStatuses(difficulty)).ToList();
                }
            }
            
            InitializeSeriesCollection(selectedStatuses);
            UpdatePieChart();
        }
        private void InitializeSeriesCollection(List<GameStatus> statuses)
        {
            SeriesCollection.Clear();
            SeriesCollection.Add(new PieSeries
            {
                Title = "Win",
                Values = new ChartValues<int> { statuses.Count(g => g == GameStatus.Win) },
                DataLabels = true,
            });
            SeriesCollection.Add(new PieSeries
            {
                Title = "Lose",
                Values = new ChartValues<int> { statuses.Count(g => g == GameStatus.Lose) },
                DataLabels = true,
            });
            SeriesCollection.Add(new PieSeries
            {
                Title = "Abandoned",
                Values = new ChartValues<int> { statuses.Count(g => g == GameStatus.Abandoned) },
                DataLabels = true,
            });
            
        }
    }
}
