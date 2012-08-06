using NSoft.NFramework.LinqEx;
using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class PascalSetFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private PascalSet _pascalSet1;
        private PascalSet _pascalSet2;
        private PascalSet _pascalSet3;

        [TestFixtureSetUp]
        public void Initialize() {
            var set = new PascalSet(0, 9);
            _pascalSet1 = set.Union(0, 1, 2, 3, 4, 5, 6, 7, 8, 9);
            _pascalSet2 = set.Union(1, 3, 5, 7, 9);
            _pascalSet3 = set.Union(0, 2, 4, 6, 8);
        }

        [Test]
        public void UnionTest() {
            var union = _pascalSet2.Union(_pascalSet3);
            Assert.IsTrue(_pascalSet1.IsSubset(union));

            union = _pascalSet2 | _pascalSet3;
            Assert.IsTrue(_pascalSet1.IsSubset(union));

            if(IsDebugEnabled)
                log.Debug(union.ToString());
        }

        [Test]
        public void IntersectTest() {
            var intersect = _pascalSet1.Intersection(_pascalSet3);
            Assert.IsTrue(intersect.Equals(_pascalSet3));

            intersect = _pascalSet1 & _pascalSet3;
            Assert.IsTrue(intersect.Equals(_pascalSet3));

            if(IsDebugEnabled)
                log.Debug(intersect.ToString());
        }

        [Test]
        public void DifferenceTest() {
            var diffSet = _pascalSet1.Difference(_pascalSet3);
            Assert.IsTrue(diffSet.Equals(_pascalSet2));

            diffSet = _pascalSet1 - _pascalSet3;
            Assert.IsTrue(diffSet.Equals(_pascalSet2));

            if(IsDebugEnabled)
                log.Debug(diffSet.ToString());
        }

        [Test]
        public void ExclusiveOrTest() {
            var set = _pascalSet1.Intersection(1, 2, 3, 4, 5);

            var xorSet = set.ExclusiveOr(_pascalSet3);
            Assert.IsTrue(xorSet.Count == 6);

            if(IsDebugEnabled)
                log.Debug("XOR : " + xorSet);

            xorSet = _pascalSet2 ^ _pascalSet2;
            Assert.IsTrue(xorSet.Count == 0);
        }

        [Test]
        public void ContainsTest() {
            int evenCount = 0;
            int oddCount = 0;

            var evenSet = _pascalSet3;
            var oddSet = _pascalSet2;

            for(int i = _pascalSet1.LowerBound; i <= _pascalSet1.UpperBound; i++) {
                if(evenSet.ContainsElement(i))
                    evenCount++;
                if(oddSet.ContainsElement(i))
                    oddCount++;
            }

            if(IsDebugEnabled)
                log.Debug("Even Count=[{0}]; Odd Count=[{1}]", evenCount, oddCount);

            Assert.IsTrue(evenCount == 5);
            Assert.IsTrue(oddCount == 5);
        }

        [Test]
        public void IsSubsetTest() {
            Assert.IsTrue(_pascalSet2.IsSubset(_pascalSet1));
            Assert.IsTrue(_pascalSet3.IsSubset(_pascalSet1));

            Assert.IsTrue(_pascalSet2.IsProperSubset(_pascalSet1));
            Assert.IsTrue(_pascalSet3.IsProperSubset(_pascalSet1));
        }

        [Test]
        public void IsSupersetTest() {
            Assert.IsTrue(_pascalSet1.IsSuperset(_pascalSet2));
            Assert.IsTrue(_pascalSet1.IsSuperset(_pascalSet3));

            Assert.IsTrue(_pascalSet1.IsProperSuperset(_pascalSet2));
            Assert.IsTrue(_pascalSet1.IsProperSuperset(_pascalSet3));
        }

        [Test]
        public void CopyToTest() {
            var array = new int[_pascalSet3.Count];

            _pascalSet3.CopyTo(array, 0);

            foreach(var item in array)
                Assert.IsTrue(_pascalSet3.ContainsElement(item));

            if(IsDebugEnabled)
                log.Debug(array.AsJoinedText(","));
        }

        [Test]
        public void CharacterCountTest() {
            // alphabet 만
            var alphabetSet = new PascalSet('A', 'z'); // asciiSet.Union(chars.ToArray());
            // 모음
            var vowels = alphabetSet.Union('A', 'a', 'E', 'e', 'I', 'i', 'O', 'o', 'U', 'u');
            // 자음
            var consonants = vowels.Complement();

            const string contents = "Hey, realweb members. make money please.";

            int contentLength = contents.Length;
            int vowelCount = 0;
            int consonantCount = 0;
            int otherCount = 0;

            for(int i = 0; i < contentLength; i++) {
                char c = contents[i];

                if(vowels.ContainsElement(c))
                    vowelCount++;
                else if(consonants.ContainsElement(c))
                    consonantCount++;
                else
                    otherCount++;
            }

            if(IsDebugEnabled) {
                log.Debug("Contents: " + contents);
                log.Debug("주어진 문장에는 {0}개의 모음, {1}개의 자음, {2}개의 비 알파벳 문자가 있습니다.", vowelCount, consonantCount, otherCount);
                log.Debug("모음 : " + vowels);
                log.Debug("자음 : " + consonants);
            }
        }
    }
}