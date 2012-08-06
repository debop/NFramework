using System.Threading;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// Castle.Windsor 환경설정에서 지정한 ISymmetricEncrytor Component 를 암호화 컴포넌트로 사용합니다.
    /// </summary>
    public class IoCEncryptStringUserType : AbstractSymmetricEncryptStringUserType {
        private static ISymmetricEncryptor _encryptor;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// 대칭형 암호기
        /// </summary>
        public override ISymmetricEncryptor Encryptor {
            get {
                if(_encryptor == null)
                    lock(_syncLock)
                        if(_encryptor == null) {
                            var encryptor = IoC.TryResolve<ISymmetricEncryptor, AriaSymmetricEncryptor>();
                            Thread.MemoryBarrier();
                            _encryptor = encryptor;
                        }
                return _encryptor;
            }
        }
    }
}