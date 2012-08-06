using NSoft.NFramework.Data.MySql.EnterpriseLibrary;

namespace NSoft.NFramework.Data.MySql {
    /// <summary>
    /// MySQL 을 위한 AdoRepository의 인터페이스입니다.
    /// </summary>
    public interface IMySqlRepository : IAdoRepository {
        /// <summary>
        /// DAAB의 MySql 용 Database 인스턴스
        /// </summary>
        new MySqlDatabase Db { get; }
    }
}