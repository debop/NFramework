using System;
using System.Threading;
using NSoft.NFramework.XmlData.Messages;
using NSoft.NFramework.XmlData.XmlDataServiceHost.Wcf;
using NUnit.Framework;

namespace NSoft.NFramework.XmlData.Services {
    /// <summary>
    /// WCF XmlDataService를 테스트합니다.
    /// </summary>
    //
    // NOTE : WCF WSHttpBinding에서 Data 크기가 문제가 되므로 App.Config 의 Binding 속성 값 중에 
    // NOTE : max 값을 모두 2147483647 (int.MaxValue)로 설정해야 한다. (예: maxReceivedMessageSize="2147483647")
    // 
    [TestFixture]
    public class XmlDataServiceUsingWcfTestFixture : AbstractXmlDataManagerFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [Test]
        public void Ping() {
            using(var xdsProxy = new XmlDataServiceClient()) {
                var resultString = xdsProxy.Ping();
                Assert.IsNotEmpty(resultString);
                Assert.IsTrue(resultString.Contains("Ping"));
            }
        }

        [Test]
        public void PingAsync() {
            // WCF 의 Async 방식 (Begin, End 방식)
            var xdsProxy = new XmlDataServiceClient();

            xdsProxy.BeginPing(ar => {
                                   var proxy = (XmlDataServiceClient)ar.AsyncState;
                                   var resultString = proxy.EndPing(ar);
                                   Assert.IsNotEmpty(resultString);
                                   Assert.IsTrue(resultString.Contains("Ping"));
                                   Console.WriteLine("Result=" + resultString);
                                   proxy.Close();
                               },
                               xdsProxy);
        }

        [Test]
        public void PingAsync2() {
            using(var xdsProxy = new XmlDataServiceClient()) {
                var reset = new AutoResetEvent(false);
                xdsProxy.PingAsync();

                xdsProxy.PingCompleted += (s, e) => {
                                              var resultString = e.Result;
                                              Assert.IsNotEmpty(resultString);
                                              Assert.IsTrue(resultString.Contains("Ping"));
                                              Console.WriteLine("Result=" + resultString);
                                              reset.Set();
                                          };

                reset.WaitOne();
            }
        }

        public override XdsResponseDocument ExecuteXmlDataManager(XdsRequestDocument requestDocument) {
            var adapter = XmlDataTool.ResolveXmlDataManagerAdapter(ProductName);
            var requestBytes = adapter.RequestSerializer.Serialize(requestDocument);

            using(var xdsProxy = new XmlDataServiceClient()) {
                var responseBytes = xdsProxy.Execute(requestBytes, ProductName, false);
                return adapter.ResponseSerializer.Deserialize(responseBytes);
            }
        }

        private static void VerifyXmlDataService(XdsRequestDocument xdsRequest, string productName, bool? compress,
                                                 bool? security) {
            using(var xdsProxy = new XmlDataServiceClient()) {
                var serializer = GetSerializer(compress, security);
                var requestBytes = serializer.Serialize(xdsRequest.ConvertToBytes());

                var responseBytes = security.GetValueOrDefault(false)
                                        ? xdsProxy.ExecuteSecurity(requestBytes, productName, compress.GetValueOrDefault(false))
                                        : xdsProxy.Execute(requestBytes, productName, compress.GetValueOrDefault(false));

                Assert.IsNotNull(responseBytes);

                var xdsResponse = ((byte[])serializer.Deserialize(responseBytes)).ConvertToXdsResponseDocument();

                Assert.IsNotNull(xdsResponse);
                Assert.IsFalse(xdsResponse.HasError);

                Assert.IsTrue(xdsResponse.Responses.Count > 0);
            }
        }

        private static void VerifyXmlDataServiceAsync(XdsRequestDocument xdsRequest, string productName, bool? compress,
                                                      bool? security) {
            var waitHandle = new AutoResetEvent(false);
            var serializer = GetSerializer(compress, security);
            var requestBytes = serializer.Serialize(xdsRequest.ConvertToBytes());

            var proxy = new XmlDataServiceClient();

            if(security.GetValueOrDefault(false)) {
                // IAsyncResult를 반환받아서 필요할 때 기다리게 할 수도 있지만, 테스트를 위해 Synchronous하게 동작하게 한다.
                proxy.BeginExecuteSecurity(requestBytes, productName, compress.GetValueOrDefault(false),
                                           (ar) => {
                                               var proxy2 = (XmlDataServiceClient)ar.AsyncState;
                                               var responseBytes = proxy2.EndExecuteSecurity(ar);
                                               var xdsResponse =
                                                   ((byte[])serializer.Deserialize(responseBytes)).ConvertToXdsResponseDocument();

                                               Assert.IsNotNull(xdsResponse);
                                               Assert.IsFalse(xdsResponse.HasError);
                                               Assert.IsTrue(xdsResponse.Responses.Count > 0);
                                               proxy2.Close();
                                           },
                                           proxy);
            }
            else {
                // IAsyncResult를 반환받아서 필요할 때 기다리게 할 수도 있지만, 테스트를 위해 Synchronous하게 동작하게 한다.
                proxy.BeginExecute(requestBytes, productName, compress.GetValueOrDefault(false),
                                   (ar) => {
                                       var proxy2 = (XmlDataServiceClient)ar.AsyncState;
                                       var responseBytes = proxy2.EndExecute(ar);
                                       var xdsResponse =
                                           ((byte[])serializer.Deserialize(responseBytes)).ConvertToXdsResponseDocument();

                                       Assert.IsNotNull(xdsResponse);
                                       Assert.IsFalse(xdsResponse.HasError);
                                       Assert.IsTrue(xdsResponse.Responses.Count > 0);
                                       proxy2.Close();
                                   },
                                   proxy);
            }
        }
    }

    [TestFixture]
    public class XmlDataServiceUsingWcfAsyncTestFixture : XmlDataServiceUsingWcfTestFixture {
        public override XdsResponseDocument ExecuteXmlDataManager(XdsRequestDocument requestDocument) {
            var adapter = XmlDataTool.ResolveXmlDataManagerAdapter(ProductName);
            var requestBytes = adapter.RequestSerializer.Serialize(requestDocument);

            using(var xdsProxy = new XmlDataServiceClient()) {
                var ar = xdsProxy.BeginExecute(requestBytes, ProductName, false, null, null);
                Thread.Sleep(1);
                var responseBytes = xdsProxy.EndExecute(ar);
                return adapter.ResponseSerializer.Deserialize(responseBytes);
            }
        }
    }
}