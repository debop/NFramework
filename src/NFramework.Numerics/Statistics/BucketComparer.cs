using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Statistics {
    /// <summary>
    /// This <c>IComparer</c> performs comparisons between a point and a bucket.
    /// </summary>
    internal sealed class BucketComparer : IComparer<Bucket> {
        /// <summary>
        /// 두 개체를 비교한 다음 한 개체가 다른 개체보다 작은지, 큰지 또는 두 개체가 같은지 여부를 나타내는 값을 반환합니다.
        /// </summary>
        /// <returns>
        /// 다음 표와 같이 <paramref name="x"/> 및 <paramref name="y"/>의 상대 값을 나타내는 부호 있는 정수입니다.값 의미 음수<paramref name="x"/>가 <paramref name="y"/>보다 작습니다.0<paramref name="x"/>가 <paramref name="y"/>와 같습니다.양수<paramref name="x"/>가 <paramref name="y"/>보다 큽니다.
        /// </returns>
        /// <param name="x">비교할 첫 번째 개체입니다.</param><param name="y">비교할 두 번째 개체입니다.</param>
        public int Compare(Bucket x, Bucket y) {
            x.ShouldNotBeNull("x");
            y.ShouldNotBeNull("y");

            if(y.Width.ApproximateEqual(0.0))
                return -x.Contains(y.UpperBound);

            return -y.Contains(x.UpperBound);
        }
    }
}