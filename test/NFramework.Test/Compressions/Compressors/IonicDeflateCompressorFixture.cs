using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [Microsoft.Silverlight.Testing.Tag("Compression")]
    [TestFixture]
    public class IonicDeflateCompressorFixture : CompressorFixtureBase {
        protected override ICompressor GetCompressor() {
            return new IonicDeflateCompressor();
        }
    }
}