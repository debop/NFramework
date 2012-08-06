using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx.Domain;
using NUnit.Framework;

namespace NSoft.NFramework.Data.NHibernateEx.WithTests {
    [TestFixture]
    public class WithTransactionFixture : NHRepositoryTestFixtureBase {
        // 어떤 작업을 Transaction 하에서 수행할 수 있다.
        [Test]
        public void TransactionTest() {
            NHWith.Transaction(FindAll_MatchingCriteria);
        }

        /// <summary>
        /// 중첩된 Transaction 하에서도 실행이 가능하다.
        /// </summary>
        [Test]
        public void NestingTransactions() {
            NHWith.Transaction(() => {
                                   FindAll_MatchingCriteria();
                                   Exist_With_Criteria();

                                   NHWith.Transaction(() => {
                                                          Count_With_Criteria();
                                                          FindOne_With_Criterion();
                                                      });

                                   DeleteAll();
                               });
        }

        #region << Methods >>

        private static void FindAll_MatchingCriteria() {
            // 중복되는 이름을 가진 새로운 Parent객체를 만들어서 DB에 저장한다.
            //
            CreateParentObjectInDB("Parent4", 1);

            ICollection<Parent> loaded = Repository<Parent>.FindAll(new[] { Restrictions.Eq("Name", "Parent4") });

            Assert.AreEqual(2, loaded.Distinct().Count());
        }

        private void Exist_With_Criteria() {
            Assert.IsFalse(Repository<Parent>.Exists(WhereNameEquals("No object with this name")));
            Assert.IsTrue(Repository<Parent>.Exists(WhereNameEquals(parentsInDB[0].Name)));
        }

        private void Count_With_Criteria() {
            Assert.AreEqual(1, Repository<Parent>.Count(WhereNameEquals(parentsInDB[0].Name)));
        }

        private void FindOne_With_Criterion() {
            var founded = Repository<Parent>.FindOne(new[] { Restrictions.Eq("Name", "Parent1") });
            Assert.AreEqual(parentsInDB[0], founded);

            Assert.IsFalse(Repository<Parent>.IsTransient(founded));
        }

        private void DeleteAll() {
            Repository<Parent>.DeleteAll();
            UnitOfWork.Current.Flush();
            Assert.AreEqual(0, Repository<Parent>.Count());
        }

        #endregion
    }

    [TestFixture]
    public class WithTransactionFixture_SQLite : WithTransactionFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.SQLiteForFile;
        }
    }

    [TestFixture]
    public class WithTransactionFixture_Oracle : WithTransactionFixture {
        protected override DatabaseEngine GetDatabaseEngine() {
            return DatabaseEngine.DevartOracle;
        }
    }
}