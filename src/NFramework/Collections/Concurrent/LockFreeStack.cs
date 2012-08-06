using System;
using System.Threading;

namespace NSoft.NFramework.Collections.Concurrent {
    /// <summary>
    /// Thread-Safe 하면서, Lock을 사용하지 않고, Stack을 구현한다.
    /// 참고 사이트 : http://msdn.microsoft.com/ko-kr/magazine/cc163427.aspx
    /// </summary>
    /// <remarks>
    /// 제한과 차단에 의해 복잡해지기는 하지만 잠금을 사용하여 스레드로부터 보호되는 컬렉션을 만들기는 상당히 쉽습니다. 
    /// 그러나 간단히 마지막에 들어간 것부터 사용(LIFO)하는 스택 데이터 구조를 통해 모든 조율이 이루어지는 경우 잠금을 사용하는 데 따르는 비용이 필요 이상으로 높아질 수 있습니다. 
    /// 스레드의 중요 영역인 잠금이 유지되는 동안에는 시작과 끝이 있으며 이 시간은 여러 명령이 실행되는 동안에 해당합니다. 
    /// 잠금을 유지하면 다른 스레드가 동시에 읽고 쓸 수 없습니다. 이를 통해 원하던 직렬화를 이룰 수 있지만 이것은 필요 이상으로 엄격하게 강력합니다. 
    /// 여기에서는 단순히 스택에 대해 요소를 푸시 및 팝하는 것이며 두 작업 모두 일반적인 읽기와 단일 compare-and-swap 쓰기를 통해 수행할 수 있습니다.
    /// 이러한 사실을 활용하여 확장성이 더 우수하며 스레드가 불필요하게 대기하도록 요구하지 않는 잠금 해제 스택을 작성할 수 있습니다.
    /// 이 알고리즘은 다음과 같이 작동합니다.
    /// 스택을 나타내는 데는 연결된 목록이 사용되며 목록의 헤드는 스택의 맨 위를 나타내고 m_head 필드에 저장됩니다.
    /// 스택에 새 항목을 푸시할 때는 스택에 푸시할 값으로 새 노드를 구성하고, m_head 필드 값을 로컬로 읽으며, 
    /// 이 값을 새 노드의 m_next 필드에 저장한 다음 원자성 Interlocked.CompareExchange를 수행하여 스택의 현재 헤드를 대체합니다. 
    /// 처음 읽은 이후에 이 시퀀스의 어떤 시점에서라도 헤드가 변경된 경우 CompareExchange는 실패하며 스레드는 루프 백을 수행하고 전체 시퀀스를 다시 시도해야 합니다. 
    /// 팝 역시 비슷하며 간단합니다. m_head를 읽고 로컬 복사본의 m_next 참조로 교환을 시도합니다.
    ///  실패하는 경우 그림 8에 서 보여 주고 있는 것처럼 계속 시도합니다. Win32에서는 비슷한 알고리즘을 사용하여 작성한 SList라는 유사 데이터 구조를 제공합니다.
    /// </remarks>
    /// 
    /// <typeparam name="T">Stack에 저장될 요소의 수형</typeparam>
    /// 
    [Serializable]
    public class LockFreeStack<T> : IStack<T> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private volatile StackNode<T> _head;

        /// <summary>
        /// 새로운 요소를 Stack에 추가합니다. 
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item) {
            if(IsDebugEnabled)
                log.Debug("새로운 요소를 Stack에 추가합니다. item=[{0}]", item);

            var node = new StackNode<T>(item);
            StackNode<T> current;

            do {
                current = _head;
                node.Next = current;
#pragma warning disable 0420
            } while(_head != current || Interlocked.CompareExchange(ref _head, node, current) != current);
#pragma warning restore 0420
        }

        /// <summary>
        /// 최상위 요소를 스택에서 꺼냅니다.
        /// </summary>
        /// <returns></returns>
        public T Pop() {
            if(IsDebugEnabled)
                log.Debug("최상위 요소를 스택에서 꺼냅니다...");

            StackNode<T> current;
            var spinWait = new System.Threading.SpinWait();


            while(true) {
                StackNode<T> next;
                do {
                    current = _head;
                    if(current == null)
                        break;
                    next = current.Next;
#pragma warning disable 0420
                } while(_head != current || Interlocked.CompareExchange(ref _head, next, current) != current);
#pragma warning restore 0420

                if(current != null)
                    break;
                spinWait.SpinOnce();
            }

            if(IsDebugEnabled)
                log.Debug("스택에서 요소를 꺼냈습니다. item=[{0}]", current.Item);

            return current.Item;
        }

        /// <summary>
        /// 스택에서 가장 최상위 요소를 조회합니다.
        /// </summary>
        /// <returns></returns>
        public T Peek() {
            if(IsDebugEnabled)
                log.Debug("스택에서 가장 최상위 요소를 조회합니다...");

            if(_head == null)
                return default(T);

            var current = _head;

            while(current.Next != null) {
                current = current.Next;
            }

            if(IsDebugEnabled)
                log.Debug("스택에서 요소를 조회했습니다. item=[{0}]", current.Item);

            return current.Item;
        }

        /// <summary>
        /// <see cref="LockFreeStack{T}"/>의 요소 노드
        /// </summary>
        /// <typeparam name="TItem">노드가 가지는 값의 수형</typeparam>
        [Serializable]
        private class StackNode<TItem> {
            internal StackNode(TItem item) {
                Item = item;
            }

            internal TItem Item { get; private set; }
            internal StackNode<TItem> Next { get; set; }
        }
    }
}