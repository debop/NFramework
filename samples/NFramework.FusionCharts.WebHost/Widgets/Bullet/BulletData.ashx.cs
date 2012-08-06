using System.Web.Services;
using NSoft.NFramework.FusionCharts.Widgets;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Bullet {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://ws.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BulletData : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var chart = SampleChart;

            return chart;
        }

        #endregion

        private static BulletGraph _sampleChart;

        public static BulletGraph SampleChart {
            get { return _sampleChart ?? (_sampleChart = CreateBulletGraph()); }
        }

        private static BulletGraph CreateBulletGraph() {
            var chart = new BulletGraph
                        {
                            Caption = "Revenue",
                            SubCaption = "US $ (1,000s)"
                        };

            chart.Axis.LowerLimit = 0;
            chart.Axis.UpperLimit = 100;
            // chart.AxisAttr.LowerLimitDisplay = "Bad";
            // chart.AxisAttr.UpperLimitDisplay = "Good";

            chart.Palette = 4;
            chart.ShowValue = true;
            chart.NumberAttr.NumberPrefix = "$";
            chart.NumberAttr.NumberSuffix = "K";

            chart.PlotFillColor = "#CC0000".FromHtml();
            chart.PlotFillAlpha = 90;
            chart.TargetColor = chart.PlotFillColor;
            chart.TargetThickness = 4;

            chart.DataStreamUrl = "BulletRealTime.ashx";
            chart.RefreshInterval = 3;

            chart.Value = 78.9;
            chart.Target = 80;

            BuildColorRange(chart);
            return chart;
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