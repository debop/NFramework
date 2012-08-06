namespace NHibernate.Dialect {
    /// <summary>
    /// Cubrid Database 용 Dialect 입니다.
    /// </summary>
    public class CubridDialect : GenericDialect {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public CubridDialect() {
            if(IsDebugEnabled)
                log.Debug("CubridDialect를 생성했습니다");
        }
    }
}