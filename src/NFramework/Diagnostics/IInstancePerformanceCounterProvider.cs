namespace NSoft.NFramework.Diagnostics {
    /// <summary>
    /// Instance 별로 PerformanceCounter를 제공하는 Provider의 인터페이스입니다.
    /// </summary>
    public interface IInstancePerformanceCounterProvider : IPerformanceCounterProvider {
        /// <summary>
        /// Instance name
        /// </summary>
        string InstanceName { get; }
    }
}