using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NSoft.NFramework.Parallelism;
using NSoft.NFramework.Parallelism.DataStructures;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// <see cref="IProducerConsumerCollection{T}"/>를 위한 확장 메소드 들입니다.
    /// </summary>
    public static class ProducerConsumerCollectionTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 컬렉션의 모든 요소를 제거합니다.
        /// </summary>
        public static void Clear<T>(this IProducerConsumerCollection<T> collection) {
            collection.ShouldNotBeNull("collection");

            if(IsDebugEnabled)
                log.Debug("공급자-소비자 컬렉션의 모든 요소를 제거합니다...");

            T ignored;
            while(collection.TryTake(out ignored)) {}
        }

        /// <summary>
        /// 컬렉션의 요소들을 소비자용의 열거자로 반환합니다.
        /// </summary>
        public static IEnumerable<T> GetConsumingEnumerable<T>(this IProducerConsumerCollection<T> collection) {
            collection.ShouldNotBeNull("collection");

            T item;
            while(collection.TryTake(out item))
                yield return item;
        }

        /// <summary>
        /// <paramref name="source"/> 시퀀스의 요소를 <paramref name="target"/> 컬렉션에 요소로 추가합니다.
        /// </summary>
        public static void AddFromEnumerable<T>(this IProducerConsumerCollection<T> target, IEnumerable<T> source) {
            target.ShouldNotBeNull("target");
            source.ShouldNotBeNull("source");

            foreach(var item in source)
                target.TryAdd(item);
        }

#if !SILVERLIGHT
        /// <summary>
        /// <paramref name="source"/>의 요소를 <paramref name="target"/> 컬렉션에 추가합니다.
        /// </summary>
        public static IDisposable AddFromObservable<T>(this IProducerConsumerCollection<T> target, IObservable<T> source) {
            target.ShouldNotBeNull("target");
            source.ShouldNotBeNull("source");

            var observer = new DelegateBaseObserver<T>(item => target.TryAdd(item));
            return source.Subscribe(observer);
        }
#endif

        /// <summary>
        /// Producer만 있는 (Add 만 가능한) collection을 반환합니다.
        /// </summary>
        public static IProducerConsumerCollection<T> ToProducerOnlyCollection<T>(this IProducerConsumerCollection<T> collection) {
            return new ProduceOrConsumeOnlyCollection<T>(collection, true);
        }

        /// <summary>
        /// Consumer만 있는 (Take 만 가능한) collection을 반환합니다.
        /// </summary>
        public static IProducerConsumerCollection<T> ToConsumerOnlyCollection<T>(this IProducerConsumerCollection<T> collection) {
            return new ProduceOrConsumeOnlyCollection<T>(collection, false);
        }
    }
}