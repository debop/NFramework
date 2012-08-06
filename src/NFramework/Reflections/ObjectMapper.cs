namespace NSoft.NFramework.Reflections {
    /// <summary>
    /// 형식이 다른 두 인스턴스에서 원본 인스턴스와 대상 인스턴스가 같은 이름을 가진 속성 값을 복사한다.
    /// </summary>
    /// <remarks>
    /// 형식은 다르지만, 일치하는 속성명이 많은 경우 속성 값 복사를 DynamicMethod를 사용하여 빠르게 할 수 있다.
    /// 원본 인스턴스 정보를 DTO (Data Transfer Object)로 변환할 때 사용한다.
    /// </remarks>
    public static partial class ObjectMapper {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 속성 매핑 옵션
        /// </summary>
        public static readonly MapPropertyOptions DefaultOptions = MapPropertyOptions.Safety;
    }
}