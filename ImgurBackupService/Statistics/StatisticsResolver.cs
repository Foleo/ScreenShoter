using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;

namespace ImgurBackupService.Statistics
{
    public class StatisticsResolver
    {
        private List<Stat> statistics;

        public StatisticsResolver()
        {
            statistics = new List<Stat>();

            for (int i = 0; i < 24; i++)
            {
                var d = DateTime.Now;
                var day = new DateTime(d.Year, d.Month, d.Day, i, 0, 0);
                statistics.Add(new Stat() { Date = day, Bytes = 0 });
            }
        }

        public void AddStat(DateTime date, long bytes)
        {
            var stat = new Stat() { Date = date, Bytes = bytes };
            statistics.Add(stat);
        }

        public void Clear()
        {
            statistics.Clear();
            for (int i = 0; i < 24; i++)
            {
                var d = DateTime.Now;
                var day = new DateTime(d.Year, d.Month, d.Day, i, 0, 0);
                statistics.Add(new Stat() { Date = day, Bytes = 0 });
            }
        }

        public string CreateStatistics()
        {
            var groupedList = from s in statistics
                              group s.Bytes by s.Date.Hour into st
                              select new { Date = st.Key, Bytes = st.ToList() };

            Trace.WriteLine("Statistics");

            foreach (var it in groupedList)
            {
                Trace.WriteLine(it.Date + " Bytes: " + it.Bytes.Sum());
            }

            return GetStatGraphics();
        }

        private string GetStatGraphics()
        {
            try
            {
                var groupedList = from s in statistics
                                  group s.Bytes by s.Date.Hour into st
                                  select new { Date = st.Key, Bytes = st.Sum() };

                var gg = groupedList.ToList();

                var chart = new Chart() { Width = 500, Height = 500 };
                var series = new Series()
                {
                    Name = "Series1",
                    Color = System.Drawing.Color.Green,
                    IsVisibleInLegend = false,
                    IsXValueIndexed = true,
                    ChartType = SeriesChartType.Column
                };

                var chartArea = new ChartArea()
                {
                    AxisX = new Axis(),
                    AxisY = new Axis(),
                };

                chartArea.AxisX.Interval = 1;
                chartArea.AxisX.Minimum = 0;
                chartArea.AxisX.Title = "Hour";
                chartArea.AxisY.Minimum = 0;
                chartArea.AxisY.Title = "Bytes";

                chart.Series.Add(series);
                chart.ChartAreas.Add(chartArea);

                series.Points.DataBindXY(gg, "Date", gg, "Bytes");
                series.Palette = ChartColorPalette.BrightPastel;

                chart.SaveImage(AppDomain.CurrentDomain.BaseDirectory + "/chart.png", ChartImageFormat.Png);
                Trace.WriteLine(AppDomain.CurrentDomain.BaseDirectory + "/chart.png");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error creating chart " + ex.Message + ex.StackTrace);
            }
            return AppDomain.CurrentDomain.BaseDirectory + "/chart.png";
        }
    }

    public class Stat
    {
        public DateTime Date { get; set; }
        public long Bytes { get; set; }
    }
}
