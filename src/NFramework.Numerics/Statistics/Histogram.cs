using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Numerics.Statistics {
    /// <summary>
    /// 데이타의 빈도를 Histogram으로 표현하기 위한 클래스입니다.
    /// 
    /// </summary>
    [Serializable]
    public class Histogram {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string FloatNumberFormat = "E4";

        private readonly List<Bucket> _buckets = new List<Bucket>();
        private bool _isBucketSorted = true;

        public Histogram() {}

        public Histogram(int bucketCount, double lower, double upper) : this(Enumerable.Empty<double>(), bucketCount, lower, upper) {}

        public Histogram(IEnumerable<double> data, int bucketCount) {
            data.ShouldNotBeNull("data");
            bucketCount.ShouldBePositive("bucketCount");

            double lower, upper;
            MathTool.GetMinMax(data, out lower, out upper);
            double width = (upper - lower) / bucketCount;

            // Add buckets for each bin; the smallest bucket's lowerbound must be slightly smaller
            // than the minimal element.
            AddBucket(new Bucket(lower.Decrement(), lower + width));
            for(var i = 1; i < bucketCount; i++) {
                var lo = lower + i * width;
                AddBucket(new Bucket(lo, lo + width));
            }

            if(IsDebugEnabled)
                log.Debug("Histogram을 생성했습니다. lower=[{0}], upper=[{1}], bucketCount=[{2}]", lower, upper, bucketCount);

            AddData(data);
        }

        public Histogram(IEnumerable<double> data, int bucketCount, double lower, double upper) {
            bucketCount.ShouldBePositive("bucketCount");
            lower.ShouldBeLessOrEqual(upper, "lower");
            upper.ShouldBeGreaterOrEqual(lower, "upper");

            double width = (upper - lower) / bucketCount;

            for(int i = 0; i < bucketCount; i++) {
                var lo = lower + i * width;
                AddBucket(new Bucket(lo, lo + width));
            }

            if(data != null)
                AddData(data);
        }

        /// <summary>
        /// Histogram의 하한 값
        /// </summary>
        public double LowerBound {
            get {
                LazySort();
                return _buckets[0].LowerBound;
            }
        }

        /// <summary>
        /// Histogram의 상한 값
        /// </summary>
        public double UpperBound {
            get {
                LazySort();
                return _buckets[BucketCount - 1].UpperBound;
            }
        }

        /// <summary>
        /// 지정된 인덱스의 Bucket
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Bucket this[int index] {
            get {
                LazySort();
                return Bucket.Copy(_buckets[index]);
            }
        }

        /// <summary>
        /// 전체 Bucket 갯수
        /// </summary>
        public int BucketCount {
            get { return _buckets.Count; }
        }

        /// <summary>
        /// 모든 변량의 갯수
        /// </summary>
        public double DataCount {
            get { return _buckets.Sum(x => x.Count); }
        }

        /// <summary>
        /// 변량 추가
        /// </summary>
        /// <param name="d"></param>
        public void AddData(double d) {
            LazySort();

            if(d < LowerBound) {
                _buckets[0].LowerBound = d.Decrement();
                _buckets[0].Count++;
            }
            else if(d > UpperBound) {
                _buckets[BucketCount - 1].UpperBound = d;
                _buckets[BucketCount - 1].Count++;
            }
            else {
                _buckets[GetBucketIndexOf(d)].Count++;
            }
        }

        /// <summary>
        /// Data를 추가합니다.
        /// </summary>
        /// <param name="data"></param>
        public void AddData(IEnumerable<double> data) {
            data.RunEach(d => AddData(d));
        }

        /// <summary>
        /// <paramref name="bucket"/> 을 현재 Histogram에 추가합니다.
        /// </summary>
        /// <param name="bucket"></param>
        public void AddBucket(Bucket bucket) {
            _buckets.Add(bucket);
            _isBucketSorted = false;
        }

        public double[] GetCounts() {
            LazySort();
            return _buckets.Select(b => b.Count).ToArray();
        }

        /// <summary>
        /// 모든 변량들의 빈도수를 Clear합니다.
        /// </summary>
        public void ResetData() {
            _buckets.RunEach(b => b.Count = 0);
        }

        private int GetBucketIndexOf(double x) {
            LazySort();

            for(var i = 0; i < _buckets.Count; i++)
                if(_buckets[i].Contains(x) == 0)
                    return i;

            if(x >= UpperBound)
                return BucketCount - 1;

            return 0;
        }

        private void LazySort() {
            if(_isBucketSorted == false) {
                _buckets.Sort();
                _isBucketSorted = true;
            }
        }

        /// <summary>
        /// Formats the contents of the Histogram into a simple acsii stem-leaf
        /// diagram.
        /// </summary>
        /// <remarks>If the bin boundaries are b0, b1, b2,...,bn-1, and the 
        /// counts for these bins are c1, c2,...,cn, respectively,
        /// then the this method returns a string with the following 
        /// format:
        /// Number SmallerCount:   ***number SmallerCount
        /// [b0,b1):     *****c1
        /// [b1,b2):     **********c2
        /// [b2,b3):     ***************c3
        /// .
        /// .
        /// .
        /// [bn-2,bn-1]: *****cn
        /// Number LargerCount : *****number LargerCount.
        /// Where the number of '*'s is for a particular bin is equal to
        /// the count for that bin minus one.</remarks>
        /// <returns>Fomatted string.</returns>
        public string StemLeaf(int maxMark) {
            LazySort();

            var buff = new StringBuilder();

            // var belowCount = _buckets[0].Count;

            //if(belowCount > 0)
            //    buff.AppendFormat("Number of belowLowerCount: {0}", belowCount).AppendLine();

            var sortedCount = _buckets.Select(b => b.Count.AsInt()).ToArray();
            Array.Sort(sortedCount);

            double scale;
            if(maxMark > 0)
                scale = Math.Max(maxMark, 10) / (double)sortedCount[sortedCount.Length - 1];
            else
                scale = 1.0;

            for(var i = 0; i < _buckets.Count; i++) {
                var bucket = _buckets[i];
                // char closing = (i == _buckets.Count - 1) ? ']' : ')';
                const char closing = ']';
                buff.AppendFormat("({0}, {1}{2}: {3}:{4}",
                                  bucket.LowerBound.ToString(FloatNumberFormat),
                                  bucket.UpperBound.ToString(FloatNumberFormat),
                                  closing,
                                  "*".Replicate((int)(bucket.Count * scale)),
                                  bucket.Count);
                buff.AppendLine();
            }

            //var overflowCount = _buckets[BucketCount - 1].Count;
            //if(overflowCount > 0)
            //    buff.AppendFormat("Number of LargerCount: {0}", overflowCount).AppendLine();

            return buff.ToString();
        }

        public override string ToString() {
            var builder = new StringBuilder();

            foreach(var bucket in _buckets)
                builder.AppendLine(bucket.ToString());

            return builder.ToString();
        }
    }
}