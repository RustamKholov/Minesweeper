using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveCharts.Defaults;

namespace Minesweeper.Application.DTO
{
    public class StatChartResult
    {
        public List<ChartSeriesData> Series { get; set; } = new();
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MinY { get; set; }
        public double MaxY { get; set; }
    }
    public class ChartSeriesData
    {
        public string SeriesTitle { get; set; } = "";
        public List<ScatterPoint> Points { get; set; } = new();
    }
}
