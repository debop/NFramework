using System.Security.Cryptography;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Cryptography.Encryptors;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// TripleDES 대칭형 알고리즘을 사용하여 문자열을 암호화하여 저장하는 UserType 입니다.
    /// </summary>
    /// <see cref="TripleDESCryptoServiceProvider"/>
    public sealed class TripleDESEncryptStringUserType : AbstractSymmetricEncryptStringUserType {
        private static readonly ISymmetricEncryptor _encryptor = new TripleDESSymmetricEncryptor();

        /// <summary>
        /// 대칭형 암호기
        /// </summary>
        public override ISymmetricEncryptor Encryptor {
            get { return _encryptor; }
        }
    }
}