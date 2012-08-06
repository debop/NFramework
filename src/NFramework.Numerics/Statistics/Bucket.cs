using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Numerics.Statistics {
    /// <summary>
    /// 히스토그램 (<see cref="Histogram"/>) 을 구성하는 Bucket 을 나타내는 클래스입니다.
    /// 하나의 Bucket은 구간 [하한, 상한) (하한(inclusive), 상한(exclusive)) 로 표현됩니다.
    /// </summary>
    [Serializable]
    public class Bucket : IComparable<Bucket>, IEquatable<Bucket> {
        private static readonly BucketComparer bucketComparer = new BucketComparer();

        /// <summary>
        /// Default Comparer
        /// </summary>
        public static IComparer<Bucket> DefaultBucketComparer {
            get { return bucketComparer; }
        }

        /// <summary>
        /// 원본 Bucket 정보를 이용하여, 새로운 Bucket을 빌드합니다.
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public static Bucket Copy(Bucket src) {
            src.ShouldNotBeNull("src");
            return new Bucket(src.LowerBound, src.UpperBound, src.Count);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="lowerBound">하한 (inclusive)</param>
        /// <param name="upperBound">상한 (exclusive)</param>
        /// <param name="count">변량 수</param>
        public Bucket(double lowerBound, double upperBound, double count = 0.0) {
            lowerBound.ShouldBeLessOrEqual(upperBound, "lowerBound");
            upperBound.ShouldBeGreaterOrEqual(lowerBound, "upperBound");
            count.ShouldBePositiveOrZero("count");

            LowerBound = lowerBound;
            UpperBound = upperBound;
            Count = count;
        }

        /// <summary>
        /// Bucket 구간의 하한 (구간에 포함됩니다)
        /// </summary>
        public double LowerBound { get; set; }

        /// <summary>
        /// Bucket 구간의 상한 (구간에 포함되지 않습니다.)
        /// </summary>
        public double UpperBound { get; set; }

        /// <summary>
        /// Bucket 구간에 속한 변량의 수
        /// </summary>
        public double Count { get; set; }

        /// <summary>
        /// Bucket 구간의 폭
        /// </summary>
        public double Width {
            get { return UpperBound - LowerBound; }
        }

        /// <summary>
        /// Bucket의 중간 값
        /// </summary>
        public double Median {
            get { return (UpperBound + LowerBound) / 2.0; }
        }

        /// <summary>
        /// <paramref name="x"/> 가 현 Bucket 구간에 속하면 0, LowerBound 아래이면 -1,  UpperBound 보다 크면 1을 반환한다.
        /// </summary>
        /// <param name="x">The point to check.</param>
        /// <returns>0 if the point falls within the bucket boundaries; -1 if the point is
        /// smaller than the bucket, +1 if the point is larger than the bucket.</returns>
        public int Contains(double x) {
            if(x < LowerBound)
                return -1;

            if(x >= UpperBound)
                return 1;

            return 0;
        }

        /// <summary>
        /// 현재 개체를 동일한 형식의 다른 개체와 비교합니다.
        /// </summary>
        /// <returns>
        /// 비교되는 개체의 상대 순서를 나타내는 값입니다.반환 값에는 다음과 같은 의미가 있습니다.값 의미 음수 이 개체는 <paramref name="other"/> 매개 변수보다 작습니다.0 이 개체는 <paramref name="other"/>와 같습니다. 양수 이 개체는 <paramref name="other"/>보다 큽니다. 
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public int CompareTo(Bucket other) {
            other.ShouldNotBeNull("other");
            return Median.CompareTo(other.Median);
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(Bucket other) {
            if(other == null)
                return false;

            return LowerBound.ApproximateEqual(other.LowerBound) &&
                   UpperBound.ApproximateEqual(other.UpperBound) &&
                   Count.ApproximateEqual(other.Count);
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is Bucket) && Equals((Bucket)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(LowerBound, UpperBound, Count);
        }

        public override string ToString() {
            return string.Format("Bucket# [{0}, {1}) = [{2}]", LowerBound, UpperBound, Count);
        }
    }
}