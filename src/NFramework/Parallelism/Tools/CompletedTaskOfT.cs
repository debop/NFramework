using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// 이미 완료된 Task (CompletedTask)에 대한 접근을 제공합니다.
    /// </summary>
    /// <typeparam name="TResult">작업 결과 값의 수형</typeparam>
    public static class CompletedTask<TResult> {
        /// <summary>
        /// 기본 작업
        /// </summary>
        public static readonly Task<TResult> Default;

        static CompletedTask() {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.TrySetResult(default(TResult));

            Default = tcs.Task;
        }
    }
}