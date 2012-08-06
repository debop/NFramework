using System;
using System.Linq;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.LinqEx {
    [TestFixture]
    public class LinqToolFixture_Partitioning {
        [Test]
        public void DefaultPartitioningArrayTest() {
            foreach(var value in Enum.GetValues(typeof(PartitioningMethod))) {
                var method = (PartitioningMethod)value;
                var array = LinqTool.Partitioning(method);

                array.SequenceEqual(LinqTool.DefaultPartitioningArray[method]).Should().Be.True();
            }
        }

        [TestCase(10, 100)]
        [TestCase(10, 1000)]
        [TestCase(100, 1000)]
        public void PartitioningArrayTest(int partitionCount, double totalAmount) {
            foreach(var value in Enum.GetValues(typeof(PartitioningMethod))) {
                var method = (PartitioningMethod)value;
                var array = LinqTool.Partitioning(method, partitionCount, totalAmount);

                Console.WriteLine("array=" + array.CollectionToString());
            }
        }
    }
}