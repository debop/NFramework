using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NSoft.NFramework.Parallelism;
using NSoft.NFramework.Parallelism.DataStructures;
using NSoft.NFramework.Parallelism.Partitioners;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// 생산자-소비자의 중간 버퍼 역할을 수행하는 <see cref="BlockingCollection{T}"/>에 대한 Extension Methods
    /// </summary>
    public static class BlockingCollectionTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="collection"/>의 GetConsumingEnumerable()의 열거자를 배분하는 Partitioner를 빌드합니다.
        /// </summary>
        /// <seealso cref="BlockingCollectionPartitioner{T}"/>
        public static Partitioner<T> GetConsumingPartitioner<T>(this BlockingCollection<T> collection) {
            collection.ShouldNotBeNull("collection");

            if(IsDebugEnabled)
                log.Debug("BlockingCollection의 ConsumingEnumerable()로부터 Partitioner를 생성합니다.");

            return new BlockingCollectionPartitioner<T>(collection);
        }

        /// <summary>
        /// <paramref name="target"/>에 <paramref name="source"/>의 요소들을 추가합니다.
        /// </summary>
        /// <typeparam name="T">요소의 수형</typeparam>
        /// <param name="target">요소를 추가할 BlockingCollection</param>
        /// <param name="source">추가할 요소를 가진 소스</param>
        /// <param name="completeAddingWhenDone">
        /// <paramref name="source"/>의 요소 추가가 완료되면, BlockingCollection인 target의 Add 작업이 완료되었다고 설정한다 (더 이상 추가작업이 없음을 알린다)
        /// </param>
        public static void AddFromEnumerable<T>(this BlockingCollection<T> target, IEnumerable<T> source,
                                                bool completeAddingWhenDone = false) {
            target.ShouldNotBeNull("target");
            source.ShouldNotBeNull("source");

            if(IsDebugEnabled)
                log.Debug("대상 BlockingCollection에 시퀀스의 요소들을 추가합니다. completeAddingWhenDown=[{0}]", completeAddingWhenDone);

            try {
                foreach(var item in source)
                    target.Add(item);
            }
            finally {
                // 요소 추가 작업이 완료되었으므로, 소비자의 GetConsumingEnumerable()가 더 이상 열거하지 않게 된다.
                if(completeAddingWhenDone)
                    target.CompleteAdding();
            }
        }

#if !SILVERLIGHT
        /// <summary>
        /// 지정된 Observable로부터 얻은 요소를 target 컬렉션에 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">요소를 추가할 BlockingCollection</param>
        /// <param name="source">추가할 요소를 가진 소스</param>
        /// <param name="completeAddingWhenDone"></param>
        public static IDisposable AddFromObservable<T>(this BlockingCollection<T> target, IObservable<T> source,
                                                       bool completeAddingWhenDone = false) {
            target.ShouldNotBeNull("target");
            source.ShouldNotBeNull("source");

            // OnNext, OnError, OnComplete 에 대한 Handler를 정의한 Observer 를 생성합니다.
            return
                source.Subscribe(new DelegateBaseObserver<T>(target.Add,
                                                             error => {
                                                                 if(completeAddingWhenDone)
                                                                     target.CompleteAdding();
                                                             },
                                                             () => {
                                                                 if(completeAddingWhenDone)
                                                                     target.CompleteAdding();
                                                             }));
        }
#endif

        /// <summary>
        /// BlockingCollection 인스턴스에 대한 IProducerConsumerCollection Wapping을 통한 Facade를 생성해 제공합니다.
        /// </summary>
        public static IProducerConsumerCollection<T> ToProducerConsumerCollection<T>(this BlockingCollection<T> collection) {
            return ToProducerConsumerCollection(collection, new CancellationToken());
        }

        /// <summary>
        /// BlockingCollection 인스턴스에 대한 IProducerConsumerCollection Wapping을 통한 Facade를 생성해 제공합니다.
        /// </summary>
        public static IProducerConsumerCollection<T> ToProducerConsumerCollection<T>(this BlockingCollection<T> collection,
                                                                                     int msecTimeout = Timeout.Infinite) {
            return ToProducerConsumerCollection(collection, new CancellationToken(), msecTimeout);
        }

        /// <summary>
        /// BlockingCollection 인스턴스에 대한 IProducerConsumerCollection Wapping을 통한 Facade를 생성해 제공합니다.
        /// </summary>
        public static IProducerConsumerCollection<T> ToProducerConsumerCollection<T>(this BlockingCollection<T> collection,
                                                                                     CancellationToken cancellationToken,
                                                                                     int msecTimeout = Timeout.Infinite) {
            collection.ShouldNotBeNull("collection");
            return new ProducerConsumerCollectionWrapper<T>(collection, msecTimeout, cancellationToken);
        }
    }
}