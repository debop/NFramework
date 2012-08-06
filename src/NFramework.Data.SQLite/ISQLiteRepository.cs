using Microsoft.Practices.EnterpriseLibrary.Data;
using NSoft.NFramework.Data.SQLite.EnterpriseLibrary;

namespace NSoft.NFramework.Data.SQLite {
    /// <summary>
    /// SQLite DB용 Repository의 인터페이스입니다.
    /// </summary>
    public interface ISQLiteRepository : IAdoRepository {
        /// <summary>
        /// SQLite용 <see cref="Database"/>
        /// </summary>
        new SQLiteDatabase Db { get; }
    }
}