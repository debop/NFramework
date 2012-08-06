using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Ado.Pallelism {
    /// <summary>
    /// AdoRepository 작업을 병렬로 수행하는 방식에 대한 테스트입니다.
    /// Task, Parallel.ForEach, PLinq 방식 모두 병렬 수행에 있어 비슷한 성능을 냅니다.
    /// NOTE: Data Loading 작업은 IO-Bound 작업이므로, MaxDegreeParallelism 을 IO-Bound 에 맞춰 높혀줘야, 병렬 수행이 효과가 좋다.
    /// </summary>
    [TestFixture]
    public class AdoRepositoryInParallelFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly string[] Sections = new[]
                                                    {
                                                        "Customer", "Order", "Order Details", "Products", "Employees", "Invoices",
                                                        "Orders Qry"
                                                    };

        private static readonly string[] Sections2 =
            Sections.Concat(Sections).Concat(Sections).Concat(Sections).Concat(Sections)
                .Concat(Sections).Concat(Sections).Concat(Sections).Concat(Sections)
                //.Concat(Sections).Concat(Sections).Concat(Sections).Concat(Sections)
                //.Concat(Sections).Concat(Sections).Concat(Sections).Concat(Sections)
                //.Concat(Sections).Concat(Sections).Concat(Sections).Concat(Sections)
                .ToArray();

        private const string QueryName = "GetAll";

        private static readonly int DegreeOfParallelism = Environment.ProcessorCount * 4;

        //[SetUp]
        //public void SetUp()
        //{
        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    GC.Collect();
        //}

        //[TearDown]
        //public void TearDown()
        //{
        //    GC.Collect();
        //    GC.WaitForPendingFinalizers();
        //    GC.Collect();
        //}

        /// <summary>
        /// 성능 테스트를 위해 미리 RDBMS 데이터를 캐시하도록 합니다.
        /// </summary>
        [Test]
        public void AWarmUpTablesForAccuracy() {
            foreach(var section in Sections) {
                var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryName);
                using(var table = NorthwindAdoRepository.ExecuteDataTable(query)) {
                    Assert.IsFalse(table.HasErrors);
                }
            }
        }

        /// <summary>
        /// 기존 순차방식의 Data Loading입니다.
        /// </summary>
        [Test]
        public void LoadDataSetBySerial() {
            foreach(var section in Sections2) {
                var queryKey = section + "," + QueryName;

                var ds = NorthwindAdoRepository.ExecuteDataSet(queryKey);
                Assert.AreEqual(1, ds.Tables.Count);
                Assert.IsFalse(ds.Tables[0].HasErrors);

                if(IsDebugEnabled)
                    log.Debug("[{0}] Table을 로드하는데 성공했습니다.", ds.Tables[0].TableName);

                ds.Dispose();
            }
        }

        /// <summary>
        /// 여러개의 쿼리문을 비동기적으로 실행하여, DataSet을 가져옵니다. 동기적으로 실행 시보다 성능이 향상됩니다.
        /// </summary>
        [Test]
        public void LoadDataSetByTasks() {
            //var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount * 8 };

            foreach(var section in Sections2) {
                string queryKey = section + ", " + QueryName;

                // 쿼리 조회 작업
                var ds = NorthwindAdoRepository.ExecuteDataSetAsync(queryKey).Result;

                Assert.AreEqual(1, ds.Tables.Count);
                Assert.IsFalse(ds.Tables[0].HasErrors);

                if(IsDebugEnabled)
                    log.Debug("[{0}] Table을 로드하는데 성공했습니다.", ds.Tables[0].TableName);

                ds.Dispose();
            }
        }

        /// <summary>
        /// Parallel.ForEach를 이용하여 여러 Query문을 병렬로 실행하는 예제입니다.
        /// </summary>
        [Test]
        public void LoadDataSetByParallelForEach() {
            var options = new ParallelOptions { MaxDegreeOfParallelism = DegreeOfParallelism };

            Parallel.ForEach(Sections2,
                             options,
                             (section, loopState) => {
                                 var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryName);

                                 using(var cmd = NorthwindAdoRepository.GetCommand(query)) {
                                     var ds = NorthwindAdoRepository.ExecuteDataSet(cmd);
                                     Assert.AreEqual(1, ds.Tables.Count);
                                     Assert.IsFalse(ds.Tables[0].HasErrors);

                                     if(IsDebugEnabled)
                                         log.Debug("[{0}] Table을 로드하는데 성공했습니다.", ds.Tables[0].TableName);

                                     ds.Dispose();
                                 }
                             });
        }

        /// <summary>
        /// PLINQ를 이용하여 여러 Query문을 병렬로 실행하는 예제입니다. - 순서에 상관없다면, Parallel.ForEach가 더 유연하고 좋은 선택입니다.
        /// </summary>
        [Test]
        public void LoadDataSetByPLinqForAll() {
            //var results = new ConcurrentDictionary<string, DataSet>();

            //Sections2
            // .Do(section => log.Debug("이건 LINQ 작업을 확인하기 위한 함수입니다... ㅋㅋ... 지금 수행하고자 하는 Table은 " + section + "입니다."))
            //.AsParallel()
            //.WithDegreeOfParallelism(DegreeOfParallelism)
            //.ForAll(section =>
            Parallel.ForEach(Sections2,
                             section => {
                                 var query = NorthwindAdoRepository.QueryProvider.GetQuery(section, QueryName);
                                 using(var cmd = NorthwindAdoRepository.GetCommand(query))
                                 using(var ds = NorthwindAdoRepository.ExecuteDataSet(cmd)) {
                                     Assert.AreEqual(1, ds.Tables.Count);
                                     Assert.IsFalse(ds.Tables[0].HasErrors);

                                     if(IsDebugEnabled)
                                         log.Debug("[{0}] Table을 로드하는데 성공했습니다.", ds.Tables[0].TableName);
                                 }
                             });
        }
    }
}