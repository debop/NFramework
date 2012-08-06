using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using Devart.Data.Oracle;
using NSoft.NFramework.Data.DevartOracle.EnterpriseLibrary;

namespace NSoft.NFramework.Data.DevartOracle {
    /// <summary>
    /// Oracle DB를 사용하기 위한 Utility class입니다.
    /// </summary>
    public static class OracleTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 Command 인스턴스의 형식이 <see cref="OracleCommand"/> 형식인지 검사합니다. 아니면 예외를 발생시킵니다.
        /// </summary>
        /// <param name="cmd">검사할 DbCommand 인스턴스</param>
        public static void AssertIsOracleCommand(this DbCommand cmd) {
            cmd.ShouldBeInstanceOf<OracleCommand>("cmd");
        }

        /// <summary>
        /// 모든 Oracle Connection Pool을 제거합니다.
        /// </summary>
        public static void ClearAllPools() {
            OracleConnection.ClearAllPools();
        }

        /// <summary>
        /// Oracle Connection을 비동기 방식으로 엽니다.
        /// </summary>
        /// <param name="db">DAAB OracleProvider 인스턴스</param>
        /// <param name="newConnectionCreated">새로운 Connenction이 생성되었는지 여부</param>
        /// <returns>새로 연결된 <see cref="OracleConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        public static OracleConnection CreateOracleConnection(this OracleDatabase db, ref bool newConnectionCreated) {
            return
                (OracleConnection)
                AdoTool.CreateTransactionScopeConnection(db,
                                                         ref newConnectionCreated,
                                                         database => ((OracleDatabase)database).OpenConnection(15));
        }

        /// <summary>
        /// Oracle 연결이 성공하지 못하는 경우가 많아, 재시도 횟수 만큼 간격을 두고 연결을 시도합니다.
        /// Oracle DB의 Process 수를 기본(40)에서 100 이상으로 늘려주면 연결이 성공할 확률이 높습니다.
        /// </summary>
        /// <param name="db">DAAB OracleProvider 인스턴스</param>
        /// <param name="tryCount">연결 실패 시, 재 시도 횟수</param>
        /// <returns>새로 연결된 <see cref="OracleConnection"/>의 인스턴스, 만약 연결에 실패했다면 null을 반환합니다.</returns>
        public static OracleConnection OpenConnection(this OracleDatabase db, int tryCount) {
            // NOTE: Oracle의 경우 Connection이 연결 안 되는 에러(ORA-12519) 가 자주 발생한다. 그래서, 시간 Time을 두고, 재시도하도록 하였다.
            // HINT: http://forums.oracle.com/forums/thread.jspa?messageID=1145120, 
            // HINT: http://kyeomstar.tistory.com/160

            if(IsDebugEnabled)
                log.Debug("Oracle Database를 연결하려고 합니다...");

            OracleConnection connection = null;

            for(var i = 0; i < tryCount; i++) {
                var count = i + 1;

                try {
                    if(IsDebugEnabled)
                        log.Debug("OracleConnection을 생성하고, 비동기 방식으로 Open합니다. 시도 횟수=[{0}]", (i + 1));

                    connection = (OracleConnection)db.CreateConnection();
                    connection.Open();

                    //Task.Factory.FromAsync(connection.BeginOpen,
                    //                       connection.EndOpen,
                    //                       null)
                    //	.WaitAsync(TimeSpan.FromSeconds(15));

                    if(connection.State == ConnectionState.Open) {
                        if(IsDebugEnabled)
                            log.Debug("OracleConnection을 연결했습니다!!!");

                        return connection;
                    }
                }
                catch(AggregateException age) {
                    age.Handle(ex => {
                                   HandleConnectionError(db, ex, count);
                                   return true;
                               });
                }
                catch(Exception ex) {
                    HandleConnectionError(db, ex, count);
                }
            }

            Guard.Assert(connection != null && connection.State != ConnectionState.Closed, "Oracle Connection을 열지 못했습니다!!!");
            return connection;
        }

        private static void HandleConnectionError(OracleDatabase db, Exception ex, int? tryCount) {
            // NOTE: Oracle의 경우, Connection 에러가 나면, 대기 후에 접속하면 성공한다. 왜 그런지는 잘 모른다...
            // HINT: http://kyeomstar.tistory.com/160
            //

            const int MaxTimeout = 500; // milliseconds

            if(ex.Message.Contains("ORA-12519") || ex.Message.Contains("Unknown connection string parameter")) {
                if(log.IsWarnEnabled) {
                    log.Warn("Database Connection 생성 및 Open 수행 시에 예외가 발생했습니다. ConnectionString=[{0}]", db.ConnectionString);
                    log.Warn("Oracle 서버에 ALTER SYSTEM SET PROCESSES=500 SCOPE=SPFILE; 를 수행해 보세요. 기본 PROCESS 수가 40입니다. 이 값을 늘려보세요.");
                    log.Warn(ex);
                }
            }
            else {
                if(log.IsErrorEnabled) {
                    log.Error("Database Connection 생성 및 Open 수행 시에 예외가 발생했습니다. ConnectionString=[{0}]", db.ConnectionString);
                    log.Error(ex);
                }
            }

            var timeout = Math.Min(MaxTimeout, Math.Abs(tryCount.GetValueOrDefault(1)) * 50);
            Thread.Sleep(timeout);
        }

        /// <summary>
        /// <paramref name="type"/>에 해당하는 <see cref="OracleDbType"/>을 반환합니다.
        /// </summary>
        public static OracleDbType TypeToOracleDbType(Type type) {
            if(IsDebugEnabled)
                log.Debug("수형[{0}]에 해당하는 OracleDbType을 반환합니다...", type);

            switch(Type.GetTypeCode(type)) {
                case TypeCode.Char:
                case TypeCode.String:
                case TypeCode.DBNull:
                    return OracleDbType.VarChar;
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    return OracleDbType.Integer;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    return OracleDbType.Number;
                case TypeCode.Single:
                    return OracleDbType.Float;
                case TypeCode.Double:
                    return OracleDbType.Double;
                case TypeCode.Decimal:
                    return OracleDbType.Number;
                case TypeCode.DateTime:
                    return OracleDbType.Date;
                case TypeCode.Object:
                    if(type.IsArray) {
                        switch(Type.GetTypeCode(type.GetElementType())) {
                            case TypeCode.Byte:
                                return OracleDbType.Blob;
                            case TypeCode.Char:
                                return OracleDbType.VarChar;
                            default:
                                throw new ArgumentException("수형을 지원하지 않습니다. type=" + type);
                        }
                    }

                    if(type == typeof(OracleIntervalDS))
                        return OracleDbType.IntervalDS;
                    if(type == typeof(OracleIntervalYM))
                        return OracleDbType.IntervalYM;
                    if(type == typeof(OracleObject))
                        return OracleDbType.Object;
                    if(type == typeof(OracleArray))
                        return OracleDbType.Array;
                    if(type == typeof(OracleTable))
                        return OracleDbType.Table;
                    if(type == typeof(OracleBinary))
                        return OracleDbType.Raw;
                    if(type == typeof(OracleString))
                        return OracleDbType.VarChar;
                    if(type == typeof(OracleCursor))
                        return OracleDbType.Cursor;

                    throw new InvalidOperationException("OracleDbType에 매핑되는 타입이 아닙니다. type=" + type);

                default:
                    throw new InvalidOperationException("OracleDbType에 매핑되는 타입이 아닙니다. type=" + type);
            }
        }

        /// <summary>
        /// <paramref name="dbType"/>에 해당하는 <see cref="OracleDbType"/>을 반환합니다.
        /// </summary>
        public static OracleDbType DbTypeToOracleDbType(DbType dbType) {
            if(IsDebugEnabled)
                log.Debug("DbType[{0}]에 해당하는 OracleDbType을 반환합니다.", dbType);

            switch(dbType) {
                case DbType.AnsiString:
                case DbType.String:
                    return OracleDbType.VarChar;
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    return OracleDbType.Char;
                case DbType.Byte:
                case DbType.Int16:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.Int32:
                    return OracleDbType.Integer;
                case DbType.Single:
                    return OracleDbType.Float;
                case DbType.Double:
                    return OracleDbType.Double;
                case DbType.Date:
                    return OracleDbType.Date;
                case DbType.DateTime:
                    return OracleDbType.TimeStamp;
                case DbType.Time:
                    return OracleDbType.IntervalDS;
                case DbType.Binary:
                    return OracleDbType.Blob;
                case DbType.Boolean:
                    return OracleDbType.Boolean;
                case DbType.Int64:
                case DbType.UInt64:
                case DbType.VarNumeric:
                case DbType.Decimal:
                case DbType.Currency:
                    return OracleDbType.Number;
                case DbType.Object:
                    return OracleDbType.Object;
                case DbType.Guid:
                    return OracleDbType.Raw;
                default:
                    throw new NotSupportedException("OracleDbType에 매핑되는 타입이 아닙니다. dbType=" + dbType);
            }
        }
    }
}