using System;

namespace NSoft.NFramework.Numerics {
    public static partial class MathTool {
        /// <summary>
        /// Random 인스턴스를 생성해주는 함수를 제공합니다. seed 값을 유일한 값을 제공하도록 합니다.
        /// </summary>
        /// <returns></returns>
        public static Func<Random> GetRandomFactory() {
            return () => new Random(Guid.NewGuid().ToString().GetHashCode());
        }
    }
}