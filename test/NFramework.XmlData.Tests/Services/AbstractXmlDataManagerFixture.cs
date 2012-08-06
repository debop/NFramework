using System.Data;
using System.Text;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.QueryProviders;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NSoft.NFramework.XmlData.Messages;
using NUnit.Framework;

namespace NSoft.NFramework.XmlData.Services {
    [TestFixture]
    public class AbstractXmlDataManagerFixture : AbstractXmlDataFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string ProductName = "Northwind";

        public const string SQL_ORDER_DETAILS = "Order Details, GetAll";
        public const string SQL_ORDERS = "Order, GetAll";
        public const string SQL_CUSTOMERS = "Customer, GetAll";

        public const string SP_CUSTOMER_ORDER_HISTORY = "CustOrderHist";

        private const bool NEED_TRANSACTION = false;
        private readonly string _dbName = AdoTool.DefaultDatabaseName;
        private static readonly object _sync = new object();

        protected virtual bool IsParallel {
            get { return false; }
        }

        protected virtual XdsRequestDocument CreateRequestDocument() {
            return new XdsRequestDocument
                   {
                       Transaction = NEED_TRANSACTION,
                       IsParallelToolecute = IsParallel
                   };
        }

        public virtual XdsResponseDocument ExecuteXmlDataManager(XdsRequestDocument requestDocument) {
            return XmlDataTool.ResolveXmlDataManager().Execute(requestDocument);
        }

        [TestCase(SQL_ORDER_DETAILS)]
        [TestCase(SQL_ORDERS)]
        [TestCase(SQL_CUSTOMERS)]
        public void OpenQuery(string query) {
            using(new OperationTimer("OpenQuery")) {
                var requestDoc = CreateRequestDocument();

                requestDoc.AddQuery(query, XmlDataResponseKind.DataSet, 10, 2);
                requestDoc.AddQuery(query, XmlDataResponseKind.DataSet, 10, 3);
                requestDoc.AddQuery(query, XmlDataResponseKind.DataSet, 10, 4);
                requestDoc.AddQuery(query, XmlDataResponseKind.DataSet, 20, 2);
                requestDoc.AddQuery(query, XmlDataResponseKind.DataSet);

                var responseDoc = ExecuteXmlDataManager(requestDoc);

                Assert.IsNotNull(responseDoc);

                if(responseDoc.HasError)
                    if(log.IsErrorEnabled)
                        log.Error(responseDoc.Errors.ToString());

                Assert.IsFalse(responseDoc.HasError, responseDoc.Errors.ToString());
                Assert.IsTrue(responseDoc.Responses.Count > 0);
                Assert.IsNotNull(responseDoc.ToDataSet("OpenQuery"));
            }
        }

        [Test]
        public void OpenProcedure([Values(SP_CUSTOMER_ORDER_HISTORY)] string method,
                                  [Values("ANATR", "BOTTM", "DRACD")] string customerId) {
            using(new OperationTimer("OpenProcedure")) {
                var requestDoc = CreateRequestDocument();

                var reqId = requestDoc.AddStoredProc(method, XmlDataResponseKind.DataSet);
                requestDoc[reqId].AddParamArray("CustomerId");
                requestDoc[reqId].AddValue(customerId);

                var responseDoc = ExecuteXmlDataManager(requestDoc);

                Assert.IsNotNull(responseDoc);

                if(responseDoc.HasError)
                    if(log.IsErrorEnabled)
                        log.Error(responseDoc.Errors);

                Assert.IsFalse(responseDoc.HasError);
                Assert.IsNotNull(responseDoc.ToDataSet("OpenProcedure"));
            }
        }

        /// <summary>
        /// Execute query and no returns
        /// </summary>
        [Test]
        public void ExecuteQuery() {
            using(new OperationTimer("ExecuteQuery")) {
                var requestDoc = CreateRequestDocument();

                requestDoc.AddQuery("SELECT COUNT(*) AS CustomerCount FROM dbo.Customers", XmlDataResponseKind.Scalar);

                // parameter가 있는 query 문장을 실행시킨다.
                int reqId = requestDoc.AddQuery("UPDATE dbo.Orders SET Freight = Freight + @FreightDelta", XmlDataResponseKind.None);
                requestDoc[reqId].Parameters.AddParameter("FreightDelta", DbType.Currency.ToString());
                requestDoc[reqId].AddValue(0.1);
                requestDoc[reqId].AddValue(-0.1);

                var responseDoc = ExecuteXmlDataManager(requestDoc);

                Assert.IsNotNull(responseDoc);

                if(responseDoc.HasError)
                    if(log.IsErrorEnabled)
                        log.Error(responseDoc.Errors);

                Assert.IsFalse(responseDoc.HasError);
                Assert.IsNotNull(responseDoc.ToDataSet("ExecuteQuery"));

                if(IsDebugEnabled)
                    log.Debug(responseDoc.ToXmlDocument(Encoding.UTF8).InnerXml.EllipsisChar(80));
            }
        }

        /// <summary>
        /// Execute procedure (aka ExecuteNonQuery)
        /// </summary>
        [Test]
        public void ExecuteProcedure() {
            lock(_sync)
                using(new OperationTimer("ExecuteProcedure")) {
                    var requestDoc = CreateRequestDocument();

                    int reqId = requestDoc.AddStoredProc("EmployeeAdd", XmlDataResponseKind.None);

                    // 먼저 EmployeeAdd Stored Procedure를 생성한다
                    //					
                    requestDoc[reqId].PreQueries.AddQuery(@"
if OBJECT_ID('EmployeeAdd') <> 0 
	DROP PROC EmployeeAdd");

                    requestDoc[reqId].PreQueries.AddQuery(
                        @"
CREATE PROC EmployeeAdd 
  @LastName nvarchar(20), 
  @FirstName nvarchar(10) 
AS 
	INSERT INTO Employees (LastName, FirstName) 
	VALUES(@LastName, @FirstName)");

                    // Employees table에 두개의 record를 넣는다
                    requestDoc[reqId].AddParamArray("LastName", "FirstName");
                    requestDoc[reqId].AddValue("Bae", "Sunghyouk");
                    requestDoc[reqId].AddValue("Bae", "Jehyoung");

                    // Employees table에서 새로 넣은 정보를 삭제한다.
                    requestDoc[reqId].PostQueries.AddQuery("DELETE FROM Employees where LastName='Bae'");

                    var responseDoc = ExecuteXmlDataManager(requestDoc);

                    Assert.IsNotNull(responseDoc);

                    if(responseDoc.HasError)
                        if(log.IsErrorEnabled)
                            log.Error(responseDoc.Errors);

                    Assert.IsFalse(responseDoc.HasError);
                    Assert.IsTrue(responseDoc.Responses.Count > 0);
                    Assert.IsNotNull(responseDoc.ToDataSet("ExecuteProcedure"));
                }
        }

        /// <summary>
        /// Execute a specified method which is defined in QueryStringFile (<see cref="IniAdoQueryProvider"/>)
        /// </summary>
        [Test]
        public void MethodCall() {
            using(new OperationTimer("Method Call")) {
                var requestDoc = CreateRequestDocument();

                requestDoc.AddMethod("Order, GetAll", XmlDataResponseKind.DataSet);
                requestDoc.AddMethod("Order Details, GetAll", XmlDataResponseKind.DataSet);

                int reqId = requestDoc.AddMethod("CustomerOrderHistory", XmlDataResponseKind.DataSet);
                requestDoc[reqId].AddParamArray("CustomerId");
                requestDoc[reqId].AddValue("ANATR");
                requestDoc[reqId].AddValue("BOTTM");
                requestDoc[reqId].AddValue("DRACD");

                reqId = requestDoc.AddMethod("CustomerOrdersDetail", XmlDataResponseKind.DataSet);
                requestDoc[reqId].AddParamArray("OrderId");
                requestDoc[reqId].AddValue(10248);
                requestDoc[reqId].AddValue(10262);

                var responseDoc = ExecuteXmlDataManager(requestDoc);

                Assert.IsNotNull(responseDoc);

                if(responseDoc.HasError)
                    if(log.IsErrorEnabled)
                        log.Error(responseDoc.Errors);

                Assert.IsFalse(responseDoc.HasError, responseDoc.Errors.ToString());

                Assert.IsTrue(responseDoc.Responses.Count > 0);
                Assert.IsNotNull(responseDoc.ToDataSet("MethodCall"));
            }
        }

        [Test]
        public void MultiThreadMixingTest() {
            TestTool.RunTasks(5,
                              () => {
                                  OpenQuery(SQL_ORDERS);
                                  OpenQuery(SQL_CUSTOMERS);
                                  OpenProcedure(SP_CUSTOMER_ORDER_HISTORY, "ANATR");
                                  OpenProcedure(SP_CUSTOMER_ORDER_HISTORY, "BOTTM");
                                  OpenProcedure(SP_CUSTOMER_ORDER_HISTORY, "DRACD");
                                  ExecuteQuery();
                                  ExecuteProcedure();
                                  MethodCall();
                              });
        }
    }
}