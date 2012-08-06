using System;
using System.Threading.Tasks;
using NSoft.NFramework.DataServices.Messages;

namespace NSoft.NFramework.DataServices.Clients {
    public static partial class ClientTool {
        private static DataService _dataService;

        public static DataService DataService {
            get { return _dataService ?? (_dataService = new DataService()); }
            set { _dataService = value; }
        }

        public static ResponseMessage Execute(this DataService client, RequestMessage requestMessage, string productName) {
            client.ShouldNotBeNull("client");
            requestMessage.ShouldNotBeNull("requestMessage");

            try {
                var requestBytes = ResolveRequestSerializer(productName).Serialize(requestMessage);
                var responseBytes = client.Execute(requestBytes, productName);

                return ResolveResponseSerializer(productName).Deserialize(responseBytes);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("웹서비스를 통한 요청이 실패했습니다.", ex);

                throw;
            }
        }

        public static Task<ResponseMessage> ExecuteTask(this DataService client,
                                                        RequestMessage requestMessage,
                                                        string productName,
                                                        TaskCompletionSource<byte[]> tcs) {
            client.ShouldNotBeNull("client");
            requestMessage.ShouldNotBeNull("requestMessage");

            try {
                var requestBytes = ResolveRequestSerializer(productName).Serialize(requestMessage);

                ExecuteCompletedEventHandler handler = null;
                handler = (sender, e) => EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => client.ExecuteCompleted -= handler);
                client.ExecuteCompleted += handler;

                client.ExecuteAsync(requestBytes, productName, tcs);


                return
                    tcs.Task
                        .ContinueWith(task => ResolveResponseSerializer(productName).Deserialize(task.Result));
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("웹서비스를 통한 요청이 실패했습니다.", ex);

                throw;
            }
        }

        public static Task<string> PingTask(this DataService client, TaskCompletionSource<string> tcs) {
            PingCompletedEventHandler handler = null;

            try {
                handler = (sender, e) => EAPCommon.HandleCompletion(tcs, e, () => e.Result, () => client.PingCompleted -= handler);
                client.PingCompleted += handler;

                client.PingAsync(tcs);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("웹서비스 비동기 호출에 예외가 발생했습니다.", ex);

                if(handler != null)
                    client.PingCompleted -= handler;

                tcs.TrySetException(ex);
            }
            return tcs.Task;
        }
    }
}