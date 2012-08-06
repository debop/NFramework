using System;
using System.Data;
using System.Linq;
using NSoft.NFramework.Data.DataObjects.Northwind;
using NSoft.NFramework.Data.Mappers;
using NSoft.NFramework.Data.Persisters;
using NSoft.NFramework.DynamicProxy;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Tools {
    /// <summary>
    /// ADO.NET DataReader, DataTable로부터 Persistent object를 생성하는 메소드에 대한 테스트입니다.
    /// </summary>
    [TestFixture]
    public class AdoToolMapFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        //[SetUp]
        //public void SetUp()
        //{
        //    if(IoC.IsNotInitialized)
        //        IoC.Initialize();
        //}

        #region << DataReader Map >>

        private static IDataReader GetCustomerOrderHistoryDataReader(string customerId) {
            return NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist2", new AdoParameter("CustomerId", customerId));
        }

        [Test]
        public void MapFromDataReader([Values(0, 3, 5)] int firstResult, [Values(0, 10, 50, 200)] int maxResults) {
            // 단순히 TrimMapper 를 이용하여 Persistence를 생성합니다.
            //
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {
                var histories = reader.Map<CustomerOrderHistory>(firstResult, maxResults);

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }

            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(SQL_CUSTOMER_SELECT)) {
                var customers = reader.Map<Customer>(firstResult, maxResults);

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, customers.Count);
                else
                    Assert.Greater(customers.Count, 0);

                customers.All(c => c.CustomerID.IsNotWhiteSpace()).Should().Be.True();
                customers.All(c => c.Address.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [Test]
        public void MapFromDataReader_OrderDetails() {
            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(SQL_ORDER_DETAIL_SELECT)) {
                var orderDetails = reader.Map<OrderDetail>().ToList();

                Assert.Greater(orderDetails.Count, 0);

                orderDetails.All(od => od.OrderID > 0).Should().Be.True();
                orderDetails.All(od => od.ProductID > 0).Should().Be.True();
                orderDetails.All(od => od.Quantity > 0).Should().Be.True();
                orderDetails.All(od => od.Discount >= 0.0f).Should().Be.True();
            }
        }

        [Test]
        public void MapFromDataReader_OrderDetails_DynamicProxy() {
            var capitalizeNameMapper = new CapitalizeNameMapper();
            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(SQL_ORDER_DETAIL_SELECT)) {
                var orderDetails = reader.Map<OrderDetail>(DynamicProxyTool.CreateEditablePropertyChanged<OrderDetail>, CapitalizeMapper);

                Assert.Greater(orderDetails.Count, 0);

                orderDetails.All(od => od.OrderID > 0).Should().Be.True();
                orderDetails.All(od => od.ProductID > 0).Should().Be.True();
                orderDetails.All(od => od.Quantity > 0).Should().Be.True();
                orderDetails.All(od => od.Discount >= 0.0f).Should().Be.True();

                // 모든 엔티티가 Proxy 된 것입니다!!!
                orderDetails.All(od => od.IsDynamicProxy()).Should().Be.True();
            }
        }

        [Test]
        public void MapFromDataReader_Employee() {
            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(SQL_EMPLOYEE_SELECT)) {
                var employees = reader.Map<Employee>(new[] { "Photo" }).ToList();

                employees.Count.Should().Be.GreaterThan(0);
                employees.All(emp => emp.EmployeeID > 0).Should().Be.True();
                employees.All(emp => emp.BirthDate.HasValue).Should().Be.True();

                // Photo 속성은 매핑에서 제외 했습니다.
                employees.All(emp => emp.Photo == null).Should().Be.True();
            }
        }

        [Test]
        public void MapFromDataReader_Employee_With_DynamicProxy() {
            var capitalizeNameMapper = new CapitalizeNameMapper();
            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(SQL_EMPLOYEE_SELECT)) {
                var employees = reader.Map<Employee>(DynamicProxyTool.CreateEditablePropertyChanged<Employee>, capitalizeNameMapper);
                Assert.Greater(employees.Count, 0);

                employees.All(emp => emp.EmployeeID > 0).Should().Be.True();
                employees.All(emp => emp.BirthDate.HasValue).Should().Be.True();

                // 모든 엔티티가 Proxy 된 것입니다!!!
                employees.All(emp => emp.IsDynamicProxy()).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataReader_Employee_Paginated(int firstResult, int maxResults) {
            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(SQL_EMPLOYEE_SELECT)) {
                var employees = reader.Map<Employee>(firstResult, maxResults, new string[0]).ToList();
                Assert.Greater(employees.Count, 0);

                employees.All(emp => emp.EmployeeID > 0).Should().Be.True();
                employees.All(emp => emp.BirthDate.HasValue).Should().Be.True();
                employees.All(emp => emp.Photo != null && emp.Photo.Length > 0).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataReaderByFactoryFunc(int firstResult, int maxResults) {
            // 단순히 TrimMapper 를 이용하여 Persistence를 생성합니다.
            //
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {
                var histories = reader.Map<CustomerOrderHistory>(() => new CustomerOrderHistory("임시", -1), firstResult, maxResults);

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }


            using(var reader = NorthwindAdoRepository.ExecuteReaderBySqlString(SQL_CUSTOMER_SELECT)) {
                var customers = reader.Map<Customer>(() => new Customer(), firstResult, maxResults);

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, customers.Count);
                else
                    Assert.Greater(customers.Count, 0);

                customers.All(c => c.CustomerID.IsNotWhiteSpace()).Should().Be.True();
                customers.All(c => c.Address.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataReaderExcludeProperty(int firstResult, int maxResults) {
            // Total 속성의 값은 매핑하지 않는다.
            //
            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {
                var histories = reader.Map<CustomerOrderHistory>(firstResult, maxResults, h => h.Total);

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                Console.WriteLine(histories.CollectionToString());

                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataReaderByMapFunc(int firstResult, int maxResults) {
            Func<IDataReader, CustomerOrderHistory> @mapper =
                dr => {
                    var reader = dr;
                    return new CustomerOrderHistory
                           {
                               ProductName = reader.AsString("ProductName"),
                               Total = reader.AsInt32Nullable("Total")
                           };
                };

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist", CustomerTestParameter)) {
                var histories = reader.Map(@mapper, firstResult, maxResults).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataReaderByPersister(int firstResult, int maxResults) {
            IReaderPersister<CustomerOrderHistory> readerPersister = new CapitalizeReaderPersister<CustomerOrderHistory>();

            using(var reader = NorthwindAdoRepository.ExecuteReaderByProcedure("CustOrderHist2", CustomerTestParameter)) {
                var histories = reader.Map(readerPersister, firstResult, maxResults).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [TestCase("ANATR", 0, 0)]
        [TestCase("ANATR", 1, 5)]
        [TestCase("ANATR", 0, 0)]
        [TestCase("ANATR", 5, 100)]
        public void MapFromDataReaderWithPaging(string customerId, int firstResult, int maxResults) {
            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                var histories = reader.Map<CustomerOrderHistory>(CapitalizeMapper, firstResult, maxResults).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [Test]
        public void Map_FromDataReader_With_Filter() {
            const string customerId = @"ANATR";

            var capitalizeReaderPersister = new CapitalizeReaderPersister<CustomerOrderHistory>();

            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                // 필터링된 데이타 (모두 통과)
                var histories = reader.MapIf<CustomerOrderHistory>(capitalizeReaderPersister.Persist, dr => true).ToList();

                Assert.Greater(histories.Count, 0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }

            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                // 필터링된 데이타 (모두 거부)
                var histories = reader.MapIf<CustomerOrderHistory>(capitalizeReaderPersister.Persist, dr => false).ToList();

                Assert.AreEqual(0, histories.Count);
            }

            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                // 필터링된 데이타 (Total 이 0보다 큰 레코드만)
                var histories =
                    reader.MapIf<CustomerOrderHistory>(capitalizeReaderPersister.Persist, dr => dr.AsInt32("Total") > 0).ToList();

                Assert.Greater(histories.Count, 0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }
        }

        [Test]
        public void MapWhile_From_DataReader() {
            const string customerId = @"ANATR";

            var capitalizeReaderPersister = new CapitalizeReaderPersister<CustomerOrderHistory>();

            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                // 종료 조건이 항상 False이므로 모든 데이타를 매핑합니다.
                var histories = reader.MapWhile<CustomerOrderHistory>(capitalizeReaderPersister.Persist, dr => true).ToList();

                Assert.Greater(histories.Count, 0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }

            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                // 종료조건이 항상 참이므로, 아무런 데이타도 매핑하지 않습니다.
                var histories = reader.MapWhile<CustomerOrderHistory>(capitalizeReaderPersister.Persist, dr => false).ToList();

                Assert.AreEqual(0, histories.Count);
            }

            using(var reader = GetCustomerOrderHistoryDataReader(customerId)) {
                // Total 값이 0이면, 매핑을 중단합니다.
                var histories =
                    reader.MapWhile<CustomerOrderHistory>(capitalizeReaderPersister.Persist, dr => dr.AsInt32("Total") > 0).ToList();

                Assert.Greater(histories.Count, 0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }
        }

        #endregion

        #region << MapAsParallel from DataReader >>

        [Test]
        public void ParallelMapFromDataReader_OrderDetails() {
            using(var reader = NorthwindAdoRepository.ExecuteReaderAsync(SQL_ORDER_DETAIL_SELECT).Result) {
                var orderDetails = reader.MapAsParallel<OrderDetail>(() => new OrderDetail());
                reader.Dispose();

                Assert.Greater(orderDetails.Count, 0);

                orderDetails.All(od => od.OrderID > 0).Should().Be.True();
                orderDetails.All(od => od.ProductID > 0).Should().Be.True();
                orderDetails.All(od => od.Quantity > 0).Should().Be.True();
                orderDetails.All(od => od.Discount >= 0.0f).Should().Be.True();
            }
        }

        [Test]
        public void ParallelMapFromDataReader_Employee() {
            using(var reader = NorthwindAdoRepository.ExecuteReaderAsync(SQL_EMPLOYEE_SELECT).Result) {
                var employees = reader.MapAsParallel<Employee>(() => new Employee(), e => e.Photo);
                reader.Dispose();

                Assert.Greater(employees.Count, 0);

                employees.All(emp => emp.EmployeeID > 0).Should().Be.True();
                employees.All(emp => emp.Address.IsNotWhiteSpace()).Should().Be.True();
                employees.All(emp => emp.Photo == null).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 5)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void ParallelMapFromDataReader_Employee_Paginated(int firstResult, int maxResults) {
            using(var reader = NorthwindAdoRepository.ExecuteReaderAsync(SQL_EMPLOYEE_SELECT).Result) {
                var employees = reader.MapAsParallel<Employee>(() => new Employee(), firstResult, maxResults, e => e.Photo);

                reader.Dispose();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, employees.Count);
                else
                    Assert.Greater(employees.Count, maxResults);

                employees.All(emp => emp.EmployeeID > 0).Should().Be.True();
                employees.All(emp => emp.Address.IsNotWhiteSpace()).Should().Be.True();
                employees.All(emp => emp.Photo == null).Should().Be.True();
            }
        }

        #endregion

        #region << DataTable Map >>

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        //[TestCase(0, 50)]
        //[TestCase(5, 200)]
        public void MapPersistenceFromDataTable(int firstResult, int maxResults) {
            // 단순히 TrimMapper 를 이용하여 Persistence를 생성합니다.
            //
            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.Map<CustomerOrderHistory>().ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.TrueForAll(h => h.Total.GetValueOrDefault(0) > 0);
            }

            using(var table = NorthwindAdoRepository.ExecuteDataTableBySqlString(SQL_CUSTOMER_SELECT, firstResult, maxResults)) {
                var customers = table.Map<Customer>().ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, customers.Count);
                else
                    Assert.Greater(customers.Count, 0);

                customers.All(c => c.CustomerID.IsNotWhiteSpace()).Should().Be.True();
                customers.All(c => c.Address.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataTableExcludeProperty(int firstResult, int maxResults) {
            // Total 속성의 값은 매핑하지 않는다.
            //
            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.Map<CustomerOrderHistory>(new[] { "Total" }).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(x => x.Total == 0).Should().Be.True();
            }

            using(var table = NorthwindAdoRepository.ExecuteDataTableBySqlString(SQL_CUSTOMER_SELECT, firstResult, maxResults)) {
                var customers = table.Map<Customer>(new string[0]);

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, customers.Count);
                else
                    Assert.Greater(customers.Count, 0);

                customers.All(c => c.CustomerID.IsNotWhiteSpace()).Should().Be.True();
                customers.All(c => c.Address.IsNotWhiteSpace()).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataTableByMapFunc(int firstResult, int maxResults) {
            Func<DataRow, CustomerOrderHistory> @mapper =
                row => {
                    var history = new CustomerOrderHistory
                                  {
                                      ProductName = row["ProductName"].ToString(),
                                      Total = row["Total"].AsIntNullable()
                                  };

                    return history;
                };

            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.Map(@mapper).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void MapFromDataTableByPersister(int firstResult, int maxResults) {
            IRowPersister<CustomerOrderHistory> rowPersister = new CapitalizeRowPersister<CustomerOrderHistory>();

            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist2", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.Map(rowPersister).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);
            }
        }

        [TestCase("ANATR", 0, 0)]
        [TestCase("ANATR", 1, 5)]
        [TestCase("ANATR", 0, 5)]
        [TestCase("ANATR", 5, 100)]
        public void MapFromDataTableByPaging(string customerId, int firstResult, int maxResults) {
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist2",
                                                                                 firstResult,
                                                                                 maxResults,
                                                                                 CustomerTestParameter)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 1);

                var histories = table.Map<CustomerOrderHistory>(CapitalizeMapper).ToList();

                if(IsDebugEnabled)
                    log.Debug(histories.CollectionToString());

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);
            }
        }

        [Test]
        public void MapFromDataRowWithFilter() {
            var capitalizeRowPersister = new CapitalizeRowPersister<CustomerOrderHistory>();

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 필터링된 데이타 (모두 통과)
                var histories = table.MapIf<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => true).ToList();

                Assert.Greater(histories.Count, 0);
                histories.TrueForAll(h => h.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 필터링된 데이타 (모두 거부)
                var histories = table.MapIf<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => false).ToList();

                Assert.AreEqual(0, histories.Count);
            }

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 필터링된 데이타 (Total 이 0보다 큰 레코드만)
                var histories =
                    table.MapIf<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => row["Total"].AsInt(0) > 0).ToList();

                Assert.Greater(histories.Count, 0);
                histories.TrueForAll(h => h.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [Test]
        public void MapWhile_From_DataRow() {
            var capitalizeRowPersister = new CapitalizeRowPersister<CustomerOrderHistory>();

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 계속 조건이 항상 참
                var histories = table.MapWhile<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => true).ToList();

                histories.Count.Should().Be.GreaterThan(0);
                histories.All(h => h.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 계속 조건이 항상 거짓 (모두 거부)
                var histories = table.MapWhile<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => false).ToList();

                histories.Count.Should().Be(0);
            }

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                //  Total 이 0보다 클 때까지만 변환
                var histories =
                    table.MapWhile<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => row["Total"].AsInt(0) > 0).ToList();

                histories.Count.Should().Be.GreaterThan(0);
                histories.All(h => h.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        #endregion

        #region << MapAsParallel from DataTable >>

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void ParallelMapPersistenceFromDataTable(int firstResult, int maxResults) {
            // 단순히 TrimMapper 를 이용하여 Persistence를 생성합니다.
            //
            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.MapAsParallel<CustomerOrderHistory>().ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void ParallelMapFromDataTableExcludeProperty(int firstResult, int maxResults) {
            // Total 속성의 값은 매핑하지 않는다.
            //
            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.MapAsParallel<CustomerOrderHistory>("Total").ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault(0) == 0).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void ParallelMapFromDataTableByMapFunc(int firstResult, int maxResults) {
            Func<DataRow, CustomerOrderHistory> @mapper =
                row => new CustomerOrderHistory
                       {
                           ProductName = row["ProductName"].AsText(),
                           Total = row["Total"].AsIntNullable()
                       };

            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.MapAsParallel(@mapper).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(5, 10)]
        [TestCase(0, 50)]
        [TestCase(5, 200)]
        public void ParallelMapFromDataTableByPersister(int firstResult, int maxResults) {
            IRowPersister<CustomerOrderHistory> rowPersister = new CapitalizeRowPersister<CustomerOrderHistory>();

            using(
                var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist2", firstResult, maxResults,
                                                                               CustomerTestParameter)) {
                var histories = table.MapAsParallel(rowPersister).ToList();

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [TestCase("ANATR", 0, 0)]
        [TestCase("ANATR", 1, 5)]
        [TestCase("ANATR", 0, 5)]
        [TestCase("ANATR", 5, 100)]
        public void ParallelMapFromDataTableByPaging(string customerId, int firstResult, int maxResults) {
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist2",
                                                                                 firstResult,
                                                                                 maxResults,
                                                                                 CustomerTestParameter)) {
                Assert.IsFalse(table.HasErrors);
                Assert.Greater(table.Rows.Count, 1);

                var histories =
                    table
                        .MapAsParallel<CustomerOrderHistory>(ActivatorTool.CreateInstance<CustomerOrderHistory>, CapitalizeMapper)
                        .ToList();

                if(IsDebugEnabled)
                    log.Debug(histories.CollectionToString());

                if(maxResults > 0)
                    Assert.GreaterOrEqual(maxResults, histories.Count);
                else
                    Assert.Greater(histories.Count, 0);

                histories.All(history => history.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(history => history.Total.GetValueOrDefault() > 0).Should().Be.True();
            }
        }

        [Test]
        public void ParallelMapFromDataRowWithFilter() {
            // const string customerId = @"ANATR";

            var capitalizeRowPersister = new CapitalizeRowPersister<CustomerOrderHistory>();

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 필터링된 데이타 (모두 통과)
                var histories = table.MapAsParallel<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => true).ToList();

                histories.Count.Should().Be.GreaterThan(0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 필터링된 데이타 (모두 거부)
                var histories = table.MapAsParallel<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => false).ToList();

                histories.Count.Should().Be(0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 필터링된 데이타 (Total 이 0보다 큰 레코드만)
                var histories =
                    table.MapAsParallel<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => row["Total"].AsValue<int>(0) > 0).
                        ToList();

                histories.Count.Should().Be.GreaterThan(0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }
        }

        [Test]
        public void ParallelMapWhile_FromDataRow() {
            var capitalizeRowPersister = new CapitalizeRowPersister<CustomerOrderHistory>();

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 계속 조건이 항상 참
                var histories = table.ParallelMapWhile<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => true).ToList();

                histories.Count.Should().Be.GreaterThan(0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }
            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // 계속 조건이 항상 거짓 (모두 거부)
                var histories = table.ParallelMapWhile<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => false).ToList();

                histories.Count.Should().Be(0);
            }

            using(var table = NorthwindAdoRepository.ExecuteDataTableByProcedure("CustOrderHist", CustomerTestParameter)) {
                // Total 이 0보다 클 때까지만 변환
                var histories =
                    table.ParallelMapWhile<CustomerOrderHistory>(capitalizeRowPersister.Persist, row => row["Total"].AsValue(0) > 0).
                        ToList();

                histories.Count.Should().Be.GreaterThan(0);
                histories.All(h => h.ProductName.IsNotWhiteSpace()).Should().Be.True();
                histories.All(h => h.Total.HasValue && h.Total.Value > 0).Should().Be.True();
            }
        }

        #endregion
    }
}