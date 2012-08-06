using System;
using System.Data;
using System.Data.SqlClient;
using NHibernate.Driver;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate에서는 BinaryBlob를 varbinary(8000)으로 설정한다. 이를 Image 로 변경한다. 
    /// Command Parameter의 형식만 변경한다고 되는게 아니다. : DB Column 자체를 변경해야 한다.
    /// </summary>
    [Obsolete("테스트에 실패한 코드입니다.")]
    public class SqlServerClientDriver : SqlClientDriver {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// NHibernate는 BinaryBlobSqlType을 기본적으로 varbinary(8000)으로 변경한다. 이를 Image 형식으로 변경히기 위해 재정의하였다.
        /// </summary>
        /// <param name="dbParam"></param>
        /// <param name="name"></param>
        /// <param name="sqlType"></param>
        protected override void InitializeParameter(System.Data.IDbDataParameter dbParam, string name,
                                                    NHibernate.SqlTypes.SqlType sqlType) {
            base.InitializeParameter(dbParam, name, sqlType);

            if(sqlType is NHibernate.SqlTypes.BinarySqlType) {
                if(IsDebugEnabled)
                    log.Debug("BinaryBlobSqlType이므로 SqlDbType.Image 로 변경합니다.");

                var parameter = (SqlParameter)dbParam;
                parameter.SqlDbType = SqlDbType.Image;
            }
        }
    }
}