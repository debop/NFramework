using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Parallelism.Partitioners {
    /// <summary>
    /// BlockingCollection{T}의 소비자용 열거자(GetConsumingEnumerable())를 동적으로 배분하는 Partitioner입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlockingCollectionPartitioner<T> : Partitioner<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly BlockingCollection<T> _collection;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="collection"></param>
        public BlockingCollectionPartitioner(BlockingCollection<T> collection) {
            collection.ShouldNotBeNull("collection");
            _collection = collection;
        }

        public override bool SupportsDynamicPartitions {
            get { return true; }
        }

        public override IList<IEnumerator<T>> GetPartitions(int partitionCount) {
            partitionCount.ShouldBePositive("partitionCount");

            var dynamicPartitioner = GetDynamicPartitions();

            // 이건 partitionCount 갯수만큼 똑 같은 Consuming Enumerator가 갑니다.
            return Enumerable.Range(0, partitionCount).Select(_ => dynamicPartitioner.GetEnumerator()).ToList();
        }

        public override IEnumerable<T> GetDynamicPartitions() {
            return _collection.GetConsumingEnumerable();
        }
    }
}