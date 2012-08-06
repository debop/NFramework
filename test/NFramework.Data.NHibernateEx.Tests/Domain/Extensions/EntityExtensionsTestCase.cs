using System.Linq;
using NHibernate;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Domain.Extensions {
    [TestFixture]
    public class EntityExtensionsTestCase : NHRepositoryTestFixtureBase {
        #region << TreeNode >>

        [Test]
        public void GetChildCount() {
            var depts = Repository<Department>.FindAll();

            foreach(var dept in depts)
                Assert.AreEqual(dept.Children.Count, dept.GetChildCount());
        }

        [Test]
        public void ExistsChildren() {
            var depts = Repository<Department>.FindAll();

            foreach(var dept in depts) {
                var hasChildren = dept.Children.Count > 0;
                Assert.AreEqual(hasChildren, dept.HasChildren());
            }
        }

        [Test]
        public void GetAncestors() {
            var depts = Repository<Department>.GetPage(new NHOrder<Department>(d => d.NodePosition.Level));

            foreach(var dept in depts) {
                var ancestors = dept.GetAncestors().ToStack().ToList();
                Assert.IsTrue(ancestors.Count > 0);

                ancestors.Contains(dept).Should().Be.True();

                if(dept.Parent != null)
                    ancestors.Any(a => dept.Parent == a).Should().Be.True();
            }
        }

        [Test]
        public void GetDesendents() {
            var depts = Repository<Department>.GetPage(new NHOrder<Department>(d => d.NodePosition.Level));

            foreach(var dept in depts) {
                var descendents = dept.GetDescendents().ToList();
                Assert.IsTrue(descendents.Count > 0);

                descendents.Contains(dept).Should().Be.True();

                if(dept.Children.Count > 0)
                    descendents.Contains(dept.Children.ElementAt(0)).Should().Be.True();

                if(descendents.Count > 1)
                    descendents.Any(d => d.Parent == dept).Should().Be.True();
            }
        }

        [Test]
        public void GetRoots() {
            var roots = EntityTool.GetRoots<Department>();

            Assert.AreEqual(EntityTool.GetRootCount<Department>(), roots.Count);
        }

        [Test]
        public void GetAncestors_By_HierarchyQuery() {
            IQuery query = null;

            if(UnitOfWork.CurrentSessionFactory.IsMsSqlServer2005OrHigher())
                query = UnitOfWork.CurrentSession.GetNamedQuery("GetDepartmentAndAncestors" + ".MsSql");

            else if(UnitOfWork.CurrentSessionFactory.IsOracle())
                query = UnitOfWork.CurrentSession.GetNamedQuery("GetDepartmentAndAncestors" + ".Oracle");

            if(query != null) {
                var depts = Repository<Department>.GetPage(new NHOrder<Department>(d => d.NodePosition.Level));

                foreach(var dept in depts) {
                    query.SetParameter("DepartmentId", dept.Id);
                    var ancestors = query.List<Department>();

                    Assert.Greater(ancestors.Count, 0);

                    ancestors.Contains(dept).Should().Be.True();

                    if(dept.Parent != null)
                        ancestors.Any(a => dept.Parent == a).Should().Be.True();
                }
            }
        }

        [Test]
        public void GetDescendents_By_HierarchyQuery() {
            IQuery query = null;

            if(UnitOfWork.CurrentSessionFactory.IsMsSqlServer2005OrHigher())
                query = UnitOfWork.CurrentSession.GetNamedQuery("GetDepartmentAndDescendents" + ".MsSql");

            else if(UnitOfWork.CurrentSessionFactory.IsOracle())
                query = UnitOfWork.CurrentSession.GetNamedQuery("GetDepartmentAndDescendents" + ".Oracle");

            if(query != null) {
                var depts = Repository<Department>.GetPage(new NHOrder<Department>(d => d.NodePosition.Level));

                foreach(var dept in depts) {
                    query.SetParameter("DepartmentId", dept.Id);
                    var descendents = query.List<Department>();

                    Assert.Greater(descendents.Count, 0);

                    descendents.Contains(dept).Should().Be.True();

                    if(descendents.Count > 1)
                        descendents.Any(d => d.Parent == dept).Should().Be.True();
                }
            }
        }

        #endregion

        #region << MapEntity / MapEntities / MapEntitiesAsParallel >>

        [Test]
        public void User_MapEntities() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(Company));
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var mapOptions = new MapPropertyOptions
                             {
                                 SuppressException = true,
                                 SkipAlreadyExistValue = false
                             };

            var users = Repository<User>.FindAll();

            Assert.Greater(users.Count, 0);

            var mappedUsers =
                users
                    .MapEntities(() => new UserDto { Password = "111" },
                                 mapOptions,
                                 (user, userDto) => { userDto.CompanyCode = user.Company.Code; },
                                 p => p.Data)
                    .ToList();

            Assert.AreEqual(users.Count, mappedUsers.Count);

            for(var i = 0; i < users.Count; i++) {
                Assert.AreEqual(users[i].Name, mappedUsers[i].Name, "mapping Name is fail. index=" + i);
                Assert.AreEqual(users[i].Password, mappedUsers[i].Password, "mapping Password is fail. index=" + i);
                Assert.AreNotEqual(users[i].Data, mappedUsers[i].Data);
            }
        }

        [Test]
        public void User_MapEntities_SkipAlreadyExistValue() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(Company));
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var mapOptions = new MapPropertyOptions { SuppressException = true, SkipAlreadyExistValue = true };

            var users = Repository<User>.FindAll();

            var mappedUsers =
                users
                    .MapEntities(() => new UserDto { Password = "111" },
                                 mapOptions,
                                 (user, userDto) => { userDto.CompanyCode = user.Company.Code; },
                                 p => p.Data)
                    .ToList();

            Assert.AreEqual(users.Count, mappedUsers.Count);

            for(var i = 0; i < users.Count; i++) {
                Assert.AreEqual(users[i].Name, mappedUsers[i].Name, "mapping Name is fail. index=" + i);
                Assert.AreNotEqual(users[i].Password, mappedUsers[i].Password, "mapping Password is fail. index=" + i);
                Assert.AreNotEqual(users[i].Data, mappedUsers[i].Data);
            }
        }

        [Test]
        public void User_MapEntities_SkipAlreadyExistValue_IgnoreCase() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(Company));
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var mapOptions = new MapPropertyOptions { SuppressException = true, SkipAlreadyExistValue = true, IgnoreCase = true };

            var users = Repository<User>.Query().ToList();

            var mappedUsers =
                users
                    .MapEntities(() => new UserDto { Password = "111" },
                                 mapOptions,
                                 (user, userDto) => { userDto.CompanyCode = user.Company.Code; },
                                 p => p.Data)
                    .ToList();

            Assert.AreEqual(users.Count, mappedUsers.Count);

            for(var i = 0; i < users.Count; i++) {
                Assert.AreEqual(users[i].Name, mappedUsers[i].Name, "mapping Name is fail. index=" + i);
                Assert.AreNotEqual(users[i].Password, mappedUsers[i].Password, "mapping Password is fail. index=" + i);
                Assert.AreNotEqual(users[i].Data, mappedUsers[i].Data);
            }
        }

        [Test]
        public void User_MapEntitiesAsParallel() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(Company));
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var mapOptions = new MapPropertyOptions { SuppressException = true, SkipAlreadyExistValue = false };

            var users = Repository<User>.Query().ToList();
            var mappedUsers = users.MapEntitiesAsParallel(() => new UserDto { Password = "111" },
                                                          mapOptions,
                                                          (user, userDto) => { userDto.CompanyCode = user.Company.Code; },
                                                          p => p.Data);

            Assert.AreEqual(users.Count, mappedUsers.Count);

            for(var i = 0; i < users.Count; i++) {
                Assert.AreEqual(users[i].Name, mappedUsers[i].Name, "mapping Name is fail. index=" + i);
                Assert.AreEqual(users[i].Password, mappedUsers[i].Password, "mapping Password is fail. index=" + i);
                Assert.AreNotEqual(users[i].Data, mappedUsers[i].Data);
            }
        }

        [Test]
        public void User_MapEntitiesAsParallel_SkipAlreadyExistValue() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(Company));
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var mapOptions = new MapPropertyOptions { SuppressException = true, SkipAlreadyExistValue = true };

            var users = Repository<User>.Query().ToList();
            var mappedUsers = users.MapEntitiesAsParallel(() => new UserDto { Password = "111" },
                                                          mapOptions,
                                                          (user, userDto) => { userDto.CompanyCode = user.Company.Code; },
                                                          p => p.Data);

            Assert.AreEqual(users.Count, mappedUsers.Count);

            for(var i = 0; i < users.Count; i++) {
                Assert.AreEqual(users[i].Name, mappedUsers[i].Name, "mapping Name is fail. index=" + i);
                Assert.AreNotEqual(users[i].Password, mappedUsers[i].Password, "mapping Password is fail. index=" + i);
                Assert.AreNotEqual(users[i].Data, mappedUsers[i].Data);
            }
        }

        [Test]
        public void User_MapEntitiesAsParallel_SkipAlreadyExistValue_IgnoreCase() {
            UnitOfWork.CurrentSessionFactory.Evict(typeof(Company));
            UnitOfWork.CurrentSessionFactory.Evict(typeof(User));
            UnitOfWork.CurrentSession.Clear();

            var mapOptions = new MapPropertyOptions { SuppressException = true, SkipAlreadyExistValue = true, IgnoreCase = true };

            var users = Repository<User>.Query().ToList();
            var mappedUsers = users.MapEntitiesAsParallel(() => new UserDto { Password = "111" },
                                                          mapOptions,
                                                          (user, userDto) => { userDto.CompanyCode = user.Company.Code; },
                                                          p => p.Data);

            Assert.AreEqual(users.Count, mappedUsers.Count);

            for(var i = 0; i < users.Count; i++) {
                Assert.AreEqual(users[i].Name, mappedUsers[i].Name, "mapping Name is fail. index=" + i);
                Assert.AreNotEqual(users[i].Password, mappedUsers[i].Password, "mapping Password is fail. index=" + i);
                Assert.AreNotEqual(users[i].Data, mappedUsers[i].Data);
            }
        }

        #endregion
    }

    [TestFixture]
    public class EntityExtensionsTestCase_SQLServer : EntityExtensionsTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class EntityExtensionsTestCase_Oracle : EntityExtensionsTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }
}