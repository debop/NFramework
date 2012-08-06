using System;
using System.Diagnostics;
using System.Linq;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// Delegate를 병렬방식으로 수행하는 메소드를 제공합니다.
    /// </summary>
    public static class DelegateTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

#if !SILVERLIGHT
        /// <summary>
        /// <paramref name="multicastDelegate"/>의 delegate들을 병렬로 DynamicInvoke() 를 수행하고, 제일 마지막 수행 결과를 반환합니다. (WaitAll)
        /// Speculative Processing (WaitAny)의 반대이지요. 
        /// </summary>
        /// <param name="multicastDelegate">호출할 델리게이트</param>
        /// <param name="args">호출 시 인자값</param>
        /// <returns>호출 결과 값</returns>
        public static object ParallelDynamicInvoke(this Delegate multicastDelegate, params object[] args) {
            multicastDelegate.ShouldNotBeNull("multicastDelegate");

            // 일반적으로 Event는 MulticastDelegate이면서, 순차적으로 EventHandler를 호출합니다. (EventHandler 중 비용이 많이 드는 것이 있다면 비효율적입니다)
            // 이를 병렬로 호출하게 된다면, 빠르게 Event를 처리할 수 있습니다.

            if(IsDebugEnabled)
                log.Debug("Delegate를 병렬로 동적 호출합니다.");

            return
                multicastDelegate
                    .GetInvocationList()
                    .AsParallel()
                    .AsOrdered()
                    .Select(d => d.DynamicInvoke(args))
                    .Last();
        }
#endif

        /// <summary>
        /// <paramref name="action"/>을 수행할 때, 예외가 발생하면 try-finally block이나 종료자를 수행하지 않고 빠져나가도록 Wrapping합니다. 
        /// (이런 수행 방식을 Fail Fast라고 합니다.)
        /// </summary>
        /// <param name="action">수행할 action</param>
        /// <returns></returns>
        public static Action WithFailFast(this Action action) {
            action.ShouldNotBeNull("action");

            return () => {
                       try {
                           action();
                       }
                       catch(Exception ex) {
                           if(log.IsWarnEnabled)
                               log.WarnException("지정된 Action 실행에 실패하여, FailFast로 마무리 짓습니다.", ex);

                           // 디버깅 모드라면, 디버깅 중단을 알립니다.
                           if(Debugger.IsAttached)
                               Debugger.Break();
                           else
                               // NET-3.5에서는 string 인자 하나만 받는 메소드 밖에 없다.
                               Environment.FailFast("예기치 못한 예외가 발생했습니다. Fail fast 방식으로 빠져나갑니다." + Environment.NewLine + ex);
                       }
                   };
        }

        /// <summary>
        /// <paramref name="function"/>을 수행할 때, 예외가 발생하면 try-finally block이나 종료자를 수행하지 않고 빠져나가도록 Wrapping합니다. 
        /// (이런 수행 방식을 Fail Fast라고 합니다.)
        /// </summary>
        /// <typeparam name="T">함수의 반환 값의 수형</typeparam>
        /// <param name="function">수행할 함수</param>
        /// <returns></returns>
        public static Func<T> WithFailFast<T>(this Func<T> function) {
            function.ShouldNotBeNull("function");

            return () => {
                       try {
                           return function();
                       }
                       catch(Exception ex) {
                           if(log.IsWarnEnabled) {
                               log.Warn("지정된 함수 실행에 실패하여, FailFast로 마무리 짓습니다.");
                               log.Warn(ex);
                           }

                           // 디버깅 모드라면, 디버깅 중단을 알립니다.
                           if(Debugger.IsAttached)
                               Debugger.Break();
                           else
                               // NET-3.5에서는 string 인자 하나만 받는 메소드 밖에 없다.
                               Environment.FailFast("예기치 못한 예외가 발생했습니다. Fail fast 방식으로 빠져나갑니다." + Environment.NewLine + ex);
                       }

                       throw new InvalidOperationException("여기까지 오면 잘못된 작업이 수행된 것입니다^^");
                   };
        }
    }
}