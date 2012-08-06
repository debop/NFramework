using System.Collections.Generic;

namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 난수 발생을 위한 Utility 클래스입니다.
    /// </summary>
    public static class RandomTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 난수발생기를 이용하여 <paramref name="values"/> 배열에 난수를 <paramref name="count"/> 갯수만큼 할당합니다.
        /// </summary>
        public static void FillValues(this IRandomizer randomizer, double[] values, int count) {
            randomizer.ShouldNotBeNull("randomizer");
            values.ShouldNotBeEmpty("values");
            Guard.Assert(values.Length >= count, "values 의 length가 count 보다 작습니다.");

            for(int i = 0; i < count; i++)
                values[i] = randomizer.Next();
        }

        /// <summary>
        /// 난수발생기를 이용하여 <paramref name="values"/> 배열에 난수를 할당합니다.
        /// </summary>
        public static void FillValues(this IRandomizer randomizer, double[] values) {
            randomizer.ShouldNotBeNull("randomizer");
            values.ShouldNotBeEmpty("values");

            FillValues(randomizer, values, values.Length);
        }

        /// <summary>
        /// 난수발생기로 지정한 갯수만큼 난수를 발생시킵니다.
        /// </summary>
        /// <param name="randomizer"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static IEnumerable<double> GenerateValues(this IRandomizer randomizer, int count) {
            randomizer.ShouldNotBeNull("randomizer");
            count.ShouldBePositive("count");

            for(var i = 0; i < count; i++)
                yield return randomizer.Next();
        }
    }
}