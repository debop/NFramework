using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// 스택 방식의 내부저장소를 가진 <see cref="BlockingCollection{T}"/>입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ConcurrentBlockingStack<T> : BlockingCollection<T> {
        public ConcurrentBlockingStack() : base(new ConcurrentStack<T>()) {}
        public ConcurrentBlockingStack(int boundedCapacity) : base(new ConcurrentStack<T>(), boundedCapacity) {}
        public ConcurrentBlockingStack(IEnumerable<T> collection) : base(new ConcurrentStack<T>(collection)) {}

        public ConcurrentBlockingStack(IEnumerable<T> collection, int boundedCapacity)
            : base(new ConcurrentStack<T>(collection), boundedCapacity) {}
    }
}