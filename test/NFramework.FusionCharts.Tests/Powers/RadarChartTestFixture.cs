using System.Drawing;
using System.Linq;
using NSoft.NFramework.FusionCharts.Charts;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Powers {
    [TestFixture]
    public class RadarChartTestFixture : ChartTestFixtureBase {
        [Test]
        public void SimpleRadarChart() {
            for(int i = 1; i < 4; i++) {
                var chart = BuildFusionChart(i);
                ValidateChartXml(chart);
            }
        }

        public IChart BuildFusionChart(int option) {
            switch(option) {
                case 2:
                    return CreateRadarSample2();
                case 3:
                    return CreateRadarSample3();
                default:
                    return CreateRadarSample1();
            }
        }

        private static RadarChart CreateRadarSample1() {
            var radar = new RadarChart { Caption = "Radar Chart", Palette = rnd.Next(1, 6) };
            radar.Anchor.Alpha = 0;

            var count = 10;
            FillCategories(radar, "Index ", count);
            AddSeries(radar, "Seriese 1", count);
            AddSeries(radar, "Seriese 2", count);

            return radar;
        }

        private static RadarChart CreateRadarSample2() {
            var radar = new RadarChart
                        {
                            Caption = "Variance Analysis",
                            Palette = rnd.Next(1, 6),
                        };

            radar.RadarFillColor = Color.White;
            radar.PlotBorder.Thickness = 2;
            radar.Anchor.Alpha = 100;
            radar.NumberAttr.NumberPrefix = "$";

            var count = 12;
            FillCategories(radar, "Index ", count);
            AddSeries(radar, "Products", count);
            AddSeries(radar, "Services", count);

            return radar;
        }

        private static RadarChart CreateRadarSample3() {
            var radar = new RadarChart { Caption = "Radar Chart", Palette = rnd.Next(1, 6) };
            radar.Anchor.Alpha = 0;

            var count = 41;
            FillCategories(radar, "Index ", count);

            for(int i = 0; i < 6; i++) {
                AddSeries(radar, "Seriese " + (i + 1).ToString(), count);

                // 뒤에 있는 set 의 value 값을 null로 설정한다.
                foreach(var set in radar.DataSets[i].SetElements.Skip(30)) {
                    if(set is ValueSetElement)
                        ((ValueSetElement)set).Value = null;
                }
            }

            return radar;
        }

        private static void FillCategories(RadarChart radar, string prefix, int count) {
            for(int i = 0; i < count; i++)
                radar.AddCategory(prefix + i.ToString(), null);
        }

        private static void AddSeries(RadarChart radar, string seriesName, int count) {
            var dataset = new DataSetElement { SeriesName = seriesName };
            for(int i = 0; i < count; i++)
                dataset.AddSet(new ValueSetElement(rnd.Next(3, 10)));

            radar.DataSets.Add(dataset);
        }
    }
}