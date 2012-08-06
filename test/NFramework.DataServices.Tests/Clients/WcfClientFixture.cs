using System;
using System.Threading.Tasks;
using NSoft.NFramework.DataServices.Messages;
using NUnit.Framework;

namespace NSoft.NFramework.DataServices.Clients {
    [TestFixture]
    public class WcfClientFixture : LocalDataServiceFixture {
        public override ResponseMessage RunDataService(RequestMessage requestMessage) {
            using(var client = new WcfDataService.DataServiceClient()) {
                var requestBytes = DataServiceTool.ResolveRequestSerializer(ProductName).Serialize(requestMessage);
                var responseBytes = client.Execute(requestBytes, ProductName);
                return DataServiceTool.ResolveResponseSerializer(ProductName).Deserialize(responseBytes);
            }
        }

        [Test]
        public void PingTest() {
            using(var client = new WcfDataService.DataServiceClient()) {
                client.Ping().Should().Not.Be.Empty();
            }
        }
    }

    [TestFixture]
    public class WcfClientAsyncFixture : WcfClientFixture {
        public override ResponseMessage RunDataService(RequestMessage requestMessage) {
            using(var client = new WcfDataService.DataServiceClient()) {
                var requestBytes = DataServiceTool.ResolveRequestSerializer(ProductName).Serialize(requestMessage);

                var asyncResult = client.BeginExecute(requestBytes, ProductName, null, null);
                var task = Task.Factory.StartNew(ar => client.EndExecute((IAsyncResult)ar), asyncResult, TaskCreationOptions.None);

                return DataServiceTool.ResolveResponseSerializer(ProductName).Deserialize(task.Result);
            }
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