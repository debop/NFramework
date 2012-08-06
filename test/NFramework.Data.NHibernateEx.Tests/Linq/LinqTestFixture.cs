using System.Linq;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.Linq {
    [TestFixture]
    public class LinqTestFixture : NHRepositoryTestFixtureBase {
        [Test]
        public void LoadParentsByLinq() {
            var parents = Repository<Parent>.Query();
            Assert.Greater(parents.Count(), 0);

            var realParent = parents.Where(p => p.Children.Any()).ToList();
            Assert.Greater(realParent.Count, 0, "realParent.Count[{0}] greater than 0", realParent.Count);

            var parentsWithChildCount =
                parents
                    .OrderBy(x => x.Name)
                    .ToList()
                    .Distinct()
                    .Select(x => new { Parent = x, ChildCount = x.Children.Count })
                    .ToList();

            Assert.Greater(parentsWithChildCount.Count, 0, "parentsWithChildCount[{0}] greater then 0.", parentsWithChildCount.Count);
        }
    }

    [TestFixture]
    public class LinqTestFixture_SQLServer : LinqTestFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class LinqTestFixture_DevartOracle : LinqTestFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }

    //[TestFixture]
    //public class LinqTestFixture_SQLiteForFile : LinqTestFixture
    //{
    //    protected override DatabaseEngine GetDatabaseEngine()
    //    {
    //        return DatabaseEngine.SQLiteForFile;
    //    }
    //}
}