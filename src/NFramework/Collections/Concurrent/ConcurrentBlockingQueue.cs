using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// 큐 방식의 내부저장소를 가진 <see cref="BlockingCollection{T}"/>입니다.
    /// </summary>
    /// <typeparam name="T">큐 항목의 수형</typeparam>
    [Serializable]
    public class ConcurrentBlockingQueue<T> : BlockingCollection<T> {
        public ConcurrentBlockingQueue() : base(new ConcurrentQueue<T>()) {}
        public ConcurrentBlockingQueue(int boundedCapacity) : base(new ConcurrentQueue<T>(), boundedCapacity) {}
        public ConcurrentBlockingQueue(IEnumerable<T> collection) : base(new ConcurrentQueue<T>(collection)) {}

        public ConcurrentBlockingQueue(IEnumerable<T> collection, int boundedCapacity)
            : base(new ConcurrentQueue<T>(collection), boundedCapacity) {}
    }
}