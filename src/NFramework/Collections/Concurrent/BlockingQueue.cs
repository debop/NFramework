using System;
using System.Collections.Generic;
using System.Threading;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// 차단 큐
    /// 참고 사이트 : http://msdn.microsoft.com/ko-kr/magazine/cc163427.aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 공유 메모리 아키텍처에서는 둘 이상 작업 간에 유일한 동기화 지점이 중앙의 공유 컬렉션 데이터 구조인 경우가 많습니다. 
    /// 한 개 이상의 작업이 한 개 이상의 다른 작업에서 소비할 "업무" 생성을 담당하는 구조를 생산자/소비자 관계라고 합니다. 
    /// 이러한 데이터 구조를 위한 간단한 동기화는 일반적으로 복잡하지 않으며 Monitor 또는 ReaderWriterLock을 사용하면 해결되지만 
    /// 버퍼가 비게 되는 경우의 조율이 까다롭습니다. 이러한 문제는 일반적으로 차단 큐를 통해 해결됩니다.
    /// 실제로 차단 큐에는 큐가 비었을 때만 소비자가 차단되는 간단한 것에서부터 
    /// 각 생산자가 정확히 한 개의 소비자와 "짝을 이루어" 소비자가 큐에 저장된 항목을 처리하기 전까지 생산자가 차단되고 
    /// 비슷하게 생산자가 항목을 제공하기 전까지 소비자가 차단되는 복잡한 것까지 몇 가지 변형이 존재합니다. 
    /// 
    /// 처음 들어간 것부터 사용(FIFO)하는 순서가 일반적이지만 필수적인 것은 아닙니다. 
    /// 이를 구현하려면 동기화로 간단한 Queue{T}를 래핑하면 됩니다. 어떤 종류의 동기화일까요? 
    /// 스레드는 큐에 요소를 추가할 때마다 소비자가 큐에서 요소를 제거할 때까지 대기하고 반환합니다. 
    /// 큐에서 요소를 제거할 때 버퍼가 비어 있으면 스레드는 새 요소가 추가될 때까지 대기해야 합니다. 
    /// 그리고 물론 큐에서 항목을 제거할 때 소비자는 생산자에게 생산자의 항목을 가져갔다는 신호를 보내야 합니다
    /// </remarks>
    [Serializable]
    public class BlockingQueue<T> : IQueue<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly Queue<Cell<T>> _queue = new Queue<Cell<T>>();

        /// <summary>
        /// 큐에 새로운 요소를 추가하고, 요소를 꺼내려고 대기하는 쓰레드에게 신호를 보냅니다.
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item) {
            if(IsDebugEnabled)
                log.Debug("큐에 새로운 요소를 추가합니다. item=[{0}]", item);

            var cell = new Cell<T>(item);

            lock(_queue) {
                _queue.Enqueue(cell);
                Monitor.Pulse(_queue);
                Monitor.Wait(_queue);
            }
        }

        /// <summary>
        /// 큐에서 요소를 꺼냅니다. 없으면 큐에 요소가 채워질때까지 기다립니다.
        /// </summary>
        /// <returns></returns>
        public T Dequeue() {
            if(IsDebugEnabled)
                log.Debug("큐에서 요소를 꺼냅니다. 없으면 큐에 요소가 채워질때까지 기다립니다.");

            Cell<T> cell;

            lock(_queue) {
                // 요소가 채워질 때가지 기다린다.
                while(_queue.Count == 0)
                    Monitor.Wait(_queue);

                cell = _queue.Dequeue();
                Monitor.Pulse(_queue);
            }

            if(IsDebugEnabled)
                log.Debug("큐에서 요소를 꺼냈습니다!!! item=" + cell.Item);

            return cell.Item;
        }

        /// <summary>
        /// 큐 요소
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        private class Cell<TItem> {
            /// <summary>
            /// 생성자
            /// </summary>
            /// <param name="item"></param>
            internal Cell(TItem item) {
                Item = item;
            }

            /// <summary>
            /// 항목 정보
            /// </summary>
            internal TItem Item { get; private set; }
        }
    }
}