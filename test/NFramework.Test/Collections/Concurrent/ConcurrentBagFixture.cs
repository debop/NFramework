using System.Collections.Concurrent;
using NUnit.Framework;

namespace NSoft.NFramework.Collections.Concurrent {
    [TestFixture]
    public class ConcurrentBagFixture {
        private ConcurrentBag<int> bag;

        [SetUp]
        public void Setup() {
            bag = new ConcurrentBag<int>();
        }

        [Test]
        public void AddStressTest() {
            bag.AddStress();
        }

        [Test]
        public void RemoveStressTest() {
            bag.RemoveStress(CheckOrderingType.DontCare);
        }
    }
}