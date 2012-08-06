using System;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.TimePeriods;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernteEx.Utils {
    [TestFixture]
    public class CriteriaUtilTestCase : NHRepositoryTestFixtureBase {
        /// <summary>
        /// ICriteria 와 같은 역할
        /// </summary>
        public ICriteria ParentCriteria {
            get { return Repository<Parent>.CreateCriteria(); }
        }

        /// <summary>
        /// DetachedCriteria.For{Parent}() 와 같은 역할입니다.
        /// </summary>
        public DetachedCriteria ParentDetachedCriteria {
            get { return DetachedCriteria.For<Parent>(); }
        }

        #region << ICriteria >>

        [Test]
        public void ICriteria_AddEqIncludeNull() {
            foreach(var parent in parentsInDB) {
                var loaded = ParentCriteria.AddEqIncludeNull("Name", parent.Name).UniqueResult<Parent>();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void ICriteria_AddEqOrNull() {
            foreach(var parent in parentsInDB) {
                var loaded = ParentCriteria.AddEqOrNull("Name", parent.Name).UniqueResult<Parent>();

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void ICriteria_AddLike() {
            var loaded = ParentCriteria.AddLike("Name", "Pa").List<Parent>();
            Assert.IsTrue(loaded.Count > 0);
            Assert.IsTrue(loaded.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void ICriteria_AddInsensitiveLike() {
            var loaded = ParentCriteria.AddInsensitiveLike("Name", "Pa").List<Parent>();
            Assert.IsTrue(loaded.Count > 0);
            Assert.IsTrue(loaded.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void ICriteria_AddInIn() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var loaded = ParentCriteria.AddIn("Age", ages).List<Parent>();
            Assert.IsTrue(loaded.Count > 0);
            Assert.AreEqual(ages.Length, loaded.Count);
        }

        [Test]
        public void ICriteria_AddInInG() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var loaded = ParentCriteria.AddInG("Age", ages).List<Parent>();
            Assert.IsTrue(loaded.Count > 0);
            Assert.AreEqual(ages.Length, loaded.Count);
        }

        [Test]
        public void ICriteria_AddBetween() {
            var loaded =
                Repository<User>.CreateCriteria().AddBetween("ActivePeriod.Start", DateTime.Today.AddDays(-2), null).List<User>();

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);

            loaded = Repository<User>.CreateCriteria().AddBetween("ActivePeriod.Start", null, DateTime.Today.AddDays(1)).List<User>();
            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);

            loaded =
                Repository<User>.CreateCriteria().AddBetween("ActivePeriod.Start", DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1))
                    .List<User>();
            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void ICriteria_AddIsInRange() {
            var loaded =
                Repository<User>.CreateCriteria().AddInRange(DateTime.Now, "ActivePeriod.Start", "ActivePeriod.End").List<User>();

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void ICriteria_AddIsOverlap_IsOverlap_HasPeriod() {
            var loaded =
                Repository<User>.CreateCriteria()
                    .AddIsOverlap(new TimeRange(DateTime.Now.AddHours(-1), DurationUtil.Hours(2)),
                                  "ActivePeriod.Start",
                                  "ActivePeriod.End")
                    .List<User>();

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void ICriteria_AddIsOverlap_IsStartOpenRange() {
            var loaded =
                Repository<User>.CreateCriteria()
                    .AddIsOverlap(new TimeRange(null, DateTime.Now),
                                  "ActivePeriod.Start",
                                  "ActivePeriod.End")
                    .List<User>();

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void ICriteria_AddIsOverlap_IsEndOpenRange() {
            var loaded =
                Repository<User>.CreateCriteria()
                    .AddIsOverlap(new TimeRange(DateTime.Now, null),
                                  "ActivePeriod.Start",
                                  "ActivePeriod.End")
                    .List<User>();

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        #endregion

        #region << DetachedCriteria >>

        [Test]
        public void DetachedCriteria_AddEqIncludeNull() {
            foreach(var parent in parentsInDB) {
                var loaded = Repository<Parent>.FindOne(ParentDetachedCriteria.AddEqIncludeNull("Name", parent.Name));
                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void DetachedCriteria_AddEqOrNull() {
            foreach(var parent in parentsInDB) {
                var loaded = Repository<Parent>.FindOne(ParentDetachedCriteria.AddEqOrNull("Name", parent.Name));

                Assert.IsNotNull(loaded);
                Assert.AreEqual(parent.Name, loaded.Name);
            }
        }

        [Test]
        public void DetachedCriteria_AddLike() {
            var loaded = Repository<Parent>.FindAll(ParentDetachedCriteria.AddLike("Name", "Pa"));
            Assert.IsTrue(loaded.Count > 0);
            Assert.IsTrue(loaded.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void DetachedCriteria_AddInsensitiveLike() {
            var loaded = Repository<Parent>.FindAll(ParentDetachedCriteria.AddInsensitiveLike("Name", "Pa"));
            Assert.IsTrue(loaded.Count > 0);
            Assert.IsTrue(loaded.Any(p => p.Name.Contains("Pa")));
        }

        [Test]
        public void DetachedCriteria_AddInIn() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var loaded = Repository<Parent>.FindAll(ParentDetachedCriteria.AddIn("Age", ages));
            Assert.IsTrue(loaded.Count > 0);
            Assert.AreEqual(ages.Length, loaded.Count);
        }

        [Test]
        public void DetachedCriteria_AddInInG() {
            var ages = parentsInDB.Select(pdb => pdb.Age).ToArray();

            var loaded = Repository<Parent>.FindAll(ParentDetachedCriteria.AddInG("Age", ages));
            Assert.IsTrue(loaded.Count > 0);
            Assert.AreEqual(ages.Length, loaded.Count);
        }

        [Test]
        public void DetachedCriteria_AddBetween() {
            var crit =
                DetachedCriteria.For<User>()
                    .AddBetween("ActivePeriod.Start", DateTime.Today.AddDays(-2), null);

            var loaded = Repository<User>.FindAll(crit);
            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);


            crit =
                DetachedCriteria.For<User>()
                    .AddBetween("ActivePeriod.Start", null, DateTime.Today.AddDays(1));

            loaded = Repository<User>.FindAll(crit);
            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);

            crit =
                DetachedCriteria.For<User>()
                    .AddBetween("ActivePeriod.Start", DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));

            loaded = Repository<User>.FindAll(crit);
            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void DetachedCriteria_AddIsInRange() {
            var crit =
                DetachedCriteria.For<User>()
                    .AddInRange(DateTime.Now, "ActivePeriod.Start", "ActivePeriod.End");

            var loaded = Repository<User>.FindAll(crit);

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void DetachedCriteria_AddIsOverlap_IsOverlap_HasRange() {
            var loaded =
                Repository<User>.FindAll(DetachedCriteria.For<User>()
                                             .AddIsOverlap(new TimeRange(DateTime.Now.AddHours(-1), DurationUtil.Hours(2)),
                                                           "ActivePeriod.Start",
                                                           "ActivePeriod.End"));


            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void DetachedCriteria_AddIsOverlap_IsStartOpenRange() {
            var loaded =
                Repository<User>.FindAll(DetachedCriteria.For<User>()
                                             .AddIsOverlap(new TimeRange(null, DateTime.Now),
                                                           "ActivePeriod.Start",
                                                           "ActivePeriod.End"));


            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        [Test]
        public void DetachedCriteria_AddIsOverlap_IsEndOpenRange() {
            var loaded =
                Repository<User>.FindAll(DetachedCriteria.For<User>()
                                             .AddIsOverlap(new TimeRange(DateTime.Now, null),
                                                           "ActivePeriod.Start",
                                                           "ActivePeriod.End"));

            Assert.IsTrue(loaded.Count > 0);
            loaded.RunEach(log.Debug);
        }

        #endregion
    }

    [TestFixture]
    public class CriteriaUtilTestCase_SQLServer : CriteriaUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class CriteriaUtilTestCase_Oracle : CriteriaUtilTestCase {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }
}