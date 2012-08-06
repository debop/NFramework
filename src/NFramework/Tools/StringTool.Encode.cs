using System;
using System.IO;
using System.Text;

namespace NSoft.NFramework.Tools {
    public static partial class StringTool {
        /// <summary>
        /// 해당 문자열을 UTF8 (<c>RwConsts.XmlEncoding</c>) 방식으로 변환한다.
        /// </summary>
        /// <param name="s">원본 문자열</param>
        /// <returns>변환된 문자열</returns>
        public static string Utf8Encode(this string s) {
            return ToText(s, Encoding.UTF8);
        }

        /// <summary>
        /// 해당 Stream 내용을 UTF8 (<c>RwConsts.XmlEncoding</c>) 방식으로 변환한다.
        /// </summary>
        /// <param name="stream">원본 스트림 객체</param>
        /// <returns>변환된 문자열</returns>
        public static string Utf8Encode(this Stream stream) {
            return ToText(stream, Encoding.UTF8);
        }

        /// <summary>
        /// UTF8 형식의 문자열을 시스템 기본 인코딩 (<see cref="System.Text.Encoding.Default"/>) 방식으로 변환한다. 
        /// </summary>
        /// <param name="s">원본 문자열</param>
        /// <returns>변환된 문자열</returns>
        public static string Utf8Decode(this string s) {
            return ToText(s.ToBytes());
        }

        /// <summary>
        /// UTF8 형식의 스트림 객체를 시스템 기본 인코딩 (<see cref="System.Text.Encoding.Default"/>) 방식으로 변환한다. 
        /// </summary>
        /// <param name="stream">원본 스트림 객체</param>
        /// <returns>변환된 문자열</returns>
        public static string Utf8Decode(this Stream stream) {
            if(stream is MemoryStream)
                return ((MemoryStream)stream).ToArray().ToText();

            return ToText(stream);
        }

        /// <summary>
        /// Byte Array를 Base64 형식으로 Encoding 한다.
        /// </summary>
        /// <param name="inArray"></param>
        /// <param name="outArray"></param>
        /// <example>
        ///	아래 예제는 일반 파일을 읽어서 Base64로 Encoding하는 예제입니다.
        ///	<code>
        ///	byte[] binData;
        ///	char[] base64CharArray;
        ///   
        ///	using(FileStream inFile = new FileStream(inFileName, FileMode.Open, FileAccess.Read))
        ///	{
        ///		binData = new byte[inFile.Length];
        ///		long readCount = inFile.Read(binData, 0, (int)inFile.Length);
        ///	}
        ///	
        ///	Base64Encode(binData, out base64CharArray);
        ///	
        ///	if( base64CharArray.Length == 0 )
        ///		throw new InvalidOperationException("Cannot Encoding");
        ///		
        ///	using( StreamWriter outFile = new StreamWriter(outFileName, false, Encoding.Default) )
        ///	{
        ///		outFile.Write(base64CharArray);
        ///	}
        ///	</code>
        /// </example>
        public static void Base64Encode(this byte[] inArray, out char[] outArray) {
            inArray.ShouldNotBeNull("inArray");

            var len = (long)(4.0D * inArray.Length / 3.0D);

            if((len % 4) != 0) {
                // 나머지 값을 위해 여분을 늘린다.
                len += (4 - (len % 4));
            }

            outArray = new char[len];
            Convert.ToBase64CharArray(inArray, 0, inArray.Length, outArray, 0);
        }

        /// <summary>
        /// Byte Array를 Base64형식의 문자열로 변환한다.
        /// </summary>
        /// <param name="inArray">바이트 배열</param>
        /// <returns>Base64형식으로 인코딩된 문자열</returns>
        public static string Base64Encode(this byte[] inArray) {
            return Convert.ToBase64String(inArray).TrimEnd(NullTerminatorChar);
        }

        /// <summary>
        /// 일반 문자열을 Base64 문자열로 만든다.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string Base64Encode(this string s) {
            if(IsEmpty(s))
                return string.Empty;

            return Base64Encode(ToBytes(s));
        }

        /// <summary>
        /// 일반 문자열을 Base64 문자열로 만든다.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string Base64Encode(this string s, Encoding enc) {
            if(IsEmpty(s))
                return string.Empty;

            enc.ShouldNotBeNull("enc");

            return Base64Encode(ToBytes(s, enc));
        }

        /// <summary>
        /// Base64형식으로 인코딩된 문자열을 디코딩하여 바이트 배열로 변환한다.
        /// </summary>
        /// <param name="s">Base64형식으로 인코딩된 문자열</param>
        /// <returns>문자열이 null이거나, 실패시에는 길이가 0인 바이트 배열을 반환한다.</returns>
        /// <example>
        ///	<code>
        ///   string base64String;
        ///   
        ///   // Base64로 Encoding 된 파일로부터 내용을 읽어온다.
        ///	using(StreamReader inFile = new streamReader(inFileName, Encoding.Default))
        ///	{
        ///		char[] base64CharArray = new char[inFile.BaseStream.Length];
        ///		inFile.Read(base64CharArray, 0, (int)inFile.BaseStream.Length);
        ///		base64String = new string(base64CharArray);
        ///	}
        ///	
        ///	byte[] binData = Base64Decode(base64String);
        ///	
        ///	// 디코딩된 바이트 배열을 파일에 쓴다.
        ///	using(FileStream outFile = new FileStream(outFileName, FileMode.Create, FileAccess.Write))
        ///	{
        ///		outFile.Write(binData, 0, binData.Length);
        ///	}
        ///	</code>
        /// </example>
        public static byte[] Base64Decode(this string s) {
            if(IsEmpty(s))
                return new byte[0];

            return Convert.FromBase64String(s);
        }

        public static string GetString(this Encoding enc, byte[] bytes) {
            return enc.GetString(bytes, 0, bytes.Length).TrimEnd('\0');
        }
    }
}