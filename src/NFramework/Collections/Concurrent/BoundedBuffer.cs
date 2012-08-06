using System;
using System.Collections.Generic;
using System.Threading;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// 요소의 수가 제한된 버퍼이고, Producer-Consumer 패턴을 구현한 버퍼입니다.
    /// 참고 사이트 : http://msdn.microsoft.com/ko-kr/magazine/cc163427.aspx
    /// </summary>
    /// <typeparam name="T">요소의 수형</typeparam>
    [Serializable]
    public sealed class BoundedBuffer<T> : IQueue<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private int _consumersWaiting;
        private int _producersWaiting;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="bufferSize">버퍼의 최대 크기</param>
        public BoundedBuffer(int bufferSize) {
            BufferSize = Math.Max(1, bufferSize);
            Queue = new Queue<T>(bufferSize);
        }

        /// <summary>
        /// 버퍼의 최대 크기
        /// </summary>
        public int BufferSize { get; private set; }

        /// <summary>
        /// 버퍼용 큐
        /// </summary>
        internal Queue<T> Queue { get; private set; }

        /// <summary>
        /// 버퍼에 새로운 요소를 추가합니다. 버퍼가 꽊 찼다면 비워질 때까지 기다립니다.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item) {
            if(IsDebugEnabled)
                log.Debug("버퍼에 새로운 요소를 추가합니다. 버퍼가 꽊 찼다면 비워질때까지 기다립니다. item=[{0}]", item);

            lock(Queue) {
                // 큐가 제한된 버퍼 사이즈로 꽉 찼다면, 비워질 때가지 기다린다.
                while(Queue.Count == BufferSize - 1) {
                    _producersWaiting++;
                    Monitor.Wait(Queue);
                    _producersWaiting--;
                }

                Queue.Enqueue(item);

                if(_consumersWaiting > 0)
                    Monitor.PulseAll(Queue);
            }

            if(IsDebugEnabled)
                log.Debug("새로운 요소를 추가했습니다. item=[{0}]", item);
        }

        /// <summary>
        /// 버퍼에서 요소를 꺼냅니다. 없으면, 요소가 채워질때까지 기다립니다.
        /// </summary>
        /// <returns></returns>
        public T Dequeue() {
            if(IsDebugEnabled)
                log.Debug("버퍼에서 요소를 꺼냅니다. 없으면, 요소가 채워질 때까지 기다립니다.");

            T item;

            lock(Queue) {
                while(Queue.Count == 0) {
                    _consumersWaiting++;
                    Monitor.Wait(Queue);
                    _consumersWaiting--;
                }

                item = Queue.Dequeue();
                if(_producersWaiting > 0)
                    Monitor.PulseAll(Queue);
            }

            if(IsDebugEnabled)
                log.Debug("버퍼에서 요소를 꺼냈습니다. item=[{0}]", item);

            return item;
        }

        /// <summary>
        /// 버퍼에서 요소 꺼내기를 시도합니다. 요소가 꺼냈으면 True를 반환하고, 요소가 없으면 False를 반환합니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryDequeue(out T item) {
            lock(Queue) {
                if(Queue.Count > 0) {
                    item = Dequeue();
                    return true;
                }
                item = default(T);
                return false;
            }
        }
    }
}