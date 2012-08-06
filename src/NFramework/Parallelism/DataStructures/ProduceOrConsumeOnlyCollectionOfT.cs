using System;
using System.Collections.Concurrent;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// Producer 또는 Consumer 둘 중 하나만 가능한 컬렉션입니다.
    /// </summary>
    [Serializable]
    public class ProduceOrConsumeOnlyCollection<T> : ProducerConsumerCollectionBase<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="containedCollection">초기 요소를 가진 컬렉션</param>
        /// <param name="produceOnly">Producer만 가능한가? (true라면 <see cref="TryAdd"/>만 가능하고, false라면 <see cref="TryTake"/>만 가능하다)</param>
        public ProduceOrConsumeOnlyCollection(IProducerConsumerCollection<T> containedCollection, bool produceOnly)
            : base(containedCollection) {
            ProducerOnly = produceOnly;
        }

        /// <summary>
        /// 공급자용 컬렉션이면 True, 소비자용 컬렉션이면 False
        /// </summary>
        public bool ProducerOnly { get; private set; }

        /// <summary>
        /// ProducerOnly Collection인 경우 요소를 추가할 수 있습니다.
        /// </summary>
        /// <param name="item">추가할 요소</param>
        /// <returns>추가 여부</returns>
        /// <exception cref="InvalidOperationException">ProducerOnly가 False이면, Consumer용 컬렉션이므로, 추가할 수 없습니다.</exception>
        protected override bool TryAdd(T item) {
            Guard.Assert(ProducerOnly, "Producer Only가 아닙니다. 즉 요소를 추가할 수 없습니다.");

            return base.TryAdd(item);
        }

        /// <summary>
        /// Consumer Only collection인 경우 요소를 꺼내갈 수 있다.
        /// </summary>
        /// <param name="item">꺼낸 요소</param>
        /// <returns>컬렉션에서 요소를 꺼낸는지 여부</returns>
        /// <exception cref="InvalidOperationException">ProducerOnly가 True이면, Producer Only 컬렉션이므로, 요소를 꺼낼 수 없습니다.</exception>
        protected override bool TryTake(out T item) {
            Guard.Assert(!ProducerOnly, "Consumer Only가 아닙니다. 즉 요소를 꺼낼 수 없습니다.");

            return base.TryTake(out item);
        }
    }
}