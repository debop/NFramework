namespace NSoft.NFramework.Data.Persisters {
    /// <summary>
    /// Source 정보로부터 TPersistent 형식의 Persistent Object를 빌드하는 Class의 인터페이스입니다.
    /// </summary>
    /// <typeparam name="TDataSource">Type of DataSource</typeparam>
    /// <typeparam name="TPersistent">Type of Persistent object.</typeparam>
    public interface IAdoPersister<TDataSource, TPersistent> {
        /// <summary>
        /// 지정된 dataReader 정보로부터 TPersistent 형식의 Persistent Object를 빌드합니다.
        /// </summary>
        /// <param name="dataSource">Data Source</param>
        /// <returns>Persistent object</returns>
        TPersistent Persist(TDataSource dataSource);
    }
}