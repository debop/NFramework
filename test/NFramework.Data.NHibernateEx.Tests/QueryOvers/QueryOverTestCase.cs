using System.Linq;
using NHibernate.Transform;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.QueryOvers {
    [TestFixture]
    public class QueryOverTestCase : NHRepositoryTestFixtureBase {
        [Test]
        public void QueryOver_Where() {
            var parents = UnitOfWork.CurrentSession.QueryOver<Parent>().List();
            Assert.AreEqual(3, parents.Count);

            if(IsDebugEnabled)
                log.Debug("Parents = " + parents.CollectionToString());
        }

        [Test]
        public void QueryOver_Projection() {
            var parents =
                UnitOfWork.CurrentSession
                    .QueryOver<Parent>()
                    .Select(p => p.Name, p => p.Age)
                    .List<object[]>()
                    .Select(list => new ParentDTO((string)list[0], (int)list[1]))
                    .Distinct()
                    .ToList();

            if(IsDebugEnabled)
                log.Debug("ParentDTO = " + parents.CollectionToString());

            Assert.AreEqual(3, parents.Count);
        }

        [Test]
        public void QueryOver_Projection_With_TransformUsing() {
            ParentDTO parentDto = null;

            var parentDtos =
                UnitOfWork.CurrentSession
                    .QueryOver<Parent>()
                    .SelectList(list => list
                                            .SelectGroup(p => p.Name).WithAlias(() => parentDto.Name)
                                            .SelectMax(p => p.Age).WithAlias(() => parentDto.Age))
                    .OrderByAlias(() => parentDto.Age).Desc
                    .TransformUsing(Transformers.AliasToBean<ParentDTO>())
                    .List<ParentDTO>();

            if(IsDebugEnabled)
                log.Debug("ParentDTO = " + parentDtos.CollectionToString());
        }
    }

    [TestFixture]
    public class QueryOverTestCase_SQLServer : QueryOverTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class QueryOverTestCase_Oracle : QueryOverTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }
}