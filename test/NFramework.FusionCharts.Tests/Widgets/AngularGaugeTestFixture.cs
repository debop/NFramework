using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Widgets.Gauges {
    [TestFixture]
    public class AngularGaugeTestFixture : ChartTestFixtureBase {
        [Test]
        public void SimpleGuage() {
            var gauge = new AngularGauge();

            gauge.Axis.LowerLimit = 0;
            gauge.Axis.UpperLimit = 100;
            gauge.Axis.LowerLimitDisplay = "Bad";
            gauge.Axis.UpperLimitDisplay = "Good";
            gauge.GaugeStartAngle = 180;
            gauge.GaugeEndAngle = 0;
            gauge.Palette = 1;
            gauge.NumberAttr.NumberSuffix = "%";
            gauge.TickMarkAttr.TickValueDistance = 20;


            gauge.DataStreamUrl = "CPUData.ashx";
            gauge.RefreshInterval = 3;

            BuildColorRange(gauge);
            BuildDials(gauge);

            BuildAlerts(gauge);
            BuildAnnotations(gauge);

            BuildStyles(gauge);

            ValidateChartXml(gauge);
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
            var textAnnotation = new TextAnnotation { X = 0, Y = 0, Label = "Current status" };
            textAnnotation.Color = "666666";
            textAnnotation.FontAttr.FontSize = "11";
            textAnnotation.Align = FusionTextAlign.Center;
            annGroup.Add(textAnnotation);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement() { Id = "GrpRED", X = 175, Y = 125, Visible = false };
            var circle = new CircleAnnotation { X = 0, Y = 0, Radius = 10, FillPattern = FillMethod.Radial, Color = "FFBFBF,FF0000" };
            circle.Border.Show = false;
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement() { Id = "GrpYELLOW", X = 175, Y = 125, Visible = false };
            circle = new CircleAnnotation { X = 0, Y = 0, Radius = 10, FillPattern = FillMethod.Radial, Color = "FFFF00,BBBB00" };
            circle.Border.Show = false;
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement() { Id = "GrpGREEN", X = 175, Y = 125, Visible = false };
            circle = new CircleAnnotation { X = 0, Y = 0, Radius = 10, FillPattern = FillMethod.Radial, Color = "00FF00,339933" };
            circle.Border.Show = false;
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);
        }

        private static void BuildAlerts(WidgetBase widget) {
            // MP3 로 경고음을 줄 수 있다.
            widget.Alerts.Add(new AlertElement
                              {
                                  MinValue = 0,
                                  MaxValue = 75,
                                  Action = AlertActionKind.PlaySound,
                                  Param = "alerts/Waterloo.mp3"
                              });

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