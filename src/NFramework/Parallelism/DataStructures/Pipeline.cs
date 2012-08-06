using System;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.TaskSchedulers;

namespace NSoft.NFramework.Parallelism.DataStructures {
    /// <summary>
    /// pipeline data processing을 수행하는 <see cref="Pipeline{TInput,TOutput}"/>을 생성시켜주는 Factory class 입니다.
    /// </summary>
    public static class Pipeline {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        internal static readonly TaskScheduler Scheduler = new ThreadPerTaskScheduler();

        /// <summary>
        /// 병렬 방식의 Pipeline 프로세스를 생성합니다.
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="func"></param>
        /// <param name="degreeOfParallelism"></param>
        /// <returns></returns>
        public static Pipeline<TInput, TOutput> Create<TInput, TOutput>(Func<TInput, TOutput> func, int degreeOfParallelism = 1) {
            func.ShouldNotBeNull("func");
            degreeOfParallelism.ShouldBePositive("degreeOfParallelism");

            if(IsDebugEnabled)
                log.Debug("Create pipeline... degreeOfParallelism=[{0}]", degreeOfParallelism);

            return new Pipeline<TInput, TOutput>(func, degreeOfParallelism);
        }
    }
}