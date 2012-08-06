using System;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// Data bining 을 위한 Thread-safe 컬렉션을 제공합니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="AsyncOperationManager"/>
    [DebuggerDisplay("Count={Count}")]
    [Serializable]
    public class ObservableConcurrentCollection<T> : ProducerConsumerCollectionBase<T>, INotifyCollectionChanged, INotifyPropertyChanged {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly SynchronizationContext _context = AsyncOperationManager.SynchronizationContext;

        public ObservableConcurrentCollection() : this(new ConcurrentQueue<T>()) {}
        public ObservableConcurrentCollection(IProducerConsumerCollection<T> innerCollection) : base(innerCollection) {}

        /// <summary>
        /// Collection 변화에 대한 이벤트
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// 속성 변화에 대한 이벤트
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 이 컬렉션의 관찰자(Observer)에게, 컬렉션의 변화를 통지합니다.
        /// </summary>
        private void NotifyObserversOfChange() {
            var collectionChanged = CollectionChanged;
            var propertyChanged = PropertyChanged;

            if(collectionChanged != null || propertyChanged != null) {
                if(IsDebugEnabled)
                    log.Debug("컬렉션의 변화를 관찰자(Observer)에게 알리기 위해 SynchronizationContext를 이용하여 이벤트를 호출합니다...");

                _context.Post(state => {
                                  if(collectionChanged != null)
                                      collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                                  if(propertyChanged != null)
                                      propertyChanged(this, new PropertyChangedEventArgs("Count"));
                              },
                              null);
            }
        }

        /// <summary>
        /// 요소 추가를 시도합니다.
        /// </summary>
        /// <param name="item">추가할 요소</param>
        /// <returns>요소 추가 여부</returns>
        protected override bool TryAdd(T item) {
            var result = base.TryAdd(item);

            if(result)
                NotifyObserversOfChange();

            return result;
        }

        /// <summary>
        /// 요소를 내부 버퍼에서 꺼내기를 시도합니다.
        /// </summary>
        /// <param name="item">꺼낸 요소</param>
        /// <returns>꺼내기 성공 여부</returns>
        protected override bool TryTake(out T item) {
            var result = base.TryTake(out item);

            if(result)
                NotifyObserversOfChange();

            return result;
        }
    }
}