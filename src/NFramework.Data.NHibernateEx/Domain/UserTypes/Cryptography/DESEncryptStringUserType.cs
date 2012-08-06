using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// DES 대칭형 알고리즘을 이용하여, 문자열을 암호화합니다.
    /// </summary>
    public sealed class DESEncryptStringUserType : AbstractSymmetricEncryptStringUserType {
        private static readonly ISymmetricEncryptor _encryptor = new DESSymmetricEncryptor();

        /// <summary>
        /// 대칭형 암호기
        /// </summary>
        public override ISymmetricEncryptor Encryptor {
            get { return _encryptor; }
        }
    }
}