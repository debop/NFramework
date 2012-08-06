namespace NSoft.NFramework.Web
{
    /// <summary>
    /// Current Thread Context에서 유지할 정보를 제공합니다.
    /// </summary>
    public static partial class WebAppContext
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        /// <summary>
        /// Current Product Name : NSoft.NFramework.Web
        /// </summary>
        public const string ProductName = "NSoft.NFramework.Web";

        /// <summary>
        /// Administrator Name : admin
        /// </summary>
        public const string Administrator = "admin";

        /// <summary>
        /// Anonymous
        /// </summary>
        public const string Anonymous = "anonymous";
    }
}