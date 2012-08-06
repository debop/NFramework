using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Type;
using NSoft.NFramework.Caching.Domain.Model;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Reflections;
using NUnit.Framework;

namespace NSoft.NFramework.Caching.SharedCache.NHCaches {
    /// <summary>
    /// MongoDB를 Second Cache로 사용하는지 검사합니다. 테스트 파일에서는 NHRepositoryTestFixtureBase의 환경설정 정보를 변경시켜야합니다.
    /// </summary>
    [TestFixture]
    public class SecondCacheTestCase : NHRepositoryTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        #region << Get >>

        [Test]
        public void SaveAndGet() {
            UnitOfWork.CurrentSession.Clear();

            var loaded = Repository<Parent>.Get(parentsInDB[0].Id);

            Assert.IsNotNull(loaded);
            Assert.AreEqual(parentsInDB[0].Name, loaded.Name);
            Assert.AreEqual(2, parentsInDB[0].Children.Count);
            Assert.AreEqual(2, loaded.Children.Count);
        }

        [Test]
        public void LoadTest() {
            UnitOfWork.CurrentSession.Clear();

            var loaded = Repository<Parent>.Load(parentsInDB[0].Id);

            Assert.IsNotNull(loaded);
            Assert.AreEqual(parentsInDB[0].Name, loaded.Name);
            Assert.AreEqual(2, parentsInDB[0].Children.Count);
            Assert.AreEqual(2, loaded.Children.Count);
        }

        #endregion

        #region << GetIn >>

        [Test]
        public void GetIn() {
            var parents = Repository<Parent>.GetIn(new List<object>
                                                   {
                                                       parentsInDB[0].Id,
                                                       parentsInDB[1].Id
                                                   });

            Assert.AreEqual(2, parents.Distinct().Count());
        }

        [Test]
        public void GetInG() {
            var ids = parentsInDB.Select(parent => parent.Id).OfType<Guid>().ToList();
            var parents = Repository<Parent>.GetInG(ids);

            Assert.AreEqual(ids.Count(), parents.Distinct().Count());
        }

        #endregion

        #region << GetPage >>

        [TestCase(0, 10)]
        [TestCase(1, 2)]
        public void GetPage(int pageIndex, int pageSize) {
            var parents = Repository<Parent>.GetPage(pageIndex, pageSize, new[] { Order.Asc("Name") },
                                                     Restrictions.In("Name", new object[] { "Parent1", "Parent2" }));
            Assert.IsTrue(parents.TotalItemCount > 0);

            Console.WriteLine(parents.ToString());
            foreach(Parent parent in parents)
                Console.WriteLine(parent.ObjectToString());
        }

        [TestCase(0, 10)]
        [TestCase(1, 2)]
        public void GetPageByDetachedCriteria(int pageIndex, int pageSize) {
            var whereName = WhereNameIn("Parent1", "Parent2", "Parent3", "Parent4");
            var parents = Repository<Parent>.GetPage(pageIndex, pageSize, whereName, Order.Asc("Name"));

            Assert.IsTrue(parents.TotalItemCount > 0);

            Console.WriteLine(parents.ToString());
            foreach(Parent parent in parents)
                Console.WriteLine(parent.ObjectToString());
        }

        #endregion

        #region << FindAll >>

        /// <summary>
        /// Parent-Child를 fetch.join 형식으로 가져오기 때문에 Parent는 중복된 것이 들어온다.
        /// </summary>
        [Test]
        public void FindAll_WillNotRemoveDuplicates() {
            ICollection<Parent> loaded = Repository<Parent>.FindAll();

            foreach(Parent parent in loaded)
                Console.WriteLine(parent.ToString(true));

            Assert.AreEqual(parentsInDB.Count, loaded.Count, "Children 속성의 bag 값의 fetch mode가 join이므로 child 갯수만큼 parent 수도 중복되어 가져온다.");

            Assert.AreEqual(parentsInDB.Count, loaded.Distinct().Count());
        }

        [Test]
        public void FindAll_MatchingCriteria() {
            // 중복되는 이름을 가진 새로운 Parent객체를 만들어서 DB에 저장한다.
            //
            CreateParentObjectInDB("Parent4", 1);

            ICollection<Parent> loaded = Repository<Parent>.FindAll(new[] { Restrictions.Eq("Name", "Parent4") });

            Assert.AreEqual(2, loaded.Distinct().Count());
        }

        [Test]
        public void FindAll_Expression() {
            // 중복되는 이름을 가진 새로운 Parent객체를 만들어서 DB에 저장한다.
            //
            CreateParentObjectInDB("Parent4", 1);

            ICollection<Parent> loaded = Repository<Parent>.FindAll(parent => parent.Name == "Parent4");

            Assert.AreEqual(2, loaded.Distinct().Count());
        }

        [Test]
        public void FindAll_MatchingCriterionWithMultipleSortOrders() {
            var secondPosition = CreateParentObjectInDB("ThisTestOnly", 3);
            var firstPosition = CreateParentObjectInDB("ThisTestOnly", 9999);

            CreateParentObjectInDB("NoMatch", 5);

            ICriterion whereName = Restrictions.Eq("Name", "ThisTestOnly");
            ICollection<Parent> loaded = Repository<Parent>.FindAll(OrderByNameAndAgeDescending, whereName);

            AssertCollectionEqual(ExpectedList(firstPosition, secondPosition), loaded);

            foreach(Parent parent in loaded)
                Console.WriteLine(parent.ToString(true));
        }

        [Test]
        public void FindAll_MatchingCriteriaWithMultipleSortOrders() {
            var secondPosition = CreateParentObjectInDB("ThisTestOnly", 3);
            var firstPosition = CreateParentObjectInDB("ThisTestOnly", 9999);

            CreateParentObjectInDB("NoMatch", 5);

            ICollection<Parent> loaded
                = Repository<Parent>.FindAll(WhereNameEquals("ThisTestOnly"), OrderByNameAndAgeDescending);

            AssertCollectionEqual(ExpectedList(firstPosition, secondPosition), loaded);

            foreach(Parent parent in loaded)
                Console.WriteLine(parent);
        }

        [Test]
        public void FindAll_MatchingCriteriaWithMultipleSortOrders_Paginated() {
            CreateParentObjectInDB("ThisTestOnly", 2);
            CreateParentObjectInDB("ThisTestOnly", 1);

            var secondPosition = CreateParentObjectInDB("ThisTestOnly", 4);
            var thirdPosition = CreateParentObjectInDB("ThisTestOnly", 3);
            var firstPosition = CreateParentObjectInDB("ThisTestOnly_First", 1);

            CreateParentObjectInDB("X", 1);

            var whereName = WhereNameIn("ThisTestOnly", "ThisTestOnly_First");

            ICollection<Parent> loaded
                = Repository<Parent>.FindAll(whereName, 0, 3, OrderByNameAndAgeDescending);

            AssertCollectionEqual(ExpectedList(firstPosition, secondPosition, thirdPosition), loaded);

            foreach(Parent parent in loaded)
                Console.WriteLine(parent);
        }

        [Test]
        public void FindAll_MatchingCriterionWithMultipleSortOrders_Paginated() {
            CreateParentObjectInDB("ThisTestOnly", 2);
            CreateParentObjectInDB("ThisTestOnly", 1);

            var secondPosition = CreateParentObjectInDB("ThisTestOnly", 4);
            var thirdPosition = CreateParentObjectInDB("ThisTestOnly", 3);
            var firstPosition = CreateParentObjectInDB("ThisTestOnly_First", 1);

            CreateParentObjectInDB("X", 1);

            ICriterion whereName = Restrictions.In("Name", new object[] { "ThisTestOnly", "ThisTestOnly_First" });

            ICollection<Parent> loaded
                = Repository<Parent>.FindAll(OrderByNameAndAgeDescending, 0, 3, whereName);

            AssertCollectionEqual(ExpectedList(firstPosition, secondPosition, thirdPosition), loaded);

            foreach(Parent parent in loaded)
                Console.WriteLine(parent);
        }

        [Test]
        public void FindAll_MatchingCriterion_Paginated() {
            CreateParentObjectInDB("X", 1);
            CreateParentObjectInDB("ThisTestOnly", 1);
            CreateParentObjectInDB("ThisTestOnly", 2);
            CreateParentObjectInDB("ThisTestOnly", 3);

            ICriterion whereName = Restrictions.Eq("Name", "ThisTestOnly");
            var loaded = Repository<Parent>.FindAll(0, 2, new[] { whereName });

            Assert.IsNotNull(loaded);
            Assert.AreEqual(2, loaded.Count, "2 objects returned");
            Assert.AreEqual("ThisTestOnly", loaded.First().Name, "first expected object returned 'ThisTestOnly' ");
            Assert.AreEqual("ThisTestOnly", loaded.Last().Name, "second expected object returned 'ThisTestOnly' ");
        }

        #endregion

        #region << FindOne - UniqueResult >>

        [Test]
        public void FindOne_With_DetachedCritera() {
            var parents = LoadAllWithDistinct<Parent>(Order.Asc("Name"));

            parents[0].Children[0].Name = "A";
            parents[0].Children[1].Name = "B";
            parents[1].Children[0].Name = "B";
            parents[1].Children[1].Name = "C";

            UnitOfWork.Current.Flush();

            // run test
            var where = DetachedCriteria.For<Parent>()
                .CreateAlias("Children", "c")
                .Add(Restrictions.Eq("c.Name", "A"));

            var matched = Repository<Parent>.FindOne(where);
            Assert.AreEqual(matched, parents[0]);
        }

        [Test]
        public void FindOne_With_Criterion() {
            var founded = Repository<Parent>.FindOne(new[] { Restrictions.Eq("Name", "Parent1") });
            Assert.AreEqual(parentsInDB[0], founded);

            Assert.IsFalse(Repository<Parent>.IsTransient(founded));
        }

        [Test]
        public void FindOne_With_Expression() {
            var founded = Repository<Parent>.FindOne(parent => parent.Name == "Parent1");
            Assert.AreEqual(parentsInDB[0], founded);

            Assert.IsFalse(Repository<Parent>.IsTransient(founded));
        }

        [Test]
        [ExpectedException(typeof(NonUniqueResultException))]
        public void FindOne_Will_Throws_NonUniqueResult() {
            CreateParentObjectInDB("Parent1", 1);

            Console.WriteLine("Count of Parent which name is 'Parent1' : " + Repository<Parent>.Count(WhereNameIn("Parent1")));
            Repository<Parent>.FindOne(WhereNameIn("Parent1"));
        }

        #endregion

        #region << FindFirst >>

        [Test]
        public void FindFirst_Matching_Criteria() {
            var first = Repository<Parent>.FindFirst(new[] { Restrictions.Eq("Name", "Parent1") });
            Assert.AreEqual(parentsInDB[0], first);
        }

        [Test]
        public void FindFirst_With_Ordering_And_Criteria() {
            var match = Repository<Parent>.FindFirst(WhereNameIn("Parent1", "Parent4"), OrderByNameAndAgeDescending);
            Assert.AreEqual(parentsInDB[2], match);
        }

        [Test]
        public void FindFirst_With_Ordering() {
            var match = Repository<Parent>.FindFirst(new Order[] { Order.Asc("Name") });
            Assert.AreEqual(parentsInDB[0], match);
        }

        [Test]
        public void FindFirst_Expression() {
            var first = Repository<Parent>.FindFirst(p => p.Name == "Parent1");
            Assert.AreEqual(parentsInDB[0], first);
        }

        #endregion

        #region << Count >>

        [Test]
        public void Count_All() {
            long count = Repository<Parent>.Count();
            Console.WriteLine("Count of All Parents : " + count);

            Assert.AreEqual(parentsInDB.Count, Repository<Parent>.Count());
        }

        [Test]
        public void Count_With_Criteria() {
            Assert.AreEqual(1, Repository<Parent>.Count(WhereNameEquals(parentsInDB[0].Name)));
        }

        #endregion

        #region << Exists >>

        [Test]
        public void Exists_All() {
            Assert.IsTrue(Repository<Parent>.Exists());
        }

        [Test]
        public void Exist_With_Criteria() {
            Assert.IsFalse(Repository<Parent>.Exists(WhereNameEquals("No object with this name")));
            Assert.IsTrue(Repository<Parent>.Exists(WhereNameEquals(parentsInDB[0].Name)));
        }

        #endregion

        #region << ReportOne >>

        [Test]
        public void ReportOne_MatchingCriteria() {
            var criteria = Repository<Parent>.CreateDetachedCriteria().Add(Restrictions.Eq("Name", "Parent1"));

            var dto = Repository<Parent>.ReportOne<ParentDTO>(criteria, ProjectByNameAndAge);
            AssertDTOCreatedFrom(parentsInDB[0], dto);
        }

        [Test]
        public void ReportOne_MatchingCriterion() {
            ICriterion eqName = Restrictions.Eq("Name", "Parent1");
            ICriterion eqAge = Restrictions.Eq("Age", 100);

            var dto = Repository<Parent>.ReportOne<ParentDTO>(ProjectByNameAndAge, new[] { eqName, eqAge });
            AssertDTOCreatedFrom(parentsInDB[0], dto);
        }

        [Test]
        [ExpectedException(typeof(NonUniqueResultException))]
        public void ReportOne_WillThrowIfMoreThanOneMatch() {
            ICriterion eqName = Restrictions.Like("Name", "Parent", MatchMode.Start);
            Repository<Parent>.ReportOne<ParentDTO>(ProjectByNameAndAge, new[] { eqName });
        }

        #endregion

        #region << ReportAll >>

        [Test]
        public void ReportAll() {
            var dtos = Repository<Parent>.ReportAll<ParentDTO>(ProjectByNameAndAge);

            Assert.AreEqual(parentsInDB.Count, dtos.Count);
            AssertDTOsCreatedFrom(parentsInDB, dtos);
        }

        [Test]
        public void ReportAll_Distinct() {
            var parents = LoadAllWithDistinct<Parent>(Order.Asc("Name"));

            parents[0].Age = parents[1].Age;
            parents[0].Name = parents[1].Name;

            UnitOfWork.Current.Flush();

            var dtos = Repository<Parent>.ReportAll<ParentDTO>(ProjectByNameAndAge, true);

            Assert.AreEqual(parentsInDB.Count - 1, dtos.Count);
        }

        // TODO: Projection에 Distinct가 있고, Criteria가 없을 경우에는 예외가 발생한다.!!!
        // TODO: 이 경우를 발생시킨 경우가 있다만... 어떻게 해야 할지... (RealAdmin-2의 Code의 Distinct Group 정보만 가져오기로 테스트가 가능할 것이다.)
        [Test]
        public void ReportAll_With_Order() {
            ICollection<ParentDTO> dtos = Repository<Parent>.ReportAll<ParentDTO>(ProjectByNameAndAge, new[] { Order.Desc("Name") });

            AssertDTOsCreatedFrom(parentsInDB, dtos);
            AssertDtosSortedByName(dtos, "Desc");
        }

        [Test]
        public void ReportAll_Matching_Criteria() {
            var eqName = DetachedCriteria.For<Parent>().Add(Restrictions.Eq("Name", parentsInDB[1].Name));

            ICollection<ParentDTO> dtos = Repository<Parent>.ReportAll<ParentDTO>(eqName, ProjectByNameAndAge);

            Assert.AreEqual(1, dtos.Count);
            AssertDTOCreatedFrom(parentsInDB[1], dtos.First());
        }

        [Test]
        public void ReportAll_Matching_Criteria_With_Order() {
            var eqName = DetachedCriteria.For<Parent>().Add(Restrictions.Like("Name", "Parent%"));

            ICollection<ParentDTO> dtos
                = Repository<Parent>.ReportAll<ParentDTO>(eqName,
                                                          ProjectByNameAndAge,
                                                          Order.Desc("Name"));

            Assert.AreEqual(parentsInDB.Count, dtos.Count);
            AssertDtosSortedByName(dtos, "Desc");
        }

        [Test]
        public void ReportAll_Matching_Criterion() {
            ICriterion eqName = Restrictions.Eq("Name", parentsInDB[0].Name);
            ICriterion eqAge = Restrictions.Eq("Age", 100);

            ICollection<ParentDTO> dtos
                = Repository<Parent>.ReportAll<ParentDTO>(ProjectByNameAndAge, new[] { eqName, eqAge });

            Assert.AreEqual(1, dtos.Count);
            AssertDTOCreatedFrom(parentsInDB[0], dtos.First());
        }

        [Test]
        public void ReportAll_Matching_Criterion_With_Sorting() {
            ICriterion likeName = Restrictions.Like("Name", "Parent%");

            Order[] orderBy = { Order.Desc("Name") };

            ICollection<ParentDTO> dtos
                = Repository<Parent>.ReportAll<ParentDTO>(ProjectByNameAndAge, orderBy, likeName);

            Assert.AreEqual(parentsInDB.Count, dtos.Count);
            AssertDtosSortedByName(dtos, "Desc");
        }

        #endregion

        #region << Delete >>

        [Test]
        public void DeleteAll() {
            Repository<Parent>.DeleteAll();
            UnitOfWork.Current.Flush();
            Assert.AreEqual(0, Repository<Parent>.Count());
        }

        [Test]
        public void DeleteAll_With_Criteria() {
            Repository<Parent>.DeleteAll(WhereNameEquals(parentsInDB[0].Name)); // 한개 삭제
            UnitOfWork.Current.Flush();

            Assert.AreEqual(parentsInDB.Count - 1, Repository<Parent>.Count());
        }

        [Test]
        public void Delete_Entity() {
            Repository<Parent>.Delete(parentsInDB[0]);
            UnitOfWork.Current.Flush();

            Assert.AreEqual(parentsInDB.Count - 1, Repository<Parent>.Count());
        }

        #endregion

        #region << ExecuteUpdate >>

        [Test]
        public void ExecuteUpdate_Update() {
            var oldAge = parentsInDB[0].Age;

            const string hql = "UPDATE VERSIONED Parent p SET p.Age = p.Age+1";
            var count = Repository<Parent>.ExecuteUpdate(hql);
            Assert.IsTrue(count > 0);

            Repository<Parent>.Session.Evict(parentsInDB[0]);
            var parent = Repository<Parent>.FindOneByHql("from Parent p where p.Name=:Name",
                                                         new NHParameter("Name", "Parent1", TypeFactory.GetAnsiStringType(255)));
            Assert.IsNotNull(parent);

            Assert.AreEqual(oldAge + 1, parent.Age);
        }

        [Test]
        [Ignore("ExecuteUpdate는 ORM처럼 cascade delete를 수행하지 못한다.")]
        public void ExecuteUpdate_Delete() {
            // 자식이 있는 모든 Parent를 제거한다. - 자식도 제거되어야 한다.
            // NOTE : ExecuteUpdate는 ORM처럼 cascade delete를 수행하지 못한다. 즉 자식이 있는 부모를 삭제하려 할 경우 제약조건 위배 예외가 발생한다.
            const string hql = "DELETE Parent p where exists (from p.Children) and p.Name=:Name";

            var count = Repository<Parent>.ExecuteUpdate(hql, new NHParameter("Name", "Parent1", TypeFactory.GetAnsiStringType(255)));

            Assert.IsTrue(count > 0);

            var exists = Repository<Parent>.ExistsByHql("from Parent p where p.Name=:Name",
                                                        new NHParameter("Name", "Parent1", TypeFactory.GetAnsiStringType(255)));
            Assert.IsFalse(exists);
        }

        [Test]
        public void ExecuteUpdate_Insert() {
            var childCount = parentsInDB[1].Children.Count;

            // NOTE : Child.Name 과 Parent.Name 속성이 같아야 한다. (하나는 String, 하나는 AnsiString이면 에러가 발생한다.
            //
            const string hql = "INSERT into Child(Id, Name, Parent) select p.Id, p.Name, p from Parent p where p.Name=:Name";
            var count = Repository<Parent>.ExecuteUpdate(hql, new NHParameter("Name", "Parent2", TypeFactory.GetAnsiStringType(255)));
            Assert.IsTrue(count > 0);

            Repository<Parent>.Session.Evict(parentsInDB[1]);
            var parent = Repository<Parent>.FindOneByHql("from Parent p where p.Name=:Name",
                                                         new NHParameter("Name", "Parent2", TypeFactory.GetAnsiStringType(255)));
            Assert.AreEqual(childCount + 1, parent.Children.Count);
        }

        [Test]
        public void ExecuteUpdateTransactonal() {
            NHWith.Transaction(delegate {
                                   ExecuteUpdate_Update();
                                   // ExecuteUpdate_Delete();
                                   ExecuteUpdate_Insert();
                               }
                );
        }

        #endregion

        #region << Entity Managements >>

        [Test]
        public void CreateInstanceOfType() {
            var parent = Repository<Parent>.Create();
            Assert.IsNotNull(parent);
            Assert.IsTrue(Repository<Parent>.IsTransient(parent));
        }

        [Test]
        public void UseQuery() {
            var query = Repository<Parent>.CreateQuery("from Parent p where not exists(from p.Children)");
            var loaded = query.List<Parent>();

            Assert.IsNotNull(loaded);
            CollectionAssert.AllItemsAreNotNull(loaded.ToArray());
        }

        #endregion
    }
}