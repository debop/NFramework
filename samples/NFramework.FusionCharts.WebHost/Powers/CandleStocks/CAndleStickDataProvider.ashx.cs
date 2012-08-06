using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Powers;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.CandleStocks {
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class CandleStickDataProvider : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        public override IChart BuildFusionChart() {
            var option = Request["Option"].AsText("Basic");

            var candle = CreateSimpleCandleStick("CandleStick Chart - " + option);

            foreach(KeyValuePair<double, string> pair in categoryMap)
                candle.Categories.Add(pair.Key, pair.Value);
            FillData(candle, categoryMap.Keys.Min(), categoryMap.Keys.Max() + 4);

            switch(option) {
                case "WithoutVolume":
                    candle.ShowVolumeChart = false;
                    candle.Dataset.ForEach(set => set.Volume = null);
                    break;

                case "LinePlot":
                    candle.PlotPriceAs = PricePlotMethod.Line;
                    break;

                case "BarPlot":
                    candle.PlotPriceAs = PricePlotMethod.Bar;
                    break;

                case "MissingData":
                    candle.Dataset.RemoveRange(7, 3);
                    candle.Categories.Add(9, "Holyday", true, true, null, null);
                    break;

                case "Highlighting":
                    //dashed='1' color='A8FFA8' borderColor='00CC33' valuetext='100% Dividend'
                    foreach(var index in new int[] { 33, 40, 42 }) {
                        var highlightSet = candle.Dataset[index];
                        highlightSet.Color = "#A8FFA8".FromHtml();
                        highlightSet.Dashed = true;
                        highlightSet.BorderColor = "#00CC33".FromHtml();
                        highlightSet.ValueText = "100% Dividend";
                    }
                    break;

                case "WithTrends":
                    candle.VolumeHeightPercent = 20;
                    FillTrendset(candle, "Simple Moving Average", categoryMap.Keys.Min(), categoryMap.Keys.Max() + 4);
                    FillTrendLines(candle);
                    FillVTrendLines(candle);
                    break;

                default:
                    break;
            }

            return candle;
        }

        private static readonly Random rnd = new ThreadSafeRandom();

        private static readonly IDictionary<double, string> categoryMap
            = new Dictionary<double, string>
              {
                  { 1, "2006" },
                  { 31, "Feb" },
                  { 59, "March" }
              };

        private static CandleStick CreateSimpleCandleStick(string caption) {
            var candle = new CandleStick
                         {
                             Caption = caption,
                             Palette = rnd.Next(1, 6),
                             BearBorderColor = "#E33C3C".FromHtml(),
                             BearFillColor = "#E33C3C".FromHtml(),
                             BullBorderColor = "#1F3165".FromHtml(),
                             PYAxisName = "Price",
                             VYAxisName = "Volume (In Millions)",
                             NumPDivLines = 5,
                             ShowVolumeChart = true,
                             VolumeHeightPercent = 30,
                             NumberAttr = { NumberPrefix = "$" }
                         };
            return candle;
        }

        private static void FillData(CandleStick candle, double minX, double maxX) {
            for(double x = minX; x < maxX; x += 1) {
                var open = rnd.Next(30, 60) * rnd.Next(90, 111) * (x / 10.0) / 2000.0;
                // var high = rnd.Next(22, 28) * rnd.Next(90, 110) * 1.1 * x / 50.0;
                var high = open + rnd.Next(-120, 120) * 0.01;
                var volume = rnd.Next(20, 100) * 1000000.0;

                candle.AddSet(x,
                              open,
                              open - rnd.Next(-50, 100) * 0.01,
                              high,
                              high - rnd.Next(-50, 100) * 0.01,
                              volume);
            }
        }

        private static void FillTrendset(CandleStick candle, string name, double minX, double maxX) {
            candle.Trendset.Name = name;
            for(int x = (int)minX; x < (int)maxX; x++) {
                var data = candle.Dataset.ElementAtOrDefault(x);
                if(data != null && data.High.HasValue && data.Low.HasValue)
                    candle.Trendset.AddSet(x, (data.High.Value + data.Low.Value) / 2.0);
            }
        }

        private static void FillTrendLines(CandleStick candle) {
            // displayvalue='S1' thickness='0' dashed='1' dashLen='2' dashGap='2'
            candle.TrendLines.Add(new DoubleLineElement
                                  {
                                      StartValue = 4.2,
                                      Color = "#0382AB".FromHtml(),
                                      DisplayValue = "S1",
                                      Thickness = 0,
                                      IsDashed = true,
                                      DashLen = 2,
                                      DashGap = 2
                                  });
            candle.TrendLines.Add(new DoubleLineElement
                                  {
                                      StartValue = 4.8,
                                      Color = "#0382AB".FromHtml(),
                                      DisplayValue = "S2",
                                      Thickness = 0,
                                      IsDashed = true,
                                      DashLen = 2,
                                      DashGap = 2
                                  });
            candle.TrendLines.Add(new DoubleLineElement
                                  {
                                      StartValue = 15.5,
                                      Color = "#0382AB".FromHtml(),
                                      DisplayValue = "R1",
                                      Thickness = 0,
                                      IsDashed = true,
                                      DashLen = 2,
                                      DashGap = 2
                                  });
            candle.TrendLines.Add(new DoubleLineElement
                                  {
                                      StartValue = 14.2,
                                      Color = "#0382AB".FromHtml(),
                                      DisplayValue = "R2",
                                      Thickness = 0,
                                      IsDashed = true,
                                      DashLen = 2,
                                      DashGap = 2
                                  });
        }

        private static void FillVTrendLines(CandleStick candle) {
            candle.VTrendLines.Add(new DoubleLineElement
                                   {
                                       StartValue = 10,
                                       EndValue = 13,
                                       Color = "#FF5904".FromHtml(),
                                       DisplayValue = "Result Impact",
                                       IsTrendZone = true,
                                       Alpha = 10
                                   });
        }
    }
}