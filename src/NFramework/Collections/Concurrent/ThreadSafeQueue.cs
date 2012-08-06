using System;
using System.Collections.Generic;
using System.Threading;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// Thread-safety Queue
    /// </summary>
    /// <typeparam name="T">큐에 저장할 항목의 수형</typeparam>
    [Serializable]
    public class ThreadSafeQueue<T> : IQueue<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 내부 큐
        /// </summary>
        protected Queue<T> InnerQueue { get; private set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ThreadSafeQueue() {
            InnerQueue = new Queue<T>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">initial capacity of buffer in Queue</param>
        public ThreadSafeQueue(int capacity) {
            InnerQueue = new Queue<T>(capacity);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="collection">큐에 추가할 요소들</param>
        public ThreadSafeQueue(IEnumerable<T> collection) {
            InnerQueue = new Queue<T>(collection);
        }

        /// <summary>
        /// 요소를 큐에 넣는다.
        /// </summary>
        /// <param name="item">큐에 추가할 요소</param>
        public virtual void Enqueue(T item) {
            if(IsDebugEnabled)
                log.Debug("큐에 지정한 요소를 추가합니다. item=[{0}]", item);

            lock(this) {
                InnerQueue.Enqueue(item);
                Monitor.Pulse(this);
            }
        }

        /// <summary>
        /// 큐에서 요소를 꺼냅니다. 꺼낼 요소가 없으면 기다립니다.
        /// </summary>
        /// <returns></returns>
        public virtual T Dequeue() {
            if(IsDebugEnabled)
                log.Debug("큐에서 요소를 꺼냅니다. 꺼낼 요소가 없으면 기다립니다.");

            lock(this) {
                while(InnerQueue.Count == 0)
                    Monitor.Wait(this);

                return InnerQueue.Dequeue();
            }
        }

        /// <summary>
        /// 큐에서 요소를 꺼내기를 시도한다. 실패시에는 False를 반환하고, item은 default(T)값을 갖는다.
        /// </summary>
        /// <param name="item">꺼낸 요소</param>
        /// <returns>큐에서 요소를 꺼낸지 결과</returns>
        public virtual bool TryDequeue(out T item) {
            if(IsDebugEnabled)
                log.Debug("큐에서 요소를 꺼내기를 시도한다. 꺼낼 요소가 없으면 바로 False를 반환합니다.");

            lock(this) {
                if(InnerQueue.Count > 0) {
                    item = Dequeue();
                    return true;
                }

                item = default(T);
                return false;
            }
        }
    }
}