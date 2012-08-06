using System;
using System.Text;
using NSoft.NFramework.IO;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class ArrayToolFixture {
        [Test]
        public void CompareTest() {
            const int Length = 10;

            var array1 = new string[Length];
            var array2 = new string[Length];

            for(int i = 0; i < Length; i++)
                array1[i] = i.ToString();

            for(int i = 0; i < Length; i++)
                array2[i] = i.ToString();

            Assert.IsTrue(ArrayTool.Compare(array1, array2));
        }

        [Test]
        public void EnsureCapacityTest() {
            const int Length = 10;

            var array = new int[Length];

            ArrayTool.InitArray(array);
            ArrayTool.EnsureCapacity(ref array, 100);

            Console.WriteLine("Array length=" + array.Length);

            Assert.AreEqual(100, array.Length);
        }

        [Test]
        public void CombineTest() {
            const int Length = 10;

            var array1 = new int[Length];
            var array2 = new int[Length];

            for(int i = 0; i < Length; i++)
                array1[i] = i;

            for(int i = 0; i < Length; i++)
                array2[i] = i;

            var array3 = ArrayTool.Combine(array1, array2);

            Assert.AreEqual(Length * 2, array3.Length);
        }

        //[Test]
        //public void GetRandomBytesTest()
        //{
        //    byte[] bytes = ArrayTool.GetRandomBytes(10);
        //    string hexStr = ArrayTool.GetHexStringFromBytes(bytes);

        //    byte[] bytes2 = ArrayTool.GetBytesFromHexString(hexStr);
        //    string hexStr2 = ArrayTool.GetHexStringFromBytes(bytes2);

        //    Console.WriteLine("hex string :" + hexStr);
        //    Console.WriteLine("hex string2:" + hexStr);

        //    Assert.AreEqual(hexStr, hexStr2);
        //}

        [Test]
        public void SortTest() {
            var array = new int[10];

            for(int i = 0; i < array.Length; i++)
                array[i] = array.Length - i;

            Console.WriteLine("Original: " + array.CollectionToString());

            Array.Sort(array);

            Console.WriteLine("Sorted : " + array.CollectionToString());
        }

        //[Test]
        //public void ByteToStringTest()
        //{
        //    string s = "abcdefg";
        //    byte[] bytes = Encoding.Default.GetBytes(s);
        //    Console.Write("BitConverter:" + BitConverter.ToString(bytes));

        //    Console.Write("ArrayTool.Get:" + ArrayTool.GetHexStringFromBytes(bytes));
        //}

        [Test]
        public void CompareStringTest() {
            const string s1 = @"동해물과 백두산이";
            const string s2 = @"마르고 닳도록";

            Assert.IsFalse(s1.CompareString(s2, Encoding.Default));
            Assert.IsTrue(s1.CompareString(s1, null));
            Assert.IsTrue(s2.CompareString(s2, null));
        }

        [Test]
        public void CompareStreamTest() {
            var file1 = @"UnitTest_Files\bookmark.htm";
            var file2 = @"UnitTest_Files\bookmark2.htm";

            // Console.WriteLine(FileTool.GetCurrentPath());
            Assert.IsTrue(file1.FileExists(), "File not Exist: {0}", file1);
            Assert.IsTrue(file2.FileExists(), "File not Exist: {0}", file2);

            using(var bs1 = FileTool.GetBufferedFileStream(file1, FileOpenMode.Read))
            using(var bs2 = FileTool.GetBufferedFileStream(file2, FileOpenMode.Read)) {
                Assert.IsFalse(bs1.CompareStream(bs2));
            }

            using(var bs1 = FileTool.GetBufferedFileStream(file1, FileOpenMode.Read))
            using(var bs2 = FileTool.GetBufferedFileStream(file1, FileOpenMode.Read)) {
                Assert.IsTrue(bs1.CompareStream(bs2));
            }

            using(var bs1 = FileTool.GetBufferedFileStream(file2, FileOpenMode.Read))
            using(var bs2 = FileTool.GetBufferedFileStream(file2, FileOpenMode.Read)) {
                Assert.IsTrue(bs1.CompareStream(bs2));
            }
        }
    }
}