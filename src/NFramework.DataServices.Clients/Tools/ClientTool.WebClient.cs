using System;
using System.Net;
using System.Threading.Tasks;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DataServices.Clients {
    public static partial class ClientTool {
        public static ResponseMessage Execute(this WebClient client,
                                              RequestMessage requestMessage,
                                              Uri uri,
                                              string productName) {
            client.ShouldNotBeNull("client");
            requestMessage.ShouldNotBeNull("requestMessage");

            try {
                var responseText = ResolveRequestSerializer(productName).Serialize(requestMessage).Base64Encode();

                var responsesText = client.UploadString(uri, "POST", responseText);

                return ResolveResponseSerializer(productName).Deserialize(responsesText.Base64Decode());
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("WebClient를 통한 요청이 실패했습니다.", ex);

                throw;
            }
        }

        public static Task<ResponseMessage> Execute(this WebClient client,
                                                    RequestMessage requestMessage,
                                                    Uri uri,
                                                    string productName,
                                                    TaskCompletionSource<string> tcs) {
            client.ShouldNotBeNull("client");
            requestMessage.ShouldNotBeNull("requestMessage");

            try {
                var responseText = ResolveRequestSerializer(productName).Serialize(requestMessage).Base64Encode();

                return
                    client
                        .UploadStringTask(uri, "POST", responseText)
                        .ContinueWith(task => {
                                          var responseBytes = task.Result;
                                          return ResolveResponseSerializer(productName).Deserialize(responseBytes.Base64Decode());
                                      });
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("WebClient를 통한 요청이 실패했습니다.", ex);

                throw;
            }
        }
    }
}