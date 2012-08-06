using System;
using System.Linq;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    [TestFixture]
    public class FutureFixture : NHRepositoryTestFixtureBase {
        [Test]
        public void CanExecuteQueryBatch() {
#pragma warning disable 0618
            var futureQueryOfParents = new FutureQueryOf<Parent>(DetachedCriteria.For<Parent>());
            var futureQueryOfChildren = new FutureQueryOf<Child>(DetachedCriteria.For<Child>());

            Assert.IsTrue(futureQueryOfParents.Results.Count > 0);
            Assert.IsTrue(futureQueryOfChildren.Results.Count > 0);
#pragma warning restore 0618

            //! NHibernate 3.x 의 Future 기능을 사용하세요!!!
            //
            var futureOfParents = Repository<Parent>.Future(QueryOver.Of<Parent>());
            var futureOfChildren = Repository<Child>.Future(QueryOver.Of<Child>());

            futureOfParents.Count().Should().Be.GreaterThan(0);
            futureOfChildren.Count().Should().Be.GreaterThan(0);
        }

        [Test]
        public void CanGetFutureEntities() {
            var parent = new Parent(Guid.NewGuid()) { Name = "abc", Age = 123 };
            var parentId = parent.Id;

            Repository<Parent>.SaveOrUpdate(parent);
            UnitOfWork.Current.Flush();

#pragma warning disable 0618

            var futureParent = Repository<Parent>.FutureGet(parentId);
            var futureChild = Repository<Child>.FutureGet(Guid.NewGuid());

#pragma warning restore 0618

            Assert.IsNull(futureChild.Value);

            Assert.IsNotNull(futureParent.Value);

            Console.WriteLine("Parent : " + futureParent.Value.ToString(true));
        }
    }

    [TestFixture]
    public class FutureFixture_SqlServer : FutureFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }
}