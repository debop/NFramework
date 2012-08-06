using System;
using System.Linq;
using System.Text;
using NHibernate.Criterion;
using NHibernate.Linq;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.TimePeriods;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.UserTypes {
    /// <summary>
    /// NOTE: 모든 UserType은 NSoft.NFramework.Data.NHibernateEx.Domin.UserTypes에 있는 것을 사용합니다.
    /// </summary>
    [TestFixture]
    public class UserTypeFixture : NHRepositoryTestFixtureBase {
        [Test]
        public void LoadByName() {
            var user = UnitOfWork.CurrentSession.Query<User>().OrderBy(u => u.Name).FirstOrDefault();
                //Repository<User>.FindFirst(new Order[] {Order.Asc("Name")});

            Assert.AreEqual(Password, user.Password);

            Assert.IsTrue(user.Data.StartsWith("동해물과"));
            Assert.AreEqual(user.Data, Encoding.UTF8.GetString(user.Blob));

            Assert.IsNotNull(user.ActiveYearWeek.Year);
            Assert.IsTrue(user.ActiveYearWeek.Week.HasValue);
            Assert.AreEqual(1, user.ActiveYearWeek.Week.Value);

            Assert.IsNotNull(user.PeriodTime);
            Assert.IsNotNull(user.PeriodTime.YearWeek);
        }

        [Test]
        public void LoadByPassword() {
            // Criteria의 인자값도 UserType으로 변환되어서 QUERY 문이 만들어진다. (즉 암호화된 비밀번호로 찾는다.)
            var user = Repository<User>.FindFirst(new[] { Restrictions.Eq("Password", Password) });

            Assert.IsNotNull(user);
            Assert.AreEqual(Password, user.Password);

            Assert.IsTrue(user.Data.StartsWith("동해물과"));
            Assert.AreEqual(user.Data, Encoding.UTF8.GetString(user.Blob));
        }

        [Test]
        public void LoadByPassword_Expression() {
            // Criteria의 인자값도 UserType으로 변환되어서 QUERY 문이 만들어진다. (즉 암호화된 비밀번호로 찾는다.)
            var user = Repository<User>.FindFirst(u => u.Password == Password);

            Assert.IsNotNull(user);
            Assert.AreEqual(Password, user.Password);

            Assert.IsTrue(user.Data.StartsWith("동해물과"));
            Assert.AreEqual(user.Data, Encoding.UTF8.GetString(user.Blob));
        }

        [Test]
        public void LoadByDateRange_IsStartOpenRange() {
            var users =
                Repository<User>.FindAll(DetachedCriteria.For<User>()
                                             .AddIsOverlap(new TimeRange(null, DateTime.Now),
                                                           "ActivePeriod.Start",
                                                           "ActivePeriod.End"));

            Assert.IsTrue(users.Count > 0);
        }

        [Test]
        public void LoadByDateRange_IsEndOpenRange() {
            var users =
                Repository<User>.FindAll(DetachedCriteria.For<User>()
                                             .AddIsOverlap(new TimeRange(DateTime.Today, null),
                                                           "ActivePeriod.Start",
                                                           "ActivePeriod.End"));

            Assert.IsTrue(users.Count > 0);
        }

        [Test]
        public void IsOverlap_HasRange() {
            var users =
                Repository<User>.FindAll(DetachedCriteria.For<User>()
                                             .AddIsOverlap(new TimeRange(DateTime.Now.AddHours(-1), DurationUtil.Hours(2)),
                                                           "ActivePeriod.Start",
                                                           "ActivePeriod.End"));

            Assert.IsTrue(users.Count > 0);
        }

        [Test]
        public void LoadByDateRange_IsStartOpenRange_Expression() {
            var users =
                Repository<User>.FindAll(QueryOver.Of<User>()
                                             .AddIsOverlap(new TimeRange(null, DateTime.Now),
                                                           u => u.ActivePeriod.Start,
                                                           u => u.ActivePeriod.End));


            Assert.IsTrue(users.Count > 0);
        }

        [Test]
        public void LoadByDateRange_IsEndOpenRange_Expression() {
            var users =
                Repository<User>.FindAll(QueryOver.Of<User>()
                                             .AddIsOverlap(new TimeRange(DateTime.Today, null),
                                                           u => u.ActivePeriod.Start,
                                                           u => u.ActivePeriod.End));

            Assert.IsTrue(users.Count > 0);
        }

        [Test]
        public void IsOverlap_HasRange_Expression() {
            var users =
                Repository<User>.FindAll(QueryOver.Of<User>()
                                             .AddIsOverlap(new TimeRange(DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1)),
                                                           u => u.ActivePeriod.Start,
                                                           u => u.ActivePeriod.End));

            Assert.IsTrue(users.Count > 0);
        }
    }

    [TestFixture]
    public class UserTypeFixture_SQLServer : UserTypeFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.MsSql2005;
        }
    }

    [TestFixture]
    public class UserTypeFixture_Oracle : UserTypeFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }

    //[TestFixture]
    //public class UserTypeFixture_SQLiteForFile : UserTypeFixture
    //{
    //    protected override DatabaseEngine GetDatabaseEngine()
    //    {
    //        return DatabaseEngine.SQLiteForFile;
    //    }
    //}
}