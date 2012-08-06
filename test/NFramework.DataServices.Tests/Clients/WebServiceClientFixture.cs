using System;
using System.Threading.Tasks;
using NSoft.NFramework.DataServices.Messages;
using NUnit.Framework;

namespace NSoft.NFramework.DataServices.Clients {
    [TestFixture]
    public class WebServiceClientFixture : LocalDataServiceFixture {
        [Test]
        public void PingTest() {
            using(var client = new WebDataService.DataService()) {
                client.Ping().Should().Not.Be.Empty();
            }
        }

        public override ResponseMessage RunDataService(RequestMessage requestMessage) {
            using(var client = new WebDataService.DataService()) {
                var requestBytes = DataServiceTool.ResolveRequestSerializer(ProductName).Serialize(requestMessage);
                var responseBytes = client.Execute(requestBytes, ProductName);
                return DataServiceTool.ResolveResponseSerializer(ProductName).Deserialize(responseBytes);
            }
        }
    }

    [TestFixture]
    public class WebServiceClientAsyncFixture : WebServiceClientFixture {
        public override ResponseMessage RunDataService(RequestMessage requestMessage) {
            using(var client = new WebDataService.DataService()) {
                var requestBytes = DataServiceTool.ResolveRequestSerializer(ProductName).Serialize(requestMessage);

                var tcs = new TaskCompletionSource<byte[]>();
                ExecuteCompletedEventHandler handler = null;
                handler =
                    (sender, args) =>
                    Parallelism.Tools.EventAsyncPattern.HandleCompletion(tcs, args, () => args.Result,
                                                                         () => client.ExecuteCompleted -= handler);

                client.ExecuteCompleted += handler;

                try {
                    client.ExecuteAsync(requestBytes, ProductName, tcs);
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled)
                        log.ErrorException("웹서비스 비동기 호출에 예외가 발생했습니다.", ex);

                    client.ExecuteCompleted -= handler;
                    tcs.TrySetException(ex);
                }

                var responseBytes = tcs.Task.Result;

                return DataServiceTool.ResolveResponseSerializer(ProductName).Deserialize(responseBytes);
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