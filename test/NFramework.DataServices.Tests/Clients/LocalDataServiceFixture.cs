using System;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.DataServices.Clients {
    [TestFixture]
    public class LocalDataServiceFixture : AbstractServicesFixture {
        /// <summary>
        /// Client 종류별로 설정할 내용입니다.
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public virtual ResponseMessage RunDataService(RequestMessage requestMessage) {
            return DataService.Execute(requestMessage);
        }

        [TestCase(SQL_ORDER_DETAILS)]
        [TestCase(SQL_ORDERS)]
        [TestCase(SQL_CUSTOMERS)]
        public void OpenQuery(string method) {
            var requestMsg = CreateRequestMessage();

            requestMsg.AddItem(method, ResponseFormatKind.ResultSet, 10, 10);
            requestMsg.AddItem(method, ResponseFormatKind.ResultSet, 20, 10);
            requestMsg.AddItem(method, ResponseFormatKind.ResultSet, 30, 10);
            requestMsg.AddItem(method, ResponseFormatKind.ResultSet, 40, 20);
            requestMsg.AddItem(method, ResponseFormatKind.ResultSet);

            var responseMsg = RunDataService(requestMsg);

            AssertResponseMessage(responseMsg);
        }

        [Test]
        public void OpenProcedure([Values(SP_CUSTOMER_ORDER_HISTORY)] string method,
                                  [Values("ANATR", "BOTTM", "DRACD")] string customerId) {
            var requestMsg = CreateRequestMessage();

            var item = new RequestItem(method, ResponseFormatKind.ResultSet);
            item.Parameters.Add(new RequestParameter("CustomerId", customerId));

            requestMsg.Items.Add(item);

            var responseMsg = RunDataService(requestMsg);

            AssertResponseMessage(responseMsg);
        }

        [Test]
        public void OpenProcedureNoParams([Values("TenMostExpensiveProduct")] string method) {
            var requestMsg = CreateRequestMessage();

            var item = new RequestItem(method, ResponseFormatKind.ResultSet);

            requestMsg.Items.Add(item);

            var responseMsg = RunDataService(requestMsg);

            responseMsg.HasError.Should().Be.False();
        }

        [Test]
        public void ExecuteScalar() {
            var requestMsg = CreateRequestMessage();
            requestMsg.AddItem("CountOfCustomer", ResponseFormatKind.Scalar);

            var responseMsg = RunDataService(requestMsg);

            responseMsg.HasError.Should().Be.False();
            responseMsg.Items[0].ResultValue.AsInt().Should().Be.GreaterThan(0);
        }

        [Test]
        public void ExecuteNonQuery([Values("Order, UpdateOrderFreight")] string method,
                                    [Values(0.1, -0.1)] decimal freightDelta) {
            // UPDATE Orders SET Freight = Freight + @FreightDelta

            var requestMsg = CreateRequestMessage();

            var item = new RequestItem(method, ResponseFormatKind.None);
            item.Parameters.Add(new RequestParameter("FreightDelta", freightDelta));

            requestMsg.Items.Add(item);

            var responseMsg = RunDataService(requestMsg);

            responseMsg.HasError.Should().Be.False();
        }

        [Test]
        public void ExecuteProcedure() {
            var requestMsg = CreateRequestMessage();

            requestMsg.PreQueries.Add(@"
if OBJECT_ID('EmployeeAdd') <> 0 
	DROP PROC EmployeeAdd");

            requestMsg.PreQueries.Add(
                @"
CREATE PROC EmployeeAdd 
  @LastName nvarchar(20), 
  @FirstName nvarchar(10) 
AS 
	INSERT INTO Employees (LastName, FirstName) 
	VALUES(@LastName, @FirstName)");

            var item = requestMsg.AddItem("EmployeeAdd", ResponseFormatKind.None);
            item.Parameters.Add(new RequestParameter("LastName", "Bae"));
            item.Parameters.Add(new RequestParameter("FirstName", "Sunghyouk"));

            item = requestMsg.AddItem("EmployeeAdd", ResponseFormatKind.None);
            item.Parameters.Add(new RequestParameter("LastName", "Bae"));
            item.Parameters.Add(new RequestParameter("FirstName", "Jehyoung"));


            requestMsg.PostQueries.Add("DELETE FROM Employees WHERE LastName='Bae'");

            var responseMsg = RunDataService(requestMsg);

            responseMsg.HasError.Should().Be.False();
            responseMsg.Items.Count.Should().Be.GreaterThan(0);
        }

        [TestCase(typeof(OrderDetail), SQL_ORDER_DETAILS)]
        [TestCase(typeof(Customer), SQL_CUSTOMERS)]
        public void MappedObjectTest(Type targetType, string method) {
            var requestMsg = CreateRequestMessage();
            requestMsg.AddItem(method, ResponseFormatKind.ResultSet, null, null);

            var responseMsg = RunDataService(requestMsg);

            AssertResponseMessage(responseMsg);

            responseMsg.Items[0].ResultSet.GetMappedObjects(targetType).All(x => x.GetType().Equals(targetType)).Should().Be.True();
        }

        [Test]
        public void MultiThreadTest() {
            TestTool.RunTasks(5,
                              () => OpenQuery(SQL_CUSTOMERS),
                              () => OpenQuery(SQL_ORDERS),
                              () => OpenQuery(SQL_ORDER_DETAILS),
                              () => OpenProcedure(SP_CUSTOMER_ORDER_HISTORY, "ANATR"),
                              () => OpenProcedure(SP_CUSTOMER_ORDER_HISTORY, "BOTTM"),
                              () => OpenProcedure(SP_CUSTOMER_ORDER_HISTORY, "DRACD"),
                              () => OpenProcedureNoParams("TenMostExpensiveProduct"),
                              () => ExecuteScalar());
        }

        protected virtual void AssertResponseMessage(ResponseMessage responseMsg) {
            if(responseMsg.HasError)
                if(log.IsErrorEnabled)
                    log.Error(responseMsg.Errors.CollectionToString());

            Assert.IsFalse(responseMsg.HasError, responseMsg.Errors.CollectionToString());
            Assert.IsTrue(responseMsg.Items.Count > 0);

            responseMsg.Items.All(item => item.ResultSet.Count > 0).Should().Be.True();
        }
    }

    [TestFixture]
    public class LocalDataServiceAsyncFixture : LocalDataServiceFixture {
        public override ResponseMessage RunDataService(RequestMessage requestMessage) {
            return
                Task.Factory
                    .StartNew(() => DataService.Execute(requestMessage), TaskCreationOptions.PreferFairness)
                    .Result;
            // return DelegateAsync.Run(msg => DataService.Execute(msg), requestMessage, null).Result;
        }

        public override bool AsParallel {
            get { return true; }
        }

        public override RequestMessage CreateRequestMessage() {
            return new RequestMessage
                   {
                       Transactional = false,
                       AsParallel = AsParallel
                   };
        }
    }
}