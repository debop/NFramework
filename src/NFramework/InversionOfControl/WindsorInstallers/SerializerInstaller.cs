using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace NSoft.NFramework.InversionOfControl.WindsorInstallers {
    /// <summary>
    /// <see cref="ISerializer"/>, <see cref="ISerializer{T}"/>를 Castle.Windsor에 등록하는 컴포넌트 인스톨러
    /// </summary>
    public sealed class SerializerInstaller : IWindsorInstaller {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">The container.</param><param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            if(IsDebugEnabled) {
                log.Debug("IHashEncryptor 를 구현한 컴포넌트를 Windsor Container에 등록합니다...");
                log.Debug("ISymmetricEncryptor 를 구현한 컴포넌트를 Windsor Container에 등록합니다...");
            }

            container.Register(
                AllTypes.FromThisAssembly()
                    .BasedOn<ISerializer>()
                    .WithServiceFromInterface(typeof(ISerializer))
                    .LifestyleSingleton(),
                AllTypes.FromThisAssembly()
                    .BasedOn(typeof(ISerializer<>))
                    .WithServiceAllInterfaces()
                    .LifestyleSingleton()
                );
        }
    }
}