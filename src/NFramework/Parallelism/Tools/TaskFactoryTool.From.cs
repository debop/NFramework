using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class TaskFactoryTool {
        /// <summary>
        /// <paramref name="exception"/> 예외를 Fault 상태인 작업을 생성합니다.
        /// </summary>
        /// <param name="factory">Task Factory 인스턴스</param>
        /// <param name="exception">예외</param>
        /// <returns>예외로 Fault 상태인 <see cref="Task"/> 인스턴스</returns>
        public static Task FromException(this TaskFactory factory, Exception exception) {
            if(IsDebugEnabled)
                log.Debug("TaskFactory를 사용하여, 예외[{0}]를 가지는 Task 인스턴스를 생성합니다.", exception);

            var tcs = new TaskCompletionSource<object>(factory.ContinuationOptions);
            tcs.TrySetException(exception);

            return tcs.Task;
        }

        /// <summary>
        /// <paramref name="exception"/> 예외를 Fault 상태인 작업을 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">Task의 결과 값 형식</typeparam>
        /// <param name="factory">Task Factory 인스턴스</param>
        /// <param name="exception">예외 정보</param>
        /// <returns>예외로 Fault 상태인 <see cref="Task{TResult}"/> 인스턴스</returns>
        public static Task<TResult> FromException<TResult>(this TaskFactory factory, Exception exception) {
            if(IsDebugEnabled)
                log.Debug("TaskFactory를 사용하여, 예외[{0}]를 가지는 Task<{1}> 인스턴스를 생성합니다.", exception, typeof(TResult).Name);

            var tcs = new TaskCompletionSource<TResult>(factory.ContinuationOptions);
            tcs.TrySetException(exception);

            return tcs.Task;
        }

        /// <summary>
        /// <paramref name="exception"/> 예외를 Fault 상태인 작업을 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">Task의 결과 값 형식</typeparam>
        /// <param name="factory">Task Factory 인스턴스</param>
        /// <param name="exception">예외</param>
        /// <returns>예외로 Fault 상태인 <see cref="Task"/> 인스턴스</returns>
        public static Task<TResult> FromException<TResult>(this TaskFactory<TResult> factory, Exception exception) {
            if(IsDebugEnabled)
                log.Debug("TaskFactory를 사용하여, 예외[{0}]를 가지는 Task<{1}> 인스턴스를 생성합니다.", exception, typeof(TResult).Name);

            var tcs = new TaskCompletionSource<TResult>(factory.ContinuationOptions);
            tcs.TrySetException(exception);

            return tcs.Task;
        }

        /// <summary>
        /// <paramref name="result"/>를 결과값으로 가지는 완료된 Task{TResult} 인스턴스를 반환합니다.
        /// </summary>
        /// <typeparam name="TResult">Task의 결과 값 형식</typeparam>
        /// <param name="factory">Task Factory 인스턴스</param>
        /// <param name="result">Task의 결과 값</param>
        /// <returns><paramref name="result"/> 결과 값이 설정된 <see cref="Task{TResult}"/> 인스턴스</returns>
        public static Task<TResult> FromResult<TResult>(this TaskFactory factory, TResult result) {
            if(IsDebugEnabled)
                log.Debug("TaskFactory를 사용하여, 결과[{0}]를 가지는 Task<{1}> 인스턴스를 생성합니다.", result, typeof(TResult).Name);

            var tcs = new TaskCompletionSource<TResult>(factory.CreationOptions);
            tcs.TrySetResult(result);

            return tcs.Task;
        }

        /// <summary>
        /// <paramref name="result"/>를 결과값으로 가지는 완료된 Task{TResult} 인스턴스를 반환합니다.
        /// </summary>
        /// <typeparam name="TResult">Task의 결과 값 형식</typeparam>
        /// <param name="factory">Task Factory 인스턴스</param>
        /// <param name="result">Task의 결과 값</param>
        /// <returns><paramref name="result"/> 결과 값이 설정된 <see cref="Task{TResult}"/> 인스턴스</returns>
        public static Task<TResult> FromResult<TResult>(this TaskFactory<TResult> factory, TResult result) {
            if(IsDebugEnabled)
                log.Debug("TaskFactory를 사용하여, 결과[{0}]를 가지는 Task<{1}> 인스턴스를 생성합니다.", result, typeof(TResult).Name);

            var tcs = new TaskCompletionSource<TResult>(factory.CreationOptions);
            tcs.TrySetResult(result);

            return tcs.Task;
        }

        /// <summary>
        /// 지정된 wait handle이 signal이 켜졌을 때 완료하는 Task를 빌드합니다.
        /// 비동기 방식의 작업 중 WaitHandle만 있을 경우, Task로 Wrapping하여, 후속 작업 등을 정의할 수 있도록 할 수 있습니다.
        /// </summary>
        /// <param name="factory">target factory</param>
        /// <param name="waitHandle">작업 완료를 결정하는 wait handle</param>
        /// <returns><paramref name="waitHandle"/>에 신호가 들어오면, 완료되는 작업</returns>
        public static Task FromAsync(this TaskFactory factory, WaitHandle waitHandle) {
            factory.ShouldNotBeNull("factory");
            waitHandle.ShouldNotBeNull("waitHandle");

            if(IsDebugEnabled)
                log.Debug("waitHandle의 Signal을 대기하고, Signal이 켜지면, 완료되는 Task를 빌드합니다.");

            var tcs = new TaskCompletionSource<object>();

            // 1. waitHandle에 signal이 켜지면, task를 완료시키는 callback 함수를 등록합니다.
            //
            var rwh = ThreadPool.RegisterWaitForSingleObject(waitHandle,
                                                             delegate { tcs.TrySetResult(null); },
                                                             null,
                                                             Timeout.Infinite,
                                                             true);

            var task = tcs.Task;

            // 2. task 완료시에 등록된 WaitHandle을 제거하기 위한 후속 작업을 정의합니다.
            //
            task.ContinueWith(_ => rwh.Unregister(waitHandle), TaskContinuationOptions.ExecuteSynchronously);

            return task;
        }

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 실행하는 <see cref="Task"/>를 빌드합니다.
        /// </summary>
        public static Task FromAsync(this TaskFactory factory,
                                     Action action,
                                     object state = null) {
            factory.ShouldNotBeNull("factory");
            action.ShouldNotBeNull("action");

            var asyncAction = new Action(action);

            return factory.FromAsync(asyncAction.BeginInvoke, asyncAction.EndInvoke, state);
        }

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 실행하는 <see cref="Task"/>를 빌드합니다.
        /// </summary>
        public static Task FromAsync<T>(this TaskFactory factory,
                                        Action<T> action,
                                        T arg1,
                                        object state = null) {
            factory.ShouldNotBeNull("factory");
            action.ShouldNotBeNull("action");

            var asyncAction = new Action<T>(action);

            return factory.FromAsync(asyncAction.BeginInvoke, asyncAction.EndInvoke, arg1, state);
        }

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 실행하는 <see cref="Task"/>를 빌드합니다.
        /// </summary>
        public static Task FromAsync<T1, T2>(this TaskFactory factory,
                                             Action<T1, T2> action,
                                             T1 arg1,
                                             T2 arg2,
                                             object state = null) {
            factory.ShouldNotBeNull("factory");
            action.ShouldNotBeNull("action");

            var asyncAction = new Action<T1, T2>(action);
            return factory.FromAsync(asyncAction.BeginInvoke, asyncAction.EndInvoke, arg1, arg2, state);
        }

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 실행하는 <see cref="Task"/>를 빌드합니다.
        /// </summary>
        public static Task FromAsync<T1, T2, T3>(this TaskFactory factory,
                                                 Action<T1, T2, T3> action,
                                                 T1 arg1,
                                                 T2 arg2,
                                                 T3 arg3,
                                                 object state = null) {
            factory.ShouldNotBeNull("factory");
            action.ShouldNotBeNull("action");

            var asyncAction = new Action<T1, T2, T3>(action);

            // Silverlight에서는 3개의 인자를 지원하지 않는다.
            var ar = asyncAction.BeginInvoke(arg1, arg2, arg3, null, state);
            return Task.Factory.StartNew(() => asyncAction.EndInvoke(ar));
        }

        /// <summary>
        /// <paramref name="func"/>을 비동기 방식으로 실행하는 <see cref="Task{TResult}"/>를 빌드합니다.
        /// </summary>
        public static Task<TResult> FromAsync<TResult>(this TaskFactory factory,
                                                       Func<TResult> func,
                                                       object state = null) {
            factory.ShouldNotBeNull("factory");
            func.ShouldNotBeNull("func");

            var asyncFunc = new Func<TResult>(func);

            return factory.FromAsync<TResult>(asyncFunc.BeginInvoke, asyncFunc.EndInvoke, state);
        }

        /// <summary>
        /// <paramref name="func"/>을 비동기 방식으로 실행하는 <see cref="Task{TResult}"/>를 빌드합니다.
        /// </summary>
        public static Task<TResult> FromAsync<T, TResult>(this TaskFactory factory,
                                                          Func<T, TResult> func,
                                                          T arg1,
                                                          object state = null) {
            factory.ShouldNotBeNull("factory");
            func.ShouldNotBeNull("func");

            var asyncFunc = new Func<T, TResult>(func);

            return factory.FromAsync<T, TResult>(asyncFunc.BeginInvoke, asyncFunc.EndInvoke, arg1, state);
        }

        /// <summary>
        /// <paramref name="func"/>을 비동기 방식으로 실행하는 <see cref="Task{TResult}"/>를 빌드합니다.
        /// </summary>
        public static Task<TResult> FromAsync<T1, T2, TResult>(this TaskFactory factory,
                                                               Func<T1, T2, TResult> func,
                                                               T1 arg1,
                                                               T2 arg2,
                                                               object state = null) {
            factory.ShouldNotBeNull("factory");
            func.ShouldNotBeNull("func");

            var asyncFunc = new Func<T1, T2, TResult>(func);

            return factory.FromAsync<T1, T2, TResult>(asyncFunc.BeginInvoke, asyncFunc.EndInvoke, arg1, arg2, state);
        }

        /// <summary>
        /// <paramref name="func"/>을 비동기 방식으로 실행하는 <see cref="Task{TResult}"/>를 빌드합니다.
        /// </summary>
        public static Task<TResult> FromAsync<T1, T2, T3, TResult>(this TaskFactory factory,
                                                                   Func<T1, T2, T3, TResult> func,
                                                                   T1 arg1,
                                                                   T2 arg2,
                                                                   T3 arg3,
                                                                   object state = null) {
            factory.ShouldNotBeNull("factory");
            func.ShouldNotBeNull("func");

            var asyncFunc = new Func<T1, T2, T3, TResult>(func);

            // Silverlight용 TPL에서는 3개의 인자를 지원하지 않습니다.
            var ar = asyncFunc.BeginInvoke(arg1, arg2, arg3, null, state);
            return Task.Factory.StartNew(() => asyncFunc.EndInvoke(ar));
        }
    }
}