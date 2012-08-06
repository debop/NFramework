using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// UI 스레드에 Post 하는 것과 같은 것 들을 비동기 방식으로 호출하도록 합니다. 
    /// SynchronizationContext.Post() 자체가 비동기 방식입니다만 (Send()와는 달리), 병렬은 아닙니다. 
    /// AsyncCall은 호출해야 할 Item을 내부 버퍼에 담아두었다가, 스케쥴에 따라, 해당 Handler를 호출해 줍니다.
    /// </summary>
    /// <typeparam name="T">처리할 데이타의 수형</typeparam>
    public sealed class AsyncCall<T> : MarshalByRefObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ConcurrentQueue<T> _queue;
        private readonly Delegate _handler;
        private readonly int _maxItemsPerTask;
        private readonly TaskFactory _taskFactory;
        private readonly ParallelOptions _parallelOptions;
        private int _processingCount;

        #region << Constructors >>

        public AsyncCall() : this(1, Int32.MaxValue, null) {}
        public AsyncCall(int maxDegreeOfParallelism) : this(maxDegreeOfParallelism, Int32.MaxValue, null) {}

        public AsyncCall(int maxDegreeOfParallelism = 1, int maxItemsPerTask = Int32.MaxValue, TaskScheduler scheduler = null) {
            maxDegreeOfParallelism.ShouldBePositiveOrZero("maxDegreeOfParallelism");
            maxItemsPerTask.ShouldBePositive("maxItemsPerTask");

            scheduler = scheduler ?? TaskScheduler.Default;

            _queue = new ConcurrentQueue<T>();
            _maxItemsPerTask = maxItemsPerTask;
            _taskFactory = new TaskFactory(scheduler);

            if(maxItemsPerTask > 1)
                _parallelOptions = new ParallelOptions
                                   {
                                       MaxDegreeOfParallelism = maxDegreeOfParallelism,
                                       TaskScheduler = scheduler
                                   };
        }

        public AsyncCall(Action<T> actionHandler, int maxDegreeOfParallelism = 1, int maxItemsPerTask = Int32.MaxValue,
                         TaskScheduler scheduler = null)
            : this(maxDegreeOfParallelism, maxItemsPerTask, scheduler) {
            actionHandler.ShouldNotBeNull("actionHandler");
            _handler = actionHandler;
        }

        public AsyncCall(Func<T, Task> functionHandler, int maxDegreeOfParallelism = 1, TaskScheduler scheduler = null)
            : this(maxDegreeOfParallelism, 1, scheduler) {
            functionHandler.ShouldNotBeNull("functionHandler");
            _handler = functionHandler;
        }

        #endregion

        /// <summary>
        /// 처리할 아이템을 등록합니다. 이 아이템은 미리 지정된 Handler에게 비동기적으로 전달됩니다.
        /// </summary>
        /// <param name="item">처리할 항목</param>
        public void Post(T item) {
            if(IsDebugEnabled)
                log.Debug("비동기 호출을 위해, 호출용 인자 값을 전달받았습니다... item=[{0}]", item);

            lock(_queue) {
                _queue.Enqueue(item);

                if(_handler is Action<T>) {
                    if(_processingCount == 0) {
                        _processingCount = 1;
                        _taskFactory.StartNew(ProcessItemsActionTaskBody);
                    }
                }
                else if(_handler is Func<T, Task>) {
                    // 병렬 처리가 가능하고, 큐가 비어있지 않다면
                    //
                    var canProcessInParallel = _parallelOptions != null &&
                                               _processingCount < _parallelOptions.MaxDegreeOfParallelism &&
                                               _queue.IsEmpty == false;

                    if(_processingCount == 0 || canProcessInParallel) {
                        _processingCount++;
                        _taskFactory.StartNew(ProcessItemsFunctionTaskBody, null);
                    }
                }
                else
                    throw new InvalidOperationException("_handler is an invalid delegate type.");
            }

            if(IsDebugEnabled)
                log.Debug("비동기 호출 대상인 Handler에게 item[{0}] 정보를 전달했습니다.", item);
        }

        /// <summary>
        /// 내부 큐에서 처리할 아이템을 가져옵니다. 단 Task당 최대 아이템 수를 넘지 않는 범위에서...
        /// </summary>
        /// <returns></returns>
        private IEnumerable<T> GetItemsToProcess() {
            var processedCount = 0;

            T nextItem;

            while(processedCount < _maxItemsPerTask && _queue.TryDequeue(out nextItem)) {
                yield return nextItem;
                processedCount++;
            }
        }

        /// <summary>
        /// Handler가 Action{T} 일 경우 실제 아이템을 처리합니다.
        /// </summary>
        private void ProcessItemsActionTaskBody() {
            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 지정된 Action을 호출합니다...");

            try {
                var handler = (Action<T>)_handler;

                if(_parallelOptions == null)
                    GetItemsToProcess().RunEach(handler);
                else
                    Parallel.ForEach(GetItemsToProcess(), _parallelOptions, handler);
            }
            finally {
                lock(_queue) {
                    // 아직도 호출할 아이템이 남아있다면, 바로 처리할 수 있도록 작업을 생성합니다.
                    if(_queue.IsEmpty == false)
                        _taskFactory.StartNew(ProcessItemsActionTaskBody, TaskCreationOptions.PreferFairness);
                    else
                        _processingCount = 0;
                }
            }

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 지정된 Action을 호출했습니다!!!");
        }

        /// <summary>
        /// Handler가 Func{T, Task} 형식일 경우, 실제 아이템을 처리합니다.
        /// </summary>
        /// <param name="state"></param>
        private void ProcessItemsFunctionTaskBody(object state) {
            var anotherTaskQueued = false;

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 지정된 함수를 호출합니다... state=[{0}]", state);

            try {
                var handler = (Func<T, Task>)_handler;

                // 내부 큐에 전달할 아이템이 있다면...
                T nextItem;
                if(_queue.TryDequeue(out nextItem)) {
                    // handler를 실행해서 반환받은 task가 null이라면, 호출할 수 없다. 그러므로 다음 아이템을 얻도록 재귀호출한다.
                    // task가 null이 아니면, 실행시키고, 다음 작업으로 재귀호출하도록 한다.
                    var task = handler(nextItem);

                    if(task != null)
                        task.ContinueWith(ProcessItemsFunctionTaskBody, _taskFactory.Scheduler);
                    else
                        _taskFactory.StartNew(ProcessItemsFunctionTaskBody, null);

                    anotherTaskQueued = true;
                }
            }
            finally {
                if(anotherTaskQueued == false) {
                    lock(_queue) {
                        // 큐에 전달할 아이템이 아직 남아 있다면, 호출합니다.
                        if(_queue.IsEmpty == false)
                            _taskFactory.StartNew(ProcessItemsFunctionTaskBody, null);
                        else
                            _processingCount--;
                    }
                }
            }

            if(IsDebugEnabled)
                log.Debug("비동기 방식으로 지정된 함수를 호출했습니다!!!");
        }
    }
}