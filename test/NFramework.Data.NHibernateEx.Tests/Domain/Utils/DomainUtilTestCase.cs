using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Domain.Utils {
    [TestFixture]
    public class DomainUtilTestCase : NHRepositoryTestFixtureBase {
        [Test]
        public void FindOne_Parameter() {
            foreach(var parent in parentsInDB) {
                var loaded = RepositoryTool.FindOne<Parent>(new NHParameter("Name", parent.Name));
                loaded.AssertExists();
            }
        }

        [Test]
        public void FindOne_Expression() {
            foreach(var parent in parentsInDB) {
                var parent1 = parent;
                var loaded = RepositoryTool.FindOne<Parent>(p => p.Name == parent1.Name);
                loaded.AssertExists();
            }
        }

        [Test]
        public void FindAll_Parameter() {
            foreach(var parent in parentsInDB) {
                var loaded = RepositoryTool.FindAll<Parent>(new NHParameter("Name", parent.Name));
                loaded.Count.Should().Be.GreaterThan(0);
            }
        }

        [Test]
        public void FindAll_Parameter_Paging() {
            foreach(var parent in parentsInDB) {
                var loaded = RepositoryTool.FindAll<Parent>(0, 1, null, new NHParameter("Name", parent.Name));
                loaded.Count.Should().Be(1);
            }
        }

        [Test]
        public void FindAll_Expression() {
            foreach(var parent in parentsInDB) {
                var parentName = parent.Name;
                var loaded = RepositoryTool.FindAll<Parent>(p => p.Name == parentName);
                loaded.Count.Should().Be.GreaterThan(0);
            }
        }

        [Test]
        public void FindAll_Expression_Paging() {
            foreach(var parent in parentsInDB) {
                var parentName = parent.Name;
                var loaded = RepositoryTool.FindAll<Parent>(0, 1, p => p.Name == parentName);
                loaded.Count.Should().Be(1);
            }
        }

        [Test]
        public void FindRoots_Parameter() {
            var company = RepositoryTool.FindOneByCode<Company>(CompanyCode);

            var depts = RepositoryTool.FindRoots<Department>(0, 0, null, new NHParameter("Company", company));
            depts.Count.Should().Be(1);
        }

        [Test]
        public void FindRoots_Expression() {
            var company = RepositoryTool.FindOneByCode<Company>(CompanyCode);

            var depts = RepositoryTool.FindRoots<Department>(d => d.Company == company);
            depts.Count.Should().Be(1);

            depts = RepositoryTool.FindRoots<Department>(0, 0, d => d.Company == company);
            depts.Count.Should().Be(1);
        }
    }

    [TestFixture]
    public class DomainUtilTestCase_SQLServer : DomainUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class DomainUtilTestCase_Oracle : DomainUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }

    //[TestFixture]
    //public class DomainUtilTestCase_SQLiteForFile: DomainUtilTestCase
    //{
    //    protected override DatabaseEngine GetDatabaseEngine()
    //    {
    //        return DatabaseEngine.SQLiteForFile;
    //    }
    //}
}