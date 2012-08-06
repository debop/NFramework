using System;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.NHibernateEx.Utils {
    [TestFixture]
    public class QueryOverUtilTestCase : NHRepositoryTestFixtureBase {
        /// <summary>
        /// ICriteria 와 같은 역할
        /// </summary>
        public IQueryOver<Parent, Parent> ParentQueryOver {
            get { return UnitOfWork.CurrentSession.QueryOver<Parent>(); }
        }

        /// <summary>
        /// DetachedCriteria.For{Parent}() 와 같은 역할입니다.
        /// </summary>
        public QueryOver<Parent, Parent> ParentQueryOverOf {
            get { return QueryOver.Of<Parent>(); }
        }

        #region << IQueryOver<T> >>

        [Test]
        public void IQueryOver_AddEqIncludeNull() {
            foreach(var parent in parentsInDB) {
                var loaded = ParentQueryOver.AddEqIncludeNull(p => p.Name, parent.Name).SingleOrDefault();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void IQueryOver_AddEqOrNull() {
            foreach(var parent in parentsInDB) {
                var loaded = ParentQueryOver.AddEqOrNull(p => p.Name, parent.Name).SingleOrDefault();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void IQueryOver_AddLike() {
            Assert.IsTrue(ParentQueryOver.AddLike(p => p.Name, "Pa").RowCount() > 0);
            var parents = ParentQueryOver.AddLike(p => p.Name, "Pa").List();
            Assert.IsTrue(parents.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void IQueryOver_AddInsensitiveLike() {
            Assert.IsTrue(ParentQueryOver.AddInsensitiveLike(p => p.Name, "Pa").RowCount() > 0);
            var parents = ParentQueryOver.AddInsensitiveLike(p => p.Name, "Pa").List();
            Assert.IsTrue(parents.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void IQueryOver_AddIn() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var expected = ParentQueryOver.AddInG(p => p.Age, ages);
            Assert.Greater(expected.RowCount(), 0);
        }

        [Test]
        public void IQueryOver_AddInG() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var expected = ParentQueryOver.AddInG(p => p.Age, ages);
            Assert.IsTrue(expected.RowCount() > 0);
        }

        [Test]
        public void IQueryOver_AddBetween() {
            var loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(-1),
                                                                                null);
            Assert.Greater(loaded.RowCount(), 0);
            loaded.List().RunEach(log.Debug);

            loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddBetween(u => u.ActivePeriod.Start, null, DateTime.Today.AddDays(1));
            Assert.Greater(loaded.RowCount(), 0);
            loaded.List().RunEach(log.Debug);

            loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(-1),
                                                                            DateTime.Today.AddDays(1));
            Assert.Greater(loaded.RowCount(), 0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void IQueryOver_AddNotBetween() {
            var loaded =
                UnitOfWork.CurrentSession
                    .QueryOver<User>()
                    .AddNotBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(1), null);

            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);

            loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddNotBetween(u => u.ActivePeriod.Start, null,
                                                                               DateTime.Today.AddDays(-1));
            Assert.Greater(loaded.RowCount(), 0);
            loaded.List().RunEach(log.Debug);

            loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddNotBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(1),
                                                                               DateTime.Today.AddDays(-1));
            Assert.Greater(loaded.RowCount(), 0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void IQueryOver_AddIsInRange() {
            var loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddInRange(DateTime.Now,
                                                                                u => u.ActivePeriod.Start,
                                                                                u => u.ActivePeriod.End);
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void IQueryOver_AddIsOverlap_IsOverlap_HasRange() {
            var loaded =
                UnitOfWork.CurrentSession.QueryOver<User>().AddIsOverlap(
                    new TimeRange(DateTime.Now.AddHours(-1), DurationUtil.Hours(2)),
                    u => u.ActivePeriod.Start,
                    u => u.ActivePeriod.End);
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void IQueryOver_AddIsOverlap_IsStartOpenRange() {
            var loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddIsOverlap(new TimeRange(null, DateTime.Now),
                                                                                  u => u.ActivePeriod.Start,
                                                                                  u => u.ActivePeriod.End);
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void IQueryOver_AddIsOverlap_IsEndOpenRange() {
            var loaded = UnitOfWork.CurrentSession.QueryOver<User>().AddIsOverlap(new TimeRange(DateTime.Today, null),
                                                                                  u => u.ActivePeriod.Start,
                                                                                  u => u.ActivePeriod.End);
            Assert.IsTrue(loaded.RowCount() > 0);
            loaded.List().RunEach(log.Debug);
        }

        #endregion

        #region << QueryOver.Of<T> >>

        [Test]
        public void QueryOverOf_AddEqIncludeNull() {
            foreach(var parent in parentsInDB) {
                var loaded =
                    ParentQueryOverOf.AddEqIncludeNull(p => p.Name, parent.Name).GetExecutableQueryOver(UnitOfWork.CurrentSession).
                        SingleOrDefault();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void QueryOverOf_AddEqOrNull() {
            foreach(var parent in parentsInDB) {
                var loaded =
                    ParentQueryOverOf.AddEqOrNull(p => p.Name, parent.Name).GetExecutableQueryOver(UnitOfWork.CurrentSession).
                        SingleOrDefault();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void QueryOverOf_AddLike() {
            Assert.IsTrue(ParentQueryOverOf.AddLike(p => p.Name, "Pa").GetExecutableQueryOver(UnitOfWork.CurrentSession).RowCount() > 0);
            var parents = ParentQueryOverOf.AddLike(p => p.Name, "Pa").GetExecutableQueryOver(UnitOfWork.CurrentSession).List();
            Assert.IsTrue(parents.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void QueryOverOf_AddInsensitiveLike() {
            Assert.IsTrue(
                ParentQueryOverOf.AddInsensitiveLike(p => p.Name, "Pa").GetExecutableQueryOver(UnitOfWork.CurrentSession).RowCount() > 0);
            var parents =
                ParentQueryOverOf.AddInsensitiveLike(p => p.Name, "Pa").GetExecutableQueryOver(UnitOfWork.CurrentSession).List();
            Assert.IsTrue(parents.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void QueryOverOf_AddIn() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var expected = ParentQueryOverOf.AddInG(p => p.Age, ages);
            Assert.IsTrue(expected.GetExecutableQueryOver(UnitOfWork.CurrentSession).RowCount() > 0);
        }

        [Test]
        public void QueryOverOf_AddInG() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var expected = ParentQueryOverOf.AddInG(p => p.Age, ages);
            Assert.IsTrue(expected.GetExecutableQueryOver(UnitOfWork.CurrentSession).RowCount() > 0);
        }

        [Test]
        public void QueryOverOf_AddBetween() {
            var loaded =
                QueryOver.Of<User>()
                    .AddBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(-1), null)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);

            loaded =
                QueryOver.Of<User>()
                    .AddBetween(u => u.ActivePeriod.Start, null, DateTime.Today.AddDays(1))
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);

            loaded =
                QueryOver.Of<User>()
                    .AddBetween(u => u.ActivePeriod.Start,
                                DateTime.Today.AddDays(-1),
                                DateTime.Today.AddDays(1))
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void QueryOverOf_AddNotBetween() {
            var loaded =
                QueryOver.Of<User>()
                    .AddNotBetween(u => u.ActivePeriod.Start, DateTime.Today.AddDays(1), null)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);

            loaded =
                QueryOver.Of<User>()
                    .AddNotBetween(u => u.ActivePeriod.Start, null, DateTime.Today.AddDays(-1))
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);

            loaded =
                QueryOver.Of<User>()
                    .AddNotBetween(u => u.ActivePeriod.Start,
                                   DateTime.Today.AddDays(1),
                                   DateTime.Today.AddDays(-1))
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void QueryOverOf_AddIsInRange() {
            var loaded =
                QueryOver.Of<User>()
                    .AddInRange(DateTime.Now,
                                u => u.ActivePeriod.Start,
                                u => u.ActivePeriod.End)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void QueryOverOf_AddIsOverlap_IsOverlap_HasRange() {
            var loaded =
                QueryOver.Of<User>()
                    .AddIsOverlap(new TimeRange(DateTime.Now.AddHours(-1), DurationUtil.Hours(2)),
                                  u => u.ActivePeriod.Start,
                                  u => u.ActivePeriod.End)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void QueryOverOf_AddIsOverlap_IsStartOpenRange() {
            var loaded =
                QueryOver.Of<User>()
                    .AddIsOverlap(new TimeRange(null, DateTime.Now),
                                  u => u.ActivePeriod.Start,
                                  u => u.ActivePeriod.End)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);
        }

        [Test]
        public void QueryOverOf_AddIsOverlap_IsEndOpenRange() {
            var loaded =
                QueryOver.Of<User>()
                    .AddIsOverlap(new TimeRange(DateTime.Today, null),
                                  u => u.ActivePeriod.Start,
                                  u => u.ActivePeriod.End)
                    .GetExecutableQueryOver(UnitOfWork.CurrentSession);

            loaded.RowCount().Should().Be.GreaterThan(0);
            loaded.List().RunEach(log.Debug);
        }

        #endregion
    }

    [TestFixture]
    public class QueryOverUtilTestCase_SQLServer : QueryOverUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class QueryOverUtilTestCase_Oracle : QueryOverUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }
}