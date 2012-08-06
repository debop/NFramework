using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.IO;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions {
    /// <summary>
    /// NSoft.NFramework.Compressions.Compressor 를 테스트합니다. 기본 Compressor를 사용하므로 환경설정에서 설정해야 합니다.
    /// </summary>
    [TestFixture]
    public class CompressorFixture : CompressorFixtureBase {
        private static IList<ICompressor> _compressors;

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            _compressors = IoC.ResolveAll<ICompressor>().ToList();
        }

        [Test]
        public void LoadCompressors() {
            Assert.IsNotNull(_compressors);
            Assert.Greater(_compressors.Count, 0);

            foreach(var compressor in _compressors) {
                Assert.IsNotNull(compressor);
                Assert.IsInstanceOf<ICompressor>(compressor);
            }
        }

        /// <summary>
        /// 모든 ICompressor 구현 Class에 대해 기본적인 압축/복원을 테스트합니다.
        /// </summary>
        [Test]
        public void AllCompressorTest() {
            var plain = PlainBytes;

            foreach(var compressor in _compressors) {
                var compressed = compressor.Compress(plain);
                var recovery = compressor.Decompress(compressed);

                Assert.AreEqual(plain, recovery);
                CollectionAssert.AreEqual(plain, recovery);
            }
        }

        [Test]
        public void ByteCompression() {
            var plain = PlainBytes;

            foreach(var compressor in _compressors) {
                // Console.WriteLine("Compressor : " + compressor.GetType().FullName);
                Assert.IsNotNull(compressor);
                Assert.IsInstanceOf<ICompressor>(compressor);

                Compressor.InnerCompressor = compressor;

                var compressed = Compressor.Compress(plain);
                var recovery = Compressor.Decompress(compressed);
                Assert.AreEqual(plain, recovery);
                CollectionAssert.AreEqual(plain, recovery);
            }
        }

        [Test]
        public void StringCompression() {
            foreach(var compressor in _compressors) {
                // Console.WriteLine("Compressor : " + compressor.GetType().FullName);
                Assert.IsNotNull(compressor);
                Assert.IsInstanceOf<ICompressor>(compressor);

                Compressor.InnerCompressor = compressor;

                string compressedString = Compressor.Compress(PlainText);
                string recoveryString = Compressor.Decompress(compressedString);
                Assert.AreEqual(PlainText, recoveryString);
            }
        }

        [Test]
        public void StreamCompression() {
            var plain = PlainText.ToStream();

            Assert.IsTrue(plain.Length > 0);

            foreach(var compressor in _compressors) {
                if(IsDebugEnabled)
                    log.Debug("Compressor : " + compressor.GetType().FullName);

                Compressor.InnerCompressor = compressor;

                plain.SetStreamPosition();
                using(var compressed = Compressor.Compress(plain)) {
                    compressed.SetStreamPosition();
                    using(var restored = Compressor.Decompress(compressed)) {
                        restored.SetStreamPosition();
                        Assert.AreEqual(PlainText, restored.ToText());
                    }
                }
            }
        }
    }
}