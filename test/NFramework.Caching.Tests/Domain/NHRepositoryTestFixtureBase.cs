using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NSoft.NFramework.Caching.Domain.Model;
using NSoft.NFramework.Cryptography;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Data.NHibernateEx.ForTesting;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Caching.Domain {
    /// <summary>
    /// NSoft.NFramework.Data.NHibernateEx 와 관련된 클래스틑 테스트하기 위한 기본 클래스입니다. 
    /// DB는 기본적으로 SQLite를 사용합니다. 
    /// 변경하고자할 때는 GetDatabaseEngine 을 재정의하세요.
    /// </summary>
    public abstract class NHRepositoryTestFixtureBase : NHDatabaseTestFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        protected override MappingInfo GetMappingInfo() {
            return MappingInfo.From(typeof(Parent).Assembly);
        }

        protected override void OnSetUp() {
            base.OnSetUp();

            Repository<Parent>.DeleteAll();
            UnitOfWork.Current.Flush();

            CreateObjectsInDB();
        }

        protected List<Parent> parentsInDB;

        protected virtual void CreateObjectsInDB() {
            parentsInDB = new List<Parent>();

            parentsInDB.Add(CreateParentObject("Parent1", 100, new Child { Description = ArrayTool.GetRandomBytes(1024).BytesToHex() },
                                               new Child { Description = ArrayTool.GetRandomBytes(1024).BytesToHex() }));
            parentsInDB.Add(CreateParentObject("Parent2", 200, new Child { Description = ArrayTool.GetRandomBytes(1024).BytesToHex() },
                                               new Child { Description = ArrayTool.GetRandomBytes(1024).BytesToHex() }));
            parentsInDB.Add(CreateParentObject("Parent4", 400, new Child { Description = ArrayTool.GetRandomBytes(1024).BytesToHex() }));

            AssumeParentObjectNamesAreUnique(parentsInDB);
            SaveAndFlushToDatabase(parentsInDB);
        }

        private static void AssumeParentObjectNamesAreUnique(IEnumerable<Parent> parents) {
            CollectionAssert.AllItemsAreUnique(parents.GroupBy(p => p.Name).ToArray());
        }

        protected static Parent CreateParentObjectInDB(string name, int age) {
            var parent = CreateParentObject(name, age);
            SaveAndFlushToDatabase(parent);
            return parent;
        }

        protected static Parent CreateParentObject(string name, int age, params Child[] children) {
            var parent = new Parent
                         {
                             Name = name,
                             Age = age,
                             Description = ArrayTool.GetRandomBytes(1024).BytesToHex()
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
    }
}