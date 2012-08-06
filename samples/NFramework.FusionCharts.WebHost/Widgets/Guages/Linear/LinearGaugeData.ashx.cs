using System;
using System.Drawing;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Widgets;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.Linear {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LinearGaugeData : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        private static readonly Random rnd = new ThreadSafeRandom();

        #region Overrides of FusionChartDataXmlHandlerBase

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public override IChart BuildFusionChart() {
            var chart = new LinearGauge
                        {
                            Axis =
                            {
                                LowerLimit = 0,
                                UpperLimit = 100,
                                LowerLimitDisplay = "Bad",
                                UpperLimitDisplay = "Good"
                            },
                            GaugeRoundRadius = 14,
                            MarginAttr = { RightMargin = 20 },
                            Palette = rnd.Next(1, 5),
                            ShowValue = true,
                            NumberAttr = { NumberSuffix = "%" },
                            DataStreamUrl = "LinearGaugeRealTime.ashx",
                            RefreshInterval = 3
                        };

            // chart.TickMarkAttr.TickValueDistance = 20;

            BuildColorRange(chart);
            BuildPointers(chart);
            BuildStyles(chart);
            // BuildAnnotations(chart);
            BuildAlerts(chart);

            AddTrendPoints(chart);

            return chart;
        }

        #endregion

        private static void BuildColorRange(GaugeBase gauge) {
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 0,
                                     MaxValue = 75,
                                     Code = "#FF654F".FromHtml(),
                                     Label = "나쁨"
                                 });
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 75,
                                     MaxValue = 90,
                                     Code = "#F6BD0F".FromHtml(),
                                     Label = "보통"
                                 });
            gauge.ColorRange.Add(new ColorElement
                                 {
                                     MinValue = 90,
                                     MaxValue = 100,
                                     Code = "#8BBA00".FromHtml(),
                                     Label = "좋음"
                                 });
        }

        private static void BuildPointers(LinearGauge gauge) {
            gauge.Pointers.Add(new PointerElement
                               {
                                   Id = "CS",
                                   Value = 12,
                                   Radius = 10,
                                   ToolText = "만족도"
                               });
        }

        private static void BuildAnnotations(WidgetBase widget) {
            widget.Annotations.AutoScale = true;

            var annGroup = new AnnotationGroupElement
                           {
                               X = 175,
                               Y = 105
                           };
            var textAnnotation = new TextAnnotation
                                 {
                                     X = 0,
                                     Y = 0,
                                     Label = "Current status",
                                     Color = "666666",
                                     FontAttr = { FontSize = "11" },
                                     Align = FusionTextAlign.Center
                                 };
            annGroup.Add(textAnnotation);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement
                       {
                           Id = "GrpRED",
                           X = 175,
                           Y = 125,
                           Visible = false
                       };
            var circle = new CircleAnnotation
                         {
                             X = 0,
                             Y = 0,
                             Radius = 10,
                             FillPattern = FillMethod.Radial,
                             Color = "FFBFBF,FF0000",
                             Border = { Show = false }
                         };
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement
                       {
                           Id = "GrpYELLOW",
                           X = 175,
                           Y = 125,
                           Visible = false
                       };
            circle = new CircleAnnotation
                     {
                         X = 0,
                         Y = 0,
                         Radius = 10,
                         FillPattern = FillMethod.Radial,
                         Color = "FFFF00,BBBB00",
                         Border = { Show = false }
                     };
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);

            annGroup = new AnnotationGroupElement
                       {
                           Id = "GrpGREEN",
                           X = 175,
                           Y = 125,
                           Visible = false
                       };
            circle = new CircleAnnotation
                     {
                         X = 0,
                         Y = 0,
                         Radius = 10,
                         FillPattern = FillMethod.Radial,
                         Color = "00FF00,339933",
                         Border = { Show = false }
                     };
            annGroup.Add(circle);
            widget.Annotations.Add(annGroup);
        }

        private static void BuildAlerts(WidgetBase widget) {
            widget.Alerts.Add(new AlertElement
                              {
                                  MinValue = 0,
                                  MaxValue = 75,
                                  Action = AlertActionKind.PlaySound,
                                  Param = WebTool.GetScriptPath("~/alerts/Waterloo.mp3")
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
                                             Font = "맑은 고딕",
                                             Size = 10,
                                             Color = Color.Red,
                                             BgColor = "#F1F1F1".FromHtml(),
                                             BorderColor = "#999999".FromHtml()
                                         });
            widget.Styles.Application.Add(new ApplyElement
                                          {
                                              ToObject = "Value",
                                              Styles = "myValueFont"
                                          });
            widget.Styles.Application.Add(new ApplyElement
                                          {
                                              ToObject = "Label",
                                              Styles = "myValueFont"
                                          });
        }

        private static void AddTrendPoints(WidgetBase chart) {
            var point = new PointElement
                        {
                            StartValue = 79,
                            DisplayValue = "Previous",
                            Color = "#666666".FromHtml(),
                            Thickness = 2,
                            Dashed = true,
                            DashLen = 3,
                            DashGap = 3,
                            UseMarker = true,
                            MarkerColor = "#F1F1F1".FromHtml(),
                            MarkerBorderColor = "#666666".FromHtml(),
                            MarkerRadius = 7
                        };
            chart.TrendPoints.Add(point);
        }
    }
}