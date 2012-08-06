using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.Ajax {
    [WebService(Namespace = "http://ws.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class NormalDistributionHandler : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        private static readonly Random rnd = new ThreadSafeRandom();

        private const int MaxPlotNumber = 100;

        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var min = Request["Min"].AsDouble(-5.0);
            var max = Request["Max"].AsDouble(5.0);
            var step = Request["Step"].AsDouble(0.2);

            var avgs = new List<double>();
            var stDevs = new List<double>();

            var i = 0;
            while(true) {
                // Request Parameter로 ND0=avg,stDev&ND1=avg2,stDev2 이런 식으로 오는 것을 파싱한다.
                //
                var nd = Request["ND" + i++].AsText();

                if(nd.IsWhiteSpace())
                    break;
                var avgStDev = nd.Split(",");

                avgs.Add(avgStDev[0].AsDouble(0.0));
                stDevs.Add(avgStDev[1].AsDouble(0.0));
            }

            var chart = new MultiSeriesChart
                        {
                            Caption = "정규분포도",
                            SubCaption = "다중",
                            Palette = rnd.Next(1, 5),
                            XAxisName = "X",
                            YAxisName = "Frequency",
                            NumVisiblePlot = MaxPlotNumber / 2,
                            LabelStep = 5,
                            BaseFontAttr = { Font = "맑은 고딕", FontSize = "16" },
                            BorderAttr = { Show = true },
                            ShowLabels = true,
                            RotateLabels = true,
                            ShowValues = false,
                            BackgroundAttr =
                            {
                                BgColor = Color.WhiteSmoke,
                                BgAlpha = 100
                            },
                            ShowShadow = true,
                        };

            chart.Categories.FontAttr.FontSize = "12";
            FillCategories(chart.Categories, min, max, step);

            for(var x = 0; x < avgs.Count; x++) {
                var dataset = new DataSetElement();
                FillDataSet(dataset, min, max, step, avgs[x], stDevs[x]);
                chart.DataSets.Add(dataset);
                chart.VTrendLines.Add(new DoubleLineElement
                                      {
                                          StartValue = avgs[x],
                                          Color = Color.Brown,
                                          DisplayValue = "Average"
                                      });
            }

            return chart;
        }

        private static void FillCategories(CategoriesElement categories, double min, double max, double step) {
            for(var x = min; x <= max; x += step) {
                categories.Add(new CategoryElement
                               {
                                   Label = x.ToString("#0.0")
                               });
            }
        }

        private static void FillDataSet(DataSetElement dataset, double min, double max, double step, double avg, double stDev) {
            var stDev2 = stDev * stDev;
            for(var x = min; x <= max; x += step) {
                var s = x - avg;
                var fs = 100 * Math.Exp(-s * s / stDev2);
                dataset.AddSet(new ValueSetElement(fs));
            }
        }

        private static void FillNormalDistributionData(SingleSeriesChart chart, double avg, double stDev) {
            var min = avg - 5.0;
            var max = avg + 5.0;
            var step = (max - min) / 100.0;

            var stDev2 = stDev * stDev;
            for(var x = min; x < max; x += step) {
                var s = x - avg;
                var y = 100 * Math.Exp(-s * s / stDev2);
                chart.SetElements.Add(new ValueSetElement(y) { Label = x.ToString("#.0") });
            }
            chart.VTrendLines.Add(new DoubleLineElement
                                  {
                                      StartValue = avg,
                                      Color = Color.Brown,
                                      DisplayValue = "Average"
                                  });
        }

        #endregion
    }
}