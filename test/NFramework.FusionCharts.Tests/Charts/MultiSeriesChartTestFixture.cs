using System.Drawing;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Charts {
    [TestFixture]
    public class MultiSeriesChartTestFixture : ChartTestFixtureBase {
        [Test]
        public void AxisAttributeTest() {
            var chart = new MultiSeriesChart
                        {
                            Caption = "프로젝트 건간 상태 추이",
                            SubCaption = "<< 프로젝트 제목 >>",
                            XAxisName = "주차",
                            BaseFontAttr = { Font = "맑은 고딕" }
                        };

            chart.Categories.FontAttr.Font = "Verdana";
            chart.Categories.FontAttr.FontSize = "8";

            chart.XAxisAttr.Name = "X축";
            chart.YAxisAttr.Name = "Y축";

            chart.PYAxisAttr.Name = "Primary Y";
            chart.SYAxisAttr.Name = "Secondary Y";

            chart.Styles.Definition.Add(new FontStyle("common")
                                        {
                                            BgColor = Color.WhiteSmoke,
                                            Bold = true
                                        });

            chart.ExProperties["확장속성"] = "확장속성값";

            ValidateChartXml(chart);
        }
    }
}