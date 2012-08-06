using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Powers {
    [TestFixture]
    public class KagiChartTestFixture : ChartTestFixtureBase {
        [Test]
        public void KagiChartXmlTest() {
            var kagi = new KagiChart
                       {
                           Caption = "Kagi",
                           ShowValues = false,
                           Animation = true,
                           LabelDisplay = LabelDisplayStyle.Rotate,
                           ReversalPercentage = 5,
                       };
            kagi.Anchor.Draw = true;

            for(int i = 1; i <= 30; i++) {
                kagi.AddSet("3/" + i, rnd.Next(2400, 2800) / 100.0);
            }

            ValidateChartXml(kagi);
        }
    }
}