using NUnit.Framework;

namespace NSoft.NFramework {
    [TestFixture]
    public class NamedIndexerFixture : AbstractFixture {
        [Test]
        public void Getter() {
            var indexers = new UsingIndexers();

            Assert.AreEqual(8, indexers.Indexer[1]);
            Assert.AreEqual(1, indexers.Key);
        }

        [Test]
        public void Setter() {
            var indexers = new UsingIndexers();
            indexers.Indexer[3] = 9;
            Assert.AreEqual(3, indexers.Key);
            Assert.AreEqual(9, indexers.Value);
        }

        [Test]
        public void GetterOnly() {
            var indexers = new UsingIndexers();

            Assert.AreEqual(8, indexers.Getter[1]);
            Assert.AreEqual(1, indexers.Key);
        }

        [Test]
        public void SetterOnly() {
            var indexers = new UsingIndexers();
            indexers.Setter[3] = 9;
            Assert.AreEqual(3, indexers.Key);
            Assert.AreEqual(9, indexers.Value);
        }
    }

    public class UsingIndexers {
        public int Key = 5;
        public int Value = 8;

        public NamedIndexer<int, int> Indexer;
        public NamedIndexerGetter<int, int> Getter;
        public NamedIndexerSetter<int, int> Setter;

        public UsingIndexers() {
            Indexer = new NamedIndexer<int, int>(Get, Set);
            Getter = new NamedIndexerGetter<int, int>(Get);
            Setter = new NamedIndexerSetter<int, int>(Set);
        }

        public int Get(int key) {
            Key = key;
            return Value;
        }

        public void Set(int key, int value) {
            Key = key;
            Value = value;
        }
    }
}