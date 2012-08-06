using System;
using System.Web.Services;
using NSoft.NFramework.Threading;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Bullet {
    /// <summary>
    /// $codebehindclassname$의 요약 설명입니다.
    /// </summary>
    [WebService(Namespace = "http://ws.reaweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class BulletRealTime : NSoft.NFramework.FusionCharts.Web.RealTimeDataProviderBase {
        public static readonly Random rnd = new ThreadSafeRandom();

        protected override string PopulateResponseData() {
            return string.Format(RealTimeResponseFormat, DateTime.Now.Ticks, rnd.Next(0, 100));
        }
    }
}