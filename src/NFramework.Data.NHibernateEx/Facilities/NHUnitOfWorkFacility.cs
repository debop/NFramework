using System.Reflection;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;

namespace NSoft.NFramework.Data.NHibernateEx.Facilities {
    /// <summary>
    /// Castle Facility class for NHibernate Unit-Of-Work Pattern
    /// </summary>
    public class NHUnitOfWorkFacility : AbstractFacility {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly NHUnitOfWorkFacilityConfig _config;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config"></param>
        public NHUnitOfWorkFacility(NHUnitOfWorkFacilityConfig config) {
            config.ShouldNotBeNull("config");
            _config = config;
        }

        /// <summary>
        /// 환경설정에 정의된 Assembly 들
        /// </summary>
        public Assembly[] Assemblies {
            get { return _config.Assemblies; }
        }

        ///<summary>
        ///  The custom initialization for the Facility.
        ///</summary>
        protected override void Init() {
            if(IsDebugEnabled)
                log.Debug("NHUnitOfWorkFacility 초기화를 시작합니다...");

            Kernel.Register(
                Component
                    .For(typeof(INHRepository<>))
                    .ImplementedBy(typeof(NHRepository<>))
                    .OnlyNewServices());
            // .Unless(Component.ServiceAlreadyRegistered));

            //if(Kernel.HasComponent(typeof(INHRepository<>)) == false)
            //{
            //    if(IsDebugEnabled)
            //        log.Debug("INHRepository<T> 형식의 제너릭 컴포넌트를 Castle을 통해 등록합니다.");

            //    Kernel.Register(Component.For(typeof(INHRepository<>)).ImplementedBy(typeof(NHRepository<>)));
            //}

            if(Kernel.HasComponent(typeof(IUnitOfWorkFactory)) == false) {
                if(IsDebugEnabled)
                    log.Debug("INHUnitOfWorkFactory 형식의 컴포넌트 등록을 시작합니다.");

                var registerFactory =
                    Component.For<IUnitOfWorkFactory>()
                        .ImplementedBy<NHUnitOfWorkFactory>();

                // configuration file이 존재한다면
                registerFactory.DependsOn(Dependency.OnValue("ConfigurationFileName", _config.NHibernateConfigurationFilename));

                //registerFactory.Parameters(Parameter
                //                            .ForKey("ConfigurationFileName")
                //                            .Eq(_config.NHibernateConfigurationFilename));

                // Test Mode라면, DatabaseTestFixtureBase에서 InitializeNHibernateAndIoC 메소드를 통해 초기화를 수행한다.
                // 실제 Runtime 시라면 NHUnitOfWorkFactory에 관련  assemblies 속성정보를 가져온다.
                if(DatabaseTestFixtureBase.IsRunningInTestMode == false) {
                    registerFactory.DependsOn(Dependency.OnValue("assemblies", Assemblies));

                    //registerFactory.DependsOn(Property
                    //                            .ForKey("assemblies")
                    //                            .Eq(Assemblies));
                }

                Kernel.Register(registerFactory.OnlyNewServices()); //.Unless(Component.ServiceAlreadyRegistered));

                if(IsDebugEnabled)
                    log.Debug("INHUnitOfWorkFactory 를 Castle Component로 등록했습니다.");
            }

            // NOTE : Repository.Wrapper 에서 등록되지 않은 것들은 등록하게끔 했으므로 필요 없다. (단 필터링 때문에 이렇게 넣은 것이다.)
            Kernel.Register(
                Component.For<INHInitializationAware>()
                    .Named("entitiesToRepositories")
                    .Instance(new NHIoCInitializationAware(_config.IsCandidateForRepository))
                    .OnlyNewServices());
            // .Unless(Component.ServiceAlreadyRegistered));
        }
    }
}