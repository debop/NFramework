using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions {
    [TestFixture]
    public class CompressAdaptorTestCase : CompressorFixtureBase {
        private static IList<ICompressor> _compressors;

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();
            _compressors = IoC.ResolveAll<ICompressor>().ToList();
        }

        [Test]
        public void LoadCompressorsTest() {
            Assert.IsNotNull(_compressors);
            Assert.Greater(_compressors.Count, 0);

            foreach(var compressor in _compressors) {
                Assert.IsNotNull(compressor);
                Assert.IsInstanceOf<ICompressor>(compressor);
            }
        }

        [Test]
        public void Adaptor_Compress_Decompress_By_ByteArray_InParallel() {
            var plainBytes = PlainText.ToBytes();

            Parallel.ForEach(_compressors,
                             compressor => {
                                 var adapter = new CompressAdapter(compressor);

                                 var compressed = adapter.Compress(plainBytes);
                                 var recovery = adapter.Decompress(compressed);
                                 Assert.AreEqual(plainBytes, recovery);
                                 CollectionAssert.AreEqual(plainBytes, recovery);
                             });
        }

        [Test]
        public void Adaptor_Compress_Decompress_By_Stream_InParallel() {
            var plainStream = PlainText.ToStream();

            Parallel.ForEach(_compressors,
                             compressor => {
                                 var adapter = new CompressAdapter(compressor);

                                 var compressed = adapter.Compress(plainStream);
                                 var recoveryStream = new MemoryStream(compressed.Length * 2);
                                 adapter.Decompress(compressed, recoveryStream);

                                 // 데이터가 써졌으므로, 다시 원 위치로 설정해줘야 합니다.
                                 recoveryStream.SetStreamPosition();

                                 Assert.IsTrue(recoveryStream.Length > 0);
                                 Assert.AreEqual(PlainText, recoveryStream.ToText());
                             });
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(4,
                              () => {
                                  LoadCompressorsTest();
                                  Adaptor_Compress_Decompress_By_ByteArray_InParallel();
                                  Adaptor_Compress_Decompress_By_Stream_InParallel();
                              });
        }
    }
}