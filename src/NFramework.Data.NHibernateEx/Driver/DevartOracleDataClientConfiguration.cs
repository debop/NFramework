using NHibernate.Dialect;
using NHibernate.Driver;

namespace FluentNHibernate.Cfg.Db {
    /// <summary>
    /// FluentNHibernate 으로 DevartOracle Driver를 사용하여 Database를 제작합니다.
    /// </summary>
    public class DevartOracleDataClientConfiguration :
        PersistenceConfiguration<DevartOracleDataClientConfiguration, DevartOracleConnectionStringBuilder> {
        protected DevartOracleDataClientConfiguration() {
            Driver<DevartOracleDriver>();
        }

        public static DevartOracleDataClientConfiguration Oracle10 {
            get { return new DevartOracleDataClientConfiguration().Dialect<Oracle10gDialect>(); }
        }

        public static DevartOracleDataClientConfiguration Oracle9 {
            get { return new DevartOracleDataClientConfiguration().Dialect<Oracle9iDialect>(); }
        }
        }
}