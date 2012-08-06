using Microsoft.Practices.EnterpriseLibrary.Data.Sql;

namespace NSoft.NFramework.Data.SqlServer {
    /// <summary>
    /// SQL Server용 AdoRepository의 인터페이스입니다.
    /// </summary>
    public interface ISqlRepository : IAdoRepository {
        new SqlDatabase Db { get; }
    }
}