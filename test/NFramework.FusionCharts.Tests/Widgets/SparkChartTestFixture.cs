using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [TestFixture]
    public class SparkChartTestFixture : ChartTestFixtureBase {
        private const int SampleCount = 100;
        private const int Max = 50;
        private const int Min = 20;

        [Test]
        public void SimpleSparkLine() {
            var chart = new SparkLine()
                        {
                            Palette = 2,
                            Caption = "Cisco",
                            SetAdaptiveYMin = true
                        };

            for(int i = 0; i < SampleCount; i++)
                chart.AddValue(rnd.NextDouble() * (Max - Min) + Min);

            ValidateChartXml(chart);
        }

        [Test]
        public void SimpleSparkColumn() {
            var chart = new SparkColumn()
                        {
                            Palette = 2,
                            Caption = "Revenue",
                            SubCaption = "12 months",
                            HighColor = "#00CC33".FromHtml(),
                            LowColor = "#CC0000".FromHtml(),
                            SetAdaptiveYMin = true
                        };
            chart.NumberAttr.NumberPrefix = "$";

            for(int i = 0; i < 12; i++)
                chart.AddValue(rnd.NextDouble() * (Max - Min) + Min);

            ValidateChartXml(chart);
        }

        [Test]
        public void SimpleSparkWinLoss() {
            var chart = new SparkWinLoss()
                        {
                            Palette = 2,
                            Caption = "두산 베어스",
                            SubCaption = "올 시즌"
                        };

            for(int i = 0; i < 42; i++) {
                switch(rnd.Next(0, 3)) {
                    case 0:
                        chart.AddValue(WinLossKind.Win);
                        break;
                    case 1:
                        chart.AddValue(WinLossKind.Loss);
                        break;
                    default:
                        chart.AddValue(WinLossKind.Draw, true);
                        break;
                }
            }

            ValidateChartXml(chart);
        }
    }
}