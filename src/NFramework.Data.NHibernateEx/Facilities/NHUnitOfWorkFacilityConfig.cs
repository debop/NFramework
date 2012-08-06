using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Configuration;
using Iesi.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Facilities {
    /// <summary>
    /// Unit Of Work Facitility에 대한 환경설정 정보
    /// </summary>
    public class NHUnitOfWorkFacilityConfig {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Unit Of Work Factory Id
        /// </summary>
        public const string FACTORY_ID = "id";

        /// <summary>
        /// Factory Alias
        /// </summary>
        public const string FACTORY_ALIAS = "alias";

        /// <summary>
        /// Factory need schema export ?
        /// </summary>
        public const string FACTORY_NEED_SCHEMA_EXPORT = "needSchemaExport";

        /// <summary>
        /// Factory configuration file
        /// </summary>
        public const string FACTORY_CONFIG_FILE = "configFile";

        private readonly ISet<Assembly> _assemblies = new HashedSet<Assembly>();
        private readonly ISet<Type> _entities = new HashedSet<Type>();
        private string _nhConfigurationFilename = "hibernate.cfg.xml";
        private Predicate<Type> _isCandidateForRegistry = type => false;

        private string _factoryId;
        private string _factoryAlias;
        private bool _needSchemaExport;

        /// <summary>
        /// Constructor
        /// </summary>
        public NHUnitOfWorkFacilityConfig() {}

        /// <summary>
        /// Initialize NHUnitOfWorkFacilityConfig with the specified assemblyName.
        /// </summary>
        /// <param name="assemblyName"></param>
        public NHUnitOfWorkFacilityConfig(string assemblyName) {
            assemblyName.ShouldNotBeWhiteSpace("assemblyName");

            _assemblies.Add(Assembly.Load(assemblyName));
        }

        /// <summary>
        /// Initialize NHUnitOfWorkFacilityConfig with the specified assembly.
        /// </summary>
        /// <param name="assembly"></param>
        public NHUnitOfWorkFacilityConfig(Assembly assembly) {
            assembly.ShouldNotBeNull("assembly");
            _assemblies.Add(assembly);
        }

        /// <summary>
        /// Castle Windsor 환경설정 정보로 부터 NHUnitOfWorkFacilityConfig를 생성한다.
        /// </summary>
        /// <param name="factoryConfig"></param>
        public NHUnitOfWorkFacilityConfig(IConfiguration factoryConfig) {
            factoryConfig.ShouldNotBeNull("factoryConfig");
            BuildConfiguration(factoryConfig);
        }

        /// <summary>
        /// NHibernate용 Configuration 파일 명을 지정한다.
        /// </summary>
        /// <param name="nhibernateConfigFilename"></param>
        /// <returns></returns>
        public NHUnitOfWorkFacilityConfig NHibernateConfiguration(string nhibernateConfigFilename) {
            nhibernateConfigFilename.ShouldNotBeWhiteSpace("nhibernateConfigFilename");

            _nhConfigurationFilename = nhibernateConfigFilename;
            return this;
        }

        /// <summary>
        /// 특정 Entity를 다루는 Repository가 정의되었는지 파악하는 Predicator를 지정한다.
        /// </summary>
        /// <param name="typeIsSpecifiedBy"></param>
        /// <returns></returns>
        public NHUnitOfWorkFacilityConfig RegisterEntitiesWhere(Predicate<Type> typeIsSpecifiedBy) {
            typeIsSpecifiedBy.ShouldNotBeNull("typeIsSpecifiedBy");
            _isCandidateForRegistry = typeIsSpecifiedBy;
            return this;
        }

        /// <summary>
        /// 지정된 Assembly 명에 해당하는 Assembly를 로드하여 NHibernate용 Mapping Assembly로 추가한다.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public NHUnitOfWorkFacilityConfig AddAssembly(string assemblyName) {
            assemblyName.ShouldNotBeWhiteSpace("assemblyName");

            return AddAssembly(Assembly.Load(assemblyName));
        }

        /// <summary>
        /// 지정된 Assembly를 NHibernate용 Mapping Assembly로 추가한다.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public NHUnitOfWorkFacilityConfig AddAssembly(Assembly assembly) {
            assembly.ShouldNotBeNull("assembly");
            _assemblies.Add(assembly);
            return this;
        }

        /// <summary>
        /// NHibernate용 Entity로 추가한다.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public NHUnitOfWorkFacilityConfig AddEntity(Type entity) {
            entity.ShouldNotBeNull("entity");
            _entities.Add(entity);
            return this;
        }

        /// <summary>
        /// 특정 Entity를 처리하는 Repository가 정의되어 있는지 파악하는 Predicator
        /// </summary>
        public Predicate<Type> IsCandidateForRepository {
            get { return _isCandidateForRegistry; }
        }

        /// <summary>
        /// 매핑된 NHibernate용 Entity가 속한 Assembly들
        /// </summary>
        public Assembly[] Assemblies {
            get { return _assemblies.ToArray(); }
        }

        /// <summary>
        /// Types of Mapped class
        /// </summary>
        public Type[] Entities {
            get { return _entities.ToArray(); }
        }

        /// <summary>
        /// NHibernate용 Configuration Filename
        /// </summary>
        public string NHibernateConfigurationFilename {
            get { return _nhConfigurationFilename; }
        }

        /// <summary>
        /// Facility 환경설정의 Factory Id
        /// </summary>
        public string FactoryId {
            get { return _factoryId; }
        }

        /// <summary>
        /// Facility 환경설정의 Factory에 대한 Alias (Session Factory Name 대용)
        /// </summary>
        public string FactoryAlias {
            get { return _factoryAlias; }
        }

        /// <summary>
        /// Session Factory를 
        /// </summary>
        public bool NeedSchemaExport {
            get { return _needSchemaExport; }
        }

        private void BuildConfiguration(IConfiguration factoryConfig) {
            if(IsDebugEnabled)
                log.Debug("Castle Windsor 환경설정에서 Factory 정보를 가져옵니다...");

            _factoryId = factoryConfig.Attributes[FACTORY_ID];
            _factoryAlias = factoryConfig.Attributes[FACTORY_ALIAS];
            _needSchemaExport = factoryConfig.Attributes[FACTORY_NEED_SCHEMA_EXPORT].AsBool(false);
            _nhConfigurationFilename = factoryConfig.Attributes[FACTORY_CONFIG_FILE];

            if(IsDebugEnabled)
                log.Debug("로드된 Factory 정보. Id=[{0}], Alias=[{1}], NeedSchemaExport=[{2}], NHibernate Cfg File=[{3}]",
                          _factoryId, _factoryAlias, _needSchemaExport, _nhConfigurationFilename);
        }
    }
}