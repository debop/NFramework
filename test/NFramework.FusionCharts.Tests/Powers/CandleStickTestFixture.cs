using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Powers {
    [TestFixture]
    public class CandleStickTestFixture : ChartTestFixtureBase {
        private static readonly IDictionary<double, string> categoryMap
            = new Dictionary<double, string>
              {
                  { 1, "2006" },
                  { 31, "Feb" },
                  { 59, "March" }
              };

        [Test]
        public void SimpleCandleStick() {
            var candle = CreateSimpleCandleStick("XYZ - 3 Months");

            foreach(KeyValuePair<double, string> pair in categoryMap)
                candle.Categories.Add(pair.Key, pair.Value);

            FillData(candle, categoryMap.Keys.Min(), categoryMap.Keys.Max());
            FillTrendset(candle, "Simple Moving Average", categoryMap.Keys.Min(), categoryMap.Keys.Max());
            FillTrendLines(candle);
            FillVTrendLines(candle);

            ValidateChartXml(candle);
        }

        private static CandleStick CreateSimpleCandleStick(string caption) {
            var chart = new CandleStick
                        {
                            Caption = caption,
                            Palette = rnd.Next(1, 6),
                            BearBorderColor = "#E33C3C".FromHtml(),
                            BearFillColor = "#E33C3C".FromHtml(),
                            BullBorderColor = "#1F3165".FromHtml(),
                            PYAxisName = "Price",
                            VYAxisName = "Volume (In Millions)",
                            NumPDivLines = 5
                        };
            chart.NumberAttr.NumberPrefix = "$";
            return chart;
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
            for(var x = (int)minX; x < (int)maxX; x++) {
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
                                      StartValue = 15.2,
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