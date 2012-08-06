using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [Microsoft.Silverlight.Testing.Tag("Compression")]
    [TestFixture]
    public class IconicGZipCompressorFixture : CompressorFixtureBase {
        protected override ICompressor GetCompressor() {
            return new IonicGZipCompressor();
        }
    }
}