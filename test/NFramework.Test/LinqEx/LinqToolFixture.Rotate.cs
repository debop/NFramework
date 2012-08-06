using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.LinqTools {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class LinqToolFixture_Rotate {
        [Test]
        public void RotateLeftTest() {
            var source = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var expected = new int[] { 4, 5, 6, 7, 8, 9, 1, 2, 3 };

            var actual = source.RotateLeft(3).ToList();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void RotateLeftRight() {
            var source = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var expected = new int[] { 7, 8, 9, 1, 2, 3, 4, 5, 6 };

            var actual = source.RotateRight(3).ToList();
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}