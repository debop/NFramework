using System.Web.Services;
using NSoft.NFramework.FusionCharts.Widgets;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.LED {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LEDGaugeData : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
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

            gauge.DataStreamUrl = "LEDGaugeRealTime.ashx";
            gauge.RefreshInterval = 3;

            gauge.Value = 102;

            return gauge;
        }

        #endregion
    }
}