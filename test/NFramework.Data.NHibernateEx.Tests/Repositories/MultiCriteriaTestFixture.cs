using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Repositories {
    //
    // NOTE: MultiCriteria 는 SQLite, SQL Server만 지원합니다. Oracle과 SqlCE는 지원하지 않습니다.
    //

    [TestFixture]
    public class MultiCriteriaTestFixture : NHRepositoryTestFixtureBase {
        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            base.CreateObjectsInDB();

            CreateParentObjectInDB("X", 1);
            CreateParentObjectInDB("ThisTestOnly", 1);
            CreateParentObjectInDB("ThisTestOnly", 2);
            CreateParentObjectInDB("ThisTestOnly", 3);
        }

        protected override void OnSetUp() {}

        [Test]
        public void FindAll_MatchingCriteria() {
            // 중복되는 이름을 가진 새로운 Parent객체를 만들어서 DB에 저장한다.
            //
            CreateParentObjectInDB("Parent4", 1);

            ICollection<Parent> loaded = Repository<Parent>.FindAll(new[] { Restrictions.Eq("Name", "Parent4") });

            Assert.AreEqual(1, loaded.Distinct().Count());
        }

        [Test]
        public void FindAll_Expression() {
            // 중복되는 이름을 가진 새로운 Parent객체를 만들어서 DB에 저장한다.
            //
            CreateParentObjectInDB("Parent4", 1);

            ICollection<Parent> loaded = Repository<Parent>.FindAll(parent => parent.Name == "Parent4");

            Assert.AreEqual(1, loaded.Distinct().Count());
        }

        [Test]
        public void FindAll_And_Count() {
            ICriterion whereName = Restrictions.Eq("Name", "ThisTestOnly");
            ICollection<Parent> loaded = Repository<Parent>.FindAll(0, 2, new[] { whereName });
            long count = Repository<Parent>.Count(GetParentCriteria("Name", "ThisTestOnly"));

            Assert.AreEqual(2, loaded.Count, "Max Results가 2입니다. 최대 2개여야 합니다.");
            Assert.AreEqual(3, count, "Name이 'ThisTestOnly' 인게 3개 입니다. ");
        }

        [Test]
        public void FindAll_ByMultiCriteria() {
            var whereCriteria = GetParentCriteria("Name", "ThisTestOnly");
            var countCriteria = CriteriaTransformer.TransformToRowCount(whereCriteria);

            // NOTE : 여러 Criteria중에 count 하는 criteria가 먼저 등록되어야 합니다.   (Projection, Where 순으로)
            var multiCriteria = UnitOfWork.CurrentSession.CreateMultiCriteria()
                .Add(countCriteria)
                .Add(whereCriteria.SetMaxResults(2));

            var multiResults = multiCriteria.List();
            int count = (int)((IList)multiResults[0])[0];
            var loaded = (IList)multiResults[1];

            Assert.AreEqual(2, loaded.Count, "Max Results가 2입니다. 최대 2개여야 합니다.");
            Assert.AreEqual(3, count, "Name이 'ThisTestOnly' 인게 3개 입니다. ");
        }

        private static DetachedCriteria GetParentCriteria(string propertyName, string value) {
            return Repository<Parent>.CreateDetachedCriteria()
                .Add(Restrictions.Eq(propertyName, value));
        }
    }

    [TestFixture]
    public class MultiCriteriaTestFixture_SQLServer : MultiCriteriaTestFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }
}