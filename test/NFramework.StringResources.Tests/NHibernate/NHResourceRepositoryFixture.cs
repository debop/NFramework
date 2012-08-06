using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using NHibernate;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.StringResources.NHibernate {
    /// <summary>
    /// NHibernate용 ResourceProvider를 사용하기 위해서는 
    /// 1. DB 생성 (StringResources.NHibernate.sql 을 대상 DB에 실행시킨다. 
    /// 2. NHibernate 환경설정을 변경한다. (/NHibernateConfigs/hibernate.resources.cfg.xml)
    /// 3. Castle.Windsor 환경설정을 변경한다. ( Windsor.ResourceProvider.config, Windsor.NHibernate.config) 
    /// </summary>
    [TestFixture]
    public class NHResourceRepositoryFixture {
        [TestFixtureSetUp]
        public void ClassSetUp() {
            if(IoC.IsNotInitialized)
                IoC.Initialize();

            // NOTE: Unit Test 시에 ThreadedRepeat를 이용해 MultiThread 환경에서 테스트 시에는 
            // NOTE: UnitOfWork에 대해서는 아래와 같이 각 테스트 메소드에서 Thread 별로 UnitOfWork를 새로 시작해야 합니다.

            //if (UnitOfWork.IsStarted == false)
            //    UnitOfWork.Start();
        }

        public NHResourceRepository Repository {
            get { return (NHResourceRepository)IoC.Resolve<INHRepository<NHResource>>(); }
        }

        [Test]
        public void WindsorComponentTest() {
            Assert.IsNotNull(Repository);
        }

        [Test]
        public void Get() {
            // NOTE: Unit Test 시에 ThreadedRepeat를 이용해 MultiThread 환경에서 테스트 시에는 
            // NOTE: UnitOfWork에 대해서는 아래와 같이 각 테스트 메소드에서 Thread 별로 UnitOfWork를 새로 시작해야 합니다.
            using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                var nhResource = Repository.Get(1);
                Assert.IsNotNull(nhResource);
                Assert.AreEqual(nhResource.Id, 1);

                Print(new[] { nhResource });
            }
        }

        [Test]
        public void FindAll() {
            using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                var resources = Repository.FindAll();
                Assert.Greater(resources.Count, 0);
                Print(resources);
            }
        }

        [Test]
        public void FutureGet() {
            using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork)) {
                var futureResources = new List<IFutureValue<NHResource>>();

#pragma warning disable 0618

                for(int i = 1; i < 4; i++)
                    futureResources.Add(Repository.FutureGet(i));

#pragma warning restore 0618

                Console.WriteLine("FutureGet을 이용하여 가져오기를 예약했습니다.");
                Console.WriteLine(".............................................");

                Thread.Sleep(100);

                Console.WriteLine("이제부터 예약된 값을 가져오겠습니다.");

                var resources = futureResources.Select(futureResource => futureResource.Value).ToList();

                Assert.Greater(resources.Count, 0);
                Print(resources);
            }
        }

        [Test]
        public void Thread_Test() {
            TestTool.RunTasks(3,
                              () => {
                                  WindsorComponentTest();
                                  Get();
                                  FindAll();
                                  FutureGet();
                              });
        }

        private static void Print(IEnumerable<NHResource> resources) {
            foreach(var res in resources) {
                Console.WriteLine(res.ToString(true));

                foreach(CultureInfo culture in res.LocaleMap.Keys)
                    Console.WriteLine("\t Locale Value : [{0}] {1}", culture, res.LocaleMap[culture]);
            }
        }
    }
}