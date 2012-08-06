using System;
using System.Web.Services;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Guages.Angular {
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class AngularGaugeRealTime : NSoft.NFramework.FusionCharts.Web.RealTimeDataProviderBase {
        public static readonly Random rnd = new ThreadSafeRandom();

        private static readonly DateTime StartTime = DateTime.Now;

        protected override string PopulateResponseData() {
            return string.Format(RealTimeResponseFormat, DateTime.Now.Ticks, rnd.Next(0, 100));
            // return string.Format(RealTimeResponseFormat, StartTime.Ticks, rnd.Next(0, 100));
        }
    }
}