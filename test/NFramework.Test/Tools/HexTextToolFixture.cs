using System.Text;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class HexTextToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public const string OriginalString = @"RealWeb Common Library. copyright 2010. 리얼웹 공용 라이브러리";
        public static readonly byte[] HexBytes = Encoding.UTF8.GetBytes(OriginalString);

        [Test]
        public void CanConvertToHexStringAndReverse() {
            var hexString = HexTextTool.GetHexStringFromBytes(HexBytes);
            var hexBytes = HexTextTool.GetBytesFromHexString(hexString);

            Assert.AreEqual(hexBytes, HexBytes);
        }

        [Test]
        public void ConvertToStringToByteToStream() {
            var hexBytes = Encoding.UTF8.GetBytes(OriginalString);
            var hexBytes2 = OriginalString.ToBytes(Encoding.UTF8);

            Assert.AreEqual(hexBytes, hexBytes2);

            var hexBytes3 = StringTool.ToStream(OriginalString, Encoding.UTF8).ToBytes();

            Assert.AreEqual(hexBytes2, hexBytes3);
        }

        [Test]
        public void CanHexDumpString() {
            var dumpString = HexTextTool.GetHexDumpString(HexBytes);
            Assert.IsNotEmpty(dumpString);

            var dumpString2 = HexTextTool.GetHexDumpString(StringTool.ToStream(OriginalString, Encoding.UTF8));
            Assert.IsNotEmpty(dumpString2);

            //Console.WriteLine(dumpString);
            //Console.WriteLine();
            //Console.WriteLine(dumpString2);

            Assert.AreEqual(dumpString, dumpString2);
        }
    }
}