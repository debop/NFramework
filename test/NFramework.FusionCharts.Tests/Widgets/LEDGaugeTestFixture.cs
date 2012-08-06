using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [TestFixture]
    public class LEDGaugeTestFixture : ChartTestFixtureBase {
        [Test]
        public void SimpleLEDGauge() {
            var gauge = new LEDGague();

            gauge.Axis.LowerLimit = 0;
            gauge.Axis.UpperLimit = 120;
            gauge.Axis.LowerLimitDisplay = "Low";
            gauge.Axis.UpperLimitDisplay = "High";

            gauge.NumberAttr.NumberSuffix = "dB";
            gauge.MarginAttr.RightMargin = 20;


            gauge.AddColor(0, 60, "FF0000".FromHtml(), null);
            gauge.AddColor(60, 90, "FFFF00".FromHtml(), null);
            gauge.AddColor(90, 120, "00FF00".FromHtml(), null);

            gauge.Value = 102;

            ValidateChartXml(gauge);
        }
    }
}