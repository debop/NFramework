using System;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Powers;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.FusionCharts.WebHost.Powers.Kagi {
    /// <summary>
    /// Kagi Chart¿ë Data Provider
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class KagiChartDataProvider : NSoft.NFramework.FusionCharts.Web.DataXmlHandlerBase {
        public override IChart BuildFusionChart() {
            var option = Request["Option"].AsInt(1);
            var kagi = CreateKagiChart("Kagi Chart - Sample " + option);

            switch(option) {
                case 2:
                    FillSetData(kagi);
                    break;
                default:
                    FillSetData(kagi);
                    break;
            }

            return kagi;
        }

        private static readonly Random rnd = new ThreadSafeRandom();

        private static KagiChart CreateKagiChart(string caption) {
            var kagi = new KagiChart
                       {
                           Caption = caption,
                           ShowValues = false,
                           Animation = true,
                           LabelDisplay = LabelDisplayStyle.Rotate,
                           ReversalPercentage = 5,
                           Anchor = { Draw = true },
                       };

            return kagi;
        }

        private static void FillSetData(KagiChart kagi) {
            for(int i = 1; i <= 30; i++) {
                kagi.AddSet("3/" + i, rnd.Next(2400, 2800) / 100.0);
            }
        }
    }
}