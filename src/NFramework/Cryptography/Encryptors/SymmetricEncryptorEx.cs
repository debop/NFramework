using System.IO;
using System.Threading.Tasks;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Cryptography.Encryptors {
    /// <summary>
    /// 대칭형 암호화 인스턴스(<see cref="ISymmetricEncryptor"/>)에 대한 확장 메소드를 제공합니다. 
    /// </summary>
    public static class SymmetricEncryptorEx {
        /// <summary>
        /// 비동기 방식으로 데이터를 암호화 합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="plainBytes"></param>
        /// <returns></returns>
        public static Task<byte[]> EncryptAsync(this ISymmetricEncryptor encryptor, byte[] plainBytes) {
            return Task.Factory.StartNew(() => encryptor.Encrypt(plainBytes));
        }

        /// <summary>
        /// 비동기 방식으로 암호화된 데이터를 복원합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="cipher"></param>
        /// <returns></returns>
        public static Task<byte[]> DecriptAsync(this ISymmetricEncryptor encryptor, byte[] cipher) {
            return Task.Factory.StartNew(() => encryptor.Decrypt(cipher));
        }

        /// <summary>
        /// 문자열을 암호화합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptString(this ISymmetricEncryptor encryptor, string text) {
            if(text.IsEmpty())
                return string.Empty;

            return encryptor.Encrypt(text.ToBytes(StringTool.DefaultEncoding)).Base64Encode();
        }

        /// <summary>
        /// 암호화된 문자열을 복원합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public static string DecryptString(this ISymmetricEncryptor encryptor, string encryptedText) {
            if(encryptedText.IsEmpty())
                return string.Empty;

            return encryptor.Decrypt(encryptedText.Base64Decode()).ToText(StringTool.DefaultEncoding);
        }

        /// <summary>
        /// 비동기 방식으로 문자열을 암호화합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static Task<string> EncryptStringAsync(this ISymmetricEncryptor encryptor, string text) {
            return Task.Factory.StartNew(() => EncryptString(encryptor, text));
        }

        /// <summary>
        /// 비동기 방식으로 암호화된 문자열을 복원합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public static Task<string> DecryptStringAsync(this ISymmetricEncryptor encryptor, string encryptedText) {
            return Task.Factory.StartNew(() => DecryptString(encryptor, encryptedText));
        }

        /// <summary>
        /// 스트림을 암호화 합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Stream EncryptStream(this ISymmetricEncryptor encryptor, Stream stream) {
            return new MemoryStream(encryptor.Encrypt(stream.ToBytes()));
        }

        /// <summary>
        /// 암호화된 스트림을 복원합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Stream DecryptStream(this ISymmetricEncryptor encryptor, Stream stream) {
            return new MemoryStream(encryptor.Decrypt(stream.ToBytes()));
        }

        /// <summary>
        /// 비동기 방식으로 스트림을 암호화합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Task<Stream> EncryptStreamAsync(this ISymmetricEncryptor encryptor, Stream stream) {
            return Task.Factory.StartNew(() => encryptor.EncryptStream(stream));
        }

        /// <summary>
        /// 비동기 방식으로 암호화된 스트림을 복원합니다.
        /// </summary>
        /// <param name="encryptor"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Task<Stream> DecryptStreamAsync(this ISymmetricEncryptor encryptor, Stream stream) {
            return Task.Factory.StartNew(() => encryptor.DecryptStream(stream));
        }
    }
}