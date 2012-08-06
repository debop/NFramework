using System;
using System.Data.Common;
using NSoft.NFramework.Data.Mappers;

namespace NSoft.NFramework.Data.AdoPoco {
    /// <summary>
    /// AdoDatabase 인터페이스
    /// </summary>
    public interface IAdoProvider : IDisposable {
        /// <summary>
        /// Database Connection
        /// </summary>
        DbConnection Connection { get; }

        /// <summary>
        /// Database Provider name (예 : System.Data.SqlClient 등)
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Database Provider Factory
        /// </summary>
        DbProviderFactory ProviderFactory { get; }

        /// <summary>
        /// Database connection string
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Database 종류
        /// </summary>
        DatabaseKind DatabaseKind { get; }

        /// <summary>
        /// Parameter Prefix
        /// </summary>
        string ParameterPrefix { get; }

        /// <summary>
        /// 자동으로 SELECT 구문이 가능하도록 할 것인지 여부
        /// </summary>
        bool EnableAutoSelect { get; set; }

        /// <summary>
        /// Named Parameter 를 지원할 것인지 여부
        /// </summary>
        bool EnableNamedParams { get; set; }

        /// <summary>
        /// DateTime 기준을 UTC 로 할 것인지 여부
        /// </summary>
        bool ForceDateTimesToUtc { get; set; }

        /// <summary>
        /// Connection을 IAdoProvider 인스턴스의 생명주기와 같이 계속 열어둘 것인가?
        /// </summary>
        bool KeepConnectionAlive { get; set; }

        /// <summary>
        /// Column 명 - Property 명 매핑을 담당하는 Mapper 
        /// </summary>
        INameMapper NameMapper { get; set; }


        /// <summary>
        /// Transaction 을 중단합니다.
        /// </summary>
        void AbortTransaction();

        /// <summary>
        /// Transaction을 완료합니다.
        /// </summary>
        void CompleteTransaction();
    }
}