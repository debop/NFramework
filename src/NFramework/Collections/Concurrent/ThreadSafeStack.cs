using System;
using System.Collections.Generic;
using System.Threading;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// Multi-thread에 안정된 Stack
    /// </summary>
    /// <typeparam name="T">Stack에 저장할 항목의 수형</typeparam>
    [Serializable]
    public class ThreadSafeStack<T> : IStack<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// Inner stack buffer
        /// </summary>
        protected Stack<T> InnerStack { get; private set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ThreadSafeStack() {
            InnerStack = new Stack<T>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">initial capacity of stack buffer</param>
        public ThreadSafeStack(int capacity) {
            InnerStack = new Stack<T>(capacity);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">initial elements to pushed internal stack</param>
        public ThreadSafeStack(IEnumerable<T> collection) {
            InnerStack = new Stack<T>(collection);
        }

        /// <summary>
        /// 스택에 있는 항목의 갯수
        /// </summary>
        public int Count {
            get { return InnerStack.Count; }
        }

        /// <summary>
        /// 요소를 스택에 추가
        /// </summary>
        /// <param name="item">추가할 요소</param>
        public virtual void Push(T item) {
            if(IsDebugEnabled)
                log.Debug("Stack에 요소를 추가합니다. item=[{0}]", item);

            lock(_syncLock) {
                InnerStack.Push(item);
                Monitor.Pulse(_syncLock);
            }
        }

        /// <summary>
        /// 스택에서 요소를 꺼냄, 꺼낼 요소가 없으면 기다립니다.
        /// </summary>
        /// <returns>요소</returns>
        public virtual T Pop() {
            if(IsDebugEnabled)
                log.Debug("Stack에서 요소를 꺼냅니다. 꺼낼 요소가 없으면 기다립니다...");

            lock(_syncLock) {
                while(InnerStack.Count == 0)
                    Monitor.Wait(_syncLock);

                return InnerStack.Pop();
            }
        }

        /// <summary>
        /// Stack에서 최상위 요소를 조회합니다. Stack이 비었으면 default(T)를 반환합니다.
        /// </summary>
        /// <returns>요소</returns>
        public virtual T Peek() {
            if(IsDebugEnabled)
                log.Debug("Stack에서 최상위 요소를 조회합니다. Stack이 비었으면 기다리지 않고, default(T)를 반환합니다.");

            lock(_syncLock) {
                if(InnerStack.Count == 0)
                    return default(T);

                return InnerStack.Peek();
            }
        }

        /// <summary>
        /// 스텍에서 요소를 꺼내본다. 꺼낼 요소가 없으면 False를 반환하고, item 은 default(T)가 설정된다.
        /// </summary>
        /// <param name="item">꺼낼 요소</param>
        /// <returns>꺼낼 요소가 없으면 False를 반환하고, item 은 default(T)가 설정된다.</returns>
        public virtual bool TryPop(out T item) {
            if(IsDebugEnabled)
                log.Debug("스텍에서 요소를 꺼내본다. 꺼낼 요소가 없으면 기다리지 않고, False를 반환하고, item 은 default(T)가 설정된다.");

            lock(_syncLock) {
                if(InnerStack.Count > 0) {
                    item = InnerStack.Pop();
                    return true;
                }

                item = default(T);
                return false;
            }
        }
    }
}