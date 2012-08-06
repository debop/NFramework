using System.Linq;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.InversionOfControl;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Compressions {
    [TestFixture]
    public class CompressorIoCFixture : CompressorFixtureBase {
        /// <summary>
        /// 정의된 ComponentId 로 Compressor들을 인스턴싱을 해본다.
        /// </summary>
        /// <param name="componentId"></param>
        [Test]
        [TestCase("GZipCompressor")]
        [TestCase("DeflateCompressor")]
        [TestCase("CloneCompressor")]
        [TestCase("SharpBZip2Compressor")]
        [TestCase("SharpGZipCompressor")]
        public void ResolveCompressors(string componentId) {
            var compressor = IoC.Resolve<ICompressor>(componentId);
            Assert.IsNotNull(compressor);
            Assert.AreEqual(compressor.GetType().Name, componentId);
        }

        [Test]
        public void GetComponent() {
            var compressor = IoC.TryResolve<ICompressor, GZipCompressor>();

            compressor.Should().Not.Be.Null();
            compressor.Should().Be.InstanceOf<ICompressor>();
        }

        [Test]
        public void ResolveAllCompressors() {
            var compressors = IoC.ResolveAll<ICompressor>();
            Assert.IsNotNull(compressors);
            Assert.Greater(compressors.Count(), 0);

            foreach(var compressor in compressors) {
                compressor.Should().Not.Be.Null();
                compressor.Should().Be.InstanceOf<ICompressor>();

                if(IsDebugEnabled)
                    log.Debug("Compressor=" + compressor.GetType().FullName);
            }
        }
    }
}