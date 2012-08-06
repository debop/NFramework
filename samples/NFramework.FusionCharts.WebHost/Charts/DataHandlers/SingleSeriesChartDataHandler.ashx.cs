using System;
using System.Drawing;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Charts;
using NSoft.NFramework.FusionCharts.WebHost.Domain.Services;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.WebHost.Charts.DataHandlers {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://ws.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SingleSeriesChartDataHandler : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var factoryId = Request["FactoryId"].AsInt(1);
            var numVisiblePlot = Request["numVisiblePlot"].AsInt(12);

            var chart = new SingleSeriesChart
                        {
                            Caption = "Factory" + factoryId + " 정보",
                            SubCaption = "일일 생산량",
                            Palette = Math.Max(1, Math.Min(5, factoryId)),
                            //RotateLabels = true,
                            //PlaceValuesInside = true,
                            //RotateValues = true,
                            XAxisName = "Day",
                            YAxisName = "Units",
                            NumVisiblePlot = numVisiblePlot,
                            BaseFontAttr = { Font = "맑은 고딕" },
                            BorderAttr = { Show = true },
                            BackgroundAttr = { BgColor = Color.White, BgAlpha = 100 },
                            ShowShadow = true
                        };

            FillData(chart, factoryId);

            return chart;
        }

        private static void FillData(SingleSeriesChart chart, int factoryId) {
            //
            // Build Data
            //
            var outputs = FactoryRepository.FindAllFactoryOutputByFactoryId(factoryId);

            var num = 0;
            foreach(var output in outputs) {
                var set = new ValueSetElement
                          {
                              Label = output.DatePro.Value.ToShortDateString(),
                              Value = output.Quantity ?? 0
                          };

                switch(num % 4) {
                    case 0:
                        set.Link.SetLink(FusionLinkMethod.Local, "javascript:PopUp('April')");
                        break;
                    case 1:
                        set.Link.SetLink(FusionLinkMethod.PopUp,
                                         WebTool.GetScriptPath("~/Charts/Ajax/Default.aspx?FactoryId=") + factoryId);
                        break;
                    case 2:
                        set.Link.SetFameLink(WebTool.GetScriptPath("~/Charts/Ajax/Default.aspx?FactoryId=") + factoryId, "_blank");
                        break;
                    case 3:
                        set.Link.SetJavascriptLink("PopUp", "April");
                        break;

                    default:
                        set.Link.SetLink(FusionLinkMethod.Local, "http://www.realweb21.com");
                        break;
                }

                num++;
                //if ((num++ % 2) == 0)
                //    set.Link.SetLink(FusionLinkMethod.Local, "javascript:PopUp('April');"); // 실제 Javascript 메소드를 쓰려면 "PopUp-April" 로만 쓰면 된다.
                //else
                //{
                //    set.Link.SetLink(FusionLinkMethod.PopUp, WebTool.GetScriptPath("~/Charts/Ajax/Default.aspx?FactoryId=") + factoryId.ToString());
                //    set.Link.Width = 600;
                //    set.Link.Height = 400;
                //}

                chart.SetElements.Add(set);
            }
        }

        #endregion
    }
}