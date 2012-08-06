using System;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Widgets;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Sparks {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SparkChartData : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        private static readonly Random rnd = new ThreadSafeRandom();

        private const int SampleCount = 100;
        private const int Max = 50;
        private const int Min = 20;

        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var kind = Request["Kind"].AsText("Line");
            switch(kind) {
                case "Column":
                    return CreateSparkColumn();

                case "WinLoss":
                    return CreateSparkWinLoss();

                default:
                    return CreateSparkLine();
            }
        }

        #endregion

        private static IChart CreateSparkLine() {
            var chart = new SparkLine
                        {
                            Palette = rnd.Next(1, 6),
                            Caption = "Cisco",
                            SetAdaptiveYMin = true,
                            ShowValues = true,
                            ShowLabels = true
                        };

            for(int i = 0; i < SampleCount; i++)
                chart.AddValue(rnd.NextDouble() * (Max - Min) + Min);
            return chart;
        }

        private static IChart CreateSparkColumn() {
            var chart = new SparkColumn
                        {
                            Palette = rnd.Next(1, 6),
                            Caption = "Revenue",
                            SubCaption = "12 months",
                            HighColor = "#00CC33".FromHtml(),
                            LowColor = "#CC0000".FromHtml(),
                            SetAdaptiveYMin = true
                        };
            chart.NumberAttr.NumberPrefix = "$";

            for(int i = 0; i < 12; i++)
                chart.AddValue(rnd.NextDouble() * (Max - Min) + Min);

            return chart;
        }

        private static IChart CreateSparkWinLoss() {
            var chart = new SparkWinLoss
                        {
                            Palette = rnd.Next(1, 6),
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
            return chart;
        }
    }
}