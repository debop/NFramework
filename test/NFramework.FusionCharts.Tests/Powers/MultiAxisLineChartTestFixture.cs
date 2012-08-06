using NSoft.NFramework.FusionCharts.Charts;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Powers {
    [TestFixture]
    public class MultiAxisLineChartTestFixture : ChartTestFixtureBase {
        [Test]
        public void SimpleMultiAxisLineChart() {
            var chart = new MultiAxisLineChart
                        {
                            Caption = "Power Generator",
                            XAxisName = "Time",
                            ShowValues = false,
                            NumDivLines = 4
                        };

            chart.VDivLine.Alpha = 100;
            chart.AlternateVGrid.Show = true;
            chart.AlternateVGrid.Alpha = 5;
            chart.CaptionPadding = 0;

            int count = 6;

            // add categories
            for(int i = 0; i < count; i++)
                chart.AddCategory((i * 2).ToString("00:00s"), null);

            var powerAxis = new MultiAxisElement
                            {
                                Title = "Power",
                                TitlePos = FusionTextAlign.Left,
                                TickWidth = 10
                            };
            powerAxis.DivLine.Dashed = true;
            FillDataSet(powerAxis, "Power [W]", count, 1.5);
            chart.Axises.Add(powerAxis);

            var tempAxis = new MultiAxisElement
                           {
                               Title = "Temp.",
                               TitlePos = FusionTextAlign.Left,
                               TickWidth = 10,
                               NumDivLines = 14
                           };
            tempAxis.DivLine.Dashed = true;
            tempAxis.NumberFormat.NumberSuffix = " F";
            FillDataSet(tempAxis, "Temperature", count, 10.0);
            chart.Axises.Add(tempAxis);

            var speedAxis = new MultiAxisElement
                            {
                                Title = "Speed[RPM]",
                                TitlePos = FusionTextAlign.Right,
                                TickWidth = 10,
                                NumDivLines = 10,
                                AxisOnLeft = false
                            };
            speedAxis.DivLine.Dashed = true;
            FillDataSet(speedAxis, "Speed", count, 10.0);
            chart.Axises.Add(speedAxis);

            ValidateChartXml(chart);
        }

        private static void FillDataSet(MultiAxisElement axis, string seriesName, int count, double? multiply) {
            axis.DataSet.SeriesName = seriesName;
            for(int i = 0; i < count; i++) {
                axis.DataSet.AddSet(new ValueSetElement
                                    {
                                        Value = rnd.Next(10, 40) * (multiply ?? 1.0)
                                    });
            }
        }
    }
}