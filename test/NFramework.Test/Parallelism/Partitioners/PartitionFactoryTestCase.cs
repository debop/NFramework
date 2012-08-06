using System;
using System.Linq;
using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Partitioners {
    [TestFixture]
    public class PartitionFactoryTestCase {
        [Test]
        [TestCase(0, 1, 1)]
        [TestCase(0, 2, 1)]
        [TestCase(0, 100, 2)]
        [TestCase(0, 150, 2)]
        public void Can_CreateRangePartition_Int32(int fromInclusive, int toExclusive, int expectedRangeCount) {
            var partition = PartitionerTool.CreateRangePartition(fromInclusive, toExclusive);

            Console.WriteLine();
            Console.WriteLine("@[{0} ~ {1}]", fromInclusive, toExclusive);
            partition.GetDynamicPartitions().RunEach(range => Console.WriteLine("Range [{0}~{1}]", range.Item1, range.Item2));

            var count = partition.GetDynamicPartitions().Count();

            var expected = (expectedRangeCount > 1) ? expectedRangeCount * Environment.ProcessorCount / 2 : expectedRangeCount;
            Assert.AreEqual(expected, count);
        }

        [Test]
        [TestCase(0, 1, 1)]
        [TestCase(0, 2, 1)]
        [TestCase(0, 100, 2)]
        [TestCase(0, 150, 2)]
        public void Can_CreateRangePartition_Int64(long fromInclusive, long toExclusive, int expectedRangeCount) {
            var partition = PartitionerTool.CreateRangePartition(fromInclusive, toExclusive);

            Console.WriteLine();
            Console.WriteLine("@[{0} ~ {1}]", fromInclusive, toExclusive);
            partition.GetDynamicPartitions().RunEach(range => Console.WriteLine("Range [{0}~{1}]", range.Item1, range.Item2));

            var count = partition.GetDynamicPartitions().Count();

            var expected = (expectedRangeCount > 1) ? expectedRangeCount * Environment.ProcessorCount / 2 : expectedRangeCount;
            Assert.AreEqual(expected, count);
        }
    }
}