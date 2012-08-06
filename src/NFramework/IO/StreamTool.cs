using System;
using System.IO;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.IO {
    /// <summary>
    /// Stream 개체에 Value type 정보를 read/write할 수있는 Utility class
    /// </summary>
    /// <remarks>
    /// Serializable 객체를 사용하면 되겠지만, 지정된 Stream에 기록할 때 쓰는 고전적인 방법이다.
    /// </remarks>
    /// <example>
    /// 스트림에 정보를 쓰고, 읽는 작업을 합니다.
    /// <code>
    /// using (ValueStream ms = new ValueStream())
    /// using (ValueStream ms2 = new ValueStream())
    /// {
    ///     string s = "동해물과 백두산이";
    ///     byte[] b = StreamTool.ToBytes(s);
    ///     ms.Write(s);
    ///     ms.Write(true);
    ///     ms.Write(1234);
    ///     ms.Write('x');
    ///     ms.Write(1245.567F);
    ///     ms.Write((double)999.99);
    ///     ms.Write(4444L);
    ///     ms.Write((short)127);
    ///     ms.Write(StreamTool.ToBytes(s));
    /// 
    ///     ms.Position = 0;
    /// 
    ///     Console.WriteLine(ms.ReadString());
    ///     Console.WriteLine(ms.ReadBoolean());
    ///     Console.WriteLine(ms.ReadInt32());
    ///     Console.WriteLine(ms.ReadChar());
    ///     Console.WriteLine(ms.ReadFloat());
    ///     Console.WriteLine(ms.ReadDouble());
    ///     Console.WriteLine(ms.ReadInt64());
    ///     Console.WriteLine(ms.ReadInt16());
    /// 
    ///     Console.WriteLine(StreamTool.AsString(ms.ReadBytes(b.Length)));
    /// 
    ///     // Stream 간의 복사
    ///     ms.Position = 0;
    ///     ms2.Write(ms);
    ///     ms2.Position = 0;
    ///     MemoryStream ms3 = (MemoryStream)ms2.ReadStream();
    ///     ms3.Position = 0;
    /// 
    ///     Console.WriteLine(StreamTool.ReadString(ms3));
    ///     Console.WriteLine(StreamTool.ReadBoolean(ms3));
    ///     Console.WriteLine(StreamTool.ReadInt32(ms3));
    ///     Console.WriteLine(StreamTool.ReadChar(ms3));
    ///     Console.WriteLine(StreamTool.ReadFloat(ms3));
    ///     Console.WriteLine(StreamTool.ReadDouble(ms3));
    ///     Console.WriteLine(StreamTool.ReadInt64(ms3));
    ///     Console.WriteLine(StreamTool.ReadInt16(ms3));
    ///     Console.WriteLine(StreamTool.AsString(StreamTool.ReadBytes(ms3, b.Length)));
    /// }
    /// </code>
    /// </example>
    public static class StreamTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const int BufferSize = 0x1000;

        #region << CopyStreamToStream >>

        /// <summary>
        /// <paramref name="srcStream"/> 스트림을 읽어, <paramref name="destStream"/>에 쓴다.
        /// </summary>
        /// <param name="srcStream">원본 스트림</param>
        /// <param name="destStream">대상 스트림</param>
        public static void CopyStreamToStream(this Stream srcStream, Stream destStream) {
            CopyStreamToStream(srcStream, destStream, BufferSize);
        }

        /// <summary>
        /// <paramref name="srcStream"/> 스트림을 읽어, <paramref name="destStream"/>에 쓴다. 
        /// </summary>
        /// <param name="srcStream">원본 스트림</param>
        /// <param name="destStream">대상 스트림</param>
        /// <param name="bufferSize">버퍼 크기</param>
        public static void CopyStreamToStream(this Stream srcStream, Stream destStream, int bufferSize) {
            srcStream.ShouldNotBeNull("srcStream");
            destStream.ShouldNotBeNull("destStream");
            bufferSize.ShouldBePositive("bufferSize");

            if(IsDebugEnabled)
                log.Debug("원본 스트림으로부터 대상 스트림으로 복사합니다. bufferSize=[{0}]", bufferSize);

            var buffer = new byte[bufferSize];
            int readCount;

            while((readCount = srcStream.Read(buffer, 0, buffer.Length)) > 0) {
                destStream.Write(buffer, 0, readCount);
            }
        }

        /// <summary>
        /// <paramref name="stream"/> 의 현재 위치부터 끝까지 읽어서 바이트배열로 변환합니다.
        /// </summary>
        /// <param name="stream">읽을 스트림</param>
        /// <returns></returns>
        public static byte[] ReadAllBytes(this Stream stream) {
            stream.ShouldNotBeNull("stream");

            if(IsDebugEnabled)
                log.Debug("스트림의 현재 위치서부터 끝까지 모두 읽어 바이트 배열로 반환합니다...");

            byte[] buffer = new byte[stream.Length - stream.Position];
            stream.Read(buffer, (int)stream.Position, buffer.Length);
            return buffer;
        }

        #endregion

        #region << SetStreamPosition >>

        /// <summary>
        /// Stream의 Position을 변경한다.
        /// </summary>
        /// <param name="stream">대상 스트림 객체</param>
        /// <param name="position">지정된 포지션</param>
        /// <exception cref="IOException">포지션이 스트림 객체의 크기보다 클때</exception>
        /// 
        public static Stream SetStreamPosition(this Stream stream, int position = 0) {
            if(stream != null && stream.CanSeek)
                stream.Position = position;

            return stream;
        }

        #endregion

        #region << Write Value >>

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, string v) {
            if(v != null) {
                var b = v.ToBytes();
                Write(stream, b.Length);
                Write(stream, b);

                //Write(stream, v.Length);
                // Write(stream, Encoding.Default.GetBytes(v));

                //for(int i=0; i < v.Length; i++)
                //	Write(stream, v[i]);
            }
            else
                Write(stream, 0);
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, int v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, long v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, double v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, float v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        [CLSCompliant(false)]
        public static void Write(this Stream stream, uint v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        [CLSCompliant(false)]
        public static void Write(this Stream stream, ulong v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, char v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, short v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        public static void Write(this Stream stream, bool v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        [CLSCompliant(false)]
        public static void Write(this Stream stream, ushort v) {
            Write(stream, BitConverter.GetBytes(v));
        }

        /// <summary>
        /// 스트림에 값 쓰기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="v">쓸 값</param>
        internal static void Write(this Stream stream, byte[] v) {
            if(v != null)
                stream.Write(v, 0, v.Length);
        }

        /// <summary>
        /// 원본 스트림을 대상 스트림에 쓴다.
        /// </summary>
        /// <remarks>
        /// 원본 스트림의 크기를 먼저 대상 스트림에 기록하고, 내용을 쓴다. 
        /// 읽어 올 때도 크기를 먼저 읽고, 그 크기 만큼 내용을 읽어온다.
        /// </remarks>
        /// <param name="stream">복사 대상 스트림</param>
        /// <param name="source">원본 스트림</param>
        public static void Write(this Stream stream, Stream source) {
            if(source != null) {
                if(source.CanSeek && source.Position != 0)
                    source.Position = 0;

                var buffer = new byte[source.Length];
                source.Read(buffer, 0, buffer.Length);

                // 쓸 내용의 크기를 먼저 기록한다.
                //
                Write(stream, buffer.Length);

                // 원본 내용을 쓴다.
                Write(stream, buffer);
            }
            else
                Write(stream, 0);
        }

        #endregion

        #region << Read Value >>

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out double v) {
            var bs = BitConverter.GetBytes(0.0);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToDouble(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out float v) {
            var bs = BitConverter.GetBytes(0.0F);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToSingle(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out int v) {
            var bs = BitConverter.GetBytes(0);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToInt32(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out long v) {
            var bs = BitConverter.GetBytes((long)0);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToInt64(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        [CLSCompliant(false)]
        public static void Read(this Stream stream, out uint v) {
            var bs = BitConverter.GetBytes((uint)0);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToUInt32(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        [CLSCompliant(false)]
        public static void Read(this Stream stream, out ulong v) {
            var bs = BitConverter.GetBytes((ulong)0);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToUInt64(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out short v) {
            var bs = BitConverter.GetBytes((short)0);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToInt16(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        [CLSCompliant(false)]
        public static void Read(this Stream stream, out ushort v) {
            var bs = BitConverter.GetBytes((ushort)0);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToUInt16(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out char v) {
            var bs = BitConverter.GetBytes('\0');

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToChar(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out bool v) {
            var bs = BitConverter.GetBytes(false);

            if(stream.Read(bs, 0, bs.Length) != bs.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");

            v = BitConverter.ToBoolean(bs, 0);
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        public static void Read(this Stream stream, out string v) {
            int len;
            Read(stream, out len);
            if(len > 0) {
                var buffer = new byte[len];
                stream.Read(buffer, 0, len);

#if !SILVERLIGHT
                // ToStringUnsafe 메소드의 속도가 2배이상 빠르다.
                v = buffer.ToTextUnsafe();
#else
				v = buffer.ToText();
#endif
            }
            else
                v = null;
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 값을 반환한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="v">읽은 값</param>
        internal static void Read(this Stream stream, byte[] v) {
            v.ShouldNotBeNull("v");

            if(stream.Read(v, 0, v.Length) != v.Length)
                throw new InvalidOperationException("Stream에서 읽은 정보가 지정한 수형과 일치하지 않습니다.");
        }

        /// <summary>
        /// 지정된 <paramref name="stream"/>을 읽어서 <paramref name="target"/>에 저장한다.
        /// </summary>
        /// <param name="stream">읽을 대상 <see cref="Stream"/> 인스턴스</param>
        /// <param name="target">읽은 값을 저장한 <see cref="Stream"/> 인스턴스</param>
        public static void Read(this Stream stream, out Stream target) {
            // 읽을 Data 길이를 먼저 읽는다
            //
            int len;
            Read(stream, out len);

            if(len > 0) {
                // Data 길이만큼만 읽는다
                //
                var buffer = new byte[len];
                stream.Read(buffer, 0, len);

                target = new MemoryStream(buffer);
            }
            else
                target = null;
        }

        #endregion

        #region << Read and Return Value >>

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static string ReadString(this Stream stream) {
            string s;
            Read(stream, out s);
            return s;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static Int16 ReadInt16(this Stream stream) {
            Int16 v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static Int32 ReadInt32(this Stream stream) {
            Int32 v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static Int64 ReadInt64(this Stream stream) {
            Int64 v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        [CLSCompliant(false)]
        public static UInt16 ReadUInt16(this Stream stream) {
            UInt16 v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        [CLSCompliant(false)]
        public static UInt32 ReadUInt32(this Stream stream) {
            UInt32 v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        [CLSCompliant(false)]
        public static UInt64 ReadUInt64(this Stream stream) {
            UInt64 v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static Char ReadChar(this Stream stream) {
            Char c;
            Read(stream, out c);
            return c;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static bool ReadBoolean(this Stream stream) {
            bool b;
            Read(stream, out b);
            return b;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static float ReadFloat(this Stream stream) {
            float v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static double ReadDouble(this Stream stream) {
            double v;
            Read(stream, out v);
            return v;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <param name="length">읽을 길이</param>
        /// <returns>읽은 값</returns>
        public static byte[] ReadBytes(this Stream stream, int length) {
            var buffer = new byte[length];
            Read(stream, buffer);
            return buffer;
        }

        /// <summary>
        /// <paramref name="stream"/>에서 값 읽어오기
        /// </summary>
        /// <param name="stream">대상 Stream 객체</param>
        /// <returns>읽은 값</returns>
        public static Stream ReadStream(this Stream stream) {
            Stream ms;
            Read(stream, out ms);
            return ms;
        }

        #endregion
    }
}