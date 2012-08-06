using System.Drawing;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Charts {
    [TestFixture]
    public class SingleSeriesChartTestFixture : ChartTestFixtureBase {
        [Test]
        public void CreateTest() {
            var chart = new SingleSeriesChart
                        {
                            Caption = "프로젝트 건간 상태 추이",
                            SubCaption = "<< 프로젝트 제목 >>",
                            XAxisName = "주차",
                            BaseFontAttr =
                            {
                                Font = "맑은 고딕"
                            }
                        };

            chart.Categories.FontAttr.Font = "Verdana";
            chart.Categories.FontAttr.FontSize = "8";

            chart.XAxisAttr.Name = "X축";
            chart.YAxisAttr.Name = "Y축";

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