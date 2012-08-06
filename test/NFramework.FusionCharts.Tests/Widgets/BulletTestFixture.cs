using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [TestFixture]
    public class BulletTestFixture : ChartTestFixtureBase {
        [Test]
        public void SampleBulletTest() {
            var chart = new BulletGraph();

            chart.Palette = 3;
            chart.Axis.LowerLimit = 0;
            chart.Axis.UpperLimit = 100;
            chart.Caption = "Revenue";
            chart.SubCaption = "US $ (1,000s)";

            chart.NumberAttr.NumberPrefix = "$";
            chart.NumberAttr.NumberSuffix = "K";
            chart.ShowValue = true;

            chart.Value = 78.3;
            chart.Target = 90;

            BuildColorRange(chart);

            ValidateChartXml(chart);
        }

        private static void BuildColorRange(GaugeBase gauge) {
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 0,
                                     MaxValue = 50,
                                     Code = "#A6A6A6".FromHtml()
                                 });
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 50,
                                     MaxValue = 75,
                                     Code = "#CCCCCC".FromHtml()
                                 });
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 75,
                                     MaxValue = 100,
                                     Code = "#E1E1E1".FromHtml()
                                 });
        }
    }
}