namespace NSoft.NFramework.Serializations.Serializers {
    public class CloneSerializer : AbstractSerializer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// CloneSerializer의 Singleton Instance
        /// </summary>
        public static CloneSerializer Instance {
            get { return SingletonTool<CloneSerializer>.Instance; }
        }
    }
}