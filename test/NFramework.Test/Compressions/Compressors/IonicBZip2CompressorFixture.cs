using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [Microsoft.Silverlight.Testing.Tag("Compression")]
    [TestFixture]
    public class IonicBZip2CompressorFixture : CompressorFixtureBase {
        protected override ICompressor GetCompressor() {
            return new IonicBZip2Compressor();
        }
    }
}