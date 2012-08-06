using NUnit.Framework;

namespace NSoft.NFramework.Compressions.Compressors {
    [TestFixture]
    public class DeflateCompressorFixture : CompressorFixtureBase {
        protected override ICompressor GetCompressor() {
            return new DeflateCompressor();
        }
    }
}