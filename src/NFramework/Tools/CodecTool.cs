using System;
using System.Security.Cryptography;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Encoding/Decoding 함수를 제공하는 Utility Class
    /// </summary>
    public static class CodecTool {
        /// <summary>
        /// 지정된 데이타를 Base64 인코딩 방식으로 인코딩을 수행한다.
        /// </summary>
        /// <param name="data">인코딩할 데이타</param>
        /// <returns>인코딩된 문자열</returns>
        public static string EncodeBase64(this byte[] data) {
            if(data == null)
                return string.Empty;

            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Base64 형식으로 인코딩된 문자열을 디코딩하여 1차원 바이트 배열로 반환한다.
        /// </summary>
        /// <param name="data">Base64로 인코딩된 문자열</param>
        /// <returns>디코딩된 문자열</returns>
        public static byte[] DecodeBase64(this string data) {
            if(data.IsEmpty())
                return new byte[0];

            return Convert.FromBase64String(data);
        }

        /// <summary>
        /// 지정된 데이타를 Md5 형식으로 인코딩한다.
        /// </summary>
        /// <param name="data">인코딩할 데이타</param>
        /// <returns>MD5 형식으로 인코딩된 데이타</returns>
        /// <exception cref="ArgumentNullException">data 가 null일 경우</exception>
        public static byte[] Md5(this byte[] data) {
            if(data == null)
                return new byte[0];

            using(MD5 md5Provider = new MD5CryptoServiceProvider())
                return md5Provider.ComputeHash(data);
        }

        /// <summary>
        /// 지정된 data를 Md5 형식으로 암호화한 후 16진수(Hex) 포맷으로 문자열을 만든다.
        /// </summary>
        /// <param name="data">암호화할 바이트 배열</param>
        /// <returns>MD5로 암호화된 바이트배열을 알기 쉽게 16진수 (Hex) 포맷으로 문자열을 반환한다.</returns>
        public static string Md5Hex(this byte[] data) {
            data.ShouldNotBeNull("data");
            return Md5(data).GetHexStringFromBytes();
        }

        /// <summary>
        /// 지정된 data를 SHA1 형식으로 암호화한다.
        /// </summary>
        /// <param name="data">암호화할 데이타</param>
        /// <returns>SHA1형식으로 암호화된 바이트 배열</returns>
        public static byte[] Sha1(this byte[] data) {
            if(data == null)
                return new byte[0];

            using(var sha1 = new SHA1Managed())
                return sha1.ComputeHash(data);
        }

        /// <summary>
        /// 지정된 데이타를 SHA1 형식으로 암호화한 다음 알기 쉽게 16진수 포맷인 Hex string으로 반환한다.
        /// </summary>
        /// <param name="data">암호화할 데이타</param>
        /// <returns>암호화한 데이타를 16진수 포맷인 문자열로 반환한다.</returns>
        public static string Sha1Hex(this byte[] data) {
            if(data == null)
                return string.Empty;

            return Sha1(data).GetHexStringFromBytes();
        }
    }
}