using NSoft.NFramework.Compressions;
using NSoft.NFramework.Compressions.Compressors;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class TypeToolFixture : AbstractFixture {
        [Test]
        public void IsSameOrSubclassOf_Test() {
            var typeCovertableDynamicAccessor = new TypeConvertableDynamicAccessor(typeof(GZipCompressor));
            Assert.IsTrue(TypeTool.IsSameOrSubclassOf(typeCovertableDynamicAccessor, typeof(DynamicAccessor)));

            Assert.IsTrue(TypeTool.IsSameOrSubclassOf(typeof(TypeConvertableDynamicAccessor), typeof(DynamicAccessor)));

            Assert.IsFalse(TypeTool.IsSameOrSubclassOf(typeof(IDynamicAccessor), typeof(DynamicAccessor)));
            Assert.IsFalse(TypeTool.IsSameOrSubclassOf(typeof(StringTool), typeof(DynamicAccessor)));
        }

        [Test]
        public void IsSameOrSubclassOrImplementOf_Test() {
            var deflate = new DeflateCompressor();
            Assert.IsTrue(TypeTool.IsSameOrSubclassOrImplementedOf(deflate, typeof(ICompressor)));

            Assert.IsTrue(TypeTool.IsSameOrSubclassOrImplementedOf(typeof(GZipCompressor), typeof(ICompressor)));

            Assert.IsFalse(TypeTool.IsSameOrSubclassOrImplementedOf(typeof(Compressor), typeof(ICompressor)));
        }
    }
}