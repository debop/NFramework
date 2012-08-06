using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Numerics.Utils {
    [TestFixture]
    public class MathToolFixture_Probability : AbstractMathToolFixtureBase {
        [Test]
        public void FrequencyTest1() {
            var input = new int[]
                        {
                            1, 2, 3, 4, 3, 2, 3, 3, 3, 3, 2, 2, 2, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 3, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 3,
                            3, 1, 1, 2, 2, 1, 2, 1, 1, 2, 1, 2
                        };
            var expected = new KeyValuePair<int, int>[]
                           {
                               new KeyValuePair<int, int>(1, 13),
                               new KeyValuePair<int, int>(2, 23),
                               new KeyValuePair<int, int>(3, 12),
                               new KeyValuePair<int, int>(4, 1)
                           };
            var actual = input.Frequency();

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void FrequencyTest2() {
            var input = new int[]
                        {
                            1, 2, 3, 4, 3, 2, 3, 3, 3, 3, 2, 2, 2, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 3, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 3,
                            3, 1, 1, 2, 2, 1, 2, 1, 1, 2, 1, 2
                        };
            Func<int, string> bucketSelector = X => X <= 2 ? "2 and under" : "Over 2";

            var expected = new KeyValuePair<string, int>[]
                           { new KeyValuePair<string, int>("2 and under", 36), new KeyValuePair<string, int>("Over 2", 13) };
            var actual = input.Frequency(bucketSelector);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void FrequencyTest3() {
            var input = new int[]
                        {
                            1, 2, 3, 4, 3, 2, 3, 3, 3, 3, 2, 2, 2, 3, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 1, 3, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 3,
                            3, 1, 1, 2, 2, 1, 2, 1, 1, 2, 1, 2
                        };
            var buckets = new string[] { "a", "b", "c", "Z1", "Z2" };
            Func<int, IEnumerable<string>, string> bucketSelector = BucketSelector;
            var expected = new KeyValuePair<string, int>[]
                           {
                               new KeyValuePair<string, int>("a", 13),
                               new KeyValuePair<string, int>("b", 23),
                               new KeyValuePair<string, int>("c", 12),
                               new KeyValuePair<string, int>("Z1", 1),
                               new KeyValuePair<string, int>("Z2", 0)
                           };
            var actual = MathTool.Frequency(input, buckets, bucketSelector);

            if(IsDebugEnabled)
                log.Debug(@"expected=[{0}], actual=[{1}]", expected.CollectionToString(), actual.CollectionToString());

            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void ProbabilityTest1() {
            var input = new int[]
                        {
                            1, 3, 3, 3, 3, 3, 3, 2, 2, 2, 3, 1, 2, 1, 2, 1, 2, 1, 2, 3, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2, 3, 3, 1, 1, 2, 2, 1,
                            2, 1, 1
                        };
            var item = 3;
            double expected = 0.3;
            double actual = input.Probability(item);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ProbabilityTest2() {
            IEnumerable<string> input;
            string item;
            double expected;
            double actual;

            input = new string[]
                    {
                        "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c",
                        "d", "a", "b", "c", "d", "a", "b", "c", "d"
                    };
            item = "d";
            expected = 0.25;
            actual = input.Probability(item);
            Assert.AreEqual(expected, actual);

            //------------------------------//

            input = new string[]
                    {
                        "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c",
                        "d", "A", "B", "C", "D", "A", "B", "C", "D"
                    };
            item = "d";
            expected = 0.1875;
            actual = input.Probability(item);
            Assert.AreEqual(expected, actual);

            //------------------------------//

            input = new string[]
                    {
                        "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c", "d", "a", "b", "c",
                        "d", "A", "B", "C", "D", "A", "B", "C", "D"
                    };
            item = "d";
            expected = 0.25;

            actual = input.Probability(item, StringComparer.InvariantCultureIgnoreCase);
            Assert.AreEqual(expected, actual);
        }

        private string BucketSelector(int value, IEnumerable<string> buckets) {
            switch(value) {
                case 1:
                    return "a";
                case 2:
                    return "b";
                case 3:
                    return "c";
                default:
                    return "Z1";
            }
        }
    }
}