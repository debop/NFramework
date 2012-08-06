using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Compressions {
    public abstract class CompressorFixtureBase : AbstractFixture {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsTraceEnabled = log.IsTraceEnabled;
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected static readonly string PlainText = ArrayTool.GetRandomBytes(0x2000 * 32).GetHexStringFromBytes();
        protected static readonly byte[] PlainBytes = PlainText.ToBytes();

        //protected static readonly string PlainText = "동해물과 백두산이 마르고 닳도록";
        //protected static readonly string PlainText = "동해물과 백두산이 마르고 닳도록".Replicate(100);

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            if(IsDebugEnabled)
                log.Debug("Compressor를 테스트하기 위해 IoC를 초기화합니다.");

            // Initialize IoC);
            if(IoC.IsNotInitialized)
                IoC.Initialize();
        }

        protected virtual ICompressor GetCompressor() {
            return new DummyCompressor();
        }

        [Test]
        public void Compressor_Compress_Decompress() {
            var compressor = GetCompressor();

            var compressed = compressor.Compress(PlainBytes);
            Assert.IsNotNull(compressed);
            Assert.IsTrue(compressed.Length > 0);

            var decompressed = compressor.Decompress(compressed);

            Assert.IsNotNull(decompressed);
            Assert.IsTrue(decompressed.Length > 0);

            Assert.AreEqual(PlainBytes, decompressed);
        }

        [Test]
        public void Compressor_ThreadTest() {
            TestTool.RunTasks(5, Compressor_Compress_Decompress);
        }
    }
}