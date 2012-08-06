using System;
using System.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Tools {
    [TestFixture]
    public class SingletonToolFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly ThreadLocal<DateTime> CreatedTime =
            new ThreadLocal<DateTime>(() => SomeSingleton.Instance.CreatedTime);

        [Test]
        public void Should_Singleton_Same_Time_By_ThreadStart() {
            // 다중의 Thread를 만들어서 Thread-safe한 Singleton Instance인가를 검사한다.
            //
            using(new OperationTimer("Using Thread"))
                new ThreadStart(SingletonTestThread).ThreadStress(100);
        }

        [Test]
        public void Should_Singleton_Same_Time_By_ThreadPool() {
            using(new OperationTimer("Using ThreadPool"))
                TestTool.ThreadStressByThreadPool(state => SingletonTestThread(), 100);
        }

        [Test]
        public void Should_Singleton_Same_Time_By_ThreadStart_With_MultiThread() {
            TestTool.RunTasks(5,
                              () => {
                                  using(new OperationTimer("Using ThreadStart with MultiThread")) {
                                      new ThreadStart(SingletonTestThread).ThreadStress(100);
                                      new ThreadStart(SingletonTestThread).ThreadStress(100);
                                  }
                              });
        }

        [Test]
        public void Should_Singleton_Same_Time_By_ThreadPool_With_MultiThread() {
            TestTool.RunTasks(5,
                              () => {
                                  using(new OperationTimer("Using ThreadPool with MultiThread")) {
                                      TestTool.ThreadStressByThreadPool(state => SingletonTestThread(), 100);
                                      TestTool.ThreadStressByThreadPool(state => SingletonTestThread(), 100);
                                  }
                              });
        }

        private static void SingletonTestThread() {
            Assert.AreEqual(CreatedTime.Value, SomeSingleton.Instance.CreatedTime);
        }
    }

    /// <summary>
    /// Thread-Safe한 Singleton 용 예제 Class
    /// </summary>
    public class SomeSingleton {
        public DateTime CreatedTime = DateTime.Now;

        /// <summary>
        /// 단 하나의 객체만을 생성한다.
        /// </summary>
        public static SomeSingleton Instance {
            get { return SingletonTool<SomeSingleton>.Instance; }
        }
    }
}