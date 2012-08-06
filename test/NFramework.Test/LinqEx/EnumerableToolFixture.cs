using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.LinqEx.Collections {
    [Microsoft.Silverlight.Testing.Tag("Linq")]
    [TestFixture]
    public class EnumerableToolFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static IList<T> GetSerialList<T>(T seed, T step, int count) where T : struct, IComparable<T> {
            return EnumerableTool.SerialSequence(seed, step, count).ToList();
        }

        [Test]
        public void Aggreagation_Sum() {
            var list = Enumerable.Repeat(2.0, 5);

            var sum = list.Aggregate(0.0, (result, x) => result + x);
            Assert.AreEqual(10.0, sum);
        }

        [Test]
        public void IsEmpty_Test() {
            var list = new List<double>();
            var sequence = list.AsEnumerable();
            Assert.IsTrue(sequence.IsEmptySequence());
            Assert.IsFalse(sequence.ItemExists());

            list.Add(5.0);
            sequence = list.AsEnumerable();
            Assert.IsFalse(sequence.IsEmptySequence());
            Assert.IsTrue(sequence.ItemExists());

            list.Add(10.0);
            sequence = list.AsEnumerable();
            Assert.IsFalse(sequence.IsEmptySequence(x => x > 5.0));
            Assert.IsTrue(sequence.ItemExists(x => x > 5.0));
        }

        [Test]
        public void Exists_Test() {
            IList<double> list = new List<double>();

            Assert.IsFalse(list.ItemExists());
            Assert.IsFalse(list.AsEnumerable().ItemExists());

            list.Add(5.0);
            Assert.IsTrue(list.ItemExists());
            Assert.IsTrue(list.AsEnumerable().ItemExists());

            list.Add(10.0);

            Assert.IsTrue(list.ItemExists(x => x > 5.0));
            Assert.IsTrue(list.AsEnumerable().ItemExists(x => x > 5.0));
        }

        [Test]
        public void All_Any_Predicate() {
            var serial = GetSerialList(0.0, 1.0, 10);

            Assert.IsTrue(serial.All(x => x >= 0.0));
            Assert.IsTrue(serial.Any(x => x >= 5.0));

            Assert.IsFalse(serial.All(x => x >= 5.0));
            Assert.IsFalse(serial.Any(x => x < 0.0));
        }

        [TestCase(1, 2, 3)]
        public void Convert_Enumerable(int a, int b, int c) {
            var list = new[] { a, b, c };
            var converted = list.ConvertUnsafe<double>().ToList();

            Console.WriteLine("Convert from int To double : " + converted.CollectionToString());

            Assert.AreEqual(c, converted.ElementAt(2));
        }

        [TestCase(2, 1, 2, 3)]
        public void Repeat_Enumerable(int count, int a, int b, int c) {
            var list = new[] { a, b, c };
            var repeat = list.RepeatSequenceUnsafe<double>(count).ToList();

            // Console.WriteLine("Repeat : " + repeat.CollectionToString());

            Assert.AreEqual(count * 3, repeat.Count());

            for(int i = 0; i < count; i++) {
                Assert.AreEqual(1, repeat.ElementAt(i * 3));
                Assert.AreEqual(2, repeat.ElementAt(i * 3 + 1));
                Assert.AreEqual(3, repeat.ElementAt(i * 3 + 2));
            }
        }

        [TestCase(2, 1, 2, 3)]
        public void Repeat_GenericEnumerable(int count, int a, int b, int c) {
            var list = new[] { a, b, c };
            var doubles = list.ConvertUnsafe<double>();
            var repeat = doubles.RepeatSequence(count).ToList();

            // Console.WriteLine("Repeat : " + repeat.CollectionToString());

            Assert.AreEqual(count * 3, repeat.Count());

            for(int i = 0; i < count; i++) {
                Assert.AreEqual(1, repeat.ElementAt(i * 3));
                Assert.AreEqual(2, repeat.ElementAt(i * 3 + 1));
                Assert.AreEqual(3, repeat.ElementAt(i * 3 + 2));
            }
        }

        [TestCase(0, 1, 10)]
        [TestCase(100, 0.1, 10)]
        public void Serial_Double(double seed, double step, int count) {
            var serial = EnumerableTool.SerialSequence(seed, step, count);

            Assert.AreEqual(count, serial.Count());
            // Console.WriteLine("Serials: " + serial.ToList().CollectionToString());
        }

        [TestCase(0, 1, 10)]
        [TestCase(100, 1, 10)]
        public void Serial_Int(int seed, int step, int count) {
            var serial = EnumerableTool.SerialSequence<double>(seed, step, count);

            Assert.AreEqual(count, serial.Count());
            // Console.WriteLine("Serials: " + serial.ToList().CollectionToString());
        }

        [TestCase(5, 0, 100, true)]
        [TestCase(-200, -100, 100, false)]
        public void Between_Double(double value, double min, double max, bool isBetween) {
            Assert.AreEqual(isBetween, value.Between(min, max));
        }

        [Test]
        public void ToDictionary_Test() {
            var serial = GetSerialList(0, 1, 10);
            var pairList = serial.Select(x => new KeyValuePair<int, int>(x, x * 2)).ToList();
            var dictionary = pairList.ToDictionary();

            Assert.AreEqual(10, dictionary.Count());
            // Console.WriteLine("Dictionary : " + dictionary.DictionaryToString());
        }

#if !SILVERLIGHT
        [Test]
        public void ToBindingList_FromEnumerable() {
            var serial = EnumerableTool.SerialSequence(0, 1, 10);
            var binding = serial.ToBindingList();

            Assert.IsTrue(binding.Count > 0);
            Assert.AreEqual(9, binding.Last());

            // Console.WriteLine("BindingList:" + binding.CollectionToString());
        }
#endif

        [Test]
        public void ForEachTest() {
            int count = 0;
            int[] arr = { 1, 2, 3, 4, 5 };

            arr.RunEach(delegate(int i) {
                            count++;
                            Assert.AreEqual(count, i);
                            Console.WriteLine("Count:{0}, i:{1}", count, i);
                        });

            Assert.AreEqual(5, count);
        }

        [Test]
        [TestCase(100)]
        [TestCase(1000)]
        public void IndexOfTest(int count) {
            IList<int> list = new List<int>(count);
            for(int i = 0; i < count; i++)
                list.Add(i);

            for(int i = 0; i < count; i++) {
                var value = Rnd.Next(count - 1);
                Assert.AreEqual(value, list.IndexOf(value));
            }
        }

        [Test]
        [TestCase(100)]
        [TestCase(1000)]
        public void LastIndexOfTest(int count) {
            IList<int> list = new List<int>(count);
            for(int i = 0; i < count; i++)
                list.Add(i);

            for(int i = 0; i < count; i++) {
                var value = Rnd.Next(count - 1);
                Assert.AreEqual(value, list.LastIndexOf(value));
            }
        }

        [Test]
        public void FirstTest() {
            int[] arr = { 1, 2, 3, 4, 5 };
            Assert.AreEqual(1, arr.First());
            Assert.AreEqual(5, arr.Last());
        }

        [Test]
        public void FirstTestWithNotList() {
            var dic = new Dictionary<int, int>
                      {
                          { 1, 2 },
                          { 2, 4 }
                      };

            var firstItem = new KeyValuePair<int, int>(1, 2);

            Assert.AreEqual(firstItem, dic.First());

            var lastItem = new KeyValuePair<int, int>(2, 4);
            Assert.AreEqual(lastItem, dic.Last());
        }

        [Test]
        public void Map_WillTransfromAllInput_ToOutput() {
            var map = new[] { "1", "2", "3" }.Select(x => int.Parse(x));
            AssertCollectionEquals(new[] { 1, 2, 3 }, new List<int>(map));
        }

        [Test]
        public void Reduce_WillAllowToWrite_Sum() {
            var list = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }.ToList();
            int total = list.Aggregate(0, (i, sum) => i + sum);
            Assert.AreEqual(45, total);

            //int total = CollectionEx.Aggregate(
            //    CollectionEx.ToList(1, 2, 3, 4, 5, 6, 7, 8, 9),
            //    0,
            //    delegate(int i, int sum) { return i + sum; });

            //Assert.AreEqual(45, total);
        }

        [Test]
        public void FindAllOdds() {
            int[] arr = { 1, 2, 3, 4, 5, 6, 7, 8 };
            int[] expected = { 1, 3, 5, 7 };

            var odds = arr.Where(item => item % 2 != 0);
            AssertCollectionEquals(expected, odds.ToList());

            //IEnumerable<int> odds = CollectionEx.FindAll(arr,
            //                                             delegate(int i) { return i%2 != 0; });
            //AssertCollectionEquals(expected, new List<int>(odds));
        }

        [Test]
        public void FindAllNotOdds() {
            int[] arr = { 1, 2, 3, 4, 5, 6, 7, 8 };
            int[] expected = { 2, 4, 6, 8 };

            var notOdds = arr.Where(item => item % 2 == 0);
            AssertCollectionEquals(expected, notOdds.ToList());
        }

        [Test]
        public void ToUniqueCollection_EmptyCollectionReturnsNewEmptyCollection() {
            var input = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };
            var actual = input.Distinct().ToList();

            Assert.AreNotSame(actual, input);
            Assert.AreEqual(input.Count, actual.Count);
        }

        [Test]
        public void ToUniqueCollection_NullCollectionRaiseException() {
            Assert.Throws<ArgumentNullException>(() => ((IEnumerable<int>)null).Distinct().ToList());
        }

        [Test]
        public void ToUniqueCollection_ReturnsGenericListImplementation() {
            ICollection<int> input = new List<int>();
            Assert.IsInstanceOf<List<int>>(input.Distinct().ToList());
        }

        [Test]
        public void ToUniqueCollection_WillRemoveDuplicatePrimatives() {
            var input = ListContaining(1, 2, 2);
            AssertCollectionEquals(ListContaining(1, 2), input.Distinct().ToList());
        }

        [Test]
        public void ToUniqueCollection_WillRemoveDuplicatePrimativesStoredAsObjects() {
            object o1 = 1;
            object o2 = 1;
            var input = ListContaining(o1, o2);
            AssertCollectionEquals(ListContaining(o1), input.Distinct().ToList());
        }

        [Test]
        public void ToUniqueCollection_WillRemoveDuplicateStringsStoredAsObjects() {
            var a = new string(new[] { 'h', 'e', 'l', 'l', 'o' });
            var b = new string(new[] { 'h', 'e', 'l', 'l', 'o' });

            var c = a;
            var d = b;

            var input = ListContaining(c, d);

            AssertCollectionEquals(ListContaining(c), input.Distinct().ToList());
        }

        [Test]
        public void ToUniqueCollection_WillRemoveDuplicateReferenceTypes() {
            var obj = new List<int>();
            var input = ListContaining(obj, obj);

            AssertCollectionEquals(ListContaining(obj), input.Distinct().ToList());
        }

        [Test]
        public void Containing_WillRetunNullWhenPassedNull() {
            ICollection<object> actual = new List<object>();
            Assert.AreEqual(actual.Count, 0);
        }

        [Test]
        public void Containing_WillRetunEmptyCollectionWhenPassedNothing() {
            ICollection<int> actual = new List<int>();
            Assert.IsEmpty((ICollection)actual);
        }

        [Test]
        public void Containing_WillRetunCollectionContainingArgumentsSupplied() {
            var expected = new List<int>();
            expected.AddRange(new int[] { 1, 2, 3 });
            AssertCollectionEquals(expected, EnumerableTool.ToList(1, 2, 3));
        }

        [Test]
        public void ListContaining_WillRetunNullWhenPassedNull() {
            ICollection<object> actual = new List<object>();
            Assert.AreEqual(actual.Count, 0);
        }

        [Test]
        public void ListContaining_WillRetunEmptyListWhenPassedNothing() {
            ICollection<int> actual = new List<int>();
            Assert.IsEmpty((ICollection)actual);
        }

        [Test]
        public void ListContaining_WillRetunListContainingArgumentsSupplied() {
            var expected = new List<int>();
            expected.AddRange(new[] { 1, 2, 3 });
            AssertCollectionEquals(expected, EnumerableTool.ToList(1, 2, 3));
        }

        private static ICollection<T> ListContaining<T>(params T[] items) {
            return new List<T>(items);
        }

        private static void AssertCollectionEquals<T>(ICollection<T> expected, ICollection<T> actual) {
            Assert.AreEqual(expected.Count, actual.Count);
            var expectedEnum = expected.GetEnumerator();
            var actualEnum = actual.GetEnumerator();
            while(expectedEnum.MoveNext()) {
                actualEnum.MoveNext(); //same size, don't need to check it
                Assert.AreEqual(expectedEnum.Current, actualEnum.Current);
            }
        }

        [Test]
        public void AsJoinedText() {
            int[] numbers = new int[] { 1, 2, 3, 4, 5 };
            const string expected = @"1|2|3|4|5";

            var joinedText = numbers.AsJoinedText("|"); // joinedText is "1|2|3|4|5"

            Assert.AreEqual(expected, joinedText);
        }
    }
}