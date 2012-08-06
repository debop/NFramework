using System;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    [TestFixture]
    public class ExpressionFixture : NHRepositoryTestFixtureBase {
        protected Sms _sms;

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();


            _sms = new Sms { Message = "R U There?" };

            Repository<Sms>.SaveOrUpdate(_sms);
            UnitOfWork.Current.Flush();

            Repository<SimpleObject>.Save(new SimpleObject());
            Repository<SimpleObject>.Save(new SimpleObject());

            Repository<SimpleObject>.Save(new SimpleObject { TwoCharactersMax = "AA" });
            Repository<SimpleObject>.Save(new SimpleObject { TwoCharactersMax = "BB" });

            UnitOfWork.Current.Flush();
        }

        protected override void OnTestFixtureTearDown() {
            Repository<Sms>.DeleteAll();
            Repository<SimpleObject>.DeleteAll();

            base.OnTestFixtureTearDown();
        }

        [Test]
        public void EqualOrNull_HasValue() {
            var loaded = UnitOfWork.CurrentSession.Load<Sms>(_sms.Id);

            // Eq 을 사용한다.
            var criteria = DetachedCriteria.For<Sms>().AddEqOrNull("Message", "R U There?");
            var where = criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<Sms>()[0];

            Assert.AreEqual(loaded, where);

            var children =
                Repository<Child>.FindAll(DetachedCriteria.For<Child>().AddEqOrNull("Parent", parentsInDB[0]));
            Assert.Greater(children.Count, 0);
        }

        [Test]
        public void EqualOrNull_Null() {
            // Value 가 null이므로 is null 을 사용한다.
            var criteria =
                DetachedCriteria.For<SimpleObject>().AddEqOrNull("TwoCharactersMax", null);

            var objects = criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<SimpleObject>();
            Assert.AreEqual(objects.Count, 2);

            foreach(SimpleObject obj in objects)
                Console.WriteLine(obj.ToString(true));
            objects.RunEach(so => Console.WriteLine(so.ObjectToString()));


            var children =
                Repository<Child>
                    .FindAll(DetachedCriteria.For<Child>()
                                 .AddEqOrNull("Parent", null));

            Assert.AreEqual(0, children.Count);
        }

        [Test]
        public void EqIncludeNull_ValueIsNotNull() {
            var criteria =
                DetachedCriteria.For<SimpleObject>().Add(CriteriaTool.EqIncludeNull("TwoCharactersMax", "AA"));
            var objects = criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<SimpleObject>();

            Assert.AreEqual(objects.Count, 3); // AA를 가진놈, NULL인놈 두개 (위에 SetUp에서 추가한 것)

            foreach(SimpleObject obj in objects)
                Console.WriteLine(obj.ToString(true));
        }

        [Test]
        public void EqIncludeNull_ValueIsNull() {
            var criteria =
                DetachedCriteria.For<SimpleObject>().Add(CriteriaTool.EqIncludeNull("TwoCharactersMax", null));
            var objects = criteria.GetExecutableCriteria(UnitOfWork.CurrentSession).List<SimpleObject>();

            Assert.AreEqual(objects.Count, 2); // NULL인놈 두개 (위에 SetUp에서 추가한 것)

            foreach(SimpleObject obj in objects)
                Console.WriteLine(obj.ToString(true));
        }

        [Test]
        public void LikeIncludeNull_Test() {
            var objects =
                DetachedCriteria
                    .For<SimpleObject>()
                    .Add(CriteriaTool.InsensitiveLikeIncludeNull("TwoCharactersMax", "A", MatchMode.Start))
                    .GetExecutableCriteria(UnitOfWork.CurrentSession)
                    .List<SimpleObject>();

            Assert.AreEqual(3, objects.Count);
            foreach(SimpleObject o in objects)
                Assert.IsTrue(o.TwoCharactersMax == null || o.TwoCharactersMax.StartsWith("A"));
        }
    }

    [TestFixture]
    public class ExpressionFixture_SqlServer : ExpressionFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }
}