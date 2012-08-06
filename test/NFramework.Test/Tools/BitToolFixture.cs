using System.Collections;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class BitToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void BitArrayToText() {
            var bitArray = new BitArray(32);

            var bitString = bitArray.ToText();
            Assert.AreEqual(32, bitString.Length);
            Assert.IsFalse(bitString.Contains("1"));
            Assert.IsTrue(bitString.All(c => c == '0'));

            bitString = bitArray.Not().ToText();
            Assert.AreEqual(32, bitString.Length);
            Assert.IsFalse(bitString.Contains("0"));
            Assert.IsTrue(bitString.All(c => c == '1'));
        }

        [Test]
        public void BitArrayToBytes() {
            var bitArray = new BitArray(32);

            var bytes = bitArray.ToBytes();
            Assert.AreEqual(32 / BitTool.ByteLength, bytes.Length);

            Assert.IsTrue(bytes.All(b => b == 0x00));

            Assert.IsTrue(bytes.GetHexStringFromBytes().All(c => c == '0'));

            bytes = bitArray.Not().ToBytes();
            Assert.IsTrue(bytes.GetHexStringFromBytes().All(c => c == 'F'));

            // Console.WriteLine(bytes.GetHexStringFromBytes());
        }

        [Test]
        public void BitFlagConversion() {
            int x = 0x00;
            var x1 = BitTool.BitOn(x, 0xFF);
            Assert.AreEqual(0xFF, x1);

            var x2 = BitTool.BitOff(x1, 0xFF);
            Assert.AreEqual(0x00, x2);

            var x3 = BitTool.BitFlip(x, 0xFF);
            Assert.AreEqual(0xFF, x3);
        }
    }
}