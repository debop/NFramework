using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate Mapping 관련 정보
    /// </summary>
    public static class MappingContext {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const int MAX_ANSI_STRING_LENGTH_DEFAULT = 4000;
        public const int MAX_ANSI_STRING_LENGTH_SQL_SERVER = 9999;

        public const int MAX_STRING_LENGTH_DEFAULT = 2000;
        public const int MAX_STRING_LENGTH_SQL_SERVER = 9999;

        static MappingContext() {
            _databaseEngine = DatabaseEngine.SQLite;

            DefaultMaxAnsiStringLength = MAX_ANSI_STRING_LENGTH_DEFAULT;
            DefaultMaxStringLength = MAX_STRING_LENGTH_DEFAULT;
            UseLOB = false;
            NamingRule = NamingRuleKind.Pascal;
        }

        private static DatabaseEngine _databaseEngine;

        /// <summary>
        /// 현재 사용하는 DB의 종류
        /// </summary>
        public static DatabaseEngine DatabaseEngine {
            get { return _databaseEngine; }
            set {
                _databaseEngine = value;

                if(IsDebugEnabled)
                    log.Debug("MappingContext에 설정된 DatabaseEngine=[{0}]", _databaseEngine);
            }
        }

        /// <summary>
        /// 기본 AnsiString의 최대 길이 (8000)
        /// </summary>
        public static int DefaultMaxAnsiStringLength { get; set; }

        /// <summary>
        /// 기본 String의 최대 길이 (4000)
        /// </summary>
        public static int DefaultMaxStringLength { get; set; }

        /// <summary>
        /// AnsiString 수형에 대한 최대 문자 길이 (SQL Server의 경우에는 VARCHAR(MAX)를 사용하고, Oracle의 경우는 CLOB가 되던가 VARCHAR(8000)이 된다)
        /// </summary>
        public static int MaxAnsiStringLength {
            get { return (UseLOB || IsSqlServer) ? MAX_ANSI_STRING_LENGTH_SQL_SERVER : DefaultMaxAnsiStringLength; }
        }

        /// <summary>
        /// String 수형에 대한 최대 문자 길이 (SQL Server의 경우에는 VARCHAR(MAX)를 사용하고, Oracle의 경우는 CLOB가 되던가 VARCHAR(4000)이 된다)
        /// </summary>
        public static int MaxStringLength {
            get { return (UseLOB || IsSqlServer) ? MAX_STRING_LENGTH_SQL_SERVER : DefaultMaxStringLength; }
        }

        /// <summary>
        /// 현재 사용하는 Database이 SQL Server 종류인지 파악합니다.
        /// </summary>
        public static bool IsSqlServer {
            get {
                return DatabaseEngine == DatabaseEngine.MsSql2005 ||
                       DatabaseEngine == DatabaseEngine.MsSql2005Express;
            }
        }

        /// <summary>
        /// Oracle의 경우도 VARCHAR/VARCHAR2 대신 CLOB를 사용할 것인가 여부 
        /// </summary>
        public static bool UseLOB { get; set; }

        /// <summary>
        /// 테이블명, 컬럼명에 대한 명명규칙 (기본은 <see cref="NamingRuleKind.Pascal"/> 입니다)
        /// </summary>
        public static NamingRuleKind NamingRule { get; set; }

        private static readonly IDictionary<string, object> _properties = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 추가 속성 정보를 제공합니다. (Region, TablePrefix 등)
        /// </summary>
        public static IDictionary<string, object> Properties {
            get { return _properties; }
        }

        /// <summary>
        /// 지정된 Naming 규칙에 따른 테이블 또는 컬럼명을 반환합니다. (예: Oracle Naming에서는 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string AsNamingText(this string name) {
            if(NamingRule == NamingRuleKind.Oracle)
                return StringTool.ToOracleNaming(name.ToAbbrName());

            return name.ToAbbrName();
        }

        private static IDictionary<string, string> _abbrNameMap = new Dictionary<string, string>();

        /// <summary>
        /// 컬럼명으로 매핑시에 약어로 매핑해야 할 이름(단어) 매핑이다. (예: Department-Dept, Locale-Loc, Configuration-Conf 등)
        /// </summary>
        public static IDictionary<string, string> AbbrNameMap {
            get { return _abbrNameMap ?? (_abbrNameMap = new Dictionary<string, string>()); }
            set { _abbrNameMap = value; }
        }

        /// <summary>
        /// <paramref name="text"/>에 <see cref="AbbrNameMap"/>에 등록된 약어 변환 단어가 있더면 약어로 변환하여 반환합니다.
        /// </summary>
        /// <param name="text">원본 데이타</param>
        /// <returns></returns>
        public static string ToAbbrName(this string text) {
            if(text.IsWhiteSpace())
                return string.Empty;

            var builder = new StringBuilder(text);
            foreach(var from in AbbrNameMap.Keys)
                if(text.Contains(from))
                    builder.Replace(from, AbbrNameMap[from]);

            return builder.ToString();
        }
    }
}