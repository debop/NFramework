using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NSoft.NFramework.TimePeriods;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Hash Code 생성 테스트
    /// </summary>
    [TestFixture]
    public class HashToolFixture {
        #region << logger >>

        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void ValueType_Combine_ViseVersa(int x, int y) {
            int hash1 = HashTool.Compute(x, y);
            int hash2 = HashTool.Compute(y, x);

            if(x != y)
                Assert.AreNotEqual(hash1, hash2, "HashCode 값의 순서에 따라 HashCode 값이 달라야 합니다.");
            else
                Assert.AreEqual(hash1, hash2);
        }

        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(1, 2)]
        public void ValueType_CombineInOrder_Test(int x, int y) {
            int hash1 = HashTool.Compute(x, y);
            int hash2 = HashTool.Compute(y, x);


            if(x == y)
                hash1.Should().Be(hash2);
            if(x != y)
                hash1.Should().Not.Be.EqualTo(hash2);

            //Assert.IsTrue(hash1 != hash2, "hash1={0}, hash2={1}", hash1, hash2);
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 1, 2)]
        [TestCase(1, 2, 3, 4)]
        [TestCase(4, 3, 2, 1)]
        [TestCase(1, 1, 2, 2)]
        [TestCase(3, 3, 4, 4)]
        public void Params_Combine_Test(int x, int y, int z, int k) {
            var codes = new[] { x, y, z, k };
            var codesReverse = new[] { k, z, y, x };

            var hash1 = HashTool.Compute(codes);
            var hash2 = HashTool.Compute(codesReverse);

            if(ArrayTool.Compare(codes, codesReverse))
                Assert.AreEqual(hash1, hash2, "두 Array의 값이 같다면, HashCode 값도 같아야 합니다.");
            else
                Assert.AreNotEqual(hash1, hash2, "HashTool.Compute은 입력 값의 순서에 따라서도 HashCode 값이 달라야 합니다.");
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(1, 1, 1, 1)]
        [TestCase(1, 2, 1, 2)]
        [TestCase(1, 2, 3, 4)]
        [TestCase(4, 3, 2, 1)]
        [TestCase(1, 1, 2, 2)]
        [TestCase(3, 3, 4, 4)]
        public void Params_CombineInOrder_Test(int x, int y, int z, int k) {
            var codes = new[] { x, y, z, k };
            var codesReverse = codes.Reverse().ToArray();

            int hash1 = HashTool.Compute(codes);
            int hash2 = HashTool.Compute(codesReverse);

            if(ArrayTool.Compare(codes, codesReverse))
                Assert.AreEqual(hash1, hash2, "hash1={0}, hash2={1}", hash1, hash2);
            else
                Assert.AreNotEqual(hash1, hash2, "hash1={0}, hash2={1}", hash1, hash2);
        }

        internal class Entity {
            internal Entity(Guid id, string code) {
                Id = id;
                Code = code;
            }

            public Guid Id { get; set; }

            public string Code { get; set; }

            public override int GetHashCode() {
                return HashTool.Compute(Id, Code);
            }

            public override string ToString() {
                return string.Format("Entity# Id={0}, Code={1}, HashCode={2}", Id, Code, GetHashCode());
            }
        }

        /// <summary>
        /// DateRange 처럼, 서로 같은 값일 경우, 순서에 따라 다른 값이 나와야 한다.
        /// </summary>
        [Test]
        public void DateRange_HashCode_Test() {
            DateTime now = DateTime.Now;

            var ranges = new[]
                         {
                             new TimeRange(null, null),
                             new TimeRange(null, now),
                             new TimeRange(now, null),
                             new TimeRange(now, now)
                         };

            foreach(var range in ranges) {
                var hash1 = HashTool.Compute(range.Start, range.End);
                var hash2 = HashTool.Compute(range.End, range.Start);

                Console.WriteLine("Range=[{0}], hash1=[{1}], hash2=[{2}]", range, hash1, hash2);

                if(Equals(range.Start, range.End) == false) {
                    Assert.AreNotEqual(hash1, hash2);
                }
                else {
                    Assert.AreEqual(hash1, hash2);
                    Assert.AreNotEqual(0, hash1, "TimeRange=" + range);
                }
            }
        }

        [Test]
        public void Guid_HashCode() {
            const int Count = 100;

            var guids = Enumerable.Range(0, Count).Select(i => Guid.NewGuid());
            var codes = Enumerable.Range(0, 50 * Count).Select(i => "CODE_ITEM_CODE_" + i.ToString("X4")).ToList();

            var entities = new HashSet<Entity>();

            foreach(var guid in guids) {
                foreach(string code in codes)
                    entities.Add(new Entity(guid, code))
                        .Should().Be.True();
            }
        }

        [Test]
        public void String_HashCode() {
            const int Count = 1000;
            const string productName = "NSoft.NFramework";

            var productHashCode = HashTool.Compute(productName);
            var codes = Enumerable.Range(0, Count).Select(i => "CODE_ITEM_CODE_" + i.ToString("X4")).ToList();

            var hashList = new HashSet<int>();

            foreach(var code in codes)
                hashList.Add(HashTool.Compute(productHashCode, HashTool.Compute(code))).Should().Be.True();

            Assert.AreEqual(Count, hashList.Count);
        }

        [TestCase(null, null)]
        [TestCase(0, 0)]
        [TestCase(null, 0)]
        [TestCase(null, 1)]
        public void NullableType_CombineInOrder_Test(int? x, int? y) {
            var hash1 = HashTool.Compute(x.GetValueOrDefault(), y.GetValueOrDefault());
            var hash2 = HashTool.Compute(y.GetValueOrDefault(), x.GetValueOrDefault());


            if(x.GetValueOrDefault() == y.GetValueOrDefault())
                Assert.AreEqual(hash1, hash2);
            else
                Assert.AreNotEqual(hash1, hash2);
        }

        [TestCase(null, null)]
        [TestCase(0, 0)]
        [TestCase(null, 0)]
        [TestCase(null, 1)]
        [TestCase(1, null)]
        [TestCase(0, null)]
        public void NullableType_Combine_Test(int? x, int? y) {
            var hash1 = HashTool.Compute(x.GetValueOrDefault(), y.GetValueOrDefault());
            var hash2 = HashTool.Compute(y.GetValueOrDefault(), x.GetValueOrDefault());

            if(x.GetValueOrDefault() != y.GetValueOrDefault())
                Assert.AreNotEqual(hash1, hash2);
            else
                Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void Object_Expression_GetHashCode() {
            var entity = new Entity(Guid.NewGuid(), DateTime.Now.ToString());

            entity.CalcHash(e => e.Id, e => e.Code).Should().Be(entity.GetHashCode());
            entity.CalcHash().Should().Be(entity.GetHashCode());

            entity.CalcHash(e => e.Id).Should().Not.Be.EqualTo(entity.GetHashCode());
            entity.CalcHash(e => e.Code, e => e.Id).Should().Not.Be.EqualTo(entity.GetHashCode());
        }

        [Test]
        public void Object_CalcHashCode() {
            Entity obj = null;

            obj.CalcHash().Should().Be(0);

            obj = new Entity(Guid.NewGuid(), "aaa");
            obj.CalcHash().Should().Not.Be(0);
        }

        [Test]
        public void String_GetHashCode() {
            string str = null;
            str.CalcHash().Should().Be(string.Empty.GetHashCode());

            str = string.Empty;
            str.CalcHash().Should().Be(string.Empty.GetHashCode());

            str = " ";
            str.CalcHash().Should().Not.Be.EqualTo(string.Empty.GetHashCode());

            str = "\t";
            str.CalcHash().Should().Not.Be.EqualTo(string.Empty.GetHashCode());
        }

        [TestCase(null, null)]
        [TestCase(0, 0)]
        [TestCase(null, 0)]
        [TestCase(null, 1)]
        [TestCase(1, null)]
        [TestCase(0, null)]
        public void NullableType_Compute_Test(int? x, int? y) {
            var hash1 = HashTool.Compute(x, y);
            var hash2 = HashTool.Compute(y, x);

            if(x.GetValueOrDefault() != y.GetValueOrDefault())
                Assert.AreNotEqual(hash1, hash2);
            else
                Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void HashCode_Compute() {
            HashToolFixture.Entity entity = null;
            HashTool.Compute(entity).Should().Be(0);

            entity = new HashToolFixture.Entity(Guid.NewGuid(), "abc");
            HashTool.Compute(entity).Should().Not.Be.EqualTo(0).And.Be(entity.GetHashCode());
        }

        public void HashCode_Compute_Objects() {
            var now = DateTime.Now;

            var ranges = new[]
                         {
                             new TimeRange(null, null),
                             new TimeRange(null, now),
                             new TimeRange(now, null),
                             new TimeRange(now, now)
                         };

            foreach(var range in ranges) {
                var hash1 = HashTool.Compute(range.Start, range.End);
                var hash2 = HashTool.Compute(range.End, range.Start);

                if(Equals(range.Start, range.End)) {
                    hash1.Should().Be(hash2);
                    hash1.Should().Not.Be(0);
                }
                else
                    hash1.Should().Not.Be.EqualTo(hash2);
            }
        }
    }
}