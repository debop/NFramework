using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NSoft.NFramework.Data {
    [TestFixture]
    public class AdoWithForTestingFixture : AdoFixtureBase {
        [Test]
        public void NotCommitted() {
            var originalCount = TotalCount();
            AdoWith.ForTesting(() => {
                                   AdoRepository.ExecuteNonQuery(SQL_REGION_INSERT);
                                   AdoRepository.ExecuteNonQuery(SQL_REGION_INSERT2);
                                   var insertedCount = TotalCount();

                                   Assert.AreEqual(originalCount + 2, insertedCount);
                               });

            var rollbackCount = TotalCount();
            Assert.AreEqual(originalCount, rollbackCount);
        }

        public static void InsertActionTest() {
            var originalCount = TotalCount();
            var row = AdoRepository.ExecuteNonQuery(SQL_REGION_INSERT);
            Assert.AreEqual(1, row);
            var row2 = AdoRepository.ExecuteNonQuery(SQL_REGION_INSERT2);
            Assert.AreEqual(1, row2);
            var insertedCount = TotalCount();

            Assert.AreEqual(originalCount + 2, insertedCount);
        }

        [Test]
        public void NotCommittedAction() {
            var originalCount = TotalCount();

            // Run Test
            AdoWith.ForTesting(InsertActionTest);

            // use Extension Method
            Action insertAction = InsertActionTest;
            insertAction.ForTesting();

            Assert.AreEqual(originalCount, TotalCount());
        }

        public static void DeleteActionTest() {
            var row = AdoRepository.ExecuteNonQueryBySqlString(SQL_REGION_DELETE);
        }

        [Test]
        public void MultipleActionsForTesting() {
            IList<Action> actions = new List<Action>
                                    {
                                        DeleteActionTest,
                                        InsertActionTest,
                                        DeleteActionTest,
                                        InsertActionTest,
                                        DeleteActionTest
                                    };

            var originalCount = TotalCount();

            actions.ForTesting(AdoTool.AdoIsolationLevel);

            Assert.AreEqual(originalCount, TotalCount());
        }

        [Test]
        // [ThreadedRepeat(3)]
        public void MultipleActionsForTesting2() {
            var originalCount = TotalCount();

            AdoWith.ForTesting(AdoTool.AdoIsolationLevel,
                               DeleteActionTest,
                               InsertActionTest,
                               DeleteActionTest,
                               InsertActionTest,
                               DeleteActionTest);

            Assert.AreEqual(originalCount, TotalCount());
        }
    }
}