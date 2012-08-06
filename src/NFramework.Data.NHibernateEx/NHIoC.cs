using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using NHibernate.Metadata;
using NSoft.NFramework.Data.NHibernateEx.Interceptors;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate 용 Entity 정보를 파악해서, INHRepository{TEntity} 를 <see cref="IoC"/>를 통해 <see cref="WindsorContainer"/>에 등록한다.
    /// </summary>
    public static class NHIoC {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 <see cref="ISessionFactory"/> 인스턴스에 등록된 모든 NHibernate용 Entity를 조사해서,
        /// 자동으로 Generic Dao (INHRepository{T} 구현 클래스)를 <see cref="IKernel"/>에 Component로 등록한다.
        /// 이렇게 하면, NHRepository{T} 하나만 만들고, 실제 Entity별의 NHRepository는 Castle에 자동으로 등록되고, Instancing될 것이다!!!
        /// (예 NHRepository{Blog}, NHRepository{Customer} 등을 Castle Component로 정의하지 않아도, 이 함수에서 자동으로 조사하여, IoC에 등록시켜 준다는 뜻!!!)
        /// </summary>
        /// <param name="windsorContainer">Instance of Castle.Windsor.IWindsorContainer</param>
        /// <param name="sessionFactory">Instance of NHibernate.ISessionFactory</param>
        /// <param name="repositoryType">INHRepository{T} 를 구현한 Concrete Class Type</param>
        /// <param name="isCandidateForRepository">NHibernate의 매핑된 Entity 중에 IoC Container에 등록할 Type을 선별하는 Predicator</param>
        public static void Register(IWindsorContainer windsorContainer,
                                    ISessionFactory sessionFactory,
                                    Type repositoryType,
                                    Predicate<Type> isCandidateForRepository) {
            Register(windsorContainer.Kernel, sessionFactory, repositoryType, isCandidateForRepository);
        }

        /// <summary>
        /// 지정된 <see cref="ISessionFactory"/> 인스턴스에 등록된 모든 NHibernate용 Entity를 조사해서,
        /// 자동으로 Generic Dao (INHRepository{T} 구현 클래스)를 <see cref="IKernel"/>에 Component로 등록한다.
        /// 이렇게 하면, NHRepository{T} 하나만 만들고, 실제 Entity별의 NHRepository는 Castle에 자동으로 등록되고, Instancing될 것이다!!!
        /// (예 NHRepository{Blog}, NHRepository{Customer} 등을 Castle Component로 정의하지 않아도, 이 함수에서 자동으로 조사하여, IoC에 등록시켜 준다는 뜻!!!)
        /// </summary>
        /// <param name="sessionFactory">NHibernate Session Factory</param>
        /// <param name="kernel">Castle.MicroKernel 인스턴스</param>
        /// <param name="repositoryType">INHRepository{T} 를 구현한 Concrete Class Type</param>
        /// <param name="isCandidateForRepository">NHibernate의 매핑된 Entity 중에 IoC Container에 등록할 Type을 선별하는 Predicator</param>
        public static void Register(IKernel kernel,
                                    ISessionFactory sessionFactory,
                                    Type repositoryType,
                                    Predicate<Type> isCandidateForRepository) {
            if(IsDebugEnabled)
                log.Debug("NHibernate SessionFactory에 등록된 Entity Class에 대한 " +
                          @"Generic Repository (INHRepository<TEntity>) 의 인스턴스를 생성합니다. repositoryType=[{0}]", repositoryType);

            if(IsImplementsOfGenericNHRepository(repositoryType) == false)
                throw new InvalidOperationException("Repository must be a type inheriting from INHRepository<T>, " +
                                                    "and must be an open generic type. Sample: typeof(NHRepository<>).");

            // GetAllClassMetadata 는 IDictionary<Type, IClassMetadata> 이고 Type은 Mapping된 entity의 Type이다.
            //
            foreach(IClassMetadata meta in sessionFactory.GetAllClassMetadata().Values) {
                var mappedClass = meta.GetMappedClass(EntityMode.Poco);
                if(mappedClass == null)
                    continue;

                foreach(Type interfaceType in mappedClass.GetInterfaces()) {
                    if(isCandidateForRepository(interfaceType)) {
                        if(IsDebugEnabled)
                            log.Debug("Register Generic Repository. INHRepository<{0}>", interfaceType.FullName);

                        // NOTE : INHRepository<TEnitity> 는 꼭 ConcreteType 속성 (Entity의 Type) 을 가져야한다.
                        //
                        kernel.Register(
                            Component
                                .For(typeof(INHRepository<>).MakeGenericType(interfaceType))
                                .ImplementedBy(repositoryType.MakeGenericType(interfaceType))
                                .DependsOn(Dependency.OnValue("ConcreteType", mappedClass))
                                .OnlyNewServices());
                        //.DependsOn(Property.ForKey("ConcreteType").Eq(mappedClass))
                        //.Unless(Component.ServiceAlreadyRegistered));
                    }
                }
            }
        }

        /// <summary>
        /// 지정된 Repository의 Type이 <see cref="INHRepository{T}"/> 인터페이스를 구현한 형식인지 판단한다.
        /// </summary>
        /// <param name="repositoryType">INHRepository{T} 를 구현한 Concrete Class Type (예: NHRepository{User})</param>
        /// <returns>지정된 Repository 형식이 INHRepository{} 형식 여부를 반환.</returns>
        private static bool IsImplementsOfGenericNHRepository(Type repositoryType) {
            if(repositoryType.IsGenericTypeDefinition)
                foreach(Type intfType in repositoryType.GetInterfaces())
                    if(intfType.GetGenericTypeDefinition() == typeof(INHRepository<>))
                        return true;

            return false;
        }

        /// <summary>
        /// IoC에 등록된 NHibernate.IInterceptor 를 가져온다. 
        /// 만약 IoC Container 에 등록되어 있지 않다면 <see cref="EntityStateInterceptor"/>를 등록하고, Resolve를 수행한다.
        /// </summary>
        /// <returns></returns>
        public static NHibernate.IInterceptor ResolveInterceptor() {
            if(IsDebugEnabled)
                log.Debug("IoC Container에 등록된 컴포넌트 중에 Service가 NHibernate.IInterceptor 를 Resolve 합니다...");

            if(IoC.IsNotInitialized)
                IoC.InitializeWithEmptyContainer();

            // Guard.Assert(IoC.IsInitialized, "IoC가 초기화 되지 않았습니다. IoC.Initialize()를 호출해 주셔야 합니다.");

            try {
                if(IoC.Container.Kernel.HasComponent(typeof(NHibernate.IInterceptor))) {
                    var interceptor = IoC.Resolve<NHibernate.IInterceptor>();

                    if(IsDebugEnabled)
                        log.Debug("NHibernate.IInterceptor 형식을 로드했습니다. Interceptor=[{0}]", interceptor);

                    return interceptor;
                }

                // 등록된 것이 없다면 EntityStateInterceptor를 등록하고, Resolve해서 반환한다.
                //
                return IoC.TryResolve<NHibernate.IInterceptor, EntityStateInterceptor>(LifestyleType.Singleton);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("NHibernate.Interceptor를 IoC Container로부터 Resolve하지 못했습니다.");
                    log.Warn(ex);
                }
            }

            return null;
        }

        private static MultipleInterceptor _interceptors = null;

        /// <summary>
        /// 이제 무조건 여러개의 NHibernate.IInterceptor를 등록할 수 있도록 했음!!! 등록된 것이 없으면 최소한 EntityStateInterceptor라도 등록되도록 했음
        /// </summary>
        /// <returns></returns>
        public static MultipleInterceptor ResolveAllInterceptors() {
            return _interceptors ?? (_interceptors = ResolveAllInterceptorsInternal());
        }

        /// <summary>
        /// 이제 무조건 여러개의 NHibernate.IInterceptor를 등록할 수 있도록 했음!!! 등록된 것이 없으면 최소한 EntityStateInterceptor라도 등록되도록 했음
        /// </summary>
        /// <returns></returns>
        public static MultipleInterceptor ResolveAllInterceptorsInternal() {
            if(IsDebugEnabled)
                log.Debug("IoC Container에 등록된 컴포넌트 중에 Service가 NHibernate.IInterceptor 를 Resolve 합니다...");

            var results = new List<IInterceptor>();

            try {
                var interceptors = IoC.ResolveAll<IInterceptor>();
                if(interceptors != null)
                    results.AddRange(interceptors);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("NHibernate.Interceptor를 IoC Container로부터 Resolve하지 못했습니다!!! 예외는 무시됩니다.", ex);
            }

            if(results.Any(intr => intr is EntityStateInterceptor) == false)
                results.Add(IoC.TryResolve<IInterceptor, EntityStateInterceptor>(LifestyleType.Singleton));

            // 최종적으로 MultipleInterceptor 형태를 가지므로, 내부에 MultipleInterceptor를 가지면 안됩니다.
            //
            return new MultipleInterceptor(results.Where(intr => intr.GetType() != typeof(MultipleInterceptor)));
        }
    }
}