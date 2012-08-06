using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.IO;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions {
    [TestFixture]
    public class CompressorExFixture : CompressorFixtureBase {
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

        [Test]
        public void ByteCompressAsyncTest() {
            var plain = PlainBytes;

            foreach(var compressor in _compressors) {
                var compressed = compressor.CompressAsync(plain).Result;
                var recovery = compressor.DecompressAsync(compressed).Result;

                Assert.AreEqual(plain, recovery);
                CollectionAssert.AreEqual(plain, recovery);
            }
        }

        [Test]
        public void CompressStringTest() {
            foreach(var compressor in _compressors) {
                Assert.IsNotNull(compressor);
                Assert.IsInstanceOf<ICompressor>(compressor);

                string compressedString = compressor.CompressString(PlainText);
                string recoveryString = compressor.DecompressString(compressedString);
                Assert.AreEqual(PlainText, recoveryString);
            }
        }

        [Test]
        public void CompressStringAsyncTest() {
            foreach(var compressor in _compressors) {
                Assert.IsNotNull(compressor);
                Assert.IsInstanceOf<ICompressor>(compressor);

                var compressedString = compressor.CompressStringAsync(PlainText).Result;
                var recoveryString = compressor.DecompressStringAsync(compressedString).Result;
                Assert.AreEqual(PlainText, recoveryString);
            }
        }

        [Test]
        public void CompressStreamTest() {
            var plain = PlainText.ToStream();

            Assert.IsTrue(plain.Length > 0);

            foreach(var compressor in _compressors) {
                if(IsDebugEnabled)
                    log.Debug("Compressor : " + compressor.GetType().FullName);

                plain.SetStreamPosition();
                using(var compressed = compressor.CompressStream(plain)) {
                    compressed.SetStreamPosition();
                    using(var restored = compressor.DecompressStream(compressed)) {
                        restored.SetStreamPosition();
                        Assert.AreEqual(PlainText, restored.ToText());
                    }
                }
            }
        }

        [Test]
        public void CompressStreamAsyncTest() {
            var plain = PlainText.ToStream();

            Assert.IsTrue(plain.Length > 0);

            foreach(var compressor in _compressors) {
                if(IsDebugEnabled)
                    log.Debug("Compressor : " + compressor.GetType().FullName);

                plain.SetStreamPosition();
                using(var compressed = compressor.CompressStreamAsync(plain).Result) {
                    compressed.SetStreamPosition();
                    using(var restored = compressor.DecompressStreamAsync(compressed).Result) {
                        restored.SetStreamPosition();
                        Assert.AreEqual(PlainText, restored.ToText());
                    }
                }
            }
        }
    }
}