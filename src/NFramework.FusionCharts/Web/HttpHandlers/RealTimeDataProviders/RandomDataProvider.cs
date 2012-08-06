using System;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// 난수를 실시간으로 제공합니다.
    /// </summary>
    public class RandomDataProvider : RealTimeDataProviderBase {
        private static readonly Random rnd = new ThreadSafeRandom();

        /// <summary>
        /// 실시간으로 데이타를 전송합니다.
        /// </summary>
        protected override string PopulateResponseData() {
            var min = Request["Min"].AsInt(0);
            var max = Request["Max"].AsInt(100);

            if(min > max)
                Swap(ref min, ref max);

            var label = DateTime.Now.ToSortableString(true);
            var value = rnd.Next(min, max).ToString();

            return string.Format(RealTimeResponseFormat, label, value);
        }

        private static void Swap(ref int first, ref int second) {
            var temp = first;
            first = second;
            second = temp;
        }
    }
}