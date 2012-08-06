using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [Microsoft.Silverlight.Testing.Tag("Compression")]
    [TestFixture]
    public class SharpGZipCompressorFixture : CompressorFixtureBase {
        [Test]
        public void GZip_Compress_Extract_Test() {
            var plainStream = PlainText.ToStream();
            plainStream.Seek(0, SeekOrigin.Begin);

            var plainData = Encoding.UTF8.GetBytes(PlainText);
            byte[] compressedData;
            byte[] extractedData;

            // Compress
            using(var compressedStream = new MemoryStream())
            using(var gzs = new GZipOutputStream(compressedStream)) {
                gzs.SetLevel(5);
                gzs.Write(plainData, 0, plainData.Length);
                gzs.Finish();
                compressedData = compressedStream.ToArray();
            }

            Assert.IsNotNull(compressedData);

            // Extract
            using(var compressedStream = new MemoryStream(compressedData)) {
                // compressedStream.Seek(0, SeekOrigin.Begin);
                using(var gzs = new GZipInputStream(compressedStream))
                using(var extractedStream = new MemoryStream()) {
                    StreamTool.CopyStreamToStream(gzs, extractedStream);
                    extractedData = extractedStream.ToArray();
                }
            }

            Assert.IsNotNull(extractedData);
            string extractedText = Encoding.UTF8.GetString(extractedData).TrimEnd('\0');

            Assert.AreEqual(PlainText, extractedText);
        }

        protected override ICompressor GetCompressor() {
            return new SharpGZipCompressor();
        }
    }
}