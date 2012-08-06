using System.IO;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Cryptography {
    public static partial class CryptoTool {
        internal const string Password = @"sunghyouk.bae@gmail.com";

        /// <summary>
        /// Hash 값을 계산합니다.
        /// </summary>
        /// <param name="hasher">Hash 알고리즘을 이용하는 암호기</param>
        /// <param name="plainText">원본 문자열</param>
        /// <returns>Hashing 된 바이트 배열</returns>
        public static byte[] ComputeHashToBytes(this IHashEncryptor hasher, string plainText) {
            return hasher.ComputeHash(plainText);
        }

        /// <summary>
        /// Hash 값을 계산해서, 문자열로 반환합니다.
        /// </summary>
        /// <param name="hasher">Hash 알고리즘을 이용하는 암호기</param>
        /// <param name="plainText">원본 문자열</param>
        /// <param name="format">암호화된 정보의 문자열 형식(Base64|HexDecimal)</param>
        /// <returns>Hashing 된 문자열</returns>
        public static string ComputeHashToString(this IHashEncryptor hasher, string plainText,
                                                 EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            return hasher.ComputeHash(plainText, format);
        }

        /// <summary>
        /// 대칭형 암호화 알고리즘을 이용하여 <paramref name="plainBytes"/>를 암호화 합니다.
        /// </summary>
        /// <param name="encryptor">대칭형 암호화 인스턴스</param>
        /// <param name="plainBytes">원본 정보</param>
        /// <returns>암호화된 정보</returns>
        public static byte[] EncryptBytes(this ISymmetricEncryptor encryptor, byte[] plainBytes) {
            return encryptor.Encrypt(plainBytes);
        }

        /// <summary>
        /// 대칭형 암호화 알고르즘을 이용하여 <paramref name="cipherBytes"/>를 복호화 합니다.
        /// </summary>
        /// <param name="encryptor">대칭형 암호화 인스턴스</param>
        /// <param name="cipherBytes">암호화된 정보</param>
        /// <returns>복호화된 정보</returns>
        public static byte[] DecryptBytes(this ISymmetricEncryptor encryptor, byte[] cipherBytes) {
            return encryptor.Decrypt(cipherBytes);
        }

        /// <summary>
        /// 대칭형 암호화 알고리즘을 이용하여 <paramref name="plainText"/>를 암호화하여, <paramref name="format"/> 형태의 문자열로 반환합니다.
        /// </summary>
        /// <param name="encryptor">대칭형 암호화 인스턴스</param>
        /// <param name="plainText">원본 문자열</param>
        /// <param name="format">암호화된 정보의 문자열 형식(Base64|HexDecimal)</param>
        /// <returns>암호화된 문자열</returns>
        public static string EncryptString(this ISymmetricEncryptor encryptor, string plainText,
                                           EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            if(plainText.IsEmpty())
                return plainText;

            return
                encryptor
                    .EncryptBytes(Encoding.Unicode.GetBytes(plainText))
                    .BytesToString(format)
                    .Trim('\0');
        }

        /// <summary>
        /// 대칭형 암호화 알고리즘을 이용하여, <paramref name="format"/>형태의 <paramref name="cipherText"/>를 복호화합니다.
        /// </summary>
        /// <param name="encryptor">대칭형 암호화 인스턴스</param>
        /// <param name="cipherText">암호화된 정보</param>
        /// <param name="format">암호화된 정보의 문자열 형식(Base64|HexDecimal)</param>
        /// <returns>복화화된 원본 문자열</returns>
        public static string DecryptString(this ISymmetricEncryptor encryptor, string cipherText,
                                           EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            if(cipherText.IsEmpty())
                return cipherText;

            var plainBytes = encryptor.Decrypt(cipherText.StringToBytes(format));
            return Encoding.Unicode.GetString(plainBytes).Trim('\0');
        }

        /// <summary>
        /// 대칭형 암호화 알고리즘을 이용하여, <paramref name="plainStream"/>을 암호화하여 스트림으로 반환합니다.
        /// </summary>
        /// <param name="encryptor">대칭형 암호화 인스턴스</param>
        /// <param name="plainStream">원본 스트림</param>
        /// <returns>암호화된 스트림</returns>
        public static Stream EncryptStream(this ISymmetricEncryptor encryptor, Stream plainStream) {
            plainStream.ShouldNotBeNull("plainStream");
            return new MemoryStream(encryptor.Encrypt(plainStream.ToBytes()));
        }

        /// <summary>
        /// 대칭형 암호화 알고리즘을 이용하여, <paramref name="cipherStream"/>을 복호화한 스트림을 반환합니다.
        /// </summary>
        /// <param name="encryptor">대칭형 암호화 인스턴스</param>
        /// <param name="cipherStream">암호화된 정보를 가진 스트림</param>
        /// <returns>복호화된 정보를 가진 스트림</returns>
        public static Stream DecryptString(this ISymmetricEncryptor encryptor, Stream cipherStream) {
            cipherStream.ShouldNotBeNull("cipherStream");
            return new MemoryStream(encryptor.Decrypt(cipherStream.ToBytes()));
        }
    }
}