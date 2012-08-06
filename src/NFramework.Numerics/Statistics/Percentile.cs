using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Numerics.Statistics {
    /// <summary>
    /// Class to calculate percentiles.
    /// </summary>
    public class Percentile {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly List<double> _data;

        public Percentile(IEnumerable<double> data) {
            data.ShouldNotBeEmpty("data");
            _data = new List<double>(data);

            Guard.Assert(_data.Count >= 3, "변량은 최소 3개 이상이어야 합니다.");
            _data.Sort();
        }

        /// <summary>
        /// Percentiles 계산 방식
        /// </summary>
        public PercentileMethod Method { get; set; }

        /// <summary>
        /// Computes the percentile.
        /// </summary>
        /// <param name="percentile">The percentile, must be between 0.0 and 1.0 (inclusive).</param>
        /// <returns>the requested percentile.</returns>
        public double Compute(double percentile) {
            if(IsDebugEnabled)
                log.Debug("Percentile[{0}] 을 계산합니다. Method=[{1}]", percentile, Method);

            percentile.ShouldBeBetween(0.0, 1.0, "percentile");

            if(percentile.ApproximateEqual(0.0))
                return _data[0];

            if(percentile.ApproximateEqual(1.0))
                return _data[_data.Count - 1];


            if(Method == PercentileMethod.Nist)
                return Nist(percentile);

            if(Method == PercentileMethod.Nearest)
                return Nearest(percentile);

            if(Method == PercentileMethod.Excel)
                return Excel(percentile);

            if(Method == PercentileMethod.Interpolation)
                return Interpolation(percentile);

            return double.NaN;
        }

        /// <summary>
        /// 주어진 값의 percentile을 계산합니다.
        /// </summary>
        /// <param name="percentiles"></param>
        /// <returns></returns>
        public IEnumerable<double> Compute(IEnumerable<double> percentiles) {
            percentiles.ShouldNotBeNull("percentiles");

            return percentiles.Select(x => Compute(x));
        }

        /// <summary>
        /// Computes the percentile using the nearest value.
        /// </summary>
        /// <param name="percentile">The percentile.</param>
        /// <returns>the percentile using the nearest value.</returns>
        private double Nearest(double percentile) {
            var n = (int)Math.Round((_data.Count * percentile) + 0.5, 0);
            return _data[n - 1];
        }

        /// <summary>
        /// Computes the percentile using Excel's method.
        /// </summary>
        /// <param name="percentile">The percentile.</param>
        /// <returns>the percentile using Excel's method.</returns>
        private double Excel(double percentile) {
            var tmp = 1 + (percentile * (_data.Count - 1.0));
            var k = (int)tmp;
            var d = tmp - k;

            return _data[k - 1] + (d * (_data[k] - _data[k - 1]));
        }

        /// <summary>
        /// Computes the percentile using interpolation.
        /// </summary>
        /// <param name="percentile">The percentile.</param>
        /// <returns>the percentile using the interpolation.</returns>
        private double Interpolation(double percentile) {
            var k = (int)(_data.Count * percentile);
            var pk = (k - 0.5) / _data.Count;
            return _data[k - 1] + (_data.Count * (percentile - pk) * (_data[k] - _data[k - 1]));
        }

        /// <summary>
        /// Computes the percentile using NIST's method.
        /// </summary>
        /// <param name="percentile">The percentile.</param>
        /// <returns>the percentile using NIST's method.</returns>
        private double Nist(double percentile) {
            var tmp = percentile * (_data.Count + 1.0);
            var k = (int)tmp;
            var d = tmp - k;

            return _data[k - 1] + (d * (_data[k] - _data[k - 1]));
        }
    }
}