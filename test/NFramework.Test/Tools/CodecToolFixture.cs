using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class CodecToolFixture {
        public const int ThreadCount = 5;

        private string originalString;
        private byte[] originalData;

        [TestFixtureSetUp]
        public void Initialize() {
            originalString = @"RealWeb Common Library 3.0 입니다.";
            originalData = originalString.ToBytes();
        }

        [Test]
        public void Base64Test() {
            TestTool
                .RunTasks(ThreadCount,
                          () => {
                              var encodedStr = originalData.EncodeBase64();
                              var decodedData = encodedStr.DecodeBase64();

                              // Console.WriteLine("Encoded String: " + encodedStr);

                              Assert.IsTrue(ArrayTool.Compare(originalData, decodedData));
                          });
        }

        [Test]
        public void Md5Test() {
            TestTool
                .RunTasks(ThreadCount,
                          () => {
                              //byte[] md5 = RwCodec.Md5(originalData);
                              var md5Hex = originalData.Md5Hex();
                              var md5Hex2 = originalString.ToBytes().Md5Hex();

                              //Console.WriteLine("Md5Hex =" + md5Hex);
                              //Console.WriteLine("Md5Hex2=" + md5Hex2);

                              Assert.AreEqual(md5Hex, md5Hex2);
                          });
        }

        [Test]
        public void Sha1Test() {
            TestTool
                .RunTasks(ThreadCount,
                          () => {
                              // byte[] sha1Data = RwCodec.Sha1(originalData);
                              var sha1Hex = originalData.Sha1Hex();
                              var sha1Hex2 = originalString.ToBytes().Sha1Hex();

                              //Console.WriteLine("SHA1 HEX  : " + sha1Hex);
                              //Console.WriteLine("SHA1 HEX2 : " + sha1Hex2);

                              Assert.AreEqual(sha1Hex, sha1Hex2);
                          });
        }
    }
}