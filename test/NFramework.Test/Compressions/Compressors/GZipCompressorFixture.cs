using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [TestFixture]
    public class GZipCompressorFixture : CompressorFixtureBase {
        protected override ICompressor GetCompressor() {
            return new GZipCompressor();
        }
    }
}