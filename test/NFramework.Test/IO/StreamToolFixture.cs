using System.IO;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.IO {

    [TestFixture]
    public class StreamToolFixture {

        [Test]
        public void Write_Read_ValueStream() {

            using(var ms = new ValueStream())
            using(var ms2 = new ValueStream()) {
                var s = @"동해물과 백두산이";
                var b = s.ToBytes();
                ms.Write(s);
                ms.Write(true);
                ms.Write(1234);
                ms.Write('x');
                ms.Write(1245.567F);
                ms.Write(999.99);
                ms.Write(4444L);
                ms.Write((short)127);
                ms.Write(s.ToBytes());

                ms.Position = 0;

                Assert.AreEqual(ms.ReadString(), s);
                Assert.AreEqual(ms.ReadBoolean(), true);
                Assert.AreEqual(ms.ReadInt32(), 1234);
                Assert.AreEqual(ms.ReadChar(), 'x');
                Assert.AreEqual(ms.ReadFloat(), 1245.567F);
                Assert.AreEqual(ms.ReadDouble(), 999.99);
                Assert.AreEqual(ms.ReadInt64(), 4444L);
                Assert.AreEqual(ms.ReadInt16(), 127);

                Assert.AreEqual(ms.ReadBytes(b.Length).ToText(), s);

                ms.Position = 0;

                // ms 의 내용을 m2에 쓴다.
                ms2.Write(ms);
                ms2.Position = 0;

                // m2 stream을 모두 복사해서 새로운 객체를 만든다.
                var ms3 = (MemoryStream)ms2.ReadStream();
                ms3.Position = 0;

                Assert.AreEqual(StreamTool.ReadString(ms3), s);
                Assert.AreEqual(StreamTool.ReadBoolean(ms3), true);
                Assert.AreEqual(StreamTool.ReadInt32(ms3), 1234);
                Assert.AreEqual(StreamTool.ReadChar(ms3), 'x');
                Assert.AreEqual(StreamTool.ReadFloat(ms3), 1245.567F);
                Assert.AreEqual(StreamTool.ReadDouble(ms3), 999.99);
                Assert.AreEqual(StreamTool.ReadInt64(ms3), 4444L);
                Assert.AreEqual(StreamTool.ReadInt16(ms3), 127);

                Assert.AreEqual(StreamTool.ReadBytes(ms3, b.Length).ToText(), s);
            }
        }
    }
}