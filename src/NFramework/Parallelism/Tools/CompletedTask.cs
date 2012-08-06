using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// 이미 완료된 Task (CompletedTask)에 대한 접근을 제공합니다.
    /// </summary>
    public static class CompletedTask {
        /// <summary>
        /// 기본 작업
        /// </summary>
        public static readonly Task Default = CompletedTask<object>.Default;
    }
}