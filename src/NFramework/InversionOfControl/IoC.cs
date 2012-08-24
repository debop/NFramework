using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.Core;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.InversionOfControl {
    /// <summary>
    /// Castle (http://www.castleproject.org) 의 IoC Library인 Castle.Windsor를 이용하여 IoC/DI 를 수행하는 Utility Class입니다.
    /// </summary>
    /// <remarks>
    /// Castle.Core.dll, Castle.MicroKernel.dll, Castle.Windsor.dll을 참조해야 합니다.
    /// </remarks>
    /// <example>
    /// <code>
    ///		if(IoC.IsNotInitialized)
    ///			IoC.Initialize(new WindsorContainer(new XmlInterpreter());  // castle configuration file로부터 환경 설정을 수행한다.
    /// 
    ///		ICompressor component = IoC.Resolve{ICompressor}();        // intanciate class that is implemented ICompressor 
    /// </code>
    /// </example>
    public static partial class IoC {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private const string LocalContainerKey = @"NSoft.NFramework.InversionOfControl.LocalContainer.Key";
                             // Guid.NewGuid().AsString(); //new object();

        private const string IoCNotInitializedMsg = @"Container가 초기화되어 있지 않습니다. 사용하기 전에 IoC.Initialize() 를 호출해주시기 바랍니다.";

        /// <summary>
        /// synchronous lock
        /// </summary>
        private static readonly object _syncLock = new object();

        /// <summary>
        /// IoC 전역 컨테이너가 초기화가 되었는지 여부
        /// </summary>
        public static bool IsInitialized {
            get { return (GlobalContainer != null); }
        }

        /// <summary>
        /// IoC 전역 컨테이너가 초기화가 되지 않았는지 여부
        /// </summary>
        public static bool IsNotInitialized {
            get { return (GlobalContainer == null); }
        }

        /// <summary>
        /// Current Container <see cref="IWindsorContainer"/>
        /// </summary>
        public static IWindsorContainer Container {
            get {
                var container = LocalContainer ?? GlobalContainer;
                Guard.Assert(() => container != null, IoCNotInitializedMsg);

                return container;
            }
        }

        /// <summary>
        /// 제한된 영역에서만 사용할 임시적인 <see cref="IWindsorContainer"/>를 나타낸다.
        /// </summary>
        private static IWindsorContainer LocalContainer {
            get {
                if(LocalContainerStack.Count == 0)
                    return null;

                return LocalContainerStack.Peek();
            }
        }

        /// <summary>
        /// 기본 Container
        /// </summary>
        internal static IWindsorContainer GlobalContainer { get; set; }

        [ThreadStatic] private static Stack<IWindsorContainer> _localContainerStack;

        /// <summary>
        /// Thread Context 영역에서 사용되는 Container가 중첩 사용이 가능하도록 Stack에 보관한다.
        /// </summary>
        private static Stack<IWindsorContainer> LocalContainerStack {
            get {
                if(_localContainerStack == null)
                    lock(_syncLock)
                        if(_localContainerStack == null) {
                            var stack = new Stack<IWindsorContainer>();
                            Thread.MemoryBarrier();
                            _localContainerStack = stack;
                        }

                return _localContainerStack;
            }
        }

        /// <summary>
        /// 초기화가 되어 있지 않다면 Castle.Windsor의 환경설정 정보를 바탕으로 IoC를 초기화 합니다.
        /// </summary>
        /// <example>
        /// <code>
        /// // 아래 두 코드는 수행하는 내용이 같다.
        /// 
        /// // Initialize IoC with default application configuration.
        /// IoC.Initialize(); 
        /// 
        /// // Initialize IoC with default configuration defined in App/Web configuration
        /// if(IoC.IsNotInitialized)
        ///		IoC.Initialize(new WindsorContainer(new XmlInterpreter()));
        /// </code>
        /// </example>
        public static void Initialize() {
#if !SILVERLIGHT
            Initialize(new WindsorContainer().Install(Castle.Windsor.Installer.Configuration.FromAppConfig()));
#else
			Initialize(new WindsorContainer().Install(FromAssembly.This()));
#endif
        }

#if !SILVERLIGHT

        /// <summary>
        /// 지정된 IoC / DI 환경설정 정보를 바탕으로  IoC Container를 초기화 합니다.
        /// </summary>
        /// <param name="xmlfile"></param>
        public static void InitializeFromXmlFile(string xmlfile) {
            if(IsInfoEnabled)
                log.Info("파일[{0}]로부터 환경설정 정보를 얻어 IoC를 초기화합니다.", xmlfile);

            Initialize(new WindsorContainer(xmlfile));
        }

        /// <summary>
        /// 리소스에 저장된 환경설정 파일을 로드하여 초기화합니다.
        /// 참고 : http://www.primordialcode.com/blog/post/castle-windsor-enabling-xml-configuration-files-silverlight
        /// </summary>
        /// <param name="assembly">리소스를 소유한 Assembly</param>
        /// <param name="resourceFilename">리소스 파일 명</param>
        public static void InitializeFromResource(Assembly assembly, string resourceFilename) {
            // 실버라이트에서는 XLinq 만을 지원하여, XmlInterpreter 방식이 지원되지 않는데, 
            // 위의 사이트에서 XLinq 방식으로 변환한 것을 NSoft.NFramework for Silverlight 에 적용시켰습니다.

            if(IsDebugEnabled)
                log.Debug("어셈블리의 리소스파일로부터 IoC 환경설정 정보를 얻어 환경설정을 수행합니다. assembly=[{0}], resourceFilename=[{1}]",
                          assembly, resourceFilename);
            try {
                using(var stream = ResourceTool.GetEmbeddedResourceFile(assembly ?? Assembly.GetExecutingAssembly(), resourceFilename)) {
                    var xmlText = stream.ToText();

                    var container = new WindsorContainer(new XmlInterpreter(new StaticContentResource(xmlText)));
                    Initialize(container);
                }
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("리소스 파일로부터 IoC 설정하는데 실패했습니다. resourceFilename=" + resourceFilename, ex);
                throw;
            }
        }
#endif

        /// <summary>
        /// 초기화가 되어 있지 않다면 빈 Container를 사용하는 IoC를 초기화 합니다. (환경설정을 하지 않고도 사용할 수 있도록 있습니다.)
        /// </summary>
        public static void InitializeWithEmptyContainer() {
            Initialize(new WindsorContainer());
        }

        /// <summary>
        /// 지정된 Container를 Global Container로 설정하여, 전역적으로 사용가능토록 한다.
        /// </summary>
        /// <example>
        /// <code>
        ///		if(IoC.IsNotInitialized)
        ///		{
        ///			// 빈 컨테이너를 만든다.
        ///			IoC.Initialize(new WindsorContainer());
        /// 
        ///			// 환경설정에서 Component 정의를 읽어오려면
        ///			// IoC.Initialize(new WindsorContainer(new XmlInterpreter());
        /// 
        ///			// 특정 환경설정 파일에서 Component 정의를 읽어오려면
        ///			// IoC.Initialize(new WindsorContainer(windsorConfigurationFileName));
        ///		}
        /// </code>
        /// </example>
        /// <param name="container"></param>
        public static void Initialize(IWindsorContainer container) {
            container.ShouldNotBeNull("container");

            if(GlobalContainer == null) {
                lock(_syncLock) {
                    if(GlobalContainer == null) {
                        if (IsDebugEnabled)
                            log.Debug("IoC 전역 컨테이너를 초기화합니다... container=[{0}], thread id=[{1}]",
                                      container, Thread.CurrentThread.ManagedThreadId);

                        GlobalContainer = container;
                        Thread.MemoryBarrier();
                        BindingKernelComponentEvents(GlobalContainer);

                        if (log.IsInfoEnabled)
                            log.Info("IoC 전역 컨테이너 초기화에 성공했습니다!!! thread id=[{0}]",
                                     Thread.CurrentThread.ManagedThreadId);
                    }
                }
            }
        }

        private static void BindingKernelComponentEvents(IWindsorContainer container) {
            container.Kernel.ComponentRegistered += ComponentRegisteredHandler;
            container.Kernel.ComponentCreated += ComponentCreatedHandler;
            container.Kernel.ComponentDestroyed += ComponentDestroyedHandler;
        }

        private static void ComponentRegisteredHandler(string key, Castle.MicroKernel.IHandler handler) {
            if(IsInfoEnabled)
                log.Info("컨테이너가 컴포넌트를 등록합니다... key=[{0}], service=[{1}], component=[{2}], lifestyle=[{3}]",
                         key, handler.ComponentModel.GetServiceNames(), handler.ComponentModel.Name,
                         handler.ComponentModel.LifestyleType);
        }

        private static void ComponentCreatedHandler(ComponentModel model, object instance) {
            if(IsDebugEnabled)
                log.Debug("컨테이너가 컴포넌트를 생성했습니다.  key=[{0}], lifestyle=[{1}], serviceNames=[{2}], instance=[{3}]",
                          model.Name, model.LifestyleType, model.GetServiceNames(), instance);
        }

        private static void ComponentDestroyedHandler(ComponentModel model, object instance) {
            if(IsDebugEnabled)
                log.Debug("컨테이너가 컴포넌트를 파괴했습니다.  key=[{0}], lifestyle=[{1}], serviceNames=[{2}], instance=[{3}]",
                          model.Name, model.LifestyleType, model.GetServiceNames(), instance);
        }

        private static string GetServiceNames(this ComponentModel model) {
            return model.Services.Select(s => s.Name).CollectionToString();
        }

        /// <summary>
        /// Global Container 대신 일시적으로 사용할 Local Container를 사용하기 위한 Utility 함수
        /// </summary>
        /// <param name="localContainer"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// using(IoC.UseLocalContainer(localContainer))
        /// {
        ///		// some code for using localContainer
        /// }
        /// // some code for using global container
        /// </code>
        /// </example>
        public static IDisposable UseLocalContainer(IWindsorContainer localContainer) {
            localContainer.ShouldNotBeNull("localContainer");

            if(IsDebugEnabled)
                log.Debug("로컬 컨테이너를 사용하려고 합니다... localContainer=[{0}]", localContainer.Name);

            lock(_syncLock) {
                LocalContainerStack.Push(localContainer);

                // Local container를 사용하고 dispoing할 때 localContainer를 reset하게 한다.
                //
                return new DisposableAction(() => Reset(localContainer));
            }
        }

        /// <summary>
        /// 지정된 Container 사용을 중지합니다.
        /// </summary>
        /// <param name="containerToReset"></param>
        public static void Reset(IWindsorContainer containerToReset) {
            if(containerToReset == null) {
                GlobalContainer = null;

                if(IsDebugEnabled)
                    log.Debug("전역 컨테이너를 리셋했습니다!!!");
                return;
            }

            if(IsDebugEnabled)
                log.Debug("컨테이너[{0}]를 리셋합니다...", containerToReset.Name);

            lock(_syncLock) {
                if(ReferenceEquals(LocalContainer, containerToReset)) {
                    LocalContainerStack.Pop();

                    if(LocalContainerStack.Count == 0)
                        Local.Data[LocalContainerKey] = null;

                    if(IsDebugEnabled)
                        log.Debug("로컬 컨테이너를 리셋했습니다!!!");

                    return;
                }

                if(ReferenceEquals(GlobalContainer, containerToReset)) {
                    GlobalContainer = null;

                    if(IsDebugEnabled)
                        log.Debug("전역 컨테이너를 리셋했습니다!!!");
                }
            }
        }

        /// <summary>
        /// 현재 활성화된 Container를 Reset 합니다.
        /// </summary>
        public static void Reset() {
            Reset(LocalContainer ?? GlobalContainer);
        }

        /// <summary>
        /// 특정 Assembly에 있는 기본이 되는 Service type에 해당하는 component를 IoC Container에 등록한다.
        /// </summary>
        /// <param name="assembly">Component가 정의된 Assembly</param>
        /// <param name="serviceType">Component의 Service type (Interface type)</param>
        /// <param name="lifestyleType">Component life style (Singleton, Transient ...)</param>
        public static void RegisterComponents(Assembly assembly, Type serviceType, LifestyleType lifestyleType) {
            assembly.ShouldNotBeNull("assembly");
            serviceType.ShouldNotBeNull("serviceType");

            if(IsDebugEnabled)
                log.Debug("Assembly에 있는 ServiceType의 Component들을 Container에 등록합니다... " +
                          "assembly=[{0}], service type=[{1}], lifestyle=[{2}]",
                          assembly.FullName, serviceType.FullName, lifestyleType);

            lock(_syncLock)
                Container.Register(AllTypes
                                       .FromAssembly(assembly)
                                       .BasedOn(serviceType)
                                       .WithServiceBase()
                                       .Configure(component => component.LifeStyle.Is(lifestyleType)));
        }

        /// <summary>
        /// Container에 지정된 인스턴스를 추가한다.
        /// </summary>
        /// <typeparam name="TService">추가할 인스턴스의 서비스 수형</typeparam>
        /// <param name="instance">추가할 인스턴스</param>
        public static void AddComponentInstance<TService>(TService instance) where TService : class {
            AddComponentInstance(instance, LifestyleType.Singleton);
        }

        /// <summary>
        /// Container에 지정된 인스턴스를 추가한다.
        /// </summary>
        /// <typeparam name="TService">추가할 인스턴스의 서비스 수형</typeparam>
        /// <param name="lifestyle">인스턴스 라이프스타일</param>
        /// <param name="instance">추가할 인스턴스</param>
        public static void AddComponentInstance<TService>(TService instance, LifestyleType lifestyle) where TService : class {
            instance.ShouldNotBeNull("instance");

            if(IsInfoEnabled)
                log.Info("IoC 컨테이너에 인스턴스를 등록합니다... serviceType=[{0}], lifestyle=[{1}], instance=[{2}]",
                         typeof(TService).FullName, lifestyle, instance);

            lock(_syncLock) {
                Container.Register(Component
                                       .For<TService>()
                                       .Instance(instance)
                                       .LifeStyle.Is(lifestyle)
                                       .OnlyNewServices());
            }
        }

        /// <summary>
        /// Container에 지정된 인스턴스를 추가한다.
        /// </summary>
        /// <typeparam name="TComponent">추가할 인스턴스의 수형</typeparam>
        /// <param name="serviceType">추가할 인스턴스의 서비스 수형 (interface)</param>
        /// <param name="instance">추가할 인스턴스</param>
        public static void AddComponentInstance<TComponent>(Type serviceType, object instance) where TComponent : class {
            AddComponentInstance<TComponent>(serviceType, instance, LifestyleType.Singleton);
        }

        /// <summary>
        /// Container에 지정된 인스턴스를 추가한다.
        /// </summary>
        /// <typeparam name="TComponent">추가할 인스턴스의 수형</typeparam>
        /// <param name="serviceType">추가할 인스턴스의 서비스 수형 (interface)</param>
        /// <param name="instance">추가할 인스턴스</param>
        /// <param name="lifestyle">lifestyle for component (ie. singleton, thread, trasient) </param>
        public static void AddComponentInstance<TComponent>(Type serviceType, object instance, LifestyleType lifestyle)
            where TComponent : class {
            serviceType.ShouldNotBeNull("serviceType");
            instance.ShouldNotBeNull("instance");

            var componentType = typeof(TComponent);

            Guard.Assert(() => componentType.IsSameOrSubclassOf(serviceType),
                         "컴포넌트[{0}]가 서비스[{1}]의 Same or Subclass 가 아닙니다.", componentType.FullName, serviceType.FullName);

            Guard.Assert(() => instance.GetType().IsSameOrSubclassOf(componentType),
                         "인스턴스[{0}]가 컴포넌트[{1}]의 수형이 아닙니다.", instance.GetType().FullName, componentType.FullName);

            lock(_syncLock) {
                Container.Register(Component.For(serviceType)
                                       .Instance(instance)
                                       .LifeStyle.Is(lifestyle)
                                       .OnlyNewServices());
            }
        }

        /// <summary>
        /// 지정된 서비스가 컨테이너에 등록되어 있는지 확인한다.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public static bool HasComponent<TService>() {
            return Container.Kernel.HasComponent(typeof(TService));
        }

        /// <summary>
        /// 지정된 서비스가 컨테이너에 등록되어 있는지 확인한다.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static bool HasComponent(Type service) {
            return Container.Kernel.HasComponent(service);
        }

        /// <summary>
        /// 지정된 ComponentId의 컴포넌트가 컨테이너에 등록되어 있는지 확인한다.
        /// </summary>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public static bool HasComponent(string componentId) {
            if(componentId.IsWhiteSpace())
                return false;

            return Container.Kernel.HasComponent(componentId);
        }

        /// <summary>
        /// 지정된 Service 형태의 Component를 인스턴싱한다.
        /// </summary>
        /// <param name="service">인스턴싱할 Component의 형식</param>
        /// <returns></returns>
        public static object Resolve(Type service) {
            service.ShouldNotBeNull("service");

            if(IsDebugEnabled)
                log.Debug("서비스 형식으로 컴포넌트를 Resolve 합니다... service=[{0}]", service.FullName);

            var component = With.TryFunction(() => Container.Resolve(service) ?? Container.Resolve(service.FullName, service), null,
                                             null, null);

            if(component != null)
                if(IsDebugEnabled)
                    log.Debug("서비스 형식으로 컴포넌트를 Resolve에 성공했습니다!!! service=[{0}], component=[{1}]", service.FullName,
                              component.GetType().FullName);

            return component;
        }

        /// <summary>
        /// 지정된 형식의 Component를 인스턴싱한다.
        /// </summary>
        /// <typeparam name="TService">Type of Component</typeparam>
        /// <returns></returns>
        public static TService Resolve<TService>() {
            if(IsDebugEnabled)
                log.Debug("Container에서 컴포넌트를 로드합니다... ServiceType=[{0}]", typeof(TService).FullName);

            return Container.Resolve<TService>();
        }

        /// <summary>
        /// 지정된 Component Id의 Component를 Resolve한다.
        /// </summary>
        /// <typeparam name="TService">Instancing을 원하는 Component의 Service Type</typeparam>
        /// <param name="componentId">Component Id</param>
        /// <returns></returns>
        public static TService Resolve<TService>(string componentId) {
            componentId.ShouldNotBeWhiteSpace("componentId");

            if(IsDebugEnabled)
                log.Debug("Container에서 컴포넌트를 로드합니다... componentId=[{0}], service=[{1}]", componentId, typeof(TService).FullName);

            return Container.Resolve<TService>(componentId);
        }

        /// <summary>
        /// 지정된 Component Id의 Component를 Resolve한다.
        /// </summary>
        /// <typeparam name="TService">Instancing을 원하는 Component의 Service Type</typeparam>
        /// <param name="componentId">Component Id</param>
        /// <param name="arguments">인스턴싱할 객체의 속성정보 (속성명=속성값)</param>
        /// <returns></returns>
        public static TService Resolve<TService>(string componentId, IDictionary arguments) {
            componentId.ShouldNotBeWhiteSpace("componentId");
            arguments.ShouldNotBeNull("arguments");

            if(IsDebugEnabled)
                log.Debug("Resolve a component by service... component id=[{0}], service=[{1}], arguments=[{2}]",
                          componentId, typeof(TService).FullName, arguments.CollectionToString());

            return Container.Resolve<TService>(componentId, arguments);
        }

        /// <summary>
        /// 지정된 Component Id의 Component를 Resolve한다.
        /// </summary>
        /// <typeparam name="TService">Instancing을 원하는 Component의 Service Type</typeparam>
        /// <param name="arguments">인스턴싱할 객체의 속성정보 (속성명=속성값)</param>
        /// <returns></returns>
        public static TService Resolve<TService>(IDictionary arguments) {
            arguments.ShouldNotBeNull("arguments");

            if(IsDebugEnabled)
                log.Debug("Resolve a new component by service... service=[{0}], arguments=[{1}]",
                          typeof(TService).FullName, arguments.CollectionToString());

            return Container.Resolve<TService>(arguments);
        }

        /// <summary>
        /// 지정된 Component Id의 Component를 Resolve한다.
        /// </summary>
        /// <typeparam name="TService">Instancing을 원하는 Component의 Service Type</typeparam>
        /// <param name="argumentsAsAnonymousType">인스턴싱할 객체의 속성정보</param>
        /// <returns></returns>
        public static TService Resolve<TService>(object argumentsAsAnonymousType) {
            if(IsDebugEnabled)
                log.Debug("Resolve a new component by service... service=[{0}], argumentsAsAnonymouysType=[{1}]",
                          typeof(TService).FullName, argumentsAsAnonymousType);

            return Container.Resolve<TService>(argumentsAsAnonymousType);
        }

        /// <summary>
        /// 해당 컴포넌트 Id의 컴포넌트가 있다면, 그 놈을 Resove 하고, 없다면, ServiceType이 {TService} 인 첫번째 컴포넌트를 반환한다.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="componentId"></param>
        /// <returns></returns>
        public static TService ResolveOrDefault<TService>(string componentId) {
            return HasComponent(componentId)
                       ? Resolve<TService>(componentId)
                       : Resolve<TService>();
        }

        /// <summary>
        /// Castle Component를 인스턴싱 작업을 시도한다. 
        /// 실패시에는 예외를 발생시키는 것이 아니라 지정된 타입의 default(T)를 반환한다.
        /// </summary>
        /// <typeparam name="TComponent">생성할 객체</typeparam>
        /// <returns>정의된 컴포넌트가 없다면 지정된 형식의 기본 인스턴스를 반환한다.</returns>
        public static TComponent TryResolve<TComponent>() where TComponent : class {
            return TryResolve<TComponent, TComponent>(LifestyleType.Singleton);
        }

        /// <summary>
        /// Castle Component를 인스턴싱 작업을 시도한다. 실패시에는 예외를 발생시키는 것이 아니라 
        /// 지정된 타입의 default를 반환하고, 성공시에는 인스턴싱을 반환한다.
        /// 옵션에 따른 Dependency Injection시에 유용하다.
        /// </summary>
        /// <typeparam name="TComponent">인스턴싱할 형식</typeparam>
        /// <param name="instance">default value (default instance.)</param>
        /// <returns></returns>
        public static TComponent TryResolve<TComponent>(TComponent instance) where TComponent : class {
            if(HasComponent<TComponent>() == false) {
                if(IsDebugEnabled)
                    log.Debug("Container에 컴포넌트[{0}]가 등록되어 있지 않습니다. 지정한 인스턴스 객체 [{1}]를 반환합니다.",
                              typeof(TComponent).FullName, instance);

                return instance;
            }
            return With.TryFunction(() => Resolve<TComponent>(), () => instance);
        }

        /// <summary>
        /// 지정된 형식의 Component를 가져옵니다. Container에 등록되어 있지 않다면, instancingFunction으로 Component를 인스턴싱 합니다.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instancingFunction"></param>
        /// <param name="registerComponent">Container에 서비스를 등록할 것인가?</param>
        /// <returns></returns>
        public static TService TryResolve<TService>(Func<TService> instancingFunction, bool registerComponent) where TService : class {
            return TryResolve(instancingFunction, registerComponent, LifestyleType.Singleton);
        }

        /// <summary>
        /// 지정된 형식의 Component를 가져옵니다. Container에 등록되어 있지 않다면, instancingFunction으로 Component를 인스턴싱 합니다.
        /// LifeStyleType은 항상 Singleton으로 해주셔야 합니다.
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="instancingFunction"></param>
        /// <param name="registerComponent">Container에 서비스를 등록할 것인가?</param>
        /// <param name="lifestyleType">무시된다. 무조건 Singleton이다.</param>
        /// <returns></returns>
        public static TService TryResolve<TService>(Func<TService> instancingFunction,
                                                    bool registerComponent,
                                                    LifestyleType lifestyleType) where TService : class {
            instancingFunction.ShouldNotBeNull("instancingFunction");

            lock(_syncLock) {
                Container.Register(
                    Component
                        .For<TService>()
                        .UsingFactoryMethod<TService>(instancingFunction)
                        .LifeStyle.Is(lifestyleType)
                        .OnlyNewServices());
            }

            return With.TryFunction(() => Resolve<TService>());
        }

        /// <summary>
        /// 지정된 형식의 Component를 인스턴스화 합니다. Container에 해당 Component가 정의되어 있지 않다면, Container에 추가하고, 인스턴스를 반환한다.
        /// </summary>
        /// <typeparam name="TService">Service type (interface)</typeparam>
        /// <typeparam name="TComponent">Component type (instance)</typeparam>
        /// <returns></returns>
        public static TService TryResolve<TService, TComponent>()
            where TService : class
            where TComponent : class, TService {
            return TryResolve<TService, TComponent>(LifestyleType.Singleton);
        }

        /// <summary>
        /// 지정된 형식의 Component를 인스턴스화 합니다. Container에 해당 Component가 정의되어 있지 않다면, Container에 추가하고, 인스턴스를 반환한다.
        /// </summary>
        /// <typeparam name="TService">Service type (interface)</typeparam>
        /// <typeparam name="TComponent">Component type (instance)</typeparam>
        /// <param name="lifestyleType">instance lifestyle (singleton, thread, transient)</param>
        /// <returns></returns>
        public static TService TryResolve<TService, TComponent>(LifestyleType lifestyleType)
            where TService : class
            where TComponent : class, TService {
            lock(_syncLock) {
                Container.Register(Component
                                       .For<TService>()
                                       .ImplementedBy<TComponent>()
                                       .LifeStyle.Is(lifestyleType)
                                       .OnlyNewServices());
            }

            return With.TryFunction(() => Container.Resolve<TService>());
        }

        /// <summary>
        /// 지정된 형식의 Component를 인스턴스화 합니다. Container에 해당 Component가 정의되어 있지 않다면, Container에 추가하고, 인스턴스를 반환한다.
        /// </summary>
        /// <typeparam name="TService">Service type (interface)</typeparam>
        /// <typeparam name="TComponent">Component type (instance)</typeparam>
        /// <param name="lifestyleType">instance lifestyle (singleton, thread, transient)</param>
        /// <param name="arguments">컴포넌트 생성자의 인자로 쓰일 값들</param>
        /// <returns></returns>
        public static TService TryResolve<TService, TComponent>(LifestyleType lifestyleType, IDictionary arguments)
            where TService : class
            where TComponent : class, TService {
            lock(_syncLock) {
                Container.Register(
                    Component
                        .For<TService>()
                        .ImplementedBy<TComponent>()
                        .LifeStyle.Is(lifestyleType)
                        .OnlyNewServices());
                //.Unless(Component.ServiceAlreadyRegistered));
            }
            return With.TryFunction(() => Container.Resolve<TService>(arguments));
        }

        /// <summary>
        /// 지정된 타입의 Component를 모두 인스턴싱합니다.
        /// </summary>
        /// <param name="serviceType">인스턴싱할 Type</param>
        /// <returns></returns>
        public static Array ResolveAll(Type serviceType) {
            serviceType.ShouldNotBeNull("serviceType");

            if(IsDebugEnabled)
                log.Debug("Resolve all component by service type=[{0}]", serviceType.FullName);

            return Container.ResolveAll(serviceType);
        }

        /// <summary>
        /// 지정된 타입의 Component를 모두 인스턴싱합니다.
        /// </summary>
        /// <typeparam name="T">인스턴싱할 Service Type</typeparam>
        /// <returns></returns>
        public static T[] ResolveAll<T>() {
            if(IsDebugEnabled)
                log.Debug("Resolve all component by service=[{0}]", typeof(T).FullName);

            return Container.ResolveAll<T>();
        }
    }
}