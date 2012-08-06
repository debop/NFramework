using System;
using System.IO;
using System.Text;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Tools {
    public static partial class StringTool {
        #region << ToStream >>

        /// <summary>
        /// 문자열을 스트림 (<see cref="System.IO.MemoryStream"/>)의 인스턴스로 변환한다.
        /// </summary>												
        /// <param name="s">대상 문자열</param>
        /// <param name="enc">인코딩 형식</param>
        /// <returns>변환된 Stream 인스턴스 객체</returns>
        /// <remarks>사용하고 난 Stream 인스턴스 객체는 Close()를 호출하여 메모리에서 제거해야 한다.</remarks>
        public static Stream ToStream(this string s, Encoding enc) {
            enc.ShouldNotBeNull("enc");

            if(s.IsNull())
                return new MemoryStream();

            var stream = new MemoryStream(s.Length * 2);
            var writer = new StreamWriter(stream, enc, 1024);

            writer.Write(s);
            writer.Flush();
            stream.SetStreamPosition();

            return stream;
        }

        /// <summary>
        /// 문자열을 기본 인코딩 방식으로 스트림 (<see cref="System.IO.MemoryStream"/>)의 인스턴스로 변환한다.
        /// </summary>
        /// <param name="s">대상문자열</param>
        /// <returns>기본인코딩 방식으로 Stream을 만든다.</returns>
        public static Stream ToStream(this string s) {
            if(s.IsNull())
                return new MemoryStream();

            var stream = new MemoryStream(s.Length * 2);
            var writer = new StreamWriter(stream);

            writer.Write(s);
            writer.Flush();
            stream.SetStreamPosition();

            return stream;
        }

        /// <summary>
        /// byte 배열을 stream으로 만듭니다.
        /// </summary>
        /// <param name="bytes">byte 배열</param>
        /// <returns></returns>
        public static Stream ToStream(this byte[] bytes) {
            bytes.ShouldNotBeNull("bytes");
            return new MemoryStream(bytes, true);
        }

        /// <summary>
        /// byte 배열을 stream으로 만듭니다.
        /// </summary>
        /// <param name="bytes">byte 배열</param>
        /// <param name="index">시작 인덱스</param>
        /// <param name="count">크기</param>
        /// <returns></returns>
        public static Stream ToStream(this byte[] bytes, int index, int count) {
            bytes.ShouldNotBeNull("bytes");
            return new MemoryStream(bytes, index, count, true);
        }

        #endregion

        #region << ToString >>

        /// <summary>
        /// 주어진 Stream 내용을 지정된 인코딩 방식으로 문자열로 변환한다.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string ToString(this Stream stream, Encoding enc) {
            stream.ShouldNotBeNull("stream");

            using(var sr = new StreamReader(stream, enc))
                return sr.ReadToEnd().TrimEnd('\0');
        }

        /// <summary>
        /// <paramref name="stream"/> 내용을 읽어 문자열로 반환합니다.
        /// </summary>
        /// <param name="stream">읽을 스트림</param>
        /// <returns></returns>
        public static string ToString(Stream stream) {
            stream.ShouldNotBeNull("stream");

            using(var reader = new StreamReader(stream))
                return reader.ReadToEnd().TrimEnd('\0');
        }

        /// <summary>
        /// 주어진 문자열을 주어진 인코딩 방식의 문자열로 변환한다.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string ToString(this string s, Encoding enc) {
            string result = string.Empty;

            if(IsEmpty(s))
                return result;

            using(var stream = ToStream(s, enc)) {
                stream.SetStreamPosition();
                return ToString(stream);
            }
        }

        /// <summary>
        /// Byte 배열을 시스템 기본 인코딩 방식의 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <returns>변환된 문자열</returns>
        public static string ToString(this byte[] bytes) {
            using(var stream = new MemoryStream(bytes)) {
                return ToString(stream);
            }

            // return ToString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Byte 배열의 지정된 범위를 기본 인코딩 방식의 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="index">시작 위치</param>
        /// <param name="count">범위</param>
        /// <returns>시스템 기본 인코딩 형식의 문자열</returns>
        public static string ToString(this byte[] bytes, int index, int count) {
            using(var stream = new MemoryStream(bytes, index, count)) {
                return ToString(stream);
            }
            // return ToString(bytes, DefaultEncoding, index, count);
        }

        /// <summary>
        /// 지정된 인코딩 방식으로 Byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns></returns>
        public static string ToString(this byte[] bytes, Encoding enc) {
            return ToString(bytes, enc, 0, bytes.Length);
        }

        /// <summary>
        /// 지정된 인코딩 방식으로 주어진 범위의 Byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="enc">인코딩 방식</param>
        /// <param name="index">시작 위치</param>
        /// <param name="count">변환할 갯수</param>
        /// <returns>변환된 문자열</returns>
        public static string ToString(this byte[] bytes, Encoding enc, int index, int count) {
            return (enc ?? DefaultEncoding).GetString(bytes, index, count).TrimEnd('\0');
        }

        #endregion

#if !SILVERLIGHT

        #region << ToStringUnsafe >>

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <returns>변환된 문자열</returns>
        public static string ToStringUnsafe(this byte[] bytes) {
            bytes.ShouldNotBeNull("bytes");

            if(bytes.Length == 0)
                return string.Empty;

            string resultStr;

            unsafe {
                fixed(byte* fixedPtr = bytes)
                    resultStr = new string((sbyte*)fixedPtr);
            }

            return resultStr;
        }

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <returns>변환된 문자열</returns>
        [CLSCompliant(false)]
        public static string ToStringUnsafe(this sbyte[] bytes) {
            bytes.ShouldNotBeNull("bytes");
            if(bytes.Length == 0)
                return string.Empty;

            string resultStr;

            unsafe {
                fixed(sbyte* fixedPtr = bytes)
                    resultStr = new string(fixedPtr);
            }
            return resultStr;
        }

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="chars">원본 문자열</param>
        /// <returns>변환된 문자열</returns>
        public static string ToStringUnsafe(this char[] chars) {
            chars.ShouldNotBeNull("chars");

            if(chars.Length == 0)
                return string.Empty;

            string resultStr;
            unsafe {
                fixed(char* fixedPtr = chars)
                    resultStr = new string(fixedPtr);
            }
            return resultStr;
        }

        #endregion

#endif

        #region << ToText >>

        public static string ToText(this string s, Encoding enc) {
            if(IsEmpty(s))
                return string.Empty;

            enc.ShouldNotBeNull("enc");

            using(var stream = ToStream(s, enc)) {
                stream.SetStreamPosition();
                return ToText(stream);
            }
        }

        /// <summary>
        /// <paramref name="stream"/> 내용을 읽어 문자열로 반환합니다.
        /// </summary>
        /// <param name="stream">문자열로 바꿀 스트림</param>
        /// <returns>변환된 문자열</returns>
        public static string ToText(this Stream stream) {
            stream.ShouldNotBeNull("stream");

            // reader 를 닫으면, stream 도 닫혀서 후속 작업에서는 사용하지 못합니다.
            //
            var reader = new StreamReader(stream);
            return reader.ReadToEnd().TrimEnd(NullTerminatorChar);
        }

        /// <summary>
        /// <paramref name="stream"/> 내용의 일부분을 읽어 문자열로 반환합니다.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string ToText(this Stream stream, int length) {
            stream.ShouldNotBeNull("stream");

            var buffer = new char[length];

            var reader = new StreamReader(stream);
            reader.Read(buffer, 0, length);

#if !SILVERLIGHT
            return ToTextUnsafe(buffer);
#else
			return new string(buffer);
#endif
        }

        /// <summary>
        /// 주어진 Stream 내용을 지정된 인코딩 방식으로 문자열로 변환한다.
        /// </summary>
        /// <param name="stream">문자열로 바꿀 스트림</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>변환된 문자열</returns>
        public static string ToText(this Stream stream, Encoding enc) {
            stream.ShouldNotBeNull("stream");
            enc.ShouldNotBeNull("enc");

            stream.SetStreamPosition();
            var reader = new StreamReader(stream, enc);
            return reader.ReadToEnd().TrimEnd(NullTerminatorChar);
        }

        /// <summary>
        /// Byte 배열의 지정된 범위를 기본 인코딩 방식의 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <returns>시스템 기본 인코딩 형식의 문자열</returns>
        public static string ToText(this byte[] bytes) {
            if(bytes == null || bytes.Length == 0)
                return string.Empty;

            return ToText(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Byte 배열의 지정된 범위를 기본 인코딩 방식의 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="index">시작 위치</param>
        /// <param name="count">범위</param>
        /// <returns>시스템 기본 인코딩 형식의 문자열</returns>
        public static string ToText(this byte[] bytes, int index, int count) {
            if(bytes == null || bytes.Length == 0)
                return string.Empty;

            using(var stream = new MemoryStream(bytes, index, count))
                return ToText(stream);
        }

        /// <summary>
        /// 지정된 인코딩 방식으로 주어진 범위의 Byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>변환된 문자열</returns>
        public static string ToText(this byte[] bytes, Encoding enc) {
            if(bytes == null || bytes.Length == 0)
                return string.Empty;

            return ToText(bytes, 0, bytes.Length, enc);
        }

        /// <summary>
        /// 지정된 인코딩 방식으로 주어진 범위의 Byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="enc">인코딩 방식</param>
        /// <param name="index">시작 위치</param>
        /// <param name="count">변환할 갯수</param>
        /// <returns>변환된 문자열</returns>
        public static string ToText(this byte[] bytes, int index, int count, Encoding enc) {
            if(bytes == null || bytes.Length == 0)
                return string.Empty;

            enc.ShouldNotBeNull("enc");

            return enc.GetString(bytes, index, count).TrimEnd(NullTerminatorChar);
        }

#if !SILVERLIGHT

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <returns>변환된 문자열</returns>
        public static string ToTextUnsafe(this byte[] bytes) {
            return ToTextUnsafe(bytes, DefaultEncoding);
        }

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="encoding">인코딩 방식</param>
        /// <returns>변환된 문자열</returns>
        public static string ToTextUnsafe(this byte[] bytes, Encoding encoding) {
            bytes.ShouldNotBeNull("bytes");
            encoding.ShouldNotBeNull("encoding");

            if(bytes.Length == 0)
                return string.Empty;

            unsafe {
                fixed(byte* fixedPtr = bytes)
                    return new string((sbyte*)fixedPtr, 0, bytes.Length, encoding);
            }
        }

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <returns>변환된 문자열</returns>
        [CLSCompliant(false)]
        public static string ToTextUnsafe(this sbyte[] bytes) {
            return ToTextUnsafe(bytes, DefaultEncoding);
        }

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="bytes">원본 바이트 배열</param>
        /// <param name="encoding">인코딩 방식</param>
        /// <returns>변환된 문자열</returns>
        [CLSCompliant(false)]
        public static string ToTextUnsafe(this sbyte[] bytes, Encoding encoding) {
            bytes.ShouldNotBeNull("bytes");
            encoding.ShouldNotBeNull("encoding");

            if(bytes.Length == 0)
                return string.Empty;

            unsafe {
                fixed(sbyte* fixedPtr = bytes)
                    return new string(fixedPtr, 0, bytes.Length, encoding);
            }
        }

        /// <summary>
        /// Unsafe 모드로 byte 배열을 문자열로 변환한다.
        /// </summary>
        /// <remarks>
        /// Managed 함수보다 2배정도 빠르다. Network 같은데서 사용하기 좋다.
        /// </remarks>
        /// <param name="chars">원본 바이트 배열</param>
        /// <returns>변환된 문자열</returns>
        public static string ToTextUnsafe(this char[] chars) {
            chars.ShouldNotBeNull("bytes");

            if(chars.Length == 0)
                return string.Empty;

            unsafe {
                fixed(char* fixedPtr = chars)
                    return new string(fixedPtr);
            }
        }
#endif

        #endregion

        #region << ToBytes >>

        /// <summary>
        /// 문자열을 시스템 기본 인코딩 방식을 이용하여 Byte Array로 변환한다.
        /// </summary>
        /// <param name="s">대상문자열</param>
        /// <returns>변환된 바이트 배열</returns>
        public static byte[] ToBytes(this string s) {
            if(IsEmpty(s))
                return new byte[0];

            using(var stream = ToStream(s)) {
                return ((MemoryStream)stream).ToArray();
            }
            // return ToBytes(s, DefaultEncoding);
        }

        /// <summary>
        /// 문자열을 주어진 인코딩 방식을 이용하여 Byte Array로 변환한다.
        /// </summary>
        /// <param name="s">변환할 문자열</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>변환된 바이트 배열</returns>
        public static byte[] ToBytes(this string s, Encoding enc) {
            enc.ShouldNotBeNull("enc");
            return enc.GetBytes(s);
        }

        /// <summary>
        /// 지정된 스트림 객체의 내용을 byte 배열로 변환한다. (멀티바이트 언어를 나타내는 접두사(3바이트)가 붙을 경우, 제거하고 반환합니다.)
        /// </summary>
        /// <param name="stream">변환할 스트림</param>
        /// <returns>변환된 바이트 배열</returns>
        public static byte[] ToBytes(this Stream stream) {
            const int BUFFER_SIZE = 4096;

            if(stream == null)
                return new byte[0];

            if(IsDebugEnabled)
                log.Debug("지정한 스트림을 바이트 배열로 변환합니다. 만약 멀티바이트 접두사가 있다면, 삭제하고 반한합니다...");

            byte[] result = null;

            if(stream.CanSeek) {
                result = new byte[stream.Length];
                stream.SetStreamPosition();
                stream.Read(result, 0, result.Length);
            }
            else {
                using(var outStream = new MemoryStream(BUFFER_SIZE)) {
                    int readCount;
                    var buffer = new byte[BUFFER_SIZE];
                    while((readCount = stream.Read(buffer, 0, buffer.Length)) > 0) {
                        outStream.Write(buffer, 0, readCount);
                    }
                    result = outStream.ToArray();
                }
            }

            // NOTE : 다국어일 경우 Header에 붙는 접두사가 붙는다. 이것을 제거하고 반환한다.
            //
            if(result.Length >= 3 && ArrayTool.CompareBytes(MultiBytesPrefixBytes, ArrayTool.Copy(result, 0, 3))) {
                result = ArrayTool.Copy(result, 3, result.Length - 3);
            }

            return result;
        }

        #endregion
    }
}