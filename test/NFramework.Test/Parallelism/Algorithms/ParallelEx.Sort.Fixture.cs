using System;
using System.Collections.Generic;
using NSoft.NFramework.Parallelism.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Algorithms {
    [TestFixture]
    public class ParallelTool_Sort_Fixture : AbstractFixture {
        private const int DataCount = 80000;
        private const int MaxValue = DataCount * 5;

        private int[] data;

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            data = new int[DataCount];

            for(var i = 0; i < data.Length; i++)
                data[i] = Rnd.Next(MaxValue);
        }

        [Test]
        public void SortTest() {
            var array = (int[])data.Clone();
            ParallelTool.Sort(array);

            AssertSorted(array);
        }

        private static void AssertSorted<T>(IList<T> array) where T : IComparable {
            for(var i = 1; i < array.Count; i++) {
                Assert.GreaterOrEqual(array[i], array[i - 1], "[{0}] >= [{1}]", array[i], array[i - 1]);
            }
        }
    }
}