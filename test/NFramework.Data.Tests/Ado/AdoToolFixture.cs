using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Transactions;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.Reflections;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data {
    // NOTE : 테스트를 위해서는 Samples\Database\Northwind for Rcl.Data Testing.sql 를 실행해야 합니다.
    /// <summary>
    /// 테스트를 위해서는 Samples\Database\Northwind for Rcl.Data Testing.sql 를 실행해야 합니다.
    /// </summary>
    [TestFixture]
    public class AdoToolFixture : AdoFixtureBase {

        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Saver Or Update a persistent object by Fluent  >>

        /// <summary>
        /// 지정한 Entity의 Parameter 세팅없이 NameMapping으로 저장한다.
        /// </summary>
        [Test]
        public void SetParameterValuesGeneric() {
            var category = new Category {
                CategoryName = "Test",
                Description = "FluentUtil"
            };

            // delete exist category
            NorthwindAdoRepository.ExecuteNonQueryBySqlString(
                @"DELETE FROM Categories where CategoryName = @CategoryName",
                new AdoParameter("CategoryName", category.CategoryName, DbType.String, 255));

            // insert
            using(var command = NorthwindAdoRepository.GetProcedureCommand("SaveOrUpdateCategory", true)) {
                AdoTool.SetParameterValues(NorthwindAdoRepository.Db,
                                           command,
                                           category,
                                           command.Mapping(NameMappingUtil.CapitalizeMappingFunc('_', ' ')));


                category.CategoryId = NorthwindAdoRepository.ExecuteCommand(command).AsInt(-1);
                Assert.AreNotEqual(-1, category.CategoryId);
            }

            // update
            using(var command = NorthwindAdoRepository.GetProcedureCommand("SaveOrUpdateCategory", true)) {
                AdoTool.SetParameterValues(NorthwindAdoRepository.Db,
                                           command,
                                           category,
                                           command.Mapping(NameMappingUtil.CapitalizeMappingFunc('_', ' ')));

                category.CategoryId = NorthwindAdoRepository.ExecuteCommand(command).AsInt(-1);
                Assert.AreNotEqual(-1, category.CategoryId);
            }
        }

        #endregion

        #region << Count Of DataReader >>

        /// <summary>
        /// SQL 의 count 함수를 사용하지 않고, DataReader의 Record 수를 계산한다. 속도는 조금 느리지만, 
        /// Count 를 위한 SQL 문장을 따로 만들 필요가 없어서 좋다.
        /// 
        /// 어떤 SQL 문장으로 Paging된 Data를 얻을 때 사용하면 좋다.(전체 Page 사이즈를 알 필요가 없으므로)
        /// </summary>
        /// <param name="firstResult"></param>
        /// <param name="maxResults"></param>
        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 0)]
        [TestCase(5, 5)]
        [TestCase(0, 100)]
        [TestCase(100, 10)]
        public void CountOfDataReader(int firstResult, int maxResults) {
            const string SQL = "CustOrderHist";

            int totalCount;
            int maxCount;

            using(var reader = NorthwindAdoRepository.ExecuteReader(SQL, CustomerTestParameter))
                totalCount = reader.Count();

            using(var reader = NorthwindAdoRepository.ExecuteReader(SQL, CustomerTestParameter)) {
                var histories = reader.Map<CustomerOrderHistory>(TrimMapper, firstResult, maxResults);
                maxCount = histories.Count;
            }

            Assert.IsTrue(totalCount >= maxCount);
        }

        #endregion

        [Test]
        public void Can_GetCommonPropertyNames() {
            var propertyNames = AdoTool.GetCommonPropertyNames(typeof(Category), "CategoryID", "CATEGORYNAME").ToList();

            var ignoreCaseComparer = StringComparer.Create(CultureInfo.InvariantCulture, true);

            Assert.IsTrue(propertyNames.Contains("CategoryID", ignoreCaseComparer));
            Assert.IsTrue(propertyNames.Contains("CATEGORYNAME", ignoreCaseComparer));

            Assert.IsFalse(propertyNames.Contains("CategoryID"));
            Assert.IsFalse(propertyNames.Contains("CATEGORYNAME"));
        }

        #region << Convert All From DataReader >>

        private static IDataReader GetCustomerOrderHistoryDataReader(string customerId) {
            return NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist2", new AdoParameter("CustomerId", customerId));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        public void MapFromDataReader(int firstResult, int maxResults) {

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {

                var orderHistories = reader.Map<CustomerOrderHistory>(firstResult, maxResults);

                if(log.IsInfoEnabled)
                    Console.WriteLine(orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        public void MapFromDataReaderExcludeProperty(int firstResult, int maxResults) {

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {

                var orderHistories = reader.Map<CustomerOrderHistory>(firstResult, maxResults, x => x.Total).OrderBy(x => x.Total).ToList();

                if(log.IsInfoEnabled)
                    Console.WriteLine(orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);

                // Total 속성 값은 설정되지 않기 때문에, 모두 초기 값이어야 한다.
                orderHistories.All(x => x.Total == 0).Should().Be.True();
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        public void MapFromDataReaderByPropertyNameMapping(int firstResult, int maxResults) {

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {

                var orderHistories = reader.Map<CustomerOrderHistory>(NameMappings, firstResult, maxResults);

                if(log.IsInfoEnabled)
                    Console.WriteLine(orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        public void MapFromDataReaderByConverter(int firstResult, int maxResults) {

            Func<IDataReader, CustomerOrderHistory> @mapFunc
                = delegate(IDataReader dr) {
                var reader = dr;
                return
                    new CustomerOrderHistory {
                        ProductName = reader.AsString("PRODUCTNAME"),
                        Total = reader.AsInt32Nullable("TOTAL")
                    };
            };

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {
                var orderHistories = reader.Map(@mapFunc, firstResult, maxResults);

                if(log.IsInfoEnabled)
                    Console.WriteLine(orderHistories.CollectionToString());
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        public void MapFromDataReaderByPersister(int firstResult, int maxResults) {
            IReaderPersister<CustomerOrderHistory> readerPersister = new CapitalizeReaderPersister<CustomerOrderHistory>();

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist2", CustomerTestParameter)) {
                var orderHistories = reader.Map(readerPersister, firstResult, maxResults);
                Assert.IsTrue(orderHistories.Count > 0);
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 15)]
        [TestCase(5, 10)]
        public void MapFromDataReaderByNameMappingFuncs(int firstResult, int maxResults) {

            // CustOrderHist2 Procedure를 만들어야 한다.  (Column 명을 대문자, '_'로 바꾼다. 즉 PRODUCT_NAME, TOTAL 로 column명만 바꾼다
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist2", CustomerTestParameter)) {
                var orderHistories =
                    reader.Map<CustomerOrderHistory>(reader.Mapping(NameMappingUtil.CapitalizeMappingFunc('_', ' ')),
                                                     firstResult,
                                                     maxResults);
                if(log.IsInfoEnabled)
                    Console.WriteLine(orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        [Test]
        [TestCase("ANATR", 0, 0)]
        [TestCase("ANATR", 1, 5)]
        [TestCase("ANATR", 0, 0)]
        [TestCase("ANATR", 5, 100)]
        public void ConvertAllFromDataReaderWithPaging(string customerId, int firstResult, int maxResults) {

            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                var orderHistories = reader.Map<CustomerOrderHistory>(CapitalizeMapper,
                                                                      firstResult,
                                                                      maxResults);

                if(maxResults > 0)
                    Assert.IsTrue(orderHistories.Count <= maxResults);

                if(log.IsInfoEnabled)
                    Console.WriteLine(orderHistories.CollectionToString());
            }
        }

        #endregion

        #region << Convert All From DataTable >>

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 15)]
        [TestCase(5, 15)]
        public void ConvertAllFromDataTable(int firstResult, int maxResults) {
            using(var dt = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist",
                                                                              firstResult,
                                                                              maxResults,
                                                                              CustomerTestParameter)) {
                Assert.IsFalse(dt.HasErrors);
                Assert.Greater(dt.Rows.Count, 1);

                var orderHistories = AdoTool.Map<CustomerOrderHistory>(dt);

                if(IsDebugEnabled)
                    log.Debug("OrderHistories:" + orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 15)]
        [TestCase(5, 15)]
        public void ConvertAllFromDataTableByPropertyNameMapping(int firstResult, int maxResults) {
            using(
                var dt = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist",
                                                                            firstResult,
                                                                            maxResults,
                                                                            CustomerTestParameter)) {
                Assert.IsFalse(dt.HasErrors);
                Assert.Greater(dt.Rows.Count, 1);

                var orderHistories = AdoTool.Map<CustomerOrderHistory>(dt, NameMappings);

                if(IsDebugEnabled)
                    log.Debug("OrderHistories:" + orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 15)]
        [TestCase(5, 15)]
        public void ConvertAllFromDataTableByConverter(int firstResult, int maxResults) {
            Func<DataRow, CustomerOrderHistory> @mapFunc =
                row =>
                new CustomerOrderHistory {
                    ProductName = row["PRODUCTNAME"].AsText(),
                    Total = row["TOTAL"].AsIntNullable()
                };

            using(var dt = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist",
                                                                              firstResult,
                                                                              maxResults,
                                                                              CustomerTestParameter)) {
                Assert.IsFalse(dt.HasErrors);
                Assert.Greater(dt.Rows.Count, 1);

                var orderHistories = AdoTool.Map(dt, @mapFunc);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 15)]
        [TestCase(5, 10)]
        public void ConvertAllFromDataTableByNameMappingFuncs(int firstResult, int maxResults) {
            // CustOrderHist2 Procedure를 만들어야 한다.  (Column 명을 대문자, '_'로 바꾼다. 즉 PRODUCT_NAME, TOTAL 로 column명만 바꾼다
            using(var dt = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist2",
                                                                              firstResult,
                                                                              maxResults,
                                                                              CustomerTestParameter)) {
                Assert.IsFalse(dt.HasErrors);
                Assert.Greater(dt.Rows.Count, 1);

                var orderHistories =
                    AdoTool.Map<CustomerOrderHistory>(dt,
                                                      dt.Mapping(NameMappingUtil
                                                                     .CapitalizeMappingFunc('_', ' ')));

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        [Test]
        [TestCase("ANATR", 0, 0)]
        [TestCase("ANATR", 1, 5)]
        [TestCase("ANATR", 0, 5)]
        [TestCase("ANATR", 5, 100)]
        public void ConvertAllFromDataTableByPaging(string customerId, int firstResult, int maxResults) {
            using(var dt = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist2",
                                                                              firstResult,
                                                                              maxResults,
                                                                              CustomerTestParameter)) {
                Assert.IsFalse(dt.HasErrors);
                Assert.Greater(dt.Rows.Count, 1);

                var orderHistories =
                    AdoTool.Map<CustomerOrderHistory>(dt, CapitalizeMapper);

                if(IsDebugEnabled)
                    log.Debug(orderHistories.CollectionToString());

                Assert.Greater(orderHistories.Count, 0);
            }
        }

        #endregion

        #region << Use TransactionScope >>

        public static void MethodInTransaction() {
            using(var table = NorthwindAdoRepository.ExecuteDataTableBySqlString("SELECT * FROM sysobjects")) {
                Assert.IsFalse(table.HasErrors);
                Console.WriteLine("Data Table has {0} records", table.Rows.Count);
            }
        }

        /// <summary>
        /// 하나의 Transaction Scope 안에서 Method들을 실행한다.
        /// </summary>
        [Test]
        public void UseTransactionScopeTest() {
            AdoWith.TransactionScope(TransactionScopeOption.Required,
                                     System.Transactions.IsolationLevel.ReadCommitted,
                                     SetParameterValuesGeneric,
                                     MethodInTransaction);
        }

        #endregion
    }
}