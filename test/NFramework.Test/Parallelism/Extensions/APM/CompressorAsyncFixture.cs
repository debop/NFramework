using System;
using System.IO;
using System.Text;
using System.Threading;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions.APM {
    /// <summary>
    /// 비동기 방식이 2 CPU에서는 약 4배 이상 빠르네요^^
    /// </summary>
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class CompressorAsyncFixture : ParallelismFixtureBase {
        protected static readonly string PlainText = ArrayTool.GetRandomBytes(4096 * 16).GetHexStringFromBytes();

        private ICompressor _compressor;

        public ICompressor CurrentCompressor {
            get { return _compressor ?? (_compressor = new SharpBZip2Compressor()); }
        }

        [Test]
        public void Compress_Decompress_Synchronous() {
            var plainBytes = PlainText.ToBytes();

            var compressed = CurrentCompressor.Compress(plainBytes);
            Assert.IsNotNull(compressed);
            Assert.IsTrue(compressed.Length > 0);

            Thread.Sleep(1);

            var decompressed = CurrentCompressor.Decompress(compressed);

            Assert.IsNotNull(decompressed);
            Assert.IsTrue(decompressed.Length > 0);

            Assert.AreEqual(plainBytes, decompressed);
        }

#if !SILVERLIGHT
        [Test]
        public void Compress_Decompress_Async_By_Stream() {
            Stream compressedStream;

            using(var inputStream = PlainText.ToStream(Encoding.UTF8)) {
                compressedStream = With.TryFunctionAsync(() => CurrentCompressor.CompressTask(inputStream).Result);
                Assert.IsNotNull(compressedStream);
                Assert.IsTrue(compressedStream.Length > 0);
            }

            Thread.Sleep(1);

            using(var decompressedStream = With.TryFunctionAsync(() => CurrentCompressor.DecompressTask(compressedStream).Result)) {
                Assert.IsNotNull(decompressedStream);
                Assert.IsTrue(decompressedStream.Length > 0);

                Assert.AreEqual(PlainText, decompressedStream.ToText(Encoding.UTF8));
            }
        }

        [Test]
        public void Comress_Decompress_Async_By_ByteArray() {
            try {
                var plainBytes = PlainText.ToBytes();
                var compressedBytes = CurrentCompressor.Compress(plainBytes);
                Assert.IsNotNull(compressedBytes);

                var decompressedBytes = CurrentCompressor.Decompress(compressedBytes);

                Assert.IsNotNull(decompressedBytes);
                Assert.AreEqual(plainBytes, decompressedBytes);
                Assert.AreEqual(PlainText, decompressedBytes.ToText(Encoding.UTF8));
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.Error(ex);
            }
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(4,
                              () => {
                                  Compress_Decompress_Synchronous();
                                  Compress_Decompress_Async_By_Stream();
                                  Comress_Decompress_Async_By_ByteArray();
                              });
        }
#endif
    }
}