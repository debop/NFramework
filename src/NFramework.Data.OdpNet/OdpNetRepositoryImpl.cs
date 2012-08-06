using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using NSoft.NFramework.Data.OdpNet.EnterpriseLibrary;
using Oracle.DataAccess.Client;

namespace NSoft.NFramework.Data.OdpNet {
    /// <summary>
    /// Oracle Database용 AdoRepository 입니다.
    /// </summary>
    public class OdpNetRepositoryImpl : AdoRepositoryImplBase, IOdpNetRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        public OdpNetRepositoryImpl() : this(AdoTool.DefaultDatabaseName) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="dbName"></param>
        public OdpNetRepositoryImpl(string dbName)
            : base(dbName) {
            if(IsDebugEnabled)
                log.Debug("Oracle 전용의 OdpNetRepositoryImpl 인스턴스를 생성합니다. 비동기 방식 실행이 가능합니다. dbName=[{0}]", dbName);
        }

        /// <summary>
        /// DAAB의 Oracle 용 Database 인스턴스
        /// </summary>
        public new OdpNetDatabase Db {
            get { return (OdpNetDatabase)base.Db; }
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
            cmd.AssertIsOracleCommand();

            return With.TryFunctionAsync(() => Db.ExecuteDataTableAsListAsync((OracleCommand)cmd,
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
            cmd.AssertIsOracleCommand();

            return With.TryFunctionAsync(() =>
                                         Db.ExecuteDataTableAsync((OracleCommand)cmd,
                                                                  firstResult,
                                                                  maxResults,
                                                                  parameters).Result,
                                         () => (DataTable)null);
        }

        /// <summary>
        /// 지정된 Command의 ExecuteNonQuery 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd">실행할 Command 객체</param>
        /// <returns></returns>
        protected override int ExecuteNonQueryInternal(DbCommand cmd) {
            cmd.AssertIsOracleCommand();

            return With.TryFunctionAsync(() => Db.ExecuteNonQueryAsync((OracleCommand)cmd).Result, () => -1);
        }

        /// <summary>
        /// 지정된 Command의 ExecuteScalar 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd">실행할 Command 객체</param>
        /// <returns></returns>
        protected override object ExecuteScalarInternal(DbCommand cmd) {
            cmd.AssertIsOracleCommand();

            return With.TryFunctionAsync(() => Db.ExecuteScalarAsync((OracleCommand)cmd).Result, () => (object)null);
        }

        /// <summary>
        /// 지정된 Command의 ExecuteReader 메소드를 실행합니다.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected override IDataReader ExecuteReaderInternal(DbCommand cmd) {
            cmd.AssertIsOracleCommand();

            return With.TryFunctionAsync(() => Db.ExecuteReaderAsync((OracleCommand)cmd).Result, () => (IDataReader)null);
        }
    }
}