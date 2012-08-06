using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    [TestFixture]
    public class CriteriaBatchTestFixture : NHRepositoryTestFixtureBase {
        protected Sms _sms;

        protected override void OnSetUp() {
            base.OnSetUp();

            _sms = new Sms { Message = "R U There?" };

            Repository<Sms>.Save(_sms);
            UnitOfWork.Current.Flush();
            UnitOfWork.Current.Clear();
        }

        protected override void OnTearDown() {
            Repository<Sms>.DeleteAll();
            UnitOfWork.Current.Flush();
            UnitOfWork.Current.Clear();

            base.OnTearDown();
        }

        [Test]
        public void CanSaveAndLoad() {
            var loaded = UnitOfWork.CurrentSession.Load<Sms>(_sms.Id);
            Assert.AreEqual(_sms.Message, loaded.Message);
        }

        [Test]
        public void CanUseCriteriaBatch() {
            ICollection<Sms> loadedMsgs = null;

            new CriteriaBatch(UnitOfWork.CurrentSession)
                .Add(DetachedCriteria.For<Sms>(), Order.Asc("Id"))
                .OnRead(delegate(ICollection<Sms> msgs) { loadedMsgs = msgs; })
                .Execute();

            Assert.IsNotNull(loadedMsgs);
            Assert.AreEqual(1, loadedMsgs.Count);
        }

        [Test]
        public void CanUseCriteriaBatchForUniqueResult() {
            ICollection<Sms> loadedMsgs = null;

            Sms loadedMsg = null;

            new CriteriaBatch(UnitOfWork.CurrentSession)
                .Add(DetachedCriteria.For<Sms>(), Order.Asc("Id"))
                .OnRead(delegate(ICollection<Sms> msgs) { loadedMsgs = msgs; })
                .Add(DetachedCriteria.For<Sms>())
                .Paging(0, 1)
                .OnRead(delegate(Sms msg) { loadedMsg = msg; })
                .Execute();

            Assert.IsNotNull(loadedMsgs);

            foreach(Sms msg in loadedMsgs)
                Console.WriteLine(msg.ToString(true));

            Assert.IsNotNull(loadedMsg);
            Console.WriteLine("Unique Result: " + loadedMsg.AsText());
        }

        [Test]
        public void CanUseCriteriaBatchWithAutomaticCountQuery() {
            ICollection<Sms> loadedMsgs = null;
            int msgCount = 0;
            Sms loadedMsg = null;

            new CriteriaBatch(UnitOfWork.CurrentSession)
                .Add(DetachedCriteria.For<Sms>(), Order.Asc("Id"))
                .OnRead(delegate(ICollection<Sms> msgs, int count) {
                            loadedMsgs = msgs;
                            msgCount = count;
                        })
                .Add(DetachedCriteria.For<Sms>())
                .Paging(0, 1)
                .OnRead(delegate(Sms msg) { loadedMsg = msg; })
                .Execute();

            Assert.IsNotNull(loadedMsgs);
            Assert.AreEqual(1, msgCount);
            Assert.IsNotNull(loadedMsg);
        }
    }

    [TestFixture]
    public class CriteriaBatchTestFixture_SQLServer : CriteriaBatchTestFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }
}