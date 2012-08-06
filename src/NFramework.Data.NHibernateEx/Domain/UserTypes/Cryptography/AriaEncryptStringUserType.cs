using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// ARIA 대칭형 암호화 알고리즘을 이용하여 값을 암호화하는 수형입니다.
    /// </summary>
    /// <seealso cref="AriaSymmetricEncryptor"/>
    public sealed class AriaEncryptStringUserType : AbstractSymmetricEncryptStringUserType {
        private static readonly ISymmetricEncryptor _encryptor = new AriaSymmetricEncryptor();

        public override ISymmetricEncryptor Encryptor {
            get { return _encryptor; }
        }
    }
}