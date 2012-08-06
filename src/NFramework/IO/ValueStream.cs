using System;
using System.IO;

namespace NSoft.NFramework.IO {
    /// <summary>
    /// 예전 방식대로 <c>Stream</c>에 값 형식의 정보를 저장하고, 조회할 때 사용하는 Class
    /// </summary>
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
    public class ValueStream : MemoryStream {
        /// <summary>
        /// 인스턴스에 <paramref name="s"/> 값 쓰기
        /// </summary>
        /// <param name="s">쓸 값</param>
        public void Write(string s) {
            StreamTool.Write(this, s);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(short v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(int v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(long v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        [CLSCompliant(false)]
        public void Write(ushort v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        [CLSCompliant(false)]
        public void Write(uint v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        [CLSCompliant(false)]
        public void Write(ulong v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(float v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(double v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(bool v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(char v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="v"/> 값 쓰기
        /// </summary>
        /// <param name="v">쓸 값</param>
        public void Write(byte[] v) {
            StreamTool.Write(this, v);
        }

        /// <summary>
        /// 인스턴스에 <paramref name="stream"/> 값 쓰기
        /// </summary>
        /// <param name="stream">쓸 값</param>
        public void Write(Stream stream) {
            StreamTool.Write(this, stream);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="s">읽은 값</param>
        public void Read(out string s) {
            StreamTool.Read(this, out s);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(out short v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(out int v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(out long v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        [CLSCompliant(false)]
        public void Read(out ushort v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        [CLSCompliant(false)]
        public void Read(out uint v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        [CLSCompliant(false)]
        public void Read(out ulong v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(out float v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(out double v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(out bool v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(out char v) {
            StreamTool.Read(this, out v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="v">읽은 값</param>
        public void Read(byte[] v) {
            StreamTool.Read(this, v);
        }

        /// <summary>
        /// 인스턴스에서 값 읽기
        /// </summary>
        /// <param name="stream">읽은 값</param>
        public void Read(out Stream stream) {
            StreamTool.Read(this, out stream);
        }

        /// <summary>
        /// 인스턴스에서 문자열 읽기
        /// </summary>
        /// <returns>읽은 문자열</returns>
        public string ReadString() {
            return StreamTool.ReadString(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="System.Int16"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        public Int16 ReadInt16() {
            return StreamTool.ReadInt16(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="System.Int32"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        public Int32 ReadInt32() {
            return StreamTool.ReadInt32(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="System.Int64"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        public Int64 ReadInt64() {
            return StreamTool.ReadInt64(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="System.UInt16"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        [CLSCompliant(false)]
        public UInt16 ReadUInt16() {
            return StreamTool.ReadUInt16(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="System.UInt32"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        [CLSCompliant(false)]
        public UInt32 ReadUInt32() {
            return StreamTool.ReadUInt32(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="System.UInt64"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        [CLSCompliant(false)]
        public UInt64 ReadUInt64() {
            return StreamTool.ReadUInt64(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="float"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        public float ReadFloat() {
            return StreamTool.ReadFloat(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="double"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        public double ReadDouble() {
            return StreamTool.ReadDouble(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="char"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        public char ReadChar() {
            return StreamTool.ReadChar(this);
        }

        /// <summary>
        /// 인스턴스에서 <see cref="System.Boolean"/> 형식의 값 읽기
        /// </summary>
        /// <returns>읽은 값</returns>
        public bool ReadBoolean() {
            return StreamTool.ReadBoolean(this);
        }

        /// <summary>
        /// 스트림에서 현위치부터 <paramref name="length"/> 길이 만큼 읽어서 일차원 바이트 배열로 반환한다.
        /// </summary>
        /// <param name="length">읽을 길이</param>
        /// <returns>일차원 바이트 배열</returns>
        public byte[] ReadBytes(int length) {
            return StreamTool.ReadBytes(this, length);
        }

        /// <summary>
        /// 인스턴스의 현재 <see cref="Stream.Position"/>에서부터 끝까지 읽어서 <see cref="Stream"/>으로 반환한다.
        /// </summary>
        /// <returns>읽은 <see cref="Stream"/> 인스턴스</returns>
        public Stream ReadStream() {
            return StreamTool.ReadStream(this);
        }
    }
}