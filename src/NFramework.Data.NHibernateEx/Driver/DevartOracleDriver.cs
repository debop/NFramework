using System;
using System.Reflection;
using NHibernate.AdoNet;

namespace NHibernate.Driver {
    /// <summary>
    /// NHibernate에서 사용할 Oracle용 Driver입니다. Devart dotConnector for Oracle 라이브러리를 사용합니다.
    /// 참고 : http://www.devart.com/blogs/dotconnect/?p=1857 (Old Version)
    /// 참고 : http://www.devart.com/forums/viewtopic.php?t=15685 (New Version)
    /// </summary>
    /// <remarks>
    /// NHibernate configuration 속성 중에 connection.driver_class 값을 
    /// "NHibernate.Driver.DevartOracleDriver, NSoft.NFramework.Data.NHibernateEx" 로 해주면 Oracle Driver를 Devart.Data.Oracle.dll을 사용합니다.
    /// NOTE: NHIbernate configuration 에 qualifyAssembly 를 정의하여, Devart.Data.Oracle.dll의 QualifiedAssemblyName을 등록해주어야 합니다.
    /// NOTE: http://www.devart.com/blogs/dotconnect/?p=1857 의 4번, 5번 항을 주의하세요.
    /// </remarks>
    /// <example>
    /// <code>
    /// // hibernate.cfg.xml 에서 다음과 같이 설정하시면 됩니다.
    /// <property name="connection.driver_class">NHibernate.Driver.DevartOracleDriver, NSoft.NFramework.Data.NHibernateEx</property>
    /// </code>
    /// </example>
    public sealed class DevartOracleDriver : ReflectionBasedDriver, IEmbeddedBatcherFactoryProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const string CommandTypeName = "Devart.Data.Oracle.NHibernate.NHibernateOracleCommand";
        private const string ConnectionTypeName = "Devart.Data.Oracle.NHibernate.NHibernateOracleConnection";
        private const string DriverAssemblyName = "Devart.Data.Oracle";

        /// <summary>
        /// Constructor
        /// </summary>
        public DevartOracleDriver()
            : base(DriverAssemblyName, ConnectionTypeName, CommandTypeName) {
            if(log.IsInfoEnabled)
                log.Info("NHibernate용 DevartOracleDriver를 생성했습니다.");

            Intialize();
        }

        public override bool UseNamedPrefixInSql {
            get { return true; }
        }

        public override bool UseNamedPrefixInParameter {
            get { return true; }
        }

        public override string NamedPrefix {
            get { return ":"; }
        }

        System.Type IEmbeddedBatcherFactoryProvider.BatcherFactoryClass {
            get { return typeof(OracleDataClientBatchingBatcherFactory); }
        }

        /// <summary>
        /// Devart dotConnecto for Oracle이 NHibernate 3.2에서 제대로 작동하려면 Devart.Data.Oracle.OracleUtils.OracleClientCompatible = true 로 설정해주어야 합니다.
        /// 이를 수행하여 NH 3.2 이상에서도 제대로 동작하도록 합니다.
        /// 참고: http://www.devart.com/forums/viewtopic.php?t=21676
        /// </summary>
        private void Intialize() {
            // NOTE: NHibernate 3.2 이상에서 Devart Oracle의 Parameter 작업이 이상 작동하는 것을 교정하기 위해 OracleClientCompatible 필드 값을 True로 설정합니다.
            // 참고: http://www.devart.com/forums/viewtopic.php?t=21676

            const string DevartDataOracleAssemblyName = "Devart.Data.Oracle";
            const string OracleUtilsTypeName = "Devart.Data.Oracle.OracleUtils";
            const string OracleClientCompatibleFieldName = "OracleClientCompatible";

            if(log.IsInfoEnabled) {
                log.Info("Devart.Data.Oracle.OracleUtils.OracleClientCompatible 값을 true로 설정합니다...");
                log.Info("NHibernate 3.2 이상에서 Devart Oracle의 Parameter 작업이 이상 작동하는 것을 교정하기 위해 OracleClientCompatible 필드 값을 True로 설정합니다.");
            }

            try {
                var asm = Assembly.Load(DevartDataOracleAssemblyName);
                var type = asm.GetType(OracleUtilsTypeName);
                var fi = type.GetField(OracleClientCompatibleFieldName, BindingFlags.Public | BindingFlags.Static);
                fi.SetValue(null, true);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Devart.Data.Oracle.OracleUtils.OracleClientCompatible 값을 true로 설정하는데 예외가 발생했습니다.", ex);

                throw;
            }
        }
    }
}