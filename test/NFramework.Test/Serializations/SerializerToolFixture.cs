using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Silverlight.Testing;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Serializations {
    [Tag("Serializer")]
    [TestFixture]
    public class SerializerToolFixture : SerializerFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private static readonly BinaryFormatter BinaryFormatter = new BinaryFormatter();

        [Test]
        public void ValueTypeSerializeBinaryTest1776368509() {
            using(var ms = new ValueStream()) {
                var strs = new[] { "가", "나", "다", "라" };

                SerializerTool.Serialize(strs, ms, BinaryFormatter);
                // SerializerTool.BinarySerialize(strs, ms);

                ms.Position = 0;
                Console.WriteLine("Original Strings  = " + ArrayTool.AsString(strs));
                Console.WriteLine("Binary Serialized =\r\n" + ms.GetHexDumpString());

                ms.Position = 0;
                // string[] strs2 = (string[])SerializerTool.BinaryDeserialize(ms);
                var strs2 = SerializerTool.Deserialize<string[]>(ms, BinaryFormatter);
                log.Debug("Deserialized      = " + ArrayTool.AsString(strs2));

                Assert.AreEqual(strs, strs2);
            }
        }

        [Test]
        public void ReferenceTypeSerializeBinaryTest477378791() {
            using(var ms = new ValueStream()) {
                var objs = new List<object>();

                objs.Add(new Uri("http://localhost"));
                objs.Add(new Rectangle(0, 0, 100, 100));

                // BinaryTree<int> btree = new BinaryTree<int>();
                // btree.AddRange(1, 2, 3, 4, 5, 6);
                // objs.Add(btree);

                // SerializerTool.BinarySerialize(objs, ms);
                SerializerTool.Serialize(objs, ms, BinaryFormatter);

                ms.Position = 0;
                Console.WriteLine("Original Objects  = " + ArrayTool.AsString(objs.ToArray()));
                Console.WriteLine("Binary Serialized =\r\n" + ms.GetHexDumpString());

                ms.Position = 0;
                // List<object> objs2 = (List<object>)SerializerTool.BinaryDeserialize(ms);
                var objs2 = SerializerTool.Deserialize<List<object>>(ms, BinaryFormatter);

                Console.WriteLine("Deserialized      = " + ArrayTool.AsString(objs2.ToArray()));

                for(int i = 0; i < objs.Count; i++) {
                    Assert.AreEqual(objs[i], objs2[i]);
                }
            }
        }

        [Test]
        public void TestOfReferenceTypeClone90882378() {
            var objs = new List<object>();

            objs.Add(new Uri("http://localhost"));
            objs.Add(new Rectangle(0, 0, 100, 100));

            //BinaryTree<int> btree = new RwBinaryTree<int>();
            //btree.AddRange(1, 2, 3, 4, 5, 6);
            //objs.Add(btree);

            Console.WriteLine("Original Object List = " + ArrayTool.AsString(objs.ToArray()));

            var objs2 = SerializerTool.DeepCopy(objs);

            if(objs2 == null)
                throw new NullReferenceException("DeepCopy failed");

            Console.WriteLine("Cloned Object List   = " + ArrayTool.AsString(objs2.ToArray()));

            for(int i = 0; i < objs.Count; i++) {
                Assert.AreEqual(objs[i], objs2[i]);
            }

            // RwBinaryTree<int> btree2 = (RwBinaryTree<int>)objs[2];
            // Console.WriteLine("Cloned Object List[2]= " + btree2.ToString());

            // Assert.AreEqual(btree, btree2);
        }

        [Test]
        public void Serialize_Deserialize() {
            var original = UserInfo.GetSample();

            foreach(var option in Options) {
                var serializer = SerializerTool.CreateSerializer<UserInfo>(option);
                serializer.Should().Not.Be.Null();

                var serialized = serializer.Serialize(original);
                var deserialized = serializer.Deserialize(serialized);

                serialized.Should().Not.Be.Null();
                deserialized.Should().Not.Be.Null();

                VerifyObject(original, deserialized);
            }
        }

        private static void VerifyObject(UserInfo original, UserInfo clone) {
            original.Should().Not.Be.Null();
            clone.Should().Not.Be.Null();

            Assert.AreEqual(original.Description, clone.Description);
            Assert.AreEqual(original.FavoriteMovies.Count, clone.FavoriteMovies.Count);

            using(var originIter = original.FavoriteMovies.GetEnumerator())
            using(var cloneIter = clone.FavoriteMovies.GetEnumerator())
                while(originIter.MoveNext() && cloneIter.MoveNext()) {
                    Assert.AreEqual(originIter.Current, cloneIter.Current);
                }
        }
    }
}