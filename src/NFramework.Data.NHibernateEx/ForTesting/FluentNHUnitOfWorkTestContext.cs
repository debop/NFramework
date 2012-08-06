using System;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions;
using NHibernate.Cfg;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// NHibernate, FluentNHibernate를 사용한 Data Layer를 테스트를 위한 UnitOfWork Context
    /// </summary>
    public class FluentNHUnitOfWorkTestContext : NHUnitOfWorkTestContext {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private IConvention[] _conventions = null;

        public FluentNHUnitOfWorkTestContext(UnitOfWorkTestContextDbStrategy dbStrategy,
                                             string windsorConfigPath,
                                             MappingInfo mappingInfo,
                                             Action<Configuration> configAction,
                                             params IConvention[] conventions)
            : base(dbStrategy, windsorConfigPath, mappingInfo, configAction) {
            _conventions = conventions;
        }

        protected override Configuration BuildConfiguration() {
            if(IsDebugEnabled)
                log.Debug("NHibernate Configuration을 FluentConfiguration을 이용하여 빌드합니다...");

            var configuration =
                Fluently.Configure()
                    .Database(DatabaseSetup())
                    .Mappings(m => {
                                  foreach(var asm in MappingInfo.MappingAssemblies) {
                                      var mappingContainer = m.FluentMappings.AddFromAssembly(asm);
                                      if(_conventions != null)
                                          mappingContainer.Conventions.Add(_conventions);

                                      m.HbmMappings.AddFromAssembly(asm);
                                  }
                              })
                    .ExposeConfiguration(cfg => cfg.SetProperties(DbStrategy.NHibernateProperties))
                    .BuildConfiguration();

            if(ConfigAction != null)
                ConfigAction(configuration);

            if(log.IsInfoEnabled)
                log.Info("NHibernate Configuration 생성에 성공했습니다!!!");

            return configuration;
        }

        protected virtual IPersistenceConfigurer DatabaseSetup() {
            switch(DbStrategy.DatabaseEngine) {
                case DatabaseEngine.DevartOracle:
                    return DevartOracleDataClientConfiguration.Oracle10;

                case DatabaseEngine.MsSql2005:
                case DatabaseEngine.MsSql2005Express:
                    return MsSqlConfiguration.MsSql2005;

                case DatabaseEngine.MsSqlCe:
                case DatabaseEngine.MsSqlCe40:
                    return MsSqlCeConfiguration.Standard;

                case DatabaseEngine.MySql:
                    return MySQLConfiguration.Standard;

                case DatabaseEngine.OdpNet:
                    return OracleDataClientConfiguration.Oracle10;

                case DatabaseEngine.PostgreSql:
                    return PostgreSQLConfiguration.PostgreSQL82;

                case DatabaseEngine.SQLite:
                    return SQLiteConfiguration.Standard.InMemory();

                case DatabaseEngine.SQLiteForFile:
                    return SQLiteConfiguration.Standard.UsingFile(DbStrategy.DatabaseName);

                default:
                    throw new NotSupportedException("지원하지 않는 Database 입니다. Database=" + DbStrategy.DatabaseEngine);
            }
        }
    }
}