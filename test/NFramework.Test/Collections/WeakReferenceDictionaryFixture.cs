using System;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class WeakReferenceDictionaryFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        private readonly WeakReferenceDictionary<string, TestObject> _cache = new WeakReferenceDictionary<string, TestObject>();

#if !SILVERLIGHT
        [Test]
        public void TestOfRwWeakReferenceMruCache2() {
            if(IsDebugEnabled)
                log.Debug("Add 5 Weak Reference...");

            lock(_cache) {
                _cache.Add("ID1", new TestObject(1, 1, "ID1"));
                _cache.Add("ID2", new TestObject(2, 1, "ID2"));
                _cache.Add("ID3", new TestObject(3, 1, "ID3"));
                _cache.Add("ID4", new TestObject(4, 1, "ID4"));
                _cache.Add("ID5", new TestObject(5, 1, "ID5"));
            }

            Console.WriteLine("\r\n --> Add 5 weak references");
            Console.WriteLine("Cache.TotalCount = " + _cache.Count);
            Console.WriteLine("Cache.AliveCount = " + _cache.ActiveCount);
            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));

            Assert.AreEqual(5, _cache.Count);
            Assert.AreEqual(5, _cache.ActiveCount);

            for(var i = 0; i < 5; i++)
                Assert.AreEqual(0, _cache.GetGeneration("ID" + (i + 1)));

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\r\n --> GC.Collect()");
            Console.WriteLine("Cache.TotalCount = " + _cache.Count);
            Console.WriteLine("Cache.AliveCount = " + _cache.ActiveCount);
            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));

            _cache.Count.Should().Be(5);
            _cache.ActiveCount.Should().Be(0);
            _cache.GetGeneration("ID1").Should().Be(-1);

            lock(_cache) {
                _cache.Add("ID1", new TestObject(1, 2, "ID1"));
                _cache.Add("ID2", new TestObject(2, 2, "ID2"));
                _cache.Add("ID3", new TestObject(3, 2, "ID3"));
                _cache.Add("ID4", new TestObject(4, 2, "ID4"));
                _cache.Add("ID5", new TestObject(5, 2, "ID5"));
            }

            Console.WriteLine("\r\n --> Add 5 weak references");
            Console.WriteLine("Cache.TotalCount = " + _cache.Count);
            Console.WriteLine("Cache.AliveCount = " + _cache.ActiveCount);
            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));

            _cache.Count.Should().Be(5);
            _cache.ActiveCount.Should().Be(5);
            _cache.GetGeneration("ID1").Should().Be(0);


            CreateObjectLongMethod();
            CreateOdd();
            CollectAll();
        }
#endif

        private void CreateObjectLongMethod() {
            Console.WriteLine("\r\n --> Obtain ID1");

            lock(_cache) {
                if(_cache.IsAlive("ID1")) {
                    if(IsDebugEnabled)
                        log.Debug("ID1 is Alive");

                    var to = _cache.GetValue("ID1");
                    to.X = 100;
                    Console.WriteLine("TestObject = " + to);
                }
                else {
                    if(IsDebugEnabled)
                        log.Debug("ID1 is not Alive");

                    Console.WriteLine("Object ID1 does not exists... Create new ID1...");
                    _cache.Add("ID1", new TestObject(1, 1, "ID1"));

                    var to = _cache.GetValue("ID1");
                    to.X = 101;
                    Console.WriteLine("TestObject = " + to);
                }
            }
        }

        private void CreateOdd() {
            if(IsDebugEnabled)
                log.Debug("Create Odd Keys...");

            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));
            Console.WriteLine("\r\n --> Obtain ID1, ID3, ID5");

            var to1 = _cache.GetValue("ID1");
            var to3 = _cache.GetValue("ID3");
            var to5 = _cache.GetValue("ID5");

            to1.X = 1000;
            to3.X = 3000;
            to5.X = 5000;
            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("Cache.Count = " + _cache.Count);
            Console.WriteLine("Cache.AliveCount = " + _cache.ActiveCount);
            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));

            Console.WriteLine("TestObject ID1 = " + to1);
            Console.WriteLine("TestObject ID3 = " + to3);
            Console.WriteLine("TestObject ID5 = " + to5);

            Console.WriteLine("\r\n --> Get ID2, which has been collected, ID2 exists == " + _cache.IsAlive("ID2"));
            var to2 = _cache.GetValue("ID2");
            if(to2 == null)
                to2 = new TestObject(2, 1, "ID2");
            Console.WriteLine("ID2 has now been re-created. ID2 exists == " + _cache.IsAlive("ID2"));
            Console.WriteLine("Cache.AliveCount = " + _cache.ActiveCount);
            to2.X = 2000;
            Console.WriteLine("TestObject ID2 = " + to2);

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Console.WriteLine("\r\n --> Collect all weak references");
            Console.WriteLine("Cache.TotalCount = " + _cache.Count);
            Console.WriteLine("Cache.AliveCount = " + _cache.ActiveCount);
            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));
            Console.WriteLine("Cache.GetGeneration('ID2') = " + _cache.GetGeneration("ID2"));
            Console.WriteLine("Cache.GetGeneration('ID3') = " + _cache.GetGeneration("ID3"));
        }

        private void CollectAll() {
            if(IsDebugEnabled)
                log.Debug("Collect All Objects...");

            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(5, _cache.Count);
            Assert.AreEqual(0, _cache.ActiveCount);
            for(var i = 0; i < 5; i++)
                Assert.AreEqual(-1, _cache.GetGeneration("ID" + (i + 1)));

            Console.WriteLine("\r\n --> Collect all weak references");
            Console.WriteLine("Cache.TotalCount = " + _cache.Count);
            Console.WriteLine("Cache.AliveCount = " + _cache.ActiveCount);

            Console.WriteLine("Cache.GetGeneration('ID1') = " + _cache.GetGeneration("ID1"));
            Console.WriteLine("Cache.GetGeneration('ID2') = " + _cache.GetGeneration("ID2"));
            Console.WriteLine("Cache.GetGeneration('ID3') = " + _cache.GetGeneration("ID3"));
            Console.WriteLine("Cache.GetGeneration('ID4') = " + _cache.GetGeneration("ID4"));
            Console.WriteLine("Cache.GetGeneration('ID5') = " + _cache.GetGeneration("ID5"));
        }

        /// <summary>
        /// ObjectCache Class Test를 위한 Test Data Class
        /// </summary>
        public class TestObject {
            public TestObject() {
                X = 0;
                Y = 0;
                S = string.Empty;
            }

            public TestObject(int x, int y, string s) {
                X = x;
                Y = y;
                S = s;
            }

            public int X { get; set; }
            public int Y { get; set; }
            public string S { get; set; }

            public override string ToString() {
                return string.Format("TestObject# X=[{0}], Y=[{1}], S=[{2}]", X, Y, S);
            }
        }
    }
}