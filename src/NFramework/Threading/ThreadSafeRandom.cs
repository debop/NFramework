using System;
using System.Threading;

namespace NSoft.NFramework.Threading {
    // NOTE: http://blogs.msdn.com/b/pfxteam/archive/2009/02/19/9434171.aspx
    // NOTE: http://msmvps.com/blogs/jon_skeet/archive/2009/11/04/revisiting-randomness.aspx?utm_source=feedburner&utm_medium=feed&utm_campaign=Feed:+JonSkeetCodingBlog+(Jon+Skeet's+Coding+Blog)

    /// <summary>
    /// Thread-safe	한 Random Number Generator입니다. (Thread 별로 Random 객체가 따로 제공되고, seed 값 또한 랜덤하므로, ramdom 값 분포가 넓게 퍼지게 됩니다.)
    /// </summary>   
    [Serializable]
    public class ThreadSafeRandom : Random {
#if !SILVERLIGHT
        private readonly ThreadLocal<Random> _local = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().ToString().GetHashCode()));
#else
		[ThreadStatic]
		private readonly Lazy<Random> _local = new Lazy<Random>(() => new Random(Guid.NewGuid().ToString().GetHashCode()));
#endif

        /// <summary>
        /// 음수가 아닌 난수를 반환합니다.
        /// </summary>
        /// <returns>
        /// 0보다 크거나 같고 <see cref="F:System.Int32.MaxValue"/>보다 작은 부호 있는 32비트 정수입니다.
        /// </returns>
        public override int Next() {
            return _local.Value.Next();
        }

        /// <summary>
        /// 지정된 최대값보다 작은 음수가 아닌 난수를 반환합니다.
        /// </summary>
        /// <returns>
        /// 0보다 크거나 같고 <paramref name="maxValue"/>보다 작은 부호 있는 32비트 정수이므로 반환 값의 범위에는 대개 0이 포함되지만 <paramref name="maxValue"/>는 포함되지 않습니다. 하지만 <paramref name="maxValue"/>가 0과 같으면 <paramref name="maxValue"/>가 반환됩니다.
        /// </returns>
        /// <param name="maxValue">생성되는 난수의 상한(제외)입니다. <paramref name="maxValue"/>는 0보다 크거나 같아야 합니다. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="maxValue"/>가 0 미만인 경우 </exception>
        public override int Next(int maxValue) {
            return _local.Value.Next(maxValue);
        }

        /// <summary>
        /// 지정된 범위 내의 난수를 반환합니다.
        /// </summary>
        /// <returns>
        /// <paramref name="minValue"/>보다 크거나 같고 <paramref name="maxValue"/>보다 작은 부호 있는 32비트 정수이므로 반환 값의 범위에는 <paramref name="minValue"/>가 포함되지만 <paramref name="maxValue"/>는 포함되지 않습니다. <paramref name="minValue"/>가 <paramref name="maxValue"/>와 같은 경우에는 <paramref name="minValue"/>가 반환됩니다.
        /// </returns>
        /// <param name="minValue">반환되는 난수의 하한(포함)입니다. </param><param name="maxValue">반환되는 난수의 상한(제외)입니다. <paramref name="maxValue"/>는 <paramref name="minValue"/>보다 크거나 같아야 합니다. </param><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="minValue"/>가 <paramref name="maxValue"/>보다 큰 경우 </exception>
        public override int Next(int minValue, int maxValue) {
            return _local.Value.Next(minValue, maxValue);
        }

        /// <summary>
        /// 지정된 바이트 배열의 요소를 난수로 채웁니다.
        /// </summary>
        /// <param name="buffer">난수를 포함하는 바이트 배열입니다. </param><exception cref="T:System.ArgumentNullException"><paramref name="buffer"/>가 null인 경우 </exception>
        public override void NextBytes(byte[] buffer) {
            buffer.ShouldNotBeEmpty("buffer");
            _local.Value.NextBytes(buffer);
        }

        /// <summary>
        /// 0.0과 1.0 사이의 난수를 반환합니다.
        /// </summary>
        /// <returns>
        /// 0.0보다 크거나 같고 1.0보다 작은 배정밀도 부동 소수점 숫자입니다.
        /// </returns>
        public override double NextDouble() {
            return _local.Value.NextDouble();
        }
    }
}