using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Cryptography {
    /// <summary>
    /// 암호화 관련 Utility Class 입니다.
    /// </summary>
    public static partial class CryptoTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Clear the specified buffer value to 0.
        /// </summary>
        /// <param name="bytes"></param>
        public static void ClearBuffer(this byte[] bytes) {
            if(bytes == null)
                return;

            Array.Clear(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Byte 배열을 16진수 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes"></param>
        public static string BytesToHex(this byte[] bytes) {
            if(bytes == null || bytes.Length == 0)
                return string.Empty;

            var hex = new StringBuilder(bytes.Length * 2);

            for(var i = 0; i < bytes.Length; i++)
                hex.Append(bytes[i].ToString("X2"));

            return hex.ToString();
        }

        /// <summary>
        /// 16진수 형태의 문자열을 파싱하여 byte 배열을 만든다.
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(this string hexString) {
            if(IsDebugEnabled)
                log.Debug("Convert Hex string to bytes. HexString=[{0}]", hexString.EllipsisChar(64));

            if(hexString.IsWhiteSpace())
                return new byte[0];

            int nBytes = hexString.Length / 2;
            var result = new byte[nBytes];

            for(int i = 0; i < nBytes; i++) {
                string hexByte = hexString.Substring(i * 2, 2);
                result[i] = byte.Parse(hexByte, NumberStyles.HexNumber);
            }

            return result;
        }

        /// <summary>
        /// 암호화된 Byte 배열을 지정된 형식의 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        /// <seealso cref="CryptoTool.StringToBytes"/>
        public static string BytesToString(this byte[] bytes, EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            if(bytes == null || bytes.Length == 0)
                return string.Empty;

            return (format == EncryptionStringFormat.HexDecimal)
                       ? BytesToHex(bytes)
                       : Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 지정된 형식의 문자열을 byte 배열로 변환한다.
        /// </summary>
        /// <param name="content">변환할 문자열</param>
        /// <param name="format">변활할 포맷</param>
        /// <returns></returns>
        /// <seealso cref="CryptoTool.BytesToString"/>
        public static byte[] StringToBytes(this string content, EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            if(content.IsEmpty())
                return new byte[0];

            return (format == EncryptionStringFormat.HexDecimal)
                       ? HexToBytes(content)
                       : Convert.FromBase64String(content);
        }

        /// <summary>
        /// <see cref="Stream"/> 객체 정보를 읽어서, Byte 배열로 만든다.
        /// </summary>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public static byte[] StreamToBytes(this Stream inStream) {
            return StringTool.ToBytes(inStream);
        }

        /// <summary>
        /// 랜덤한 숫자를 만들어낸다. 비밀번호 키를 만들 때 좋다.
        /// </summary>
        /// <param name="saltLength"></param>
        /// <returns></returns>
        public static byte[] GenerateSalt(int saltLength) {
            var salt = new byte[(saltLength > 0) ? saltLength : 0];
            new RNGCryptoServiceProvider().GetBytes(salt);
            return salt;
        }

        /// <summary>
        /// 보통 문자열을 SALT 를 이용해 암호화된 새로운 비밀번호를 만든다.
        /// </summary>
        /// <param name="originalPassword">원본 문자열</param>
        /// <param name="newPasswordLength">암호화된 새로운 비밀번호의 길이</param>
        /// <returns>암호화된 비밀번호를 반환</returns>
        public static byte[] DerivePassword(string originalPassword, int newPasswordLength) {
            var derivedBytes = new Rfc2898DeriveBytes(originalPassword, InitVector.SALT_BYTES, 5);
            return derivedBytes.GetBytes(newPasswordLength);
        }

        /// <summary>
        /// 원본 문자열을 SALT 를 이용해 암호화 하여, 원하는 형식의 문자열로 반환한다.
        /// </summary>
        /// <param name="originalPassword">원본 문자열</param>
        /// <param name="newPasswordLength">새로운 문자열의 byte stream 크기</param>
        /// <param name="format">Base64 or HexaDecimal ContentFormat</param>
        /// <returns></returns>
        public static string DerivePasswordAsText(string originalPassword, int newPasswordLength,
                                                  EncryptionStringFormat format = EncryptionStringFormat.HexDecimal) {
            var password = DerivePassword(originalPassword, newPasswordLength);
            return BytesToString(password, format);
        }

        /// <summary>
        /// SymmetricAlgorithm 에서 KeySize에 해당하는 Initial Vector를 구한다.
        /// </summary>
        /// <param name="keyLength"></param>
        /// <returns></returns>
        public static byte[] GetInitialVector(int keyLength) {
            switch(keyLength / 8) {
                case 8:
                    return InitVector.IV_8;

                case 16:
                    return InitVector.IV_16;

                case 24:
                    return InitVector.IV_24;

                case 32:
                    return InitVector.IV_32;

                case 64:
                case 128:
                default:
                    throw new CryptographicException(SR.ErrNoKeyForSymmetric);
            }
        }
    }
}