using System.Threading;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Reflections {
    [TestFixture]
    [RequiresThread(ApartmentState.STA)]
    public class DynamicAccessorFactoryCase : AbstractFixture {
        public const int ThreadCount = 5;

        [Test]
        public void CreateDynamicAccessor_By_Type() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var gzipAccessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(GZipCompressor));
                                  Assert.IsNotNull(gzipAccessor);

                                  var deflateAccessor = DynamicAccessorFactory.CreateDynamicAccessor(typeof(DeflateCompressor));
                                  Assert.IsNotNull(deflateAccessor);
                              });
        }

        [Test]
        public void CreateDynamicAccessor_Generic() {
            TestTool.RunTasks(ThreadCount,
                              () => {
                                  var gzipAccessor = DynamicAccessorFactory.CreateDynamicAccessor<GZipCompressor>();
                                  Assert.IsNotNull(gzipAccessor);

                                  var deflateAccessor = DynamicAccessorFactory.CreateDynamicAccessor<DeflateCompressor>();
                                  Assert.IsNotNull(deflateAccessor);
                              });
        }

        [Test]
        public void Clear_Test() {
            CreateDynamicAccessor_By_Type();
            CreateDynamicAccessor_Generic();

            DynamicAccessorFactory.ResetCache();

            CreateDynamicAccessor_By_Type();
            CreateDynamicAccessor_Generic();
        }
    }
}