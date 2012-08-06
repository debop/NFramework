using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Compressions;

namespace NSoft.NFramework.InversionOfControl.WindsorInstallers {
    /// <summary>
    /// Castle.Windsor의 컨테이너에 <see cref="ICompressor"/>로 구현된 클래스들을 등록 합니다.
    /// </summary>
    public sealed class CompressorInstaller : IWindsorInstaller {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">The container.</param><param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            if(IsDebugEnabled)
                log.Debug("ICompressor 를 구현한 컴포넌트를 Windsor Container에 등록합니다...");

            container.Register(
                AllTypes.FromThisAssembly()
                    .BasedOn<ICompressor>()
                    .WithServiceFromInterface(typeof(ICompressor))
                    .LifestyleSingleton()
                );
        }
    }
}