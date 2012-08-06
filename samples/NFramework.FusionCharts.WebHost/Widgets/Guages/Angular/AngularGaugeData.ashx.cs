using System.Web.Services;
using NSoft.NFramework.FusionCharts.Widgets;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.Angular {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class AngularGaugeData : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var chart = SampleChart;

            return chart;
        }

        #endregion

        private static AngularGauge _sampleChart;

        public static AngularGauge SampleChart {
            get { return _sampleChart ?? (_sampleChart = CreateAngularGauge()); }
        }

        private static AngularGauge CreateAngularGauge() {
            var chart = new AngularGauge();

            chart.Axis.LowerLimit = 0;
            chart.Axis.UpperLimit = 100;
            chart.Axis.LowerLimitDisplay = "Bad";
            chart.Axis.UpperLimitDisplay = "Good";
            chart.GaugeStartAngle = 180;
            chart.GaugeEndAngle = 0;

            chart.Palette = 1;
            chart.ShowValue = true;
            chart.NumberAttr.NumberSuffix = "%";
            chart.TickMarkAttr.TickValueDistance = 20;
            chart.DataStreamUrl = "AngularGaugeRealTime.ashx";
            chart.RefreshInterval = 3;

            BuildColorRange(chart);
            BuildDials(chart);
            BuildStyles(chart);
            BuildAnnotations(chart);
            BuildAlerts(chart);

            return chart;
        }

        private static void BuildColorRange(GaugeBase gauge) {
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 0,
                                     MaxValue = 75,
                                     Code = "#FF654F".FromHtml()
                                 });
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 75,
                                     MaxValue = 90,
                                     Code = "#F6BD0F".FromHtml()
                                 });
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 90,
                                     MaxValue = 100,
                                     Code = "#8BBA00".FromHtml()
                                 });
        }

        private static void BuildDials(AngularGauge gauge) {
            gauge.Dials.Add(new DialElement
                            {
                                Id = "CS",
                                Value = 12,
                                RearExtension = 10
                            });
        }

        private static void BuildAnnotations(WidgetBase widget) {
            widget.Annotations.AutoScale = true;

            var annGroup = new AnnotationGroupElement { X = 175, Y = 105 };
            var textAnnotation = new TextAnnotation { X = 0, Y = 0, Label = "Current status", Color = "666666" };
            textAnnotation.FontAttr.FontSize = "11";
            textAnnotation.Align = FusionTextAlign.Center;
            annGroup.Add(textAnnotation);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement { Id = "GrpRED", X = 175, Y = 125, Visible = false };
            var circle = new CircleAnnotation
                         { X = 0, Y = 0, Radius = 10, FillPattern = FillMethod.Radial, Color = "FFBFBF,FF0000" };
            circle.Border.Show = false;
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement { Id = "GrpYELLOW", X = 175, Y = 125, Visible = false };
            circle = new CircleAnnotation { X = 0, Y = 0, Radius = 10, FillPattern = FillMethod.Radial, Color = "FFFF00,BBBB00" };
            circle.Border.Show = false;
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement { Id = "GrpGREEN", X = 175, Y = 125, Visible = false };
            circle = new CircleAnnotation { X = 0, Y = 0, Radius = 10, FillPattern = FillMethod.Radial, Color = "00FF00,339933" };
            circle.Border.Show = false;
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);
        }

        private static void BuildAlerts(WidgetBase widget) {
            //widget.Alerts.Add(new AlertElement
            //{
            //    MinValue = 0,
            //    MaxValue = 75,
            //    Action = AlertActionKind.PlaySound,
            //    Param = WebTool.GetScriptPath("~/alerts/Waterloo.mp3")
            //});
            widget.Alerts.Add(new AlertElement
                              {
                                  MinValue = 0,
                                  MaxValue = 75,
                                  Action = AlertActionKind.ShowAnnotation,
                                  Param = "GrpRED"
                              });
            widget.Alerts.Add(new AlertElement
                              {
                                  MinValue = 75,
                                  MaxValue = 90,
                                  Action = AlertActionKind.ShowAnnotation,
                                  Param = "GrpYELLOW"
                              });
            widget.Alerts.Add(new AlertElement
                              {
                                  MinValue = 90,
                                  MaxValue = 100,
                                  Action = AlertActionKind.ShowAnnotation,
                                  Param = "GrpGREEN"
                              });
        }

        private static void BuildStyles(WidgetBase widget) {
            widget.Styles.Definition.Add(new NSoft.NFramework.FusionCharts.FontStyle("myValueFont")
                                         {
                                             BgColor = "#F1F1F1".FromHtml(),
                                             BorderColor = "#999999".FromHtml()
                                         });
            widget.Styles.Application.Add(new ApplyElement
                                          {
                                              ToObject = "Value",
                                              Styles = "myValueFont"
                                          });
        }
    }
}