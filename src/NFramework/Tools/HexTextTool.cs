using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// 16진수 문자열에 대한 변환을 수행하는 Helper Class 입니다.
    /// </summary>
    public static class HexTextTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        ///// <summary>
        ///// 16진수를 표현한 문자열을 byte 배열로 변환한다.
        ///// </summary>
        ///// <example>
        ///// <code>
        /////     byte[] byte = HexTextTool.GetBytesFromHexString("D3B5DA375E350B9A1EAF");
        ///// </code>
        ///// </example>
        ///// <param name="hexString">16진수 포맷의 문자열</param>
        ///// <returns>파싱된 바이트 배열</returns>
        //public static byte[] GetBytesFromHexString(this string hexString)
        //{
        //	if(IsDebugEnabled)
        //		log.Debug("Hex Decimal 문자열을 바이트배열로 변환합니다... hexString=[{0}]", hexString.EllipsisChar(80));

        //	if(hexString.IsWhiteSpace())
        //		return new byte[0];

        //	var result = new StringBuilder(hexString.ToUpper(CultureInfo.CurrentCulture));

        //	if(result.Length >= 2)
        //		if(result[0].Equals('0') && result[1].Equals('X'))
        //			result.Remove(0, 2);

        //	Guard.Assert(() => result.Length % 2 == 0, "16진수 문자열이 아닙니다. hexString=[{0}]", hexString.EllipsisChar(80));

        //	var hexBytes = new byte[result.Length / 2];

        //	try
        //	{
        //		for(var i = 0; i < hexBytes.Length; i++)
        //		{
        //			hexBytes[i] = Convert.ToByte(result.ToString(i * 2, 2), 16);
        //		}
        //	}
        //	catch(FormatException e)
        //	{
        //		throw new ArgumentException(string.Format(SR.ErrorInvalidHexDecimalString, "hexString"), e);
        //	}

        //	return hexBytes;
        //}

        /// <summary>
        /// 16진수를 표현한 문자열을 byte 배열로 변환한다.
        /// </summary>
        /// <example>
        /// <code>
        ///     byte[] byte = RwHexText.GetBytesFromHexString("D3B5DA375E350B9A1EAF");
        /// </code>
        /// </example>
        /// <param name="hexString">16진수 포맷의 문자열</param>
        /// <returns>파싱된 바이트 배열</returns>
        public static byte[] GetBytesFromHexString(this string hexString) {
            if(IsDebugEnabled)
                log.Debug("Hex Decimal 문자열을 바이트배열로 변환합니다... hexString=[{0}]", hexString.EllipsisChar(80));

            if(hexString.IsWhiteSpace())
                return new byte[0];

            var result = new StringBuilder(hexString.ToUpper(CultureInfo.CurrentCulture));

            if(result.Length >= 2)
                if(result[0].Equals('0') && result[1].Equals('X'))
                    result.Remove(0, 2);

            Guard.Assert(result.Length % 2 == 0, "16진수 문자열이 아닙니다. hexString=[{0}]", hexString.EllipsisChar(80));

            var hexBytes = new byte[result.Length / 2];

            try {
                for(var i = 0; i < hexBytes.Length; i++) {
                    hexBytes[i] = Convert.ToByte(result.ToString(i * 2, 2), 16);
                }
            }
            catch(FormatException e) {
                throw new ArgumentException(string.Format(SR.ErrorInvalidHexDecimalString, "hexString"), e);
            }

            return hexBytes;
        }

        /// <summary>
        /// 16진수 byte 배열을 BitConverter를 통해서 변환된 문자열은 '-' 를 구분자로 하고 있으므로
        /// 다시 byte 배열로 바꿀 때에는 구분자로 '-' 를 주어야 한다.
        /// </summary>
        /// <param name="hexString">16진수 형식의 문자열</param>
        /// <param name="delimiter">바이트문자열 구분자</param>
        /// <returns>파싱된 바이트 배열</returns>
        /// <example>
        /// <code>
        ///     byte[] byte = HexTextTool.GetBytesFromHexString("D3B5DA375E350B9A1EAF");
        /// </code>
        /// </example>
        public static byte[] GetBytesFromHexString(this string hexString, string delimiter) {
            if(IsDebugEnabled)
                log.Debug("Hex Decimal 문자열을 바이트배열로 변환합니다... hexString=[{0}], delimiter=[{1}]", hexString.EllipsisChar(), delimiter);

            if(hexString.IsWhiteSpace())
                return new byte[0];

            string[] hexStrs = hexString.Split(delimiter);

            var length = hexStrs.Length;
            var hexBytes = new byte[length];

            for(var i = 0; i < length; i++)
                hexBytes[i] = Convert.ToByte(hexStrs[i]);

            return hexBytes;
        }

        /// <summary>
        /// Byte 배열을 Hex 형식의 string으로 변환한다.
        /// </summary>
        /// <remarks>byte.AsString("X2") 를 사용한다.</remarks>
        /// <param name="bytes">바이트 배열</param>
        /// <param name="delimiter">구분자</param>
        /// <returns>16진수 형식의 문자열</returns>
        public static string GetHexStringFromBytes(this byte[] bytes, string delimiter = null) {
            if(ArrayTool.IsZeroLengthArray(bytes))
                return string.Empty;

            delimiter = delimiter ?? string.Empty;
            var result = new StringBuilder(bytes.Length * (delimiter.Length + 2));

            var first = true;
            for(var i = 0; i < bytes.Length; i++) {
                if(!first)
                    result.Append(delimiter);

                result.Append(bytes[i].ToString("X2", CultureInfo.InvariantCulture));

                if(first)
                    first = false;
            }

            return result.ToString();
        }

        /// <summary>
        /// Stream 객체를 Hex Code로 Dump해서 문자열로 만든다.
        /// Ultra Edit의 Hex Code Editor에 나타내는 것 처럼
        /// </summary>
        /// <param name="stream">원본 데이타</param>
        /// <returns>
        ///	야래와 같은 형식의 문자열을 반환한다.<br/>
        ///	LINE####: XX XX XX XX  XX XX XX XX  XX XX XX XX  XX XX XX XX  CCCCCCCCCCCCCCCC
        /// </returns>
        public static string GetHexDumpString(this Stream stream) {
            return GetHexDumpString(stream.ToBytes());
        }

        /// <summary>
        /// 바이트 배열을 받아 Hex Code로 Dump해서 문자열로 만든다.
        /// Ultra Edit의 Hex Code Editor에 나타내는 것 처럼
        /// </summary>
        /// <param name="data">원본 데이타</param>
        /// <returns>
        ///	야래와 같은 형식의 문자열을 반환한다.<br/>
        ///	LINE####: XX XX XX XX  XX XX XX XX  XX XX XX XX  XX XX XX XX  CCCCCCCCCCCCCCCC
        /// </returns>
        public static string GetHexDumpString(this byte[] data) {
            if(ArrayTool.IsZeroLengthArray(data))
                return string.Empty;

            // 속도를 높히기 위해서 StringBuilder에 Initail capacity를 충분히 준다.
            //
            var result = new StringBuilder(data.Length * 6);
            int dataLength = data.Length;

            for(int r = 0; r < dataLength; r += 16) {
                result.AppendFormat("{0, 8:}: ", r);

                // Hex Code 넣기
                //
                for(var c = 0; c < 16; c++) {
                    if(r + c < data.Length)
                        result.AppendFormat("{0:X2}", data[r + c]);
                    else
                        result.Append("   ");

                    if(c % 4 == 3)
                        result.Append(" ");
                }

                // 옆에 Char 표시
                //
                for(var c = 0; c < 16; c++) {
                    if(r + c < data.Length) {
                        if(data[r + c] < 32 || data[r + c] > 255)
                            result.Append(".");
                        else
                            result.Append((char)data[r + c]);
                    }
                    else
                        result.Append(" ");
                }
                result.Append(Environment.NewLine);
            }

            return result.ToString();
        }

        /// <summary>
        /// 지정한 Integer 값을 해당 character로 변경합니다. 예: 6 => '6', 12 => 'c'  로
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static char IntToHex(this int n) {
            if(n <= 9)
                return (char)(n + 48);

            return (char)((n - 10) + 97);
        }

        /// <summary>
        /// char 를 int 수형으로 변환한다. ('a' => 10, '8' => 8)
        /// </summary>
        /// <param name="h"></param>
        /// <returns></returns>
        public static int HexToInt(this char h) {
            if(h >= '0' && h <= '9')
                return (h - '0');

            if(h >= 'a' && h < 'f')
                return (h - 'a') + 10;

            if(h >= 'A' && h < 'F')
                return (h - 'A') + 10;

            return -1;
        }
    }
}