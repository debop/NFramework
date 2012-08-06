using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.TimePeriods.TimeRanges;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NSoft.NFramework.Data.NHibernateEx 와 관련된 클래스틑 테스트하기 위한 기본 클래스입니다. 
    /// DB는 기본적으로 SQLite를 사용합니다. 
    /// 변경하고자할 때는 GetDatabaseEngine 을 재정의하세요.
    /// </summary>
    public abstract class NHRepositoryTestFixtureBase : NHDatabaseTestFixtureBase {
        protected override MappingInfo GetMappingInfo() {
            return MappingInfo.From(Assembly.GetExecutingAssembly(),
                                    typeof(Domain.Parent).Assembly);
        }

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();
            BuildUserEntities();
        }

        protected override void OnSetUp() {
            base.OnSetUp();
            CreateObjectsInDB();
        }

        protected override void OnTearDown() {
            Repository<Parent>.DeleteAll();
            base.OnTearDown();
        }

        protected override void OnTestFixtureTearDown() {
            ClearUserEntities();
            base.OnTestFixtureTearDown();
        }

        protected List<Parent> parentsInDB;

        protected virtual void CreateObjectsInDB() {
            parentsInDB = new List<Parent>
                          {
                              CreateParentObject("Parent1", 100, new Child(), new Child()),
                              CreateParentObject("Parent2", 200, new Child(), new Child()),
                              CreateParentObject("Parent4", 400, new Child() { Name = "child1-of-parent4" })
                          };

            AssumeParentObjectNamesAreUnique(parentsInDB);
            SaveAndFlushToDatabase(parentsInDB);
        }

        private static void AssumeParentObjectNamesAreUnique(IEnumerable<Parent> parents) {
            CollectionAssert.AllItemsAreUnique(parents.GroupBy(p => p.Name).ToArray());
            //foreach (var parent in parents)
            //{
            //    if (parents.FindAll(p => p.Name.Equals(parent.Name)).Count > 1)
            //        Assert.Fail("Parent 이름이 Unique 하지 않습니다.");
            //}
        }

        protected static Parent CreateParentObjectInDB(string name, int age) {
            var parent = CreateParentObject(name, age);
            SaveAndFlushToDatabase(parent);
            return parent;
        }

        protected static Parent CreateParentObject(string name, int age, params Child[] children) {
            var parent = new Parent(Guid.NewGuid())
                         {
                             Name = name,
                             Age = age
                         };

            foreach(var child in children) {
                parent.Children.Add(child);
                child.Parent = parent;
            }
            return parent;
        }

        protected static Order[] OrderByNameAndAgeDescending {
            get { return new[] { Order.Desc("Name"), Order.Desc("Age") }; }
        }

        protected static DetachedCriteria WhereNameEquals(string nameToMatch) {
            return DetachedCriteria.For<Parent>().Add(Restrictions.Eq("Name", nameToMatch));
        }

        protected static DetachedCriteria WhereNameIn(params string[] names) {
            return DetachedCriteria.For<Parent>().Add(Restrictions.In("Name", names.ToArray()));
        }

        protected static ProjectionList ProjectByNameAndAge {
            get {
                return Projections.ProjectionList()
                    .Add(Projections.Property("Name"))
                    .Add(Projections.Property("Age"));
            }
        }

        protected static void AssertDTOCreatedFrom(Parent parent, ParentDTO dto) {
            Assert.AreEqual(parent.Name, dto.Name);
            Assert.AreEqual(parent.Age, dto.Age);
        }

        protected static void AssertDTOsCreatedFrom(IList<Parent> parents, ICollection<ParentDTO> dtos) {
            foreach(Parent parent in parents) {
                //Predicate<ParentDTO> matchByNameAndAge =
                //    dto => dto.Age == parent.Age && dto.Name == parent.Name;

                // iteration 시에 로칼복사본을 만드는 이유는 lazy-execution에 따라 문제가 발생할 소지가 있기 때문이다.
                var currentParent = parent;

                if(dtos.FirstOrDefault(dto => dto.Age == currentParent.Age && dto.Name == currentParent.Name) == default(ParentDTO))
                    Assert.Fail("원하는 DTO가 없습니다.");
            }
        }

        protected static void AssertDtosSortedByName(ICollection<ParentDTO> dtos, string sortOrder) {
            Comparison<ParentDTO> sortedByName = (x, y) => x.Name.CompareTo(y.Name);

            AssertSorted(dtos, sortOrder, sortedByName);
        }

        public const string CompanyCode = @"RealWeb";
        public const string Password = "@aaaa";

        protected static void BuildUserEntities() {
            //
            // Company
            //
            var company = new Company
                          {
                              Name = "리얼웹",
                              Code = CompanyCode
                          };

            Repository<Company>.SaveOrUpdate(company);
            UnitOfWork.Current.TransactionalFlush();

            //
            // Department
            //
            Department parent = null;
            foreach(var code in GetCodes("Dept", 5)) {
                var dept = new Department(company, code)
                           {
                               Name = "부서." + code
                           };

                if(parent != null)
                    dept.SetParent(parent);

                Repository<Department>.SaveOrUpdate(dept);

                parent = dept;
            }
            UnitOfWork.Current.TransactionalFlush();

            //
            // User 
            //
            var i = 0;
            foreach(var code in GetCodes("User", 25)) {
                var user = new User(company, code, code)
                           {
                               Password = Password
                           };

                company.AddUser(user);

                user.Data = "동해물과 백두산이 마르고 닳도록, 하느님이 보우하사 우리나라 만세".Replicate(10);
                user.Blob = Encoding.UTF8.GetBytes(user.Data);

                user.ActivePeriod = (i++ % 2 == 0)
                                        ? new TimeRange(DateTime.Now.AddDays(-1), null)
                                        : new TimeRange(null, DateTime.Now.AddDays(1));

                user.ActiveYearWeek = new YearAndWeek(2009, 1);

                user.PeriodTime = new PeriodTime
                                  {
                                      Period = new MonthRange(DateTime.Now),
                                      //DateTime.Now.GetRelativeMonthRange(1),
                                      YearWeek = new YearAndWeek(2009, 50)
                                  };

                Repository<User>.SaveOrUpdate(user);
            }

            UnitOfWork.Current.TransactionalFlush();

            //
            // DepartmentMember
            //
            var depts = Repository<Department>.FindAll();
            var users = Repository<User>.FindAll();

            foreach(var dept in depts) {
                foreach(var user in users) {
                    var deptMember = new DepartmentMember(dept, user) { IsActive = true };
                    Repository<DepartmentMember>.SaveOrUpdate(deptMember);
                }
            }
            UnitOfWork.Current.TransactionalFlush();
        }

        protected static void ClearUserEntities() {
            // NOTE: StatelessSession을 이용하여 삭제 시, 여러 DB를 테스트 할 때 꼬인다. 그래서 Session 상태에서만 테스트 합니다. (2011-04-10)
            //
            With.TryAction(() => {
                               Repository<DepartmentMember>.DeleteAll();
                               Repository<User>.DeleteAll();
                               Repository<Department>.DeleteAll();
                               Repository<Company>.DeleteAll();

                               UnitOfWork.Current.Flush();
                           },
                           ex => log.ErrorException("샘플 데이타를 삭제하는데 실패했습니다.", ex));
        }

        public static IList<string> GetCodes(string prefix, int count) {
            var result = new List<string>(count);

            for(var i = 0; i < count; i++)
                result.Add(prefix + i.ToString("X4"));

            return result;
        }
    }
}