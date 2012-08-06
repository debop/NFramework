using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// Thread-Safe 하면서 Producer-Consumer 패턴을 지원하는 객체 풀을 제공합니다.
    /// </summary>
    /// <remarks>
    /// 객체 풀에 객체를 담고, 없다면, 객체를 생성해서 제공할 수 있도록 object factory delegate를 사용합니다.
    /// 아래 요소의 수형이 <see cref="Task"/>라면, 비동기적인 작업의 풀을 가지게 됩니다.
    /// </remarks>
    /// <typeparam name="T">요소의 수형</typeparam>
    [Serializable]
    public sealed class ObjectPool<T> : ProducerConsumerCollectionBase<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="objectFactory">pool에 요소가 없을 때, 요소를 생성해서 제공하는 함수</param>
        public ObjectPool(Func<T> objectFactory) : this(objectFactory, new ConcurrentQueue<T>()) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="objectFactory">pool에 요소가 없을 때, 요소를 생성해서 제공하는 함수</param>
        /// <param name="containedCollection">Pool의 요소들이 저장될 컬렉션</param>
        public ObjectPool(Func<T> objectFactory, IProducerConsumerCollection<T> containedCollection)
            : base(containedCollection) {
            objectFactory.ShouldNotBeNull("objectFactory");
            ObjectFactory = objectFactory;

            if(IsDebugEnabled)
                log.Debug("새로운 ObjectPool<{0}> 이 생성되었습니다.", typeof(T).Name);
        }

        /// <summary>
        /// 요소 생성 함수
        /// </summary>
        public Func<T> ObjectFactory { get; private set; }

        /// <summary>
        /// Pool에 요소를 추가합니다.
        /// </summary>
        /// <param name="item"></param>
        public void PutObject(T item) {
            base.TryAdd(item);
        }

        /// <summary>
        /// Pool에서 요소를 취한다. 없으면 <see cref="ObjectFactory"/>를 통해 생성한다.
        /// </summary>
        /// <returns></returns>
        public T GetObject() {
            T value;
            return base.TryTake(out value) ? value : ObjectFactory();
        }

        /// <summary>
        /// Pool에 있는 모든 요소를 배열로 반환하고, Pool을 비운다
        /// </summary>
        /// <returns></returns>
        public T[] ToArrayAndClear() {
            var items = new List<T>();

            T value;

            while(base.TryTake(out value))
                items.Add(value);

            return items.ToArray();
        }

        protected override bool TryAdd(T item) {
            if(IsDebugEnabled)
                log.Debug("풀에 요소를 추가합니다. item=[{0}]", item);

            PutObject(item);
            return true;
        }

        protected override bool TryTake(out T item) {
            if(IsDebugEnabled)
                log.Debug("풀에서 요소를 꺼내기를 시도합니다...");

            item = GetObject();
            return true;
        }
    }
}