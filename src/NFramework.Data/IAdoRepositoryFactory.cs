namespace NSoft.NFramework.Data {
    /// <summary>
    /// Factory interface for IAdoRepository
    /// </summary>
    public interface IAdoRepositoryFactory {
        /// <summary>
        /// Create AdoRepository using a given database source.
        /// </summary>
        /// <param name="dbName">database name used by Repository that defined in connectionStrings section.</param>
        /// <returns>New instance of implemented class from <see cref="IAdoRepository"/></returns>
        IAdoRepository CreateRepository(string dbName = null);
    }
}