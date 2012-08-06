using System;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// Delegate를 비동기적으로 수행하도록 합니다.
    /// </summary>
    public static class DelegateAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 수행합니다.
        /// </summary>
        /// <returns>Task 인스턴스</returns>
        public static Task Run(Action action, object state = null) {
            action.ShouldNotBeNull("action");

            return Task.Factory.FromAsync(action.BeginInvoke,
                                          action.EndInvoke,
                                          state,
                                          TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 수행합니다.
        /// </summary>
        /// <returns>Task 인스턴스</returns>
        public static Task Run<T>(Action<T> action, T arg, object state = null) {
            action.ShouldNotBeNull("action");

            return Task.Factory.FromAsync(action.BeginInvoke,
                                          action.EndInvoke,
                                          arg,
                                          state,
                                          TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 수행합니다.
        /// </summary>
        /// <returns>Task 인스턴스</returns>
        public static Task Run<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2, object state = null) {
            action.ShouldNotBeNull("action");

            return Task.Factory.FromAsync(action.BeginInvoke,
                                          action.EndInvoke,
                                          arg1,
                                          arg2,
                                          state,
                                          TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="action"/>을 비동기 방식으로 수행합니다.
        /// </summary>
        /// <returns>Task 인스턴스</returns>
        public static Task Run<T1, T2, T3>(Action<T1, T2, T3> action, T1 arg1, T2 arg2, T3 arg3, object state = null) {
            action.ShouldNotBeNull("action");

            // Silverlight용 TPL에서는 지원하지 3개 이상의 인자를 지원하지 않는다.
            var ar = action.BeginInvoke(arg1, arg2, arg3, null, state);
            return Task.Factory.StartNew(() => action.EndInvoke(ar), TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="function"/>을 비동기 방식으로 수행하는 Task{TResult} 를 반환합니다.
        /// </summary>
        public static Task<TResult> Run<TResult>(Func<TResult> function, object state = null) {
            function.ShouldNotBeNull("function");

            return Task<TResult>.Factory.FromAsync(function.BeginInvoke,
                                                   function.EndInvoke,
                                                   state,
                                                   TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="function"/>을 비동기 방식으로 수행하는 Task{TResult} 를 반환합니다.
        /// </summary>
        public static Task<TResult> Run<T, TResult>(Func<T, TResult> function, T arg, object state = null) {
            function.ShouldNotBeNull("function");

            return Task<TResult>.Factory.FromAsync(function.BeginInvoke,
                                                   function.EndInvoke,
                                                   arg,
                                                   state,
                                                   TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="function"/>을 비동기 방식으로 수행하는 Task{TResult} 를 반환합니다.
        /// </summary>
        public static Task<TResult> Run<T1, T2, TResult>(Func<T1, T2, TResult> function, T1 arg1, T2 arg2, object state = null) {
            function.ShouldNotBeNull("function");

            return Task<TResult>.Factory.FromAsync(function.BeginInvoke,
                                                   function.EndInvoke,
                                                   arg1,
                                                   arg2,
                                                   state,
                                                   TaskCreationOptions.None);
        }

        /// <summary>
        /// <paramref name="function"/>을 비동기 방식으로 수행하는 Task{TResult} 를 반환합니다.
        /// </summary>
        public static Task<TResult> Run<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> function, T1 arg1, T2 arg2, T3 arg3,
                                                             object state = null) {
            function.ShouldNotBeNull("function");

            //! NOTE: Silverlight용 TPL에서는 지원하지 3개 이상의 인자를 지원하지 않는다.
            //
            var ar = function.BeginInvoke(arg1, arg2, arg3, null, state);
            return Task.Factory.StartNew(() => function.EndInvoke(ar), TaskCreationOptions.None);
        }
    }
}