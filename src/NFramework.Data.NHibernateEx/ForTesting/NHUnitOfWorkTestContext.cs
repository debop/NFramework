using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// NHibernate용 테스트를 위한 UnitOfWork Context
    /// </summary>
    public class NHUnitOfWorkTestContext : UnitOfWorkTestContext {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private Configuration _config;
        private ISessionFactory _sessionFactory;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="dbStrategy">테스트용 DB 생성 전략</param>
        /// <param name="windsorConfigPath">IoC 환경설정 파일경로</param>
        /// <param name="mappingInfo">NHibernate 매핑 정보</param>
        /// <param name="configAction">Configuration 빌드 시 추가할 사항을 정의한 Action</param>
        public NHUnitOfWorkTestContext(UnitOfWorkTestContextDbStrategy dbStrategy,
                                       string windsorConfigPath,
                                       MappingInfo mappingInfo,
                                       Action<Configuration> configAction)
            : base(dbStrategy, windsorConfigPath, mappingInfo, configAction) {}

        /// <summary>
        /// NHibernate 환경설정 정보
        /// </summary>
        public override Configuration NHConfiguration {
            get { return _config ?? (_config = BuildConfiguration()); }
        }

        /// <summary>
        /// NHibernate ISessionFactory for Testing
        /// </summary>
        public override ISessionFactory SessionFactory {
            get { return _sessionFactory ?? (_sessionFactory = BuildSessionFactory()); }
        }

        /// <summary>
        /// IoC Container와 UnitOfWorkFactory를 초기화한다.
        /// </summary>
        public override void InitializeContainerAndUnitOfWorkFactory() {
            if(IsDebugEnabled)
                log.Debug("Initialize Windsor Container and UnitOfWorkFactory");

            UnitOfWork.Stop();
            UnitOfWork.DisposeUnitOfWorkFactory();

            ResetWindsorContainer();

            _config = null;
            _sessionFactory = null;

            if(Container != null) {
                IoC.Initialize(Container);

                if(IsDebugEnabled)
                    log.Debug("Resolve INHUnitOfWorkFactory");

                var nhUnitOfWorkFactory = (NHUnitOfWorkFactory)IoC.Resolve<IUnitOfWorkFactory>();

                nhUnitOfWorkFactory.RegisterSessionFactory(SessionFactory);
            }
        }

        protected virtual ISessionFactory BuildSessionFactory() {
            if(IsDebugEnabled)
                log.Debug("Build Session Factory...");

            var factory = NHConfiguration.BuildSessionFactory();

            foreach(INHInitializationAware initializer in GetNHInitializer())
                initializer.Initialized(NHConfiguration, factory);

            if(log.IsInfoEnabled)
                log.Info("Build SessionFactory is Success!!!");

            return factory;
        }

        protected virtual Configuration BuildConfiguration() {
            if(IsDebugEnabled)
                log.Debug("NHibernate Configuration을 빌드합니다.");

            foreach(INHInitializationAware initializer in GetNHInitializer())
                initializer.BeforeInitialzation();

            var cfg = new Configuration
                      {
                          Properties = DbStrategy.NHibernateProperties
                      };

            foreach(var import in MappingInfo.QueryLanguageImports)
                cfg.Imports[import.Key] = import.Value;

            foreach(var asm in MappingInfo.MappingAssemblies) {
                if(IsDebugEnabled)
                    log.Debug("NHibernate configuration에 Mapping Assembly를 추가합니다. assembly=[{0}]", asm.FullName);

                cfg.AddAssembly(asm);
            }

            foreach(INHInitializationAware initializer in GetNHInitializer())
                initializer.Configured(cfg);

            if(ConfigAction != null)
                With.TryAction(() => ConfigAction(cfg));

            if(log.IsInfoEnabled)
                log.Info("Create NHibernate Configuration is Success!!!");

            return cfg;
        }

        protected virtual IEnumerable<INHInitializationAware> GetNHInitializer() {
            var initializers = new List<INHInitializationAware>();

            if(MappingInfo.NHInitializationAware != null)
                initializers.Add(MappingInfo.NHInitializationAware);
            else if(IoC.IsInitialized && IoC.Container.Kernel.HasComponent(typeof(INHInitializationAware)))
                initializers.AddRange(IoC.ResolveAll<INHInitializationAware>());

            if(initializers.Count == 0)
                yield break;

            foreach(INHInitializationAware initializer in initializers)
                yield return initializer;
        }

        /// <summary>
        /// Reset Castle.Winsor Container
        /// </summary>
        protected void ResetWindsorContainer() {
            if(_containerConfigPath.IsNotWhiteSpace()) {
                IoC.Reset();
                _container = null;
            }
            else {
                IoC.Reset(_container);
            }
        }
    }
}