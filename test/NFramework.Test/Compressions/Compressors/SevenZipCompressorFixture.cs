using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [Microsoft.Silverlight.Testing.Tag("Compression")]
    [TestFixture]
    public class SevenZipCompressorFixture : CompressorFixtureBase {
        protected override ICompressor GetCompressor() {
            return new SevenZipCompressor();
        }
    }
}