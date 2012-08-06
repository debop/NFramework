namespace NSoft.NFramework.Data.QueryProviders {
    /// <summary>
    /// Ini 파일을 이용한 NHibernate 용 Query Provider
    /// </summary>
    public sealed class IniNHibernateQueryProvider : InIQueryProviderBase {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="queryFilePath">Query String 이 정의된 파일의 전체 경로</param>
        public IniNHibernateQueryProvider(string queryFilePath) : base(queryFilePath) {}
    }
}