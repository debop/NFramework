using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [Microsoft.Silverlight.Testing.Tag("Compression")]
    [TestFixture]
    public class IonicZlibCompressorFixture : CompressorFixtureBase {
        protected override ICompressor GetCompressor() {
            return new IonicZlibCompressor();
        }
    }
}