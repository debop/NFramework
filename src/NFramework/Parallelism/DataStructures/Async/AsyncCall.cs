using System;
using System.Threading.Tasks;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// 비동기 호출 클래스의 팩토리
    /// </summary>
    public static class AsyncCall {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 비동기 호출을 수행하는 <see cref="AsyncCall{T}"/>를 생성합니다.
        /// </summary>
        public static AsyncCall<T> Create<T>(Action<T> actionHandler, int maxDegreeOfParallelism = 1,
                                             int maxItemsPerTask = Int32.MaxValue, TaskScheduler scheduler = null) {
            actionHandler.ShouldNotBeNull("actionHandler");
            return new AsyncCall<T>(actionHandler, maxDegreeOfParallelism, maxItemsPerTask, scheduler);
        }

        /// <summary>
        /// 비동기 호출을 수행하는 <see cref="AsyncCall{T}"/>를 생성합니다.
        /// </summary>
        public static AsyncCall<T> Create<T>(Func<T, Task> functionHandler, int maxDegreeOfParallelism = 1,
                                             TaskScheduler scheduler = null) {
            functionHandler.ShouldNotBeNull("functionHandler");
            return new AsyncCall<T>(functionHandler, maxDegreeOfParallelism, scheduler);
        }

        /// <summary>
        /// 지정한 AppDomain에서 비동기 호출을 수행하는 <see cref="AsyncCall{T}"/>를 생성합니다.
        /// </summary>
        public static AsyncCall<T> CreateInTargetAppDomain<T>(AppDomain targetDomain,
                                                              Action<T> actionHandler,
                                                              int maxDegreeOfParallelism = 1,
                                                              int maxItemsPerTask = Int32.MaxValue,
                                                              TaskScheduler scheduler = null) {
            targetDomain.ShouldNotBeNull("targetDomain");
            actionHandler.ShouldNotBeNull("actionHandler");

            return ActivatorTool.CreateInstance<AsyncCall<T>>(new object[]
                                                              {
                                                                  actionHandler,
                                                                  maxDegreeOfParallelism,
                                                                  maxItemsPerTask,
                                                                  scheduler
                                                              });
        }
    }
}