using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.LinqTools {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class LinqToolFixture_Generate {
        [Test]
        public void SequenceGeneratorTest1() {
            var expected = new TestPet[] { new TestPet(), new TestPet() };
            var actual = LinqTool.Generate(2, () => (new TestPet()));

            CollectionAssert.AreEqual(expected, actual);
            //Assert.IsTrue(expected.SequenceRelation(actual) == SequenceRelationType.Equal);

            //------------------------------//

            expected = new TestPet[] { new TestPet { Name = "Daisy", Type = "Dog" }, new TestPet { Name = "Tux", Type = "Cat" } };
            actual = LinqTool.Generate(1, 2, (x) => (new TestPet(x)));
            CollectionAssert.AreEqual(expected, actual);
            //Assert.IsTrue(expected.SequenceRelation(actual, new PetComparer()) == SequenceRelationType.Equal && expected.SequenceRelation(actual) == SequenceRelationType.Equal);
        }

        [Test]
        public void SequenceGeneratorTest2() {
            var expected = new decimal[] { .03m, .04m, .05m, .06m, .07m, .08m, .09m };
            const decimal factor = .01m;
            var actual = LinqTool.Generate(3, 7, (x) => x * factor);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest3() {
            var expected = new int[] { 9, 16, 25, 36, 49, 64, 81 };
            var actual = LinqTool.Generate(3, 7, (x) => x * x);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            expected = new int[] { 3, 6, 9, 12, 15, 18, 21 };
            actual = LinqTool.Generate<int>(3, 7, (x, y) => x + x * y);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            expected = new int[] { 1, 3, 5, 7, 9, 11 };
            actual = LinqTool.Generate(1, 6, 2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest4() {
            IEnumerable<int> expected = new int[] { 1, 4, 9, 16, 25, 36, 49, 64, 81 };
            IEnumerable<int> actual = LinqTool.Generate(1, 9, (x) => x * x);
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            expected = new int[] { 10, 11, 13, 16, 20, 25, 31 };
            actual = LinqTool.Generate(10, 7, .5, (x, y, z) => (int)(x + y + z * 2));
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest5() {
            IEnumerable<double> expected = new double[] { 1, 3, 5, 7, 9, 11 };
            IEnumerable<double> actual = LinqTool.Generate(1d, 6, 2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest6() {
            IEnumerable<float> expected = new float[] { 1, 3, 5, 7, 9, 11 };
            IEnumerable<float> actual = LinqTool.Generate(1f, 6, 2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest7() {
            IEnumerable<decimal> expected = new decimal[] { 1, 3, 5, 7, 9, 11 };
            IEnumerable<decimal> actual = LinqTool.Generate(1m, 6, 2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest8() {
            IEnumerable<long> expected = new long[] { 1, 3, 5, 7, 9, 11 };
            IEnumerable<long> actual = LinqTool.Generate(1L, 6, 2);
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest9() {
            double angleX = Math.PI * 45 / 180.0;
            IEnumerable<double> expected = new double[]
                                           {
                                               Math.Sin(angleX + .000), Math.Sin(angleX + .004), Math.Sin(angleX + .008),
                                               Math.Sin(angleX + .012), Math.Sin(angleX + .016), Math.Sin(angleX + .020),
                                               Math.Sin(angleX + .024), Math.Sin(angleX + .028), Math.Sin(angleX + .032),
                                               Math.Sin(angleX + .036), Math.Sin(angleX + .040), Math.Sin(angleX + .044)
                                           };
            IEnumerable<double> actual = LinqTool.Generate<double>(angleX, .004d, 12, (X) => Math.Sin(X));
            Assert.IsTrue(expected.SequenceEqual(actual));
        }

        [Test]
        public void SequenceGeneratorTest10() {
            try {
                var source = new bool[] { true, true, true, true };
                var actual = LinqTool.Generate(false, 3, true).ToArray();
                Assert.Fail();
            }
            catch(InvalidOperationException) {
                //Assert.IsTrue(e.Message == "Generate<T> cannot be invoked using 'Boolean' type, only numeric values are valid.");
            }
            catch(Exception) {
                Assert.Fail();
            }
        }

        [Test]
        public void SequenceGeneratorTest11() {
            IEnumerable<DateTime> expected = new DateTime[]
                                             {
                                                 new DateTime(2011, 1, 1, 0, 0, 0),
                                                 new DateTime(2011, 1, 2, 0, 0, 0),
                                                 new DateTime(2011, 1, 3, 0, 0, 0),
                                                 new DateTime(2011, 1, 4, 0, 0, 0),
                                                 new DateTime(2011, 1, 5, 0, 0, 0),
                                                 new DateTime(2011, 1, 6, 0, 0, 0)
                                             };
            IEnumerable<DateTime> actual = LinqTool.Generate(new DateTime(2011, 1, 1, 0, 0, 0), 6, new TimeSpan(24, 0, 0));
            Assert.IsTrue(expected.SequenceEqual(actual));

            //------------------------------//

            try {
                DateTime[] actualRes = LinqTool.Generate(new DateTime(2011, 1, 1, 0, 0, 0), 6, TimeSpan.Zero).ToArray();
                Assert.IsTrue(expected.SequenceEqual(actual));
            }
            catch(ArgumentOutOfRangeException) {}
            catch(InvalidOperationException) {}
            catch(Exception) {
                Assert.Fail();
            }
        }
    }
}