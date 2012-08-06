using System;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// Lazy{T} 를 위한 Extension Methods 입니다.
    /// </summary>
    public static class LazyTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 Lazy{T} 인스턴스의 실제 값을 현 시점에서 생성하는 작업을 수행하도록 한다.
        /// </summary>
        public static Lazy<T> Force<T>(this Lazy<T> lazy) {
            var ignored = lazy.Value;
            return lazy;
        }

        /// <summary>
        /// Future Pattern을 사용하여 Lazy{T}의 실제 값을 구하는 Task{T}를 빌드합니다.
        /// Lazy 자체가 그 일을 하는 것인데 뭔말인가? 궁금해 하는 사람들에게... 
        /// Lazy.Value를 구하는 작업이 많은 부하를 가질 때, 다른 작업으로 분리해서 비동기적으로 수행하도록 하면, 전체적으로 더 좋은 결과를 얻을 수 있다.
        /// </summary>
        public static Task<T> GetValueAsync<T>(this Lazy<T> lazy) {
            return Task.Factory.FromResult<T>(lazy.Value);
        }

        /// <summary>
        /// Lazy{T} 값 계산이 많은 시간이 걸릴 경우, 비동기 델리게이트 호출 이용한 계산에 의해 값을 구하는 Task{T} 를 빌드합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lazy"></param>
        /// <returns></returns>
        public static Task<T> GetValueTask<T>(this Lazy<T> lazy) {
            return Task.Factory.StartNew(() => lazy.Value, TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// Thread-safe 한 <paramref name="value"/> 값으로 초기화된 Lazy{T} 인스턴스를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="isThreadSafe"></param>
        /// <returns></returns>
        public static Lazy<T> CreateLazy<T>(this T value, bool isThreadSafe = true) {
            if(IsDebugEnabled)
                log.Debug("값을 설정한 Lazy<{0}>를 생성합니다...", typeof(T).Name);

            return new Lazy<T>(() => value, isThreadSafe).Force();
        }
    }
}