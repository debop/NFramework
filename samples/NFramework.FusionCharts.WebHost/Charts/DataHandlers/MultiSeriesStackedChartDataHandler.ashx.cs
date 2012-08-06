using System;
using System.Drawing;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.FusionCharts.WebHost.Domain.Services;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.DataHandlers {
    [WebService(Namespace = "http://ws.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class MultiSeriesStackedChartDataHandler : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        private static readonly Random rnd = new ThreadSafeRandom();

        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var factoryId = Request["FactoryId"].AsInt(1);
            var lineDual = Request["LineDual"].AsInt(0);
            var numVisiblePlot = Request["numVisiblePlot"].AsInt(12);

            var chart = new MultiSeriesStackedChart
                        {
                            Caption = "Factory 정보",
                            SubCaption = "일일 생산량",
                            Palette = rnd.Next(1, 5),
                            RotateLabels = true,
                            // PlaceValuesInside = true,
                            // RotateValues = true,
                            SlantLabels = true,
                            XAxisName = "Day",
                            YAxisName = "Units",
                            NumVisiblePlot = numVisiblePlot,
                            BaseFontAttr = { Font = "맑은 고딕" },
                            BorderAttr = { Show = true },
                            BackgroundAttr = { BgColor = Color.White, BgAlpha = 100 }
                        };

            chart.ShowShadow = true;

            var outputs = FactoryRepository.FindAllFactoryOutputByFactoryId(factoryId);
            foreach(var output in outputs) {
                chart.AddCategory(output.DatePro.Value.ToMonthDayString(), true);
                //var category = new CategoryElement
                //               {
                //                   Label = output.DatePro.Value.ToMonthDayString()
                //               };
                //chart.Categories.CategoryElements.Add(category);
            }


            for(int d = 0; d < 2; d++) {
                var datasetCollection = new DataSetCollection();

                for(int i = 1; i < 4; i++) {
                    var dataSet = new DataSetElement
                                  {
                                      SeriesName = "Factory " + i,
                                      ShowValues = false
                                  };

                    if(lineDual == 1 && i == 3) {
                        dataSet.RenderAs = GraphKind.Line;
                        dataSet.ParentYAxis = YAxisKind.S;
                    }

                    FillData(dataSet, i);
                    datasetCollection.Add(dataSet);
                }
                chart.DataSetCollections.Add(datasetCollection);
            }

            // add LineSet
            chart.LineSet.SeriesName = "Cost as % of Revenue";

            foreach(var output in outputs) {
                chart.LineSet.SetElements.Add(new ValueSetElement { Value = rnd.Next(40, 100) });
            }

            return chart;
        }

        private static void FillData(DataSetElement dataSet, int factoryId) {
            //
            // Build Data
            //
            var outputs = FactoryRepository.FindAllFactoryOutputByFactoryId(factoryId);

            var num = 0;
            foreach(var output in outputs) {
                var set = new ValueSetElement
                          {
                              //Label = output.DatePro.Value.ToShortDateString(),
                              Value = output.Quantity ?? 0
                          };

                if((num++ % 2) == 0)
                    set.Link.SetLink(FusionLinkMethod.Local, "javascript:PopUp('April');");
                    // 실제 Javascript 메소드를 쓰려면 "PopUp-April" 로만 쓰면 된다.
                else {
                    set.Link.SetLink(FusionLinkMethod.PopUp,
                                     WebTool.GetScriptPath("~/Charts/Ajax/Default.aspx?FactoryId=") + factoryId);
                    set.Link.Width = 600;
                    set.Link.Height = 400;
                }

                dataSet.AddSet(set);
            }
        }

        #endregion
    }
}