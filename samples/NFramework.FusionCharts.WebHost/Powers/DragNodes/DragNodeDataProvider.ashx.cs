using System;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Powers;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.DragNodes {
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class DragNodeDataProvider : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        private static readonly Random _random = new ThreadSafeRandom();

        public override IChart BuildFusionChart() {
            var option = Request["Option"].AsText("Basic");

            var chart = new DragNodeChart
                        {
                            Palette = _random.Next(5),
                            Is2D = true,
                            ShowFormBtn = true,
                            ViewMode = true,
                            FormAction = "DataHandler.html",
                            XAxisAttr = { MinValue = 0, MaxValue = 100 },
                            YAxisAttr = { MinValue = 0, MaxValue = 100 }
                        };

            switch(option) {
                case "Basic":
                    BuildBasicNodes(chart);
                    break;

                case "Network":
                    BuildNetworkNodes(chart);
                    break;
            }


            return chart;
        }

        private void BuildBasicNodes(DragNodeChart chart) {
            chart.DataSet.SeriesName = "DS1";

            for(int i = 1; i <= 10; i++) {
                var set = new DragNodeSetElement
                          {
                              Id = i.ToString(),
                              X = _random.Next(5, 90),
                              Y = _random.Next(5, 90),
                              Width = _random.Next(30, 60),
                              Height = _random.Next(30, 60),
                              Radius = _random.Next(30, 70),
                              Label = "Node " + i.ToString(),
                              NumSides = _random.Next(3, 9),
                              Shape = DragNodeShapes.Polygon
                          };

                set.Link.SetLink(FusionLinkMethod.NewWindow, "http://www.realweb21.com");

                chart.DataSet.Add(set);
            }

            chart.Connectors.Color = "#FF0000".FromHtml();
            chart.Connectors.StdThickness = 5;

            for(int i = 1; i <= 5; i++) {
                var connector = new ConnectorElement
                                {
                                    Strength = _random.Next(100) / 100.0,
                                    Label = "Link " + i.ToString(),
                                    From = i.ToString(),
                                    To = (_random.Next(10 - i) + i).ToString(),
                                    Color = "#BBBB00".FromHtml(),
                                    ArrowAtStart = false,
                                    ArrowAtEnd = true
                                };
                chart.Connectors.Add(connector);
            }
        }

        private void BuildNetworkNodes(DragNodeChart chart) {}
    }
}