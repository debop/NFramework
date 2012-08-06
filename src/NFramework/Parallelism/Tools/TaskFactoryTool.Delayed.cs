using System;
using System.Threading;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    public static partial class TaskFactoryTool {
        /// <summary>
        /// <paramref name="millisecondsDelay"/>만큼 아무 작업없이 지연하다가, 작업을 끝냅니다. 
        /// 반환되는 작업의 후속 작업으로 실제 작업을 정의하여 사용하면 작업 전에 Delay를 주는 효과가 있습니다.
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="millisecondsDelay">지연할 시간 (밀리초)</param>
        /// <param name="cancellationToken">취소 토큰</param>
        /// <returns>지연 작업</returns>
        public static Task StartNewDelayed(this TaskFactory factory,
                                           int millisecondsDelay,
                                           CancellationToken cancellationToken) {
            factory.ShouldNotBeNull("factory");
            millisecondsDelay.ShouldBePositiveOrZero("millisecondsDelay");

            if(IsDebugEnabled)
                log.Debug("일정 시간동안 아무 일도 하지 않는 작업(Delayed Task)을 빌드합니다.. 지연 시간=[{0}] msecs", millisecondsDelay);

            // Task 자체를 생성하기 보다는 TaskComletionSource를 생성하게 되면, 결과 값 제어, 취소여부 등을 외부에서 설정할 수 있습니다.
            //
            var tcs = new TaskCompletionSource<object>(factory.CreationOptions);
            var ctr = default(CancellationTokenRegistration);

            // Timer를 정의합니다. 아직 시작하지는 않고, 만약 시작하게 되면, Timer 만료 시에  Task를 종료합니다. (지연을 끝냅니다)
            //
            var timer = new Timer(self => {
                                      // 지정된 시간 후 호출되면, 관련 리소스를 정리하고, 작업을 완료한다.
                                      //
                                      ctr.Dispose();

                                      ((Timer)self).Dispose();

                                      tcs.TrySetResult(null);
                                  });

            // Timer 작업 중에 작업 취소 시, 뒷 처리 작업을 등록합니다. - 이런게 Task의 장점입니다. (ThreadPool에 비해)
            //
            if(cancellationToken.CanBeCanceled) {
                if(IsDebugEnabled)
                    log.Debug("작업 취소가 가능하다면, 작업 취소시에 뒷처리를 정의합니다.");

                ctr = cancellationToken.Register(() => {
                                                     timer.Dispose();
                                                     tcs.TrySetCanceled();
                                                 });
            }

            // timer를 시작합니다.
            timer.Change(millisecondsDelay, Timeout.Infinite);

            return tcs.Task;
        }

        /// <summary>
        /// <paramref name="millisecondsDelay"/>만큼 시간을 지연 시킨 후, <paramref name="action"/>을 실행시키는 Task를 빌드합니다.
        /// </summary>
        /// <param name="factory">TaskFactory</param>
        /// <param name="millisecondsDelay">지연 시간 (밀리초)</param>
        /// <param name="action">실행할 Action</param>
        /// <param name="state">실행할 Action의 인자 값</param>
        /// <param name="cancellationToken">작업 취소 토큰</param>
        /// <param name="creationOptions">작업 생성 옵션</param>
        /// <param name="scheduler">작업 스케쥴러</param>
        /// <returns>지연 작업 후 실제 작업을 수행하는 Task</returns>
        public static Task StartNewDelayed(this TaskFactory factory,
                                           int millisecondsDelay = 1000,
                                           Action<object> action = null,
                                           object state = null,
                                           CancellationToken? cancellationToken = null,
                                           TaskCreationOptions? creationOptions = null,
                                           TaskScheduler scheduler = null) {
            factory.ShouldNotBeNull("factory");
            millisecondsDelay.ShouldBePositiveOrZero("millisecondsDelay");

            return
                factory
                    .StartNewDelayed(millisecondsDelay, cancellationToken ?? factory.CancellationToken)
                    .ContinueWith(_ => {
                                      if(action != null)
                                          action(state);
                                  },
                                  cancellationToken ?? factory.CancellationToken,
                                  TaskContinuationOptions.OnlyOnRanToCompletion,
                                  scheduler ?? factory.GetTargetScheduler());
        }

        /// <summary>
        /// <paramref name="millisecondsDelayed"/> 이후에 <paramref name="function"/>을 수행하는 작업을 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">작업 결과 수형</typeparam>
        /// <param name="factory">Task Factory</param>
        /// <param name="millisecondsDelayed">실행 전에 지연할 시간(밀리초)</param>
        /// <param name="function">실행할 함수</param>
        /// <param name="cancellationToken">작업 취소용 토큰</param>
        /// <param name="creationOptions">작업 생성 옵션</param>
        /// <param name="scheduler">작업 스케쥴러</param>
        /// <returns>일정 시간 이후에 함수를 실행하는 작업</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory,
                                                             int millisecondsDelayed,
                                                             Func<TResult> function,
                                                             CancellationToken? cancellationToken = null,
                                                             TaskCreationOptions? creationOptions = null,
                                                             TaskScheduler scheduler = null) {
            factory.ShouldNotBeNull("factory");
            millisecondsDelayed.ShouldBePositiveOrZero("millisecondsDelayed");
            function.ShouldNotBeNull("function");
            scheduler = scheduler ?? factory.GetTargetScheduler();

            if(IsDebugEnabled)
                log.Debug("일정 시간 이후에 지정한 함수를 수행하는 작업을 생성합니다. 작업지연 시간=[{0}] msecs", millisecondsDelayed);

            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(obj => ((TaskCompletionSource<object>)obj).SetResult(null),
                                  tcs,
                                  millisecondsDelayed,
                                  Timeout.Infinite);

            return tcs.Task.ContinueWith(_ => {
                                             timer.Dispose();
                                             return function();
                                         },
                                         cancellationToken ?? factory.CancellationToken,
                                         ContinuationOptionsFromCreationOptions(creationOptions ?? factory.CreationOptions),
                                         scheduler);
        }

        /// <summary>
        /// <paramref name="millisecondsDelayed"/> 이후에 <paramref name="function"/>을 수행하는 작업을 생성합니다.
        /// </summary>
        /// <typeparam name="TResult">작업 결과 수형</typeparam>
        /// <param name="factory">Task Factory</param>
        /// <param name="millisecondsDelayed">실행 전에 지연할 시간(밀리초)</param>
        /// <param name="function">실행할 함수</param>
        /// <param name="state">함수 인자 값</param>
        /// <param name="cancellationToken">작업 취소용 토큰</param>
        /// <param name="creationOptions">작업 생성 옵션</param>
        /// <param name="scheduler">작업 스케쥴러</param>
        /// <returns>일정 시간 이후에 함수를 실행하는 작업</returns>
        public static Task<TResult> StartNewDelayed<TResult>(this TaskFactory<TResult> factory,
                                                             int millisecondsDelayed,
                                                             Func<object, TResult> function,
                                                             object state = null,
                                                             CancellationToken? cancellationToken = null,
                                                             TaskCreationOptions? creationOptions = null,
                                                             TaskScheduler scheduler = null) {
            factory.ShouldNotBeNull("factory");
            millisecondsDelayed.ShouldBePositiveOrZero("millisecondsDelayed");
            function.ShouldNotBeNull("function");

            scheduler = scheduler ?? factory.GetTargetScheduler();

            if(IsDebugEnabled)
                log.Debug("일정 시간 이후에 지정한 함수를 수행하는 작업을 생성합니다. 작업지연 시간=[{0}] msecs", millisecondsDelayed);

            var tcs = new TaskCompletionSource<TResult>(state);
            Timer timer = null;

            // 주어진 함수를 수행할 Task를 정의
            var functionTask = new Task<TResult>(function, state, creationOptions ?? factory.CreationOptions);

            functionTask.ContinueWith(antecedent => {
                                          // tcs의 작업 결과를 antecedent.Result로 설정합니다.
                                          tcs.SetFromTask(antecedent);
                                          if(timer != null)
                                              timer.Dispose();
                                      },
                                      cancellationToken ?? factory.CancellationToken,
                                      ContinuationOptionsFromCreationOptions(creationOptions ?? factory.CreationOptions) |
                                      TaskContinuationOptions.ExecuteSynchronously,
                                      scheduler);

            // 이렇게 타이머의 Time Callback 함수에서, 사용자 작업을 수행하도록 한다.
            timer = new Timer(task => ((Task)task).Start(scheduler),
                              functionTask,
                              millisecondsDelayed,
                              Timeout.Infinite);

            return tcs.Task;
        }
    }
}