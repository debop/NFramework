namespace NHibernate.Driver {
    /// <summary>
    /// NHibernate에서 사용할 Cubrid RDBMS용 Driver입니다.
    /// </summary>
    /// <remarks>
    /// NHibernate configuration 속성 중에 connection.driver_class 값을 "NHibernate.Driver.CubridDataDriver, RCL.Data" 로 해주면 Cubrid Driver를 Cubrid.Data.dll을 사용합니다.
    /// </remarks>
    /// <example>
    /// <code>
    /// // hibernate.cfg.xml 에서 다음과 같이 설정하시면 됩니다.
    /// <property name="connection.driver_class">NHibernate.Driver.CubridDataDriver, RCL.Data</property>
    /// </code>
    /// </example>
    public class CubridDataDriver : ReflectionBasedDriver, ISqlParameterFormatter {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public CubridDataDriver()
            : base("CUBRID.Data", "CUBRID.Data.CUBRIDClient.CUBRIDConnection", "CUBRID.Data.CUBRIDClient.CUBRIDCommand") {
            if(log.IsInfoEnabled)
                log.Info("NHibernate용 CubridDataDriver를 생성했습니다.");
        }

        public override bool UseNamedPrefixInSql {
            get { return true; }
        }

        public override bool UseNamedPrefixInParameter {
            get { return true; }
        }

        public override string NamedPrefix {
            get { return "?"; }
        }

        string ISqlParameterFormatter.GetParameterName(int index) {
            return "?";
        }

        public override bool SupportsMultipleOpenReaders {
            get { return false; }
        }
    }
}