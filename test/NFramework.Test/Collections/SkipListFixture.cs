using System;
using NUnit.Framework;

namespace NSoft.NFramework.Collections {
    [TestFixture]
    public class SkipListFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly SkipList<string> _list = new SkipList<string>();

        protected override void OnFixtureSetUp() {
            base.OnFixtureSetUp();

            _list.Clear();
            _list.AddRange("가", "나", "하", "바", "카");
        }

        [Test]
        public void AddTest() {
            Console.WriteLine(_list.ToStringDetail());
        }

        [Test]
        public void RemoveTest() {
            _list.Remove("카");
            Assert.IsFalse(_list.Contains("카"));
            Console.WriteLine(_list.ToStringDetail());
        }

        [Test]
        public void ContainsTest() {
            Assert.IsTrue(_list.Contains("나"));
        }

        [Test]
        public void RunTest() {
            var rnd = new Random((int)DateTime.Now.Ticks);
            var tempSL = new SkipList<int>();

            for(int i = 0; i < 512; i++) {
                int adding = rnd.Next(512);
                tempSL.Add(adding);
            }

            for(int i = 0; i < 512; i++) {
                int val = rnd.Next(512);

                switch(rnd.Next(3)) {
                    case 0:
                        tempSL.Add(val);
                        break;
                    case 1:
                        tempSL.Remove(val);
                        break;
                    case 2:
                        tempSL.Contains(val);
                        break;
                }
            }

            Console.WriteLine("SkipList : " + tempSL);
        }
    }
}