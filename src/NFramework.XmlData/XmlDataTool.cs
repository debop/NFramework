using NSoft.NFramework.Data;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.XmlData {
    /// <summary>
    /// XmlManager 관련 Utility Class
    /// </summary>
    public static partial class XmlDataTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Invalid Identity Value of Object (-1)
        /// </summary>
        public const int INVALID_ID = -1;

        /// <summary>
        /// No Page Index (0)
        /// </summary>
        public const int NO_PAGE_INDEX = 0;

        /// <summary>
        /// No Request Message provided.
        /// </summary>
        public const string NoRequestProvided = "요청 메시지 (Request Message)가 제공되지 않았습니다.";

        public const string XmlDataManagerAdapterPrefix = "XmlDataManagerAdapter";
        public const string XmlDataManagerPrefix = "XmlDataManager";
        public const string MessageSerializerPrefix = "MessageSerializer";
        public const string AdoRepositoryPrefix = "AdoRepository";

        /// <summary>
        /// Parameter로 제공되는 Product 키
        /// </summary>
        public const string ProductParameterName = "Product";

        internal static string GetDatabaseName(this string dbName) {
            return dbName.IsWhiteSpace() ? AdoTool.DefaultDatabaseName : dbName;
        }
    }
}