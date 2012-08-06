using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Web.HttpApplications {
    /// <summary>
    /// Future Pattern을 이용하여, 웹 Application 시작 시에 미리 준비해야 할 작업이 있다면, 그 작업을 비동기적으로 실행시켜줍니다.
    /// 이렇게 하면, 실제 데이터를 필요로 할 때, 미리 준비된 데이터를 사용할 수 있으므로, 성능상에 유리하게 됩니다.
    /// </summary>
    /// <remarks>
    /// Application_Start, Session_Start, Session_End, Application_End 시점에 비동기적으로 실행할 메소드를 재정의하면, 각 시점마다, 비동기적으로 실행해줍니다.
    /// </remarks>
    public class WindsorAsyncHttpApplication : AsyncHttpApplicationBase, IContainerAccessor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        /// <summary>
        /// 생성자
        /// </summary>
        public WindsorAsyncHttpApplication() {
            InitializeIoC();
        }

        public override void Application_Start(object sender, System.EventArgs e) {
            base.Application_Start(sender, e);

            // Application
            InitializeIoC();
        }

        /// <summary>
        /// IoC/DI 패턴을 위한 Castle.Windsor의 Container입니다.
        /// </summary>
        public virtual IWindsorContainer Container {
            get { return IoC.Container; }
        }

        /// <summary>
        /// <see cref="IoC"/>를 초기화 합니다.
        /// </summary>
        protected void InitializeIoC() {
            if(IoC.IsNotInitialized) {
                try {
                    IoC.Initialize(SetUpContainer());
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled)
                        log.Error("IoC를 초기화하는데 실패했습니다", ex);
                }
            }
        }

        /// <summary>
        /// Windsor Container를 초기화하여 반환합니다. <see cref="IWindsorInstaller"/> 를 이용한 초기화를 수행할 수 있습니다.
        /// </summary>
        /// <returns></returns>
        protected virtual IWindsorContainer SetUpContainer() {
            if(IsDebugEnabled)
                log.Debug("새로운 WindsorContainer를 생성하고 초기화합니다...");

            var container = new WindsorContainer();
            container.Install(Configuration.FromAppConfig());
            return container;
        }
    }
}