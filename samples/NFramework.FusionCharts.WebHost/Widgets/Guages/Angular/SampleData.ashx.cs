using System.Drawing;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Widgets;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.Angular {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class SampleData : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var option = Request["Option"].AsInt(1);

            var gauge = new AngularGauge();

            switch(option) {
                case 1:
                    FeatherEffectAngularGauge(gauge);
                    break;
                case 2:
                    MetalEffectAngularGague(gauge);
                    break;
            }

            return gauge;
        }

        #endregion

        private static void FeatherEffectAngularGauge(AngularGauge gauge) {
            gauge.BackgroundAttr.BgColor = Color.White;
            gauge.Axis.UpperLimit = 100;
            gauge.Axis.LowerLimit = 0;
            gauge.BaseFontAttr.FontColor = "#666666".FromHtml();

            gauge.TickMarkAttr.ShowLimits = true;

            gauge.TickMarkAttr.MajorTM.Number = 11;
            gauge.TickMarkAttr.MajorTM.Color = "#666666".FromHtml();
            gauge.TickMarkAttr.MajorTM.Height = 8;

            gauge.TickMarkAttr.MinorTM.Number = 5;
            gauge.TickMarkAttr.MinorTM.Color = "#666666".FromHtml();
            gauge.TickMarkAttr.MinorTM.Height = 3;

            gauge.Border.Show = false;

            gauge.GaugeOuterRadius = 100;
            gauge.GaugeInnerRadius = 90;
            gauge.GaugeOriginX = 170;
            gauge.GaugeOriginY = 170;
            gauge.GaugeScaleAngle = 320;
            // gauge.GaugeStartAngle = 320;

            gauge.TickMarkAttr.TickValueDistance = 10;
            gauge.TickMarkAttr.PlaceValuesInside = true;

            gauge.GaugeFillMix = "";
            gauge.Pivot.Radius = 20;
            gauge.Pivot.FillMix = "{F0EFEA}, {BEBCBO}";
            gauge.Pivot.BorderColor = "#BEBCB0".FromHtml();
            gauge.Pivot.FillRatio = "80, 20";

            gauge.ShowShadow = false;

            gauge.AddColor(0, 80, "#00FF00".FromHtml(), null).Alpha = 0;
            gauge.AddColor(80, 100, "#FF0000".FromHtml(), "Danger").Alpha = 50;

            var dial = gauge.AddDial(null, 65);

            dial.BgColor = "BEBCB0, F0EFEA, BEBCB0";
            dial.Border.Color = Color.White;
            dial.Border.Alpha = 0;
            dial.BaseWidth = 10;
            dial.TopWidth = 3;

            var annGrp = gauge.AddAnnotationGroup(170, 170);

            var ca = new CircleAnnotation { X = 0, Y = 0, Radius = 150 };
            ca.Border.Color = "#BEBCB0".FromHtml();
            ca.FillColor = "F0EFEA, BEBCB0";
            ca.FillRatio = "85,15";
            ca.FillPattern = FillMethod.Linear;
            annGrp.Add(ca);

            ca = new CircleAnnotation { X = 0, Y = 0, Radius = 120 };
            ca.Border.Color = "#BEBCB0".FromHtml();
            ca.FillColor = "F0EFEA, BEBCB0";
            ca.FillRatio = "85,15";
            annGrp.Add(ca);

            ca = new CircleAnnotation { X = 0, Y = 0, Radius = 100, Color = Color.White.ToHexString() };
            ca.Border.Color = "#F0EFEA".FromHtml();
            annGrp.Add(ca);
        }

        private static void MetalEffectAngularGague(AngularGauge gauge) {}
    }
}