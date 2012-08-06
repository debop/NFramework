using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices.Clients {
    public static partial class ClientTool {
        private static DataServiceClient _dataServiceClient;

        public static DataServiceClient DataServiceClient {
            get { return _dataServiceClient ?? (_dataServiceClient = new DataServiceClient()); }
            set { _dataServiceClient = value; }
        }

        public static ResponseMessage Execute(this DataServiceClient client, RequestMessage requestMessage, string productName) {
            client.ShouldNotBeNull("client");
            requestMessage.ShouldNotBeNull("requestMessage");

            try {
                var requestBytes = ResolveRequestSerializer(productName).Serialize(requestMessage);
                var responseBytes = client.Execute(requestBytes, productName);

                return ResolveResponseSerializer(productName).Deserialize(responseBytes);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("WCF DataServiceClient를 통한 요청이 실패했습니다.", ex);

                throw;
            }
        }

        public static Task<ResponseMessage> ExecuteTask(this DataServiceClient client,
                                                        RequestMessage requestMessage,
                                                        string productName,
                                                        TaskCompletionSource<byte[]> tcs) {
            client.ShouldNotBeNull("client");
            requestMessage.ShouldNotBeNull("requestMessage");

            try {
                var requestBytes = ResolveRequestSerializer(productName).Serialize(requestMessage);

                var asyncResult = client.BeginExecute(requestBytes, productName, null, null);
                var executeTask = Task.Factory.StartNew(ar => client.EndExecute((IAsyncResult)ar),
                                                        asyncResult,
                                                        TaskCreationOptions.None);

                return
                    executeTask
                        .ContinueWith(task => {
                                          var responseBytes = executeTask.Result;
                                          return ResolveResponseSerializer(productName).Deserialize(responseBytes);
                                      });
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("WCF DataServiceClient를 통한 요청이 실패했습니다.", ex);

                throw;
            }
        }

        public static Task<string> PingTask(this DataServiceClient client, TaskCompletionSource<string> tcs) {
            EventHandler<PingCompletedEventArgs> handler = null;
            handler = (sender, e) => EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => client.PingCompleted -= handler);

            try {
                client.PingCompleted += handler;
                client.PingAsync(tcs);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("웹서비스 비동기 호출에 예외가 발생했습니다.", ex);

                client.PingCompleted -= handler;
                tcs.TrySetException(ex);
            }
            return tcs.Task;
        }
    }
}