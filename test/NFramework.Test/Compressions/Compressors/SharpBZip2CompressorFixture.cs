using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [Microsoft.Silverlight.Testing.Tag("Compression")]
    [TestFixture]
    public class SharpBZip2CompressorFixture : CompressorFixtureBase {
        [Test]
        public void BZip2_Compress_Extract_Test() {
            var plainStream = PlainText.ToStream();
            plainStream.Seek(0, SeekOrigin.Begin);

            var plainData = Encoding.UTF8.GetBytes(PlainText);
            byte[] compressedData;
            byte[] extractedData;

            // Compress
            using(var compressedStream = new MemoryStream())
            using(var bz2 = new BZip2OutputStream(compressedStream)) {
                bz2.Write(plainData, 0, plainData.Length);
                bz2.Close();
                compressedData = compressedStream.ToArray();
            }

            Assert.IsNotNull(compressedData);

            // Array.Resize(ref compressedData, compressedData.Length+1);
            // compressedData[compressedData.Length - 1] = (byte)0;

            // Extract
            using(var compressedStream = new MemoryStream(compressedData))
            using(var bz2 = new BZip2InputStream(compressedStream))
            using(var extractedStream = new MemoryStream()) {
                StreamTool.CopyStreamToStream(bz2, extractedStream);
                extractedData = extractedStream.ToArray();
            }


            Assert.IsNotNull(extractedData);
            string extractedText = Encoding.UTF8.GetString(extractedData).TrimEnd('\0');

            Assert.AreEqual(PlainText, extractedText);
        }

        protected override ICompressor GetCompressor() {
            return new SharpBZip2Compressor();
        }
    }
}