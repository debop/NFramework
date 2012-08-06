namespace NSoft.NFramework.Data.DevartOracle {
    /// <summary>
    /// Oracle용 Repository의 Interface입니다. <see cref="IAdoRepository"/>를 상속받습니다.
    /// </summary>
    public interface IOracleRepository : IAdoRepository {
        new EnterpriseLibrary.OracleDatabase Db { get; }
    }
}