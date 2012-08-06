using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NSoft.NFramework.Cryptography;

namespace NSoft.NFramework.InversionOfControl.WindsorInstallers {
    /// <summary>
    /// 암호화 기능을 수행하는 <see cref="IHashEncryptor"/>, <see cref="ISymmetricEncryptor"/> 컴포넌트를 Windsor Container에 등록합니다.
    /// </summary>
    public sealed class EncryptorInstaller : IWindsorInstaller {
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
                    .BasedOn<IHashEncryptor>()
                    .WithServiceFromInterface(typeof(IHashEncryptor))
                    .LifestyleSingleton(),
                AllTypes.FromThisAssembly()
                    .BasedOn<ISymmetricEncryptor>()
                    .WithServiceFromInterface(typeof(ISymmetricEncryptor))
                    .LifestyleSingleton()
                );
        }
    }
}