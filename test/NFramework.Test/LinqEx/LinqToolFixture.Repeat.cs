using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.LinqTools {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class LinqToolFixture_Repeat {
        [Test]
        public void RepeatTest() {
            IEnumerable<int> expected = new int[] { 1, 1, 2, 3, 1, 1, 2, 3, 1, 1, 2, 3, 1, 1, 2, 3 };
            IEnumerable<int> actual = LinqTool.Repeat(new int[] { 1, 1, 2, 3 }, 4);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }
    }
}