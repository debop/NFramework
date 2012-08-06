namespace NSoft.NFramework.Data.QueryProviders {
    /// <summary>
    /// 쿼리문이 정의된 파일로부터 쿼리문을 제공하는 프로바이더입니다. 동적으로 파일의 변화에 따른 갱신을 수행합니다.
    /// NOTE: 파일을 취급하므로, Thread-Safe 하지 않습니다!!!
    /// </summary>
    public sealed class DynamicQueryProvider : DynamicQueryProviderBase {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="queryPath">쿼리문이 정의된 파일의 전체경로</param>
        public DynamicQueryProvider(string queryPath) : base(queryPath) {}
    }
}