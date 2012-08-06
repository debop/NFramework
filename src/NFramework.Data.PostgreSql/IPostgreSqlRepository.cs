using NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary;

namespace NSoft.NFramework.Data.PostgreSql {
    public interface IPostgreSqlRepository : IAdoRepository {
        /// <summary>
        /// DAAB의 PostgreSql 용 Database 인스턴스
        /// </summary>
        new NpgsqlDatabase Db { get; }
    }
}