using System;
using System.Collections.Concurrent;
using NSoft.NFramework.Data.SqlServer;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Factory for <see cref="IAdoRepository"/>
    /// </summary>
    public class AdoRepositoryFactory : IAdoRepositoryFactory {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// database name : repository 
        /// </summary>
        private static readonly ConcurrentDictionary<string, IAdoRepository> _repositories =
            new ConcurrentDictionary<string, IAdoRepository>();

        private static readonly object _syncLock = new object();

        ///<summary>
        /// Singleton instance.
        ///</summary>
        public static AdoRepositoryFactory Instance {
            get { return SingletonTool<AdoRepositoryFactory>.Instance; }
        }

        /// <summary>
        /// Create Repository for ADO.NET using specified dbName
        /// </summary>
        /// <param name="dbName">database name</param>
        /// <returns></returns>
        public IAdoRepository CreateRepository(string dbName = null) {
            if(dbName.IsWhiteSpace())
                dbName = AdoTool.DefaultDatabaseName;

            if(IsDebugEnabled)
                log.Debug("캐시에서 IAdoRepository를 구해보고, 없으면 새로운 IAdoRepository를 생성하여 캐시에 저장 후 반환합니다. dbName=[{0}]", dbName);

            try {
                lock(_syncLock)
                    return _repositories.GetOrAdd(dbName, name => new AdoRepositoryImpl(name));
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("IAdoRepository 인스턴스를 생성하는데 실패했습니다. dbName=[{0}]", dbName);
                    log.Error(ex);
                }

                throw;
            }
        }

        public IAdoRepository CreateRepositoryByProvider(string dbName = null) {
            if(dbName.IsWhiteSpace())
                dbName = AdoTool.DefaultDatabaseName;

            if(IsDebugEnabled)
                log.Debug("캐시에서 IAdoRepository를 구해보고, 없으면 새로운 IAdoRepository를 생성하여 캐시에 저장 후 반환합니다. dbName=[{0}]", dbName);

            try {
                lock(_syncLock)
                    return _repositories.GetOrAdd(dbName, GetAdoRepositoryFactoryFunction(dbName));
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("IAdoRepository 인스턴스를 생성하는데 실패했습니다. dbName=[{0}]", dbName);
                    log.Error(ex);
                }
                throw;
            }
        }

        /// <summary>
        /// Repository Cache를 제거합니다.
        /// </summary>
        public void ClearRepositoryCache() {
            lock(_syncLock)
                _repositories.Clear();
        }

        /// <summary>
        /// 지정된 DB Name의 Provider에 따라, IAdoRepository 인스턴스 생성 함수를 다르게 합니다. 
        /// SQL Server인 경우, 자동으로 <see cref="SqlRepositoryImpl"/>을 사용하게 하여, 확장을 보장받을 수 있게 하였습니다.
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        internal static Func<string, IAdoRepository> GetAdoRepositoryFactoryFunction(string dbName) {
            if(dbName.IsNotWhiteSpace()) {
                var settings = ConfigTool.GetConnectionSettings(dbName);

                var isSqlClient = settings != null && settings.ProviderName.Contains("SqlClient");

                if(isSqlClient) {
                    if(IsDebugEnabled)
                        log.Debug("Database가 SQL Server이므로, SqlRepositoryImpl을 생성하는 Factory 함수를 반환합니다. dbName=[{0}]", dbName);

                    return name => new SqlRepositoryImpl(name);
                }
            }

            if(IsDebugEnabled)
                log.Debug("Database가 SQL Server가 아니므로, 기본 Repository인 AdoRepositoryImpl를 생성하는 Factory 함수를 반환합니다. dbName=[{0}]", dbName);

            return name => new AdoRepositoryImpl(name);
        }
    }
}