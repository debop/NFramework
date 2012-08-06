using System;
using System.Linq;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernteEx.Utils {
    [TestFixture]
    public class LinqUtilTestCase : NHRepositoryTestFixtureBase {
        public IQueryable<Parent> Parents {
            get { return NHTool.GetQuery<Parent>(); }
        }

        public IQueryable<User> Users {
            get { return NHTool.GetQuery<User>(); }
        }

        [Test]
        public void IQueryable_AddEqIncludeNull() {
            foreach(var parent in parentsInDB) {
                var loaded = Parents.AddEqIncludeNull(p => p.Name, parent.Name).SingleOrDefault();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void IQueryable_AddEqOrNull() {
            foreach(var parent in parentsInDB) {
                var loaded = Parents.AddEqOrNull(p => p.Name, parent.Name).SingleOrDefault();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void IQueryable_AddLike() {
            Assert.IsTrue(Parents.AddLike(p => p.Name, "Pa").Any());
            var parents = Parents.AddLike(p => p.Name, "Pa").ToList();
            Assert.IsTrue(parents.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void IQueryable_AddInsensitiveLike() {
            Assert.IsTrue(Parents.AddInsensitiveLike(p => p.Name, "pa").Any());
            var parents = Parents.AddInsensitiveLike(p => p.Name, "PA").ToList();
            Assert.IsTrue(parents.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void IQueryable_AddInIn() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var expected = Parents.Where(p => ages.Contains(p.Age)).ToList();
            Assert.IsTrue(expected.Count > 0);
        }

        [Test]
        public void IQueryable_AddInInG() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var expected = Parents.Where(p => ages.Contains(p.Age));

            Assert.IsTrue(expected.Any());
        }

        [Test]
        public void IQueryable_AddBetween() {
            var loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(-1),
                                                                                null);
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);

            loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddBetween(u => u.ActivePeriod.Start, null, DateTime.Today.AddDays(1));
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);

            loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(-1),
                                                                            DateTime.Today.AddDays(1));
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void IQueryable_AddIsInRange() {
            var loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddInRange(DateTime.Now,
                                                                                u => u.ActivePeriod.Start,
                                                                                u => u.ActivePeriod.End);
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void IQueryable_AddIsOverlap_IsOverlap_HasRange() {
            var loaded =
                Users
                    .AddIsOverlap(new TimeRange(DateTime.Now.AddHours(-1), DurationUtil.Hours(2)),
                                  u => u.ActivePeriod.Start,
                                  u => u.ActivePeriod.End)
                    .ToList();
            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void IQueryable_AddIsOverlap_IsStartOpenRange() {
            var loaded =
                Users.AddIsOverlap(new TimeRange(null, DateTime.Now),
                                   u => u.ActivePeriod.Start,
                                   u => u.ActivePeriod.End);


            Assert.IsTrue(loaded.Any());
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void IQueryable_AddIsOverlap_IsEndOpenRange() {
            var loaded =
                Users.AddIsOverlap(new TimeRange(DateTime.Today, null),
                                   u => u.ActivePeriod.Start,
                                   u => u.ActivePeriod.End)
                    .ToList();

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void IQueryable_AddWhere() {
            foreach(var user in Users.ToList()) {
                var loaded1 = Users.AddWhere("Name==@0", user.Name).SingleOrDefault();
                var loaded2 = Users.AddWhere(u => u.Name == user.Name).SingleOrDefault();
                Assert.AreEqual(user.Name, loaded1.Name);
                Assert.AreEqual(user.Name, loaded2.Name);
            }
        }

        [Test]
        public void IQueryable_OrderBy() {
            var users = Users.AddOrderBy("ActivePeriod desc, Name asc").ToList();
            users.RunEach(log.Debug);
        }

        [Test]
        public void IQueryable_AddOrders() {
            var users = Users.AddOrders(new NHOrder<User>(u => u.ActivePeriod, false), new NHOrder<User>(u => u.Name)).ToList();
            users.RunEach(log.Debug);

            users = Users.AddOrders(new NHOrder<User>("ActivePeriod", false), new NHOrder<User>(u => u.Name)).ToList();
            users.RunEach(log.Debug);
        }
    }

    [TestFixture]
    public class LinqUtilTestCase_SQLServer : LinqUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class LinqUtilTestCase_Oracle : LinqUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }
}