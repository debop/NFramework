using System.IO;
using System.IO.Compression;
using NSoft.NFramework.IO;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    /// <summary>
    /// 스트림 처리와 관련하여, 실제 파일과 같은 IO 비동기 처리에 대해서는 효과가 좋지만, 
    /// 메모리에 대해 비동기 방식 즉 Compute-bound 작업에 대해서는 비동기 방식은 별 효과없다. (스레드가 늘어날 수록 느려진다)
    /// </summary>
    [TestFixture]
    public class AsyncCompressorFixture : CompressorFixtureBase {
        [Test]
        public void Memory_Compress_Decompress() {
            var input = (byte[])PlainBytes.Clone();
            byte[] compressed;

            using(var outStream = new MemoryStream(input.Length)) {
                using(var gzip = new GZipStream(outStream, CompressionMode.Compress)) {
                    gzip.Write(input, 0, input.Length);
                }
                compressed = outStream.ToArray();
                Assert.IsNotNull(compressed);
                Assert.IsTrue(compressed.Length > 0);
            }

            using(var outStream = new MemoryStream(input.Length))
            using(var gzip = new GZipStream(new MemoryStream(compressed), CompressionMode.Decompress)) {
                var readCount = 0;
                var buffer = new byte[CompressorTool.BUFFER_SIZE];

                while((readCount = gzip.Read(buffer, 0, buffer.Length)) > 0) {
                    outStream.Write(buffer, 0, readCount);
                }

                byte[] decompressed = outStream.ToArray();

                Assert.IsTrue(decompressed.Length > 0);
                Assert.AreEqual(PlainBytes, decompressed);
            }
        }

        [Test]
        public void Memory_Compress_Decompress_Async() {
            var input = (byte[])PlainBytes.Clone();
            byte[] compressed;

            using(var outStream = new MemoryStream(input.Length)) {
                using(var gzip = new GZipStream(outStream, CompressionMode.Compress)) {
                    gzip.WriteAsync(input, 0, input.Length).Wait();
                }
                compressed = outStream.ToArray();
                Assert.IsNotNull(compressed);
                Assert.IsTrue(compressed.Length > 0);
            }

            using(var gzip = new GZipStream(new MemoryStream(compressed), CompressionMode.Decompress)) {
                var decompressed = With.TryFunctionAsync(() => gzip.ReadAllBytesAsync().Result);

                Assert.IsTrue(decompressed.Length > 0);
                Assert.AreEqual(PlainBytes, decompressed);
            }
        }

        /// <summary>
        /// 비동기 파일 스트림에 대해 압축/복원을 동기 방식으로 수행
        /// </summary>
        [Test]
        public void FileAsync_Compress_Decompress() {
            var filename = FileTool.GetTempFileName();

            using(var fs = FileAsync.OpenWrite(filename))
            using(var gzip = new GZipStream(fs, CompressionMode.Compress)) {
                gzip.Write(PlainBytes, 0, PlainBytes.Length);
            }

            var fi = new FileInfo(filename);
            Assert.IsTrue(fi.Exists);
            Assert.IsTrue(PlainBytes.Length > fi.Length);


            var outStream = new MemoryStream((int)fi.Length * 2);
            using(var fs = FileAsync.OpenRead(filename))
            using(var gzip = new GZipStream(fs, CompressionMode.Decompress, true)) {
                var readCount = 0;
                var buffer = new byte[CompressorTool.BUFFER_SIZE];

                while((readCount = gzip.Read(buffer, 0, buffer.Length)) > 0) {
                    outStream.Write(buffer, 0, readCount);
                }

                var output = outStream.ToArray();
                Assert.AreEqual(PlainBytes.Length, output.Length);
                Assert.AreEqual(PlainBytes, output);
            }

            fi = new FileInfo(filename);
            fi.Delete();
        }

        /// <summary>
        /// 비동기 파일 스트림에 대해 대한 압축/복원을 비동기 방식으로 수행
        /// </summary>
        [Test]
        public void FileAsync_Compress_Decompress_Async() {
            var filename = FileTool.GetTempFileName();

            using(var fs = FileAsync.OpenWrite(filename))
            using(var gzip = new GZipStream(fs, CompressionMode.Compress)) {
                gzip.WriteAsync(PlainBytes, 0, PlainBytes.Length).Wait();
            }

            var fi = new FileInfo(filename);
            Assert.IsTrue(fi.Exists);
            Assert.IsTrue(PlainBytes.Length > fi.Length);

            using(var fs = FileAsync.OpenRead(filename))
            using(var gzip = new GZipStream(fs, CompressionMode.Decompress, true)) {
                var output = With.TryFunctionAsync(() => gzip.ReadAllBytesAsync().Result);

                Assert.AreEqual(PlainBytes.Length, output.Length);
                Assert.AreEqual(PlainBytes, output);
            }

            fi = new FileInfo(filename);
            fi.Delete();
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(5,
                              Memory_Compress_Decompress,
                              Memory_Compress_Decompress_Async,
                              FileAsync_Compress_Decompress,
                              FileAsync_Compress_Decompress_Async);
        }
    }
}