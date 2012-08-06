using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// <see cref="TaskFactory"/>의 확장 메소드를 제공하는 클래스입니다.
    /// </summary>
    public static partial class TaskFactoryTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Non-generic task factory의 속성 값을 기초로, Generic task factory를 생성합니다.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static TaskFactory<TResult> ToGeneric<TResult>(this TaskFactory factory) {
            factory.ShouldNotBeNull("factory");

            return new TaskFactory<TResult>(factory.CancellationToken,
                                            factory.CreationOptions,
                                            factory.ContinuationOptions,
                                            factory.Scheduler);
        }

        /// <summary>
        /// Generic TaskFactory의 속성값을 기초로, Non-generic TaskFactory를 생성합니다.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="factory">Generic Task Factory</param>
        /// <returns>Non-generic TaskFactory</returns>
        public static TaskFactory ToNonGeneric<TResult>(this TaskFactory<TResult> factory) {
            factory.ShouldNotBeNull("factory");

            return new TaskFactory(factory.CancellationToken,
                                   factory.CreationOptions,
                                   factory.ContinuationOptions,
                                   factory.Scheduler);
        }

        /// <summary>
        /// Task의 schedule을 관리하는 <see cref="TaskScheduler"/>의 인스턴스를 제공합니다. factory의 Scheduler가 없을 시에 <see cref="TaskScheduler.Current"/>를 제공합니다.
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <returns>TaskScheduler의 인스턴스</returns>
        public static TaskScheduler GetTargetScheduler(this TaskFactory factory) {
            return factory.Scheduler ?? TaskScheduler.Current ?? TaskScheduler.Default;
        }

        /// <summary>
        /// Task의 schedule을 관리하는 <see cref="TaskScheduler"/>의 인스턴스를 제공합니다. factory의 Scheduler가 없을 시에 <see cref="TaskScheduler.Current"/>를 제공합니다.
        /// </summary>
        /// <typeparam name="TResult">Task 결과 값의 수형</typeparam>
        /// <param name="factory">TaskFactory</param>
        /// <returns>TaskScheduler의 인스턴스</returns>
        public static TaskScheduler GetTargetScheduler<TResult>(this TaskFactory<TResult> factory) {
            return factory.Scheduler ?? TaskScheduler.Current ?? TaskScheduler.Default;
        }

        /// <summary>
        /// 지정된 TaskCreationOptions 정보를 기초로, TaskContinuationOptions 설정 값을 결정하여 반환합니다.
        /// </summary>
        /// <param name="creationOptions"></param>
        /// <returns></returns>
        internal static TaskContinuationOptions ContinuationOptionsFromCreationOptions(TaskCreationOptions creationOptions) {
            return (TaskContinuationOptions)((creationOptions & TaskCreationOptions.AttachedToParent) |
                                             (creationOptions & TaskCreationOptions.PreferFairness) |
                                             (creationOptions & TaskCreationOptions.LongRunning));
        }

        /// <summary>
        /// 제공된 Task들이 모두 완료되었을 때에, 완료된 Task들을 결과로 제공하는 Task를 빌드합니다. Fork-Join에 해당합니다.
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="tasks">실행할 Task 들</param>
        /// <returns>실행 완료된 Task들을 결과로 가지는 Task</returns>
        public static Task<Task[]> WhenAll(this TaskFactory factory, params Task[] tasks) {
            return factory.ContinueWhenAll(tasks, completedTasks => completedTasks);
        }

        /// <summary>
        /// 제공된 Task들이 모두 완료되었을 때에, 완료된 Task들을 결과로 제공하는 Task를 빌드합니다. Fork-Join에 해당합니다.
        /// </summary>
        /// <typeparam name="TAntecedentResult">실행할 Task의 결과값의 수형</typeparam>
        /// <param name="factory">TaskFactory</param>
        /// <param name="tasks">실행할 Task 들</param>
        /// <returns>실행 완료된 Task들을 결과로 가지는 Task</returns>
        public static Task<Task<TAntecedentResult>[]> WhenAll<TAntecedentResult>(this TaskFactory factory,
                                                                                 params Task<TAntecedentResult>[] tasks) {
            return factory.ContinueWhenAll(tasks, completedTasks => completedTasks);
        }

        /// <summary>
        /// 제공된 Task들 중에 하나라도 완료하면, 제일 처음 완료된 Task를 결과로 제공하는 Task를 빌드합니다. 
        /// 선착순 작업 완료와 의미가 같습니다만, 나머지 작업들에 대한 취소는 하지 않습니다. 
        /// <see cref="ParallelTool.SpeculativeInvoke{T}(System.Func{T}[])"/> 참고.
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="tasks">실행할 Task들</param>
        /// <returns>제일 처음 실행한 Task를 결과로 가지는 Task</returns>
        public static Task<Task> WhenAny(this TaskFactory factory, params Task[] tasks) {
            return factory.ContinueWhenAny(tasks, completedTask => completedTask);
        }

        /// <summary>
        /// 제공된 Task들 중에 하나라도 완료하면, 제일 처음 완료된 Task를 결과로 제공하는 Task를 빌드합니다. 
        /// 선착순 작업 완료와 의미가 같습니다만, 나머지 작업들에 대한 취소는 하지 않습니다. 
        /// <see cref="ParallelTool.SpeculativeInvoke{TSource,TResult}(TSource,System.Func{TSource,TResult}[])"/> 참고.
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="tasks">실행할 Task들</param>
        /// <returns>제일 처음 실행한 Task를 결과로 가지는 Task</returns>
        public static Task<Task<TAntecedentResult>> WhenAny<TAntecedentResult>(this TaskFactory factory,
                                                                               params Task<TAntecedentResult>[] tasks) {
            return factory.ContinueWhenAny(tasks, completedTask => completedTask);
        }

        /// <summary>
        /// 여러 Action들을 순차적으로 실행하도록 Task 실행 프로세스를 구성합니다
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="actions">실행할 Action</param>
        /// <returns></returns>
        public static Task ContinueWithActions(this TaskFactory factory, params Action[] actions) {
            return ContinueWithActions(factory, TaskContinuationOptions.ExecuteSynchronously, actions);
        }

        /// <summary>
        /// 여러 Action들을 순차적으로 실행하도록 Task 실행 프로세스를 구성합니다
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="taskContinuationOptions">작업 간의 연속 시의 옵션</param>
        /// <param name="actions">실행할 Action</param>
        /// <returns></returns>
        public static Task ContinueWithActions(this TaskFactory factory, TaskContinuationOptions taskContinuationOptions,
                                               params Action[] actions) {
            if(IsDebugEnabled)
                log.Debug("여러 Action들을 순차적으로 실행하도록 Task 실행 프로세스를 구성합니다...");

            Task task = null;

            foreach(var action in actions) {
                var toRun = action;

                task = (task == null)
                           ? Task.Factory.StartNew(toRun)
                           : task.ContinueWith(_ => toRun(), taskContinuationOptions);
            }
            return task;
        }

        /// <summary>
        /// <paramref name="factory"/> 환경하에서, <paramref name="action"/>을 수행하는 Task를 생성합니다.
        /// </summary>
        /// <param name="factory">TaskFactory 인스턴스</param>
        /// <param name="action">수행할 메소드</param>
        /// <param name="creationOptions">Task 생성 옵션</param>
        /// <returns>생성된 Task 인스턴스</returns>
        public static Task Create(this TaskFactory factory, Action action, TaskCreationOptions? creationOptions = null) {
            factory.ShouldNotBeNull("factory");
            action.ShouldNotBeNull("action");

            if(IsDebugEnabled)
                log.Debug("Action을 수행하는 Task를 생성합니다. creationOptions=[{0}]", creationOptions);

            return new Task(action, factory.CancellationToken, creationOptions ?? factory.CreationOptions);
        }

        /// <summary>
        /// <paramref name="action"/>을 수행하는 Task를 생성합니다.
        /// </summary>
        /// <param name="factory">TaskFactory 인스턴스</param>
        /// <param name="action">수행할 메소드</param>
        /// <param name="state">메소드 인자값</param>
        /// <param name="creationOptions">Task 생성 옵션</param>
        /// <returns>생성된 Task 인스턴스</returns>
        public static Task Create(this TaskFactory factory, Action<object> action, object state = null,
                                  TaskCreationOptions? creationOptions = null) {
            factory.ShouldNotBeNull("factory");
            action.ShouldNotBeNull("action");

            if(IsDebugEnabled)
                log.Debug("Action<object>을 수행하는 Task를 생성합니다. state=[{0}], creationOptions=[{1}]", state, creationOptions);

            return new Task(action, state, factory.CancellationToken, creationOptions ?? factory.CreationOptions);
        }

        /// <summary>
        /// <paramref name="function"/>을 수행할 Task를 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">function의 반환 값의 수형</typeparam>
        /// <param name="factory">TaskFactory</param>
        /// <param name="function">실행할 함수</param>
        /// <param name="creationOptions">Task 생성 옵션</param>
        /// <returns><paramref name="function"/>을 수행할 Task</returns>
        public static Task<TResult> Create<TResult>(this TaskFactory factory, Func<TResult> function,
                                                    TaskCreationOptions? creationOptions = null) {
            factory.ShouldNotBeNull("factory");
            function.ShouldNotBeNull("function");

            if(IsDebugEnabled)
                log.Debug("Func<{0}>을 수행하는 Task<{0}>를 생성합니다. creationOptions=[{1}]", typeof(TResult).Name, creationOptions);

            return new Task<TResult>(function, factory.CancellationToken, creationOptions ?? factory.CreationOptions);
        }

        /// <summary>
        /// <paramref name="function"/>을 수행할 Task를 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">function의 반환 값의 수형</typeparam>
        /// <param name="factory">TaskFactory</param>
        /// <param name="function">실행할 함수</param>
        /// <param name="state">함수의 입력 인자</param>
        /// <param name="creationOptions">Task 생성 옵션</param>
        /// <returns><paramref name="function"/>을 수행할 Task</returns>
        public static Task<TResult> Create<TResult>(this TaskFactory factory, Func<object, TResult> function, object state = null,
                                                    TaskCreationOptions? creationOptions = null) {
            factory.ShouldNotBeNull("factory");
            function.ShouldNotBeNull("function");

            if(IsDebugEnabled)
                log.Debug("Func<object, {0}>을 수행하는 Task<{0}>를 생성합니다. state=[{1}], creationOptions=[{2}]",
                          typeof(TResult).Name, state, creationOptions);

            return new Task<TResult>(function, state, factory.CancellationToken, creationOptions ?? factory.CreationOptions);
        }

        /// <summary>
        /// <paramref name="function"/>을 수행할 Task를 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">function의 반환 값의 수형</typeparam>
        /// <param name="factory">TaskFactory</param>
        /// <param name="function">실행할 함수</param>
        /// <param name="creationOptions">Task 생성 옵션</param>
        /// <returns><paramref name="function"/>을 수행할 Task</returns>
        public static Task<TResult> Create<TResult>(this TaskFactory<TResult> factory, Func<TResult> function,
                                                    TaskCreationOptions? creationOptions = null) {
            factory.ShouldNotBeNull("factory");
            function.ShouldNotBeNull("function");

            if(IsDebugEnabled)
                log.Debug("Func<{0}>을 수행하는 Task<{0}>를 생성합니다. creationOptions=[{1}]", typeof(TResult).Name, creationOptions);

            return new Task<TResult>(function, factory.CancellationToken, creationOptions ?? factory.CreationOptions);
        }

        /// <summary>
        /// <paramref name="function"/>을 수행할 Task를 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">function의 반환 값의 수형</typeparam>
        /// <param name="factory">TaskFactory</param>
        /// <param name="function">실행할 함수</param>
        /// <param name="state">함수의 입력 인자</param>
        /// <param name="creationOptions">Task 생성 옵션</param>
        /// <returns><paramref name="function"/>을 수행할 Task</returns>
        public static Task<TResult> Create<TResult>(this TaskFactory<TResult> factory,
                                                    Func<object, TResult> function,
                                                    object state = null,
                                                    TaskCreationOptions? creationOptions = null) {
            factory.ShouldNotBeNull("factory");
            function.ShouldNotBeNull("function");

            if(IsDebugEnabled)
                log.Debug("Func<object, {0}>을 수행하는 Task<{0}>를 생성합니다. state=[{1}], creationOptions=[{2}]",
                          typeof(TResult).Name, state, creationOptions);

            return new Task<TResult>(function, state, factory.CancellationToken, creationOptions ?? factory.CreationOptions);
        }

        /// <summary>
        /// <paramref name="source"/>의 요소들 (Task/TaskScheduler)을 비동기적으로 반복하도록 합니다.
        /// </summary>
        /// <param name="factory">대상 TaskFactory</param>
        /// <param name="source">반복할 Task나 TaskScheduler를 요소로 가진 열거자</param>
        /// <param name="state">반환할 Task의 비동기적 상태 값</param>
        /// <param name="cancellationToken">취소를 위한 CancellationToken</param>
        /// <param name="creationOptions">Task 생성 옵션</param>
        /// <param name="scheduler">실행할 Task들의 스케쥴러 </param>
        /// <returns>비동기 반복 작업의 표현하는 Task</returns>
        public static Task Iterate(this TaskFactory factory,
                                   IEnumerable<object> source,
                                   object state = null,
                                   CancellationToken? cancellationToken = null,
                                   TaskCreationOptions? creationOptions = null,
                                   TaskScheduler scheduler = null) {
            factory.ShouldNotBeNull("factory");

            cancellationToken = cancellationToken ?? factory.CancellationToken;
            creationOptions = creationOptions ?? factory.CreationOptions;
            scheduler = scheduler ?? factory.GetTargetScheduler();

            var enumerator = source.GetEnumerator();

            if(enumerator == null)
                throw new InvalidOperationException("Invalid enumrable from source parameter - GetEnumerator() returned null.");

            if(IsDebugEnabled)
                log.Debug("Task의 시퀀스를 비동기적으로 반복하도록 하는 작업을 빌드합니다...");

            // 반환할 Task를 생성하고, Task 완료 시에, enumerator를 메모리 해제하도록 후속 작업을 정의합니다.
            //
            var tcs = new TaskCompletionSource<object>(state, creationOptions.Value);
            tcs.Task.ContinueWith(_ => enumerator.Dispose(),
                                  CancellationToken.None,
                                  TaskContinuationOptions.ExecuteSynchronously,
                                  TaskScheduler.Default);

            // 재귀 호출을 하는 메소드 정의
            //
            Action<Task> recursiveBody = null;

            recursiveBody = antecedent => {
                                try {
                                    // 반복 작업을 계속해야 하고, 반복해야 할 것이 계속될 때, 연속 프로세싱을 위해 continuation을 생성합니다.
                                    // 현재 Task에 대해 한번만 연속 프로세싱을 하기 위해, enumerator가 종료되면 작업을 완료합니다.
                                    if(enumerator.MoveNext()) {
                                        var nextItem = enumerator.Current;

                                        // 요소가 Task라면, 이 Task로부터 반복 작업을 시작한다.
                                        if(nextItem is Task) {
                                            var nextTask = (Task)nextItem;
                                            nextTask.IgnoreExceptions();
                                            nextTask.ContinueWith(recursiveBody).IgnoreExceptions();
                                        }
                                        else if(nextItem is TaskScheduler) {
                                            Task.Factory
                                                .StartNew(() => recursiveBody(null),
                                                          CancellationToken.None,
                                                          TaskCreationOptions.None,
                                                          (TaskScheduler)nextItem)
                                                .IgnoreExceptions();
                                        }
                                        else {
                                            tcs.TrySetException(
                                                new InvalidOperationException("source의 요소가 Task나 TaskScheduler 수형이어야 합니다."));
                                        }
                                    }
                                    else {
                                        // 작업 완료!!!
                                        tcs.TrySetResult(null);
                                    }
                                }
                                catch(Exception ex) {
                                    var oce = ex as OperationCanceledException;

                                    if(oce != null && oce.CancellationToken == cancellationToken.Value)
                                        tcs.TrySetCanceled();
                                    else
                                        tcs.TrySetException(ex);
                                }
                            };

            // 재귀호출을 시작하도록, Task를 실행시킵니다.
            //
            factory.StartNew(() => recursiveBody(null),
                             CancellationToken.None,
                             TaskCreationOptions.None,
                             scheduler ?? TaskScheduler.Default)
                .IgnoreExceptions();

            return tcs.Task;
        }

        /// <summary>
        /// Task를 반환하는 함수의 시퀀스를 순차적으로 비동기 방식으로 실행하고, 그 결과(Task들)를 반환하도록 합니다.
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="functions"></param>
        /// <returns></returns>
        public static Task<IList<Task>> TrackedSequence(this TaskFactory factory, Func<Task>[] functions) {
            factory.ShouldNotBeNull("factory");
            functions.ShouldNotBeNull("functions");

            if(IsDebugEnabled)
                log.Debug("함수 시퀀스를 반복적으로 호출하여, 결과로 반환되는 Task의 컬렉션을 결과로 하는 Task를 반환합니다. 즉 Task<IList<Task>> 를 반환합니다.");

            var tcs = new TaskCompletionSource<IList<Task>>();

            if(functions.Length > 0)
                TrackedSequenceInternal(functions, tcs);
            else
                tcs.TrySetResult(new List<Task>());

            return tcs.Task;
        }

        internal static IEnumerable<Task> TrackedSequenceInternal(IEnumerable<Func<Task>> functions,
                                                                  TaskCompletionSource<IList<Task>> tcs) {
            tcs.ShouldNotBeNull("tcs");

            if(IsDebugEnabled)
                log.Debug("함수 시퀀스를 반복적으로 호출하여, 결과로 반환되는 Task를 열거합니다.");

            var tasks = new List<Task>();

            foreach(var func in functions) {
                Task nextTask = null;
                try {
                    nextTask = func();
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled)
                        log.WarnException("함수 호출에서 예외가 발생했습니다.", ex);

                    tcs.TrySetException(ex);
                }

                if(nextTask == null)
                    yield break;

                yield return nextTask;

                if(nextTask.IsFaulted)
                    break;
            }

            tcs.TrySetResult(tasks);
        }
    }
}