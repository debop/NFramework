using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NSoft.NFramework.Data.PostgreSql.EnterpriseLibrary;
using Npgsql;

namespace NSoft.NFramework.Data.PostgreSql {
    /// <summary>
    /// AdoRepository for PostgreSQL Database
    /// </summary>
    public class PostgreSqlRepositoryImpl : AdoRepositoryImplBase, IPostgreSqlRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public PostgreSqlRepositoryImpl() : this(AdoTool.DefaultDatabaseName) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="dbName"></param>
        public PostgreSqlRepositoryImpl(string dbName)
            : base(dbName) {
            if(IsDebugEnabled)
                log.Debug("PostgreSql 전용의 PostgreSqlRepository를 생성합니다. 비동기 방식 실행이 가능합니다. dbName=[{0}]", dbName);
        }

        /// <summary>
        /// DAAB의 PostgreSql 용 Database 인스턴스
        /// </summary>
        public new NpgsqlDatabase Db {
            get { return (NpgsqlDatabase)base.Db; }
        }

        /// <summary>
        /// <paramref name="cmd"/>를 실행하여, 결과를 DataSet으로 반환합니다.
        /// </summary>
        /// <param name="cmd">실행할 <see cref="DbCommand"/> instance.</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">collectio of parameters of Command</param>
        /// <returns>결과 셋이 담긴 DataSet</returns>
        public override DataSet ExecuteDataSet(DbCommand cmd, int firstResult, int maxResults, params IAdoParameter[] parameters) {
            cmd.ShouldNotBeNull("cmd");

            var result = new DataSet();

            var tables = ExecuteDataTableAsList(cmd, firstResult, maxResults, parameters);

            if(tables != null)
                result.Tables.AddRange(tables.ToArray());

            return result;
        }

        /// <summary>
        /// 여러 ResultSet을 반환할 수 있으므로, DataTable의 컬렉션으로 반환합니다.
        /// </summary>
        public override IList<DataTable> ExecuteDataTableAsList(DbCommand cmd, int firstResult, int maxResults,
                                                                params IAdoParameter[] parameters) {
            cmd.ShouldBeInstanceOf<NpgsqlCommand>("cmd");

            return With.TryFunctionAsync(() => Db.ExecuteDataTableAsListAsync((NpgsqlCommand)cmd,
                                                                              firstResult,
                                                                              maxResults,
                                                                              parameters)
                                                   .Result,
                                         () => new List<DataTable>());
        }

        /// <summary>
        /// 지정된 Command를 실행하여, DataTable을 빌드합니다.
        /// </summary>
        /// <param name="cmd">실행할 Select용 Command</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스 (0부터 시작)</param>
        /// <param name="maxResults">최대 레코드 수 (0 이면 무시하고, 마지막 레코드까지 가져온다</param>
        /// <param name="parameters">DbCommand의 Parameter 정보</param>
        /// <returns>instance of <see cref="DataTable"/></returns>
        public override DataTable ExecuteDataTable(DbCommand cmd, int firstResult, int maxResults, params IAdoParameter[] parameters) {
            cmd.ShouldBeInstanceOf<NpgsqlCommand>("cmd");

            return With.TryFunctionAsync(() => Db.ExecuteDataTableAsync((NpgsqlCommand)cmd,
                                                                        firstResult,
                                                                        maxResults,
                                                                        parameters)
                                                   .Result,
                                         () => (DataTable)null);
        }

        /// <summary>
        /// 지정된 Command의 ExecuteNonQuery 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd">실행할 Command 객체</param>
        /// <returns></returns>
        protected override int ExecuteNonQueryInternal(DbCommand cmd) {
            cmd.ShouldBeInstanceOf<NpgsqlCommand>("cmd");

            return With.TryFunctionAsync(() => Db.ExecuteNonQueryAsync((NpgsqlCommand)cmd).Result, () => -1);
        }

        /// <summary>
        /// 지정된 Command의 ExecuteScalar 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd">실행할 Command 객체</param>
        /// <returns></returns>
        protected override object ExecuteScalarInternal(DbCommand cmd) {
            cmd.ShouldBeInstanceOf<NpgsqlCommand>("cmd");

            return With.TryFunctionAsync(() => Db.ExecuteScalarAsync((NpgsqlCommand)cmd).Result, () => (object)null);
        }

        /// <summary>
        /// 지정된 Command의 ExecuteReader 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override IDataReader ExecuteReaderInternal(DbCommand cmd) {
            cmd.ShouldBeInstanceOf<NpgsqlCommand>("cmd");

            return With.TryFunctionAsync(() => Db.ExecuteReaderAsync((NpgsqlCommand)cmd).Result, () => (IDataReader)null);
        }
    }
}