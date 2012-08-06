using System;
using System.Linq;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Configuration.Interpreters;
using NHibernate;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Facilities {
    /// <summary>
    /// Multiple UnitOfWork를 지원하기 위한 Castle의 Facility
    /// </summary>
    public class NHMultipleUnitOfWorkFacility : AbstractFacility {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// UnitOfWork Facility의 환경설정에서 기본 Factory를 지정하는 attribute name
        /// </summary>
        public const string FACILITY_DEFAULT_FACTORY = "defaultFactory";

        /// <summary>
        /// UnitOfWork Facility의 환경설정에서 Factory를 지정하는 attribute name
        /// </summary>
        public const string FACTORY = "factory";

        private NHUnitOfWorkFacilityConfig[] _configs;
        private string _defaultFactoryName;

        /// <summary>
        /// default constructor
        /// </summary>
        public NHMultipleUnitOfWorkFacility() {}

        /// <summary>
        /// Initialize a new instance of NHMultipleUnitOfWorkFacility with the specified NHUnitOfWorkFacilityConfig array.
        /// </summary>
        /// <param name="configs"></param>
        public NHMultipleUnitOfWorkFacility(NHUnitOfWorkFacilityConfig[] configs) {
            if(IsDebugEnabled)
                log.Debug("NHMultipleUnitOfWorkFacitity 생성.");

            _configs = configs;
        }

        /// <summary>
        /// Initialize current facility
        /// </summary>
        protected override void Init() {
            if(IsDebugEnabled)
                log.Debug("Multiple UnitOfWork를 위한 Facility를 초기화 합니다.");

            Kernel.Register(Component.For(typeof(INHRepository<>)).ImplementedBy(typeof(NHRepository<>)));

            var multipleUowFactory = new NHMultipleUnitOfWorkFactory();

            if(_configs == null) {
                if(IsDebugEnabled)
                    log.Debug("프로그램에서 지정한 환경설정 정보가 없으므로, 환경설정 파일에서 정보를 얻습니다.");

                AssertConfigurations();
                _configs = FacilityConfig.Children.Select(factoryConfig => new NHUnitOfWorkFacilityConfig(factoryConfig)).ToArray();
                multipleUowFactory.DefaultFactoryName = _defaultFactoryName;
            }

            if(IsDebugEnabled)
                log.Debug("환경설정에 지정된 Factory 정보로부터, NHUnitOfWorkFactory를 생성합니다.");

            foreach(NHUnitOfWorkFacilityConfig config in _configs) {
                var nestedUnitOfWorkFactory = new NHUnitOfWorkFactory(config.NHibernateConfigurationFilename);
                nestedUnitOfWorkFactory.RegisterSessionFactory(CreateSessionFactory(config));
                multipleUowFactory.Add(nestedUnitOfWorkFactory);
            }

            Kernel.Register(Component.For<IUnitOfWorkFactory>().Instance(multipleUowFactory));
            // Kernel.AddComponentInstance<IUnitOfWorkFactory>(multipleUowFactory);

            if(IsDebugEnabled)
                log.Debug("Kernel에 NHMultipleUnitOfWorkFactory의 인스턴스를 등록했습니다.");
        }

        private ISessionFactory CreateSessionFactory(NHUnitOfWorkFacilityConfig config) {
            config.ShouldNotBeNull("config");

            if(IsDebugEnabled)
                log.Debug("SessionFactory를 생성합니다. NHibernate Configuration Filename:" + config.NHibernateConfigurationFilename);

            try {
                var cfg = config.NHibernateConfigurationFilename.BuildConfiguration();

                if(config.FactoryAlias.IsNotWhiteSpace())
                    cfg.SetProperty(NHibernate.Cfg.Environment.SessionFactoryName, config.FactoryAlias);

                foreach(Type mappedEntity in config.Entities) {
                    if(cfg.GetClassMapping(mappedEntity) == null)
                        cfg.AddClass(mappedEntity);
                }

                var sessionFactory = cfg.BuildSessionFactory();

                NHIoC.Register(Kernel, sessionFactory, typeof(NHRepository<>), config.IsCandidateForRepository);

                if(IsDebugEnabled)
                    log.Debug("새로운 SessionFactory를 생성하고, Entity들을 등록하고, NHRepository<TEntity> 를 IoC에 등록했습니다.");

                return sessionFactory;
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error(ex);

                throw;
            }
        }

        private void AssertConfigurations() {
            if(FacilityConfig == null)
                throw new ConfigurationProcessingException("NHMultipleUnitOfWorkFacility의 환경설정 정보가 없습니다!!!");

            if(FacilityConfig.Children[FACTORY] == null)
                throw new ConfigurationProcessingException("적어도 하나 이상의 factory element가 정의되어야 합니다!!!");

            _defaultFactoryName = FacilityConfig.Attributes[FACILITY_DEFAULT_FACTORY];
        }
    }
}