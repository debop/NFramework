namespace NSoft.NFramework.Numerics {
    /// <summary>
    /// 난수발생기의 기본 인터페이스
    /// </summary>
    public interface IRandomizer {
        /// <summary>
        /// 난수 발생
        /// </summary>
        /// <returns>난수</returns>
        double Next();
    }
}