using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.Xml;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Reflections {
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class ReflectionTestCase : ChartTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void ChartToXmlText() {
            var cosmetic = new CosmeticAttribute
                           {
                               BgSWF = "~/FusionCharts/BgChart.swf",
                               BgSWFAlpha = 100,
                               CanvasBackgroundAttr = new BackgroundAttribute() { BgColor = Color.Red, BgRatio = "12,12" }
                           };

            var sb = new StringBuilder();

            using(var writer = new XmlTextWriter(new StringWriter(sb))) {
                writer.WriteStartDocument(true);

                writer.WriteStartElement("chart");

#pragma warning disable 0618

                cosmetic.WriteChartAttribute(writer);
                // GenerateXmlAttributes(cosmetic, writer);

#pragma warning restore 0618

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }

            Console.WriteLine(sb.ToString());
        }

        [Test]
        public void MultiSeriesChart_WriteXml() {
            var chart = new MultiSeriesChart
                        {
                            Caption = "프로젝트 건간 상태 추이",
                            SubCaption = "<< 프로젝트 제목 >>",
                            XAxisName = "주차",
                            BaseFontAttr = { Font = "맑은 고딕" }
                        };

            chart.Categories.FontAttr.Font = "Verdana";
            chart.Categories.FontAttr.FontSize = "8";

            chart.Categories.Add(new CategoryElement() { Label = "C1", LinePosition = HorizontalPosition.Right });
            chart.Categories.Add(new CategoryElement() { Label = "C2" });

            chart.XAxisAttr.Name = "X축";
            chart.YAxisAttr.Name = "Y축";

            chart.PYAxisAttr.Name = "Primary Y";
            chart.PYAxisAttr.ShowValues = true;
            chart.SYAxisAttr.Name = "Secondary Y";
            chart.SYAxisAttr.ShowValues = true;

            chart.Styles.Definition.Add(new FontStyle("common") { BgColor = Color.WhiteSmoke, Bold = true });

            chart.ExProperties["확장속성"] = "확장속성값";

            ValidateChartXmlByReflection(chart);
        }

        public static void ValidateChartXmlByReflection(IChart chart) {
            var xmlText = ChartExtensions.BuildChartXml(chart, true);

            if(IsDebugEnabled)
                log.Debug(xmlText);

            var doc = new XmlDoc(xmlText);

            Assert.IsTrue(doc.IsValidDocument);
        }
    }
}