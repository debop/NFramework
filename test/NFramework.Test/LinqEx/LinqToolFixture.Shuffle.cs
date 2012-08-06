using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.LinqEx.LinqTools {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class LinqToolFixture_Shuffle {
        [Test]
        public void ShuffleTest1() {
            var source = Enumerable.Range(1, 1000).ToArray();
            var expected = source;

            var actual = source.Shuffle().ToList();
            Assert.AreEqual(expected.Count(), actual.Count());
            Assert.AreEqual(expected.Sum(), actual.Sum());

            source.All(x => actual.Contains(x)).Should().Be.True();

            //Assert.IsTrue(expected.SequenceRelation(actual) == SequenceRelationType.Similar);
        }

        [Test]
        public void ShuffleTest2() {
            const int seed = 1231;
            var source = Enumerable.Range(1, 1000).ToArray();
            var expected = source;

            var actual = source.Shuffle(seed).ToList();
            Assert.AreEqual(expected.Count(), actual.Count());
            Assert.AreEqual(expected.Sum(), actual.Sum());
            source.All(x => actual.Contains(x)).Should().Be.True();

            //Assert.IsTrue(expected.SequenceRelation(actual) == SequenceRelationType.Similar);
        }
    }
}