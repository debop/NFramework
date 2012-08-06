using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NSoft.NFramework.Parallelism.Tools {
    // NOTE: Task.Wait()를 호출하면, Task에 예외가 있으면, AggregateException을 발생시킵니다!!!
    // NOTE: 이를 이용하여, 의도적으로 AggregateException을 발생시키기 위해 Wait() 를 호출합니다.

    /// <summary>
    /// <see cref="Task"/>와 <see cref="Task{TResult}"/>에 대한 확장 메소드 들입니다.
    /// </summary>
    public static class TaskTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="task"/>의 후속 작업으로 <paramref name="continuationAction"/>을 설정합니다. 
        /// 후속 작업에 대한 옵션은 <paramref name="factory"/>의 속성을 사용합니다.
        /// </summary>
        /// <param name="task">작업 본체</param>
        /// <param name="continuationAction">후속 작업의 delegate</param>
        /// <param name="factory">후속 작업 연속에 대한 옵션을 설정하기 위한 정보를 담은 TaskFactory</param>
        /// <returns></returns>
        public static Task ContinueWith(this Task task, Action<Task> continuationAction, TaskFactory factory) {
            task.ShouldNotBeNull("task");
            continuationAction.ShouldNotBeNull("continuationAction");
            factory.ShouldNotBeNull("factory");

            return task.ContinueWith(continuationAction,
                                     factory.CancellationToken,
                                     factory.ContinuationOptions,
                                     factory.Scheduler);
        }

        /// <summary>
        /// <paramref name="task"/>의 후속 작업으로 <paramref name="continuationFunction"/>을 설정합니다. 
        /// 후속 작업에 대한 옵션은 <paramref name="factory"/>의 속성을 사용합니다.
        /// </summary>
        /// <param name="task">작업 본체</param>
        /// <param name="continuationFunction">후속 작업의 delegate</param>
        /// <param name="factory">후속 작업 연속에 대한 옵션을 설정하기 위한 정보를 담은 TaskFactory</param>
        /// <returns></returns>
        public static Task<TResult> ContinueWith<TResult>(this Task task, Func<Task, TResult> continuationFunction, TaskFactory factory) {
            task.ShouldNotBeNull("task");
            continuationFunction.ShouldNotBeNull("continuationFunction");
            factory.ShouldNotBeNull("factory");

            return task.ContinueWith(continuationFunction,
                                     factory.CancellationToken,
                                     factory.ContinuationOptions,
                                     factory.Scheduler);
        }

        /// <summary>
        /// <paramref name="task"/>의 후속 작업으로 <paramref name="continuationAction"/>을 설정합니다. 
        /// 후속 작업에 대한 옵션은 <paramref name="factory"/>의 속성을 사용합니다.
        /// </summary>
        /// <param name="task">작업 본체</param>
        /// <param name="continuationAction">후속 작업의 delegate</param>
        /// <param name="factory">후속 작업 연속에 대한 옵션을 설정하기 위한 정보를 담은 TaskFactory</param>
        /// <returns></returns>
        public static Task ContinueWith<TResult>(this Task<TResult> task, Action<Task<TResult>> continuationAction,
                                                 TaskFactory<TResult> factory) {
            task.ShouldNotBeNull("task");
            continuationAction.ShouldNotBeNull("continuationAction");
            factory.ShouldNotBeNull("factory");

            return task.ContinueWith(continuationAction,
                                     factory.CancellationToken,
                                     factory.ContinuationOptions,
                                     factory.Scheduler);
        }

        /// <summary>
        /// <paramref name="task"/>의 후속 작업으로 <paramref name="continuationFunction"/>을 설정합니다. 
        /// 후속 작업에 대한 옵션은 <paramref name="factory"/>의 속성을 사용합니다.
        /// </summary>
        /// <param name="task">작업 본체</param>
        /// <param name="continuationFunction">후속 작업의 delegate</param>
        /// <param name="factory">후속 작업 연속에 대한 옵션을 설정하기 위한 정보를 담은 TaskFactory</param>
        /// <returns></returns>
        public static Task ContinueWith<TResult, TNewResult>(this Task<TResult> task,
                                                             Func<Task<TResult>, TNewResult> continuationFunction,
                                                             TaskFactory<TResult> factory) {
            task.ShouldNotBeNull("task");
            continuationFunction.ShouldNotBeNull("continuationFunction");
            factory.ShouldNotBeNull("factory");

            return task.ContinueWith(continuationFunction,
                                     factory.CancellationToken,
                                     factory.ContinuationOptions,
                                     factory.Scheduler);
        }

        /// <summary>
        /// <paramref name="task"/>가 완료되면, <paramref name="callback"/>을 호출하도록 예약된 새로운 Task를 생성하여, 
        /// <paramref name="task"/>의 수행 결과를 설정한 후 제공합니다.
        /// </summary>
        /// <param name="task">실제 수행할 작업</param>
        /// <param name="callback">작업 후 호출할 callback 함수</param>
        /// <param name="state">callback 함수 호출 시 인자값</param>
        /// <returns>작업 완료와 callback 호출까지 모두를 포함한 작업</returns>
        public static Task ToAsync(this Task task, AsyncCallback callback = null, object state = null) {
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled)
                log.Debug("지정된 Task가 완료되면, callback을 호출하도록 하는 새로운 Task를 빌드합니다.");

            var tcs = new TaskCompletionSource<object>(state);

            task.ContinueWith(_ => {
                                  tcs.SetFromTask(task);
                                  if(callback != null)
                                      callback(tcs.Task);
                              },
                              TaskContinuationOptions.ExecuteSynchronously);
            return tcs.Task;
        }

        /// <summary>
        /// <paramref name="task"/>가 완료되면, <paramref name="callback"/>을 호출하도록 예약된 새로운 Task를 생성하여, 
        /// <paramref name="task"/>의 수행 결과를 설정한 후 제공합니다.
        /// </summary>
        /// <param name="task">실제 수행할 작업</param>
        /// <param name="callback">작업 후 호출할 callback 함수</param>
        /// <param name="state">callback 함수 호출 시 인자값</param>
        /// <returns>작업 완료와 callback 호출까지 모두를 포함한 작업</returns>
        public static Task<TResult> ToAsync<TResult>(this Task<TResult> task, AsyncCallback callback = null, object state = null) {
            task.ShouldNotBeNull("task");

            var tcs = new TaskCompletionSource<TResult>(state);

            task.ContinueWith(_ => {
                                  tcs.SetFromTask(task);
                                  if(callback != null)
                                      callback(tcs.Task);
                              });
            return tcs.Task;
        }

        /// <summary>
        /// 지정한 Task 작업 중에 발생하는 예외를 무시하도록, 작업을 생성합니다.
        /// </summary>
        public static Task IgnoreExceptions(this Task task) {
            task.ShouldNotBeNull("task");

            task.ContinueWith(t => { var ignored = t.Exception; },
                              CancellationToken.None,
                              TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                              TaskScheduler.Default);
            return task;
        }

        /// <summary>
        /// 지정한 Task 작업 중에 발생하는 예외를 무시하도록, 작업을 생성합니다.
        /// </summary>
        public static Task<T> IgnoreExceptions<T>(this Task<T> task) {
            return (Task<T>)((Task)task).IgnoreExceptions();
        }

        /// <summary>
        /// 지정한 Task 작업 중 예외가 발생했을 경우, finally 블럭을 수행하지 않고, 빠져나오도록 하는 작업을 생성합니다.
        /// </summary>
        public static Task FailFastOnException(this Task task) {
            task.ShouldNotBeNull("task");

            task.ContinueWith(t => Environment.FailFast("A task faulted." + Environment.NewLine + t.Exception),
                              CancellationToken.None,
                              TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted,
                              TaskScheduler.Default);
            return task;
        }

        /// <summary>
        /// 지정한 Task 작업 중 예외가 발생했을 경우, finally 블럭을 수행하지 않고, 빠져나오도록 하는 작업을 생성합니다.
        /// </summary>
        public static Task<T> FailFastOnException<T>(this Task<T> task) {
            return (Task<T>)((Task)task).FailFastOnException();
        }

        /// <summary>
        /// 지정한 Task 작업 중 발생한 예외를 전파시킵니다.
        /// </summary>
        public static void PropagateExceptions(this Task task) {
            task.ShouldNotBeNull("task");
            Guard.Assert(() => task.IsCompleted, @"Task가 완료되지 않았습니다. Task Status=[{0}]", task.Status);

            // NOTE: 예외 발생 시에 AggregateException을 발생시키기 위해서, Wait() 를 호출합니다. (Wait()를 호출하였을 때, Task에 예외가 있으면, AggregateException을 발생시킵니다.)
            if(task.IsFaulted)
                task.Wait();
        }

        /// <summary>
        /// 지정한 Task 작업 중 발생한 예외를 전파시킵니다.
        /// </summary>
        public static void PropagateExceptions(this Task[] tasks) {
            tasks.ShouldNotBeNull("tasks");
            Guard.Assert(() => tasks.All(t => t != null), @"모든 Task가 not null 이여야 합니다.");
            Guard.Assert(() => tasks.All(t => t.IsCompleted), @"모든 Task가 완료상태여야 합니다.");

            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 작업 완료를 감시하는 <see cref="TaskObservable{TResult}"/>로 변환합니다.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task) {
            task.ShouldNotBeNull("task");
            return new TaskObservable<TResult> { _task = task };
        }

        internal class TaskObservable<TResult> : IObservable<TResult> {
            internal Task<TResult> _task;

            public IDisposable Subscribe(IObserver<TResult> observer) {
                observer.ShouldNotBeNull("observer");

                var cts = new CancellationTokenSource();

                _task.ContinueWith(antecedent => {
                                       switch(antecedent.Status) {
                                           case TaskStatus.RanToCompletion:
                                               observer.OnNext(antecedent.Result);
                                               observer.OnCompleted();
                                               break;

                                           case TaskStatus.Faulted:
                                               observer.OnError(antecedent.Exception);
                                               break;

                                           case TaskStatus.Canceled:
                                               observer.OnError(new TaskCanceledException(antecedent));
                                               break;
                                       }
                                   },
                                   cts.Token,
                                   TaskContinuationOptions.ExecuteSynchronously,
                                   TaskScheduler.Current);

                return new CancelOnDispose { Source = cts };
            }
        }

        internal class CancelOnDispose : IDisposable {
            internal CancellationTokenSource Source { get; set; }

            void IDisposable.Dispose() {
                Source.Cancel();
            }
        }

        /// <summary>
        /// 지정한 Task가 제한 시간이 지난 후에도 완료되지 못했다면, 작업을 취소하도록 하는 Task로 래핑합니다.
        /// </summary>
        /// <param name="task">실행할 Task</param>
        /// <param name="timeout">제한 시간</param>
        /// <returns>제한 시간이 설정된 Task</returns>
        public static Task WithTimeout(this Task task, TimeSpan timeout) {
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled)
                log.Debug("지정한 작업이 제한시간안에 완료되지 못하면, 작업취소하도록 한다. task=[{0}], timeout=[{1}]", task, timeout);

            var tcs = new TaskCompletionSource<object>(task.AsyncState);

            // timer가 호출되면, task를 취소하도록 합니다.
            //
            var timer = new Timer(state => ((TaskCompletionSource<object>)state).TrySetCanceled(),
                                  tcs,
                                  timeout,
                                  TimeSpan.FromMilliseconds(Timeout.Infinite));

            // 작업이 끝났으면(혹은 취소되었으면), timer를 해제하고, 결과를 설정합니다.
            //
            task.ContinueWith(antecedent => {
                                  timer.Dispose();
                                  tcs.TrySetFromTask(antecedent);
                              },
                              TaskContinuationOptions.ExecuteSynchronously);

            return tcs.Task;
        }

        /// <summary>
        /// 지정한 Task가 제한 시간이 지난 후에도 완료되지 못했다면, 작업을 취소하도록 하는 Task로 래핑합니다.
        /// </summary>
        /// <param name="task">실행할 Task</param>
        /// <param name="timeout">제한 시간</param>
        /// <returns>제한 시간이 설정된 Task</returns>
        public static Task<TResult> WithTimeout<TResult>(this Task<TResult> task, TimeSpan timeout) {
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled)
                log.Debug("지정한 작업이 제한시간안에 완료되지 못하면, 작업취소하도록 한다. task=[{0}], timeout=[{1}]", task, timeout);

            var result = new TaskCompletionSource<TResult>(task.AsyncState);

            // timer가 호출되면, task를 취소하도록 합니다.
            var timer = new Timer(state => ((TaskCompletionSource<TResult>)state).TrySetCanceled(),
                                  result,
                                  timeout,
                                  TimeSpan.FromMilliseconds(-1));

            // 작업이 끝났으면(혹은 취소되었으면), timer를 해제하고, 결과를 설정합니다.
            task.ContinueWith(t => {
                                  timer.Dispose();
                                  result.TrySetFromTask(t);
                              },
                              TaskContinuationOptions.ExecuteSynchronously);

            return result.Task;
        }

        /// <summary>
        /// 지정한 Task가 완료 상태로 되기 전까지는, Parent Task가 완료 상태가 되는 것을 막습니다.
        /// </summary>
        /// <param name="task"></param>
        public static void AttachToParent(this Task task) {
            task.ShouldNotBeNull("task");

            task.ContinueWith(antecedent => antecedent.Wait(),
                              CancellationToken.None,
                              TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.ExecuteSynchronously,
                              TaskScheduler.Default);
        }

#if !SILVERLIGHT
        /// <summary>
        /// 지정한 Task가 WPF의 실행 루프에서 완전히 완료되었을 때까지 기다립니다. (WPF Dispatcher 가 실행완료 될 때까지 기다립니다)
        /// </summary>
        /// <param name="task"></param>
        public static void WaitWithPumping(this Task task) {
            task.ShouldNotBeNull("task");

            var nestedFrame = new DispatcherFrame();
            task.ContinueWith(_ => nestedFrame.Continue = false);
            Dispatcher.PushFrame(nestedFrame);

            task.WaitAsync();
        }
#endif

        /// <summary>
        /// 지정한 Task 가 완료될 때까지 CPU 및 Thread Context Changing에 영향을 안주면서, 기다렸다가, 작업 완료 상태를 반환합니다. (완료/취소/예외)
        /// </summary>
        /// <remarks>
        /// Wait() 함수와는 달리 작업 결과가 취소/예외 시에 <see cref="AggregateException"/>을 발생시키지 않습니다.
        /// Wait() 나 Result 를 호출하면, Thread가 blocking이 된다. 
        /// AsyncWaitHandle.WaitOne()을 사용하면, CPU Cycle에는 영향을 주지 않는다.
        /// </remarks>
        /// <param name="task">실행할 Task</param>
        /// <returns>작업 완료 상태</returns>
        public static TaskStatus WaitAsync(this Task task) {
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled)
                log.Debug("지정한 작업이 완료되기를 기다립니다... AsyncWaitHandle.WaitOne을 통해 Signal을 기다립니다.");

            ((IAsyncResult)task).AsyncWaitHandle.WaitOne();

            return task.Status;
        }

        /// <summary>
        /// 지정한 Task 가 완료될 때까지 기다렸다가, 작업 완료 상태를 반환합니다. (완료/취소/예외)
        /// </summary>
        /// <remarks>
        /// Wait() 함수와는 달리 작업 결과가 취소/예외 시에 <see cref="AggregateException"/>을 발생시키지 않습니다.
        /// Wait() 나 Result 를 호출하면, Thread가 blocking이 된다. 
        /// AsyncWaitHandle.WaitOne()을 사용하면, CPU Cycle에는 영향을 주지 않는다.
        /// </remarks>
        /// <param name="task">실행할 Task</param>
        /// <param name="msecTimeout">실행 제한 시간</param>
        /// <returns>작업 완료 상태</returns>
        public static TaskStatus WaitAsync(this Task task, int msecTimeout) {
            task.ShouldNotBeNull("task");

            var isExpired = ((IAsyncResult)task).AsyncWaitHandle.WaitOne(msecTimeout);

            if(IsDebugEnabled)
                log.Debug("제한시간 [{0}](msec) 동안 작업완료를 위해 대기했습니다. 작업 상태=[{1}], 제한시간 만료 전에 작업완료=[{2}]",
                          msecTimeout, task.Status, isExpired);

            return task.Status;
        }

        /// <summary>
        /// 지정한 Task 가 완료될 때까지 기다렸다가, 작업 완료 상태를 반환합니다. (완료/취소/예외)
        /// </summary>
        /// <remarks>
        /// Wait() 함수와는 달리 작업 결과가 취소/예외 시에 <see cref="AggregateException"/>을 발생시키지 않습니다.
        /// Wait() 나 Result 를 호출하면, Thread가 blocking이 된다. 
        /// AsyncWaitHandle.WaitOne()을 사용하면, CPU Cycle에는 영향을 주지 않는다.
        /// </remarks>
        /// <param name="task">실행할 Task</param>
        /// <param name="timeSpan">실행 제한 시간</param>
        /// <returns>작업 완료 상태</returns>
        public static TaskStatus WaitAsync(this Task task, TimeSpan timeSpan) {
            task.ShouldNotBeNull("task");

            var isExpired = ((IAsyncResult)task).AsyncWaitHandle.WaitOne(timeSpan);

            if(IsDebugEnabled)
                log.Debug("제한시간 [{0}](msec) 동안 작업완료를 위해 대기했습니다. 작업 상태=[{1}], 제한시간 만료 전 작업완료=[{2}]",
                          timeSpan.TotalMilliseconds, task.Status, isExpired);

            return task.Status;
        }

        /// <summary>
        /// 지정한 Task 가 완료될 때까지 기다렸다가, 작업 완료 상태를 반환합니다. (완료/취소/예외)
        /// </summary>
        /// <remarks>
        /// Wait() 함수와는 달리 작업 결과가 취소/예외 시에 <see cref="AggregateException"/>을 발생시키지 않습니다.
        /// Wait() 나 Result 를 호출하면, Thread가 blocking이 된다. 
        /// AsyncWaitHandle.WaitOne()을 사용하면, CPU Cycle에는 영향을 주지 않는다.
        /// </remarks>
        /// <param name="task">실행할 Task</param>
        /// <returns>작업 완료 상태</returns>
        public static TaskStatus WaitForCompletionStatus(this Task task) {
            task.ShouldNotBeNull("task");

            if(IsDebugEnabled)
                log.Debug("지정한 작업이 완료되기를 기다립니다... WaitHandle.WaitOne을 통해 Signal을 기다립니다.");

            ((IAsyncResult)task).AsyncWaitHandle.WaitOne();

            return task.Status;
        }

        /// <summary>
        /// 지정한 Task 가 완료될 때까지 기다렸다가, 작업 완료 상태를 반환합니다. (완료/취소/예외)
        /// </summary>
        /// <remarks>
        /// Wait() 함수와는 달리 작업 결과가 취소/예외 시에 <see cref="AggregateException"/>을 발생시키지 않습니다.
        /// Wait() 나 Result 를 호출하면, Thread가 blocking이 된다. 
        /// AsyncWaitHandle.WaitOne()을 사용하면, CPU Cycle에는 영향을 주지 않는다.
        /// </remarks>
        /// <param name="task">실행할 Task</param>
        /// <param name="millisecondTimeout">실행 제한 시간</param>
        /// <returns>작업 완료 상태</returns>
        public static TaskStatus WaitForCompletionStatus(this Task task, int millisecondTimeout) {
            task.ShouldNotBeNull("task");

            var isNotTimeout = ((IAsyncResult)task).AsyncWaitHandle.WaitOne(millisecondTimeout);

            if(IsDebugEnabled)
                log.Debug("제한시간 [{0}](msec) 동안 기다렸습니다. 작업 상태=[{1}], 제한시간 만료 전 작업완료=[{2}]",
                          millisecondTimeout, task.Status, isNotTimeout);

            return task.Status;
        }

        /// <summary>
        /// 지정한 Task 가 완료될 때까지 기다렸다가, 작업 완료 상태를 반환합니다. (완료/취소/예외)
        /// </summary>
        /// <remarks>
        /// Wait() 함수와는 달리 작업 결과가 취소/예외 시에 <see cref="AggregateException"/>을 발생시키지 않습니다.
        /// Wait() 나 Result 를 호출하면, Thread가 blocking이 된다. 
        /// AsyncWaitHandle.WaitOne()을 사용하면, CPU Cycle에는 영향을 주지 않는다.
        /// </remarks>
        /// <param name="task">실행할 Task</param>
        /// <param name="timeSpan">실행 제한 시간</param>
        /// <returns>작업 완료 상태</returns>
        public static TaskStatus WaitForCompletionStatus(this Task task, TimeSpan timeSpan) {
            task.ShouldNotBeNull("task");

            var isExpired = ((IAsyncResult)task).AsyncWaitHandle.WaitOne(timeSpan);

            if(IsDebugEnabled)
                log.Debug("제한시간 [{0}] msec 동안 작업완료를 위해 대기했습니다. 작업 상태=[{1}], 제한시간 만료 전 작업완료=[{2}]",
                          timeSpan.TotalMilliseconds, task.Status, isExpired);

            return task.Status;
        }
    }
}