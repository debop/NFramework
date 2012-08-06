using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Data.DevartOracle.NH.Domains.Models;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Threading;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains {
    [TestFixture]
    public class BulkInsertFixture : FluentDomainTestCaseBase {
        private const int SampleCount = 100000;

        private static readonly Random Rnd = new ThreadSafeRandom();

        private static ProjectTaskTimeSheet CreateTimeSheet() {
            var timeRange = new TimeRange(DateTime.Today.AddDays(Rnd.Next(0, 356)), TimeSpan.FromDays(1));

            return new ProjectTaskTimeSheet(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
                   {
                       StartTime = timeRange.Start,
                       EndTime = timeRange.End,
                       PlanCostValue = Rnd.Next(0, 100).AsDecimal(),
                       PlanProgressValue = Rnd.Next(0, 100),
                       PlanWeightValue = Rnd.Next(0, 100).AsDecimal(),
                       PlanWorkValue = Rnd.Next(0, 100).AsDecimal(),
                       ActualCostValue = Rnd.Next(0, 100).AsDecimal(),
                       ActualProgressValue = Rnd.Next(0, 100),
                       ActualWeightValue = Rnd.Next(0, 100).AsDecimal(),
                       ActualWorkValue = Rnd.Next(0, 100).AsDecimal(),
                       Creator = "debop",
                       LastUpdator = "debop",
                       CreateDate = DateTime.Today
                   };
        }

        private static IList<ProjectTaskTimeSheet> TimeSheets;

        protected override void OnTestFixtureSetUp() {
            base.OnTestFixtureSetUp();

            TimeSheets =
                Enumerable
                    .Range(0, SampleCount)
                    .Select(i => CreateTimeSheet())
                    .ToList();
        }

        protected override void OnSetUp() {
            base.OnSetUp();

            NHTool.DeleteStatelessByHql("from TimeSheetBase");
            //Repository<ProjectTaskTimeSheet>.DeleteAll();
            //UnitOfWork.Current.TransactionalFlush();
        }

        [Test]
        public void InsertTimeSheet() {
            var timeSheet = CreateTimeSheet();

            timeSheet.InsertStateless();

            var loaded = Repository<ProjectTaskTimeSheet>.Get(timeSheet.Id);

            Assert.AreEqual(timeSheet.Id, loaded.Id);
            Assert.AreEqual(timeSheet.ProjectId, loaded.ProjectId);
            Assert.AreEqual(timeSheet.TaskId, loaded.TaskId);

            Repository<ProjectTaskTimeSheet>.Delete(loaded);
            UnitOfWork.Current.TransactionalFlush();
        }

        [TestCase(1000)]
        [TestCase(7000)]
        public void BulkInsertTimeSheetAsSerialByStateless(int count) {
            using(new OperationTimer("Build Insert ProjectTask TimeSeet " + count)) {
                TimeSheets.Take(count).InsertStateless();
            }
        }

        [TestCase(1000)]
        [TestCase(7000)]
        public void BulkInsertTimeSheetAsParallel(int count) {
            using(new OperationTimer("Build Insert ProjectTask TimeSeet " + count)) {
                ParallelTool.ForRange(0,
                                      count,
                                      (s, e, loop) => {
                                          using(UnitOfWork.Start(UnitOfWorkNestingOptions.CreateNewOrNestUnitOfWork))
                                              TimeSheets.Skip(s).Take(e - s).InsertStateless(UnitOfWork.CurrentSession);
                                      });
            }
        }
    }
}