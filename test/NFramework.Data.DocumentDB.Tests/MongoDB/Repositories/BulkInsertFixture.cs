using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Threading;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Data.MongoDB.Repositories {
    [TestFixture]
    public class BulkInsertFixture : MongoFixtureBase {
        private readonly object _syncLock = new object();

        private MongoRepositoryImpl _repository;

        public MongoRepositoryImpl Repository {
            get {
                if(_repository == null)
                    lock(_syncLock)
                        if(_repository == null) {
                            var repository = new MongoRepositoryImpl(DefaultConnectionString);
                            Thread.MemoryBarrier();
                            _repository = repository;
                        }
                return _repository;
            }
        }

        private const int SampleCount = 100000;

        private static readonly Random Rnd = new ThreadSafeRandom();

        private static ProjectTaskTimeSheet CreateTimeSheet() {
            return new ProjectTaskTimeSheet(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
                   {
                       StartTime = DateTime.Now,
                       EndTime = DateTime.Now.AddDays(Rnd.Next(0, 1000)),
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
            Repository.DropAllCollection();
        }

        protected override void OnTestFixtureTearDown() {
            base.OnTestFixtureTearDown();
            Repository.DropAllCollection();
        }

        [TestCase(10000)]
        [TestCase(70000)]
        public void InsertAsSerial(int count) {
            using(new OperationTimer("InsertAsSerial - " + count))
                foreach(var timesheet in TimeSheets.Take(count))
                    Repository.Insert(timesheet);
        }

        [TestCase(10000)]
        [TestCase(70000)]
        public void InsertAsParallel(int count) {
            using(new OperationTimer("InsertAsParallel - " + count)) {
                ParallelTool.ForRange(0,
                                      count,
                                      (s, e, loop) => {
                                          foreach(var timesheet in TimeSheets.Skip(s).Take(e - s))
                                              Repository.Insert(timesheet);
                                      });
            }
        }

        [Serializable]
        public abstract class TimeSheetBase {
            protected TimeSheetBase() {
                Id = Guid.NewGuid();
            }

            protected TimeSheetBase(Guid id) {
                Id = id;
            }

            public virtual Guid Id { get; set; }

            /// <summary>
            /// 시작일
            /// </summary>
            public virtual DateTime StartTime { get; set; }

            /// <summary>
            /// 종료일
            /// </summary>
            public virtual DateTime EndTime { get; set; }

            /// <summary>
            /// 보할 계획 값
            /// </summary>
            public virtual Decimal? PlanWeightValue { get; set; }

            /// <summary>
            /// 보할 실적 값
            /// </summary>
            public virtual Decimal? ActualWeightValue { get; set; }

            /// <summary>
            /// 진척률 계획 값
            /// </summary>
            public virtual Decimal? PlanProgressValue { get; set; }

            /// <summary>
            /// 진척률 실적 값
            /// </summary>
            public virtual Decimal? ActualProgressValue { get; set; }

            /// <summary>
            /// 계획 작업량
            /// </summary>
            public virtual Decimal? PlanWorkValue { get; set; }

            /// <summary>
            /// 실적 작업량
            /// </summary>
            public virtual Decimal? ActualWorkValue { get; set; }

            /// <summary>
            /// 계획 비용
            /// </summary>
            public virtual Decimal? PlanCostValue { get; set; }

            /// <summary>
            /// 실적 비용
            /// </summary>
            public virtual Decimal? ActualCostValue { get; set; }

            /// <summary>
            /// 완료 여부
            /// </summary>
            public virtual bool IsComplete { get; set; }

            /// <summary>
            /// 마감 여부
            /// </summary>
            public virtual bool IsClosed { get; set; }

            /// <summary>
            /// 등록자
            /// </summary>
            public virtual string Creator { get; set; }

            /// <summary>
            /// 수정자
            /// </summary>
            public virtual string LastUpdator { get; set; }

            public virtual DateTime? CreateDate { get; set; }

            public virtual DateTime? UpdateTimestamp { get; set; }
        }

        [Serializable]
        public class ProjectTaskTimeSheet : TimeSheetBase {
            protected ProjectTaskTimeSheet() {}
            public ProjectTaskTimeSheet(Guid projectId, Guid taskId) : this(projectId, taskId, Guid.NewGuid()) {}

            public ProjectTaskTimeSheet(Guid projectId, Guid taskId, Guid id) {
                ProjectId = projectId;
                TaskId = taskId;
                base.Id = id;
            }

            public virtual Guid ProjectId { get; protected set; }

            public virtual Guid TaskId { get; protected set; }

            public override int GetHashCode() {
                return HashTool.Compute(ProjectId,
                                        TaskId,
                                        StartTime,
                                        EndTime);
            }
        }
    }
}