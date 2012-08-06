using System;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.XmlData.Messages;
using NSoft.NFramework.XmlData.XmlDataServiceHost.Ws;
using NUnit.Framework;

namespace NSoft.NFramework.XmlData.Services {
    [TestFixture]
    public class XmlDataServiceByWebServiceFixture : AbstractXmlDataManagerAsyncFixture // AbstractXmlDataFixture
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private const int TRY_COUNT = 4;
        private const bool NEED_TRANSACTION = true;
        private const bool USE_SECURITY = false;
        private const string XmlDataServiceKey = "NSoft.NFramework.XmlData.XmlDataService.Key";

        public static XmlDataService XmlDataService {
            get {
                if(Local.Data[XmlDataServiceKey] == null)
                    Local.Data[XmlDataServiceKey] = new XmlDataService();
                return (XmlDataService)Local.Data[XmlDataServiceKey];
            }
        }

        public override XdsResponseDocument ExecuteXmlDataManager(XdsRequestDocument requestDocument) {
            using(var xmlDataService = new XmlDataService()) {
                var adapter = XmlDataTool.ResolveXmlDataManagerAdapter(ProductName);
                var requestBytes = adapter.RequestSerializer.Serialize(requestDocument);
                var responseBytes = xmlDataService.Execute(requestBytes, ProductName, false);

                return adapter.ResponseSerializer.Deserialize(responseBytes);
            }
        }

        [Test]
        public void PingTest() {
            Console.WriteLine(XmlDataService.Ping());
        }

        /// <summary>
        /// 비동기적으로 Web Service를 호출합니다. 비동기 호출을 동시다발적으로 호출 시 UserState로 각각의 호출을 구분할 수 있어야 한다.
        /// </summary>
        /// <param name="xdsRequest"></param>
        /// <param name="productName"></param>
        /// <param name="compress"></param>
        /// <param name="security"></param>
        private static void VerifyXmlDataServiceAsync(XdsRequestDocument xdsRequest, string productName, bool? compress, bool? security) {
            var serializer = GetSerializer(compress, security);
            var requestBytes = serializer.Serialize(xdsRequest.ConvertToBytes());
            var xdsService = new XmlDataService();

            var tcs = new TaskCompletionSource<byte[]>(null);

            if(security.GetValueOrDefault(false)) {
                ExecuteSecurityCompletedEventHandler handler = null;
                handler =
                    (s, e) =>
                    EventAsyncPattern.HandleCompletion(tcs, e, () => e.Result, () => xdsService.ExecuteSecurityCompleted -= handler);
                xdsService.ExecuteSecurityCompleted += handler;

                try {
                    xdsService.ExecuteSecurityAsync(requestBytes, productName, compress.GetValueOrDefault(false), tcs);
                }
                catch(Exception ex) {
                    xdsService.ExecuteSecurityCompleted -= handler;
                    tcs.TrySetException(ex);
                }
            }
            else {
                ExecuteCompletedEventHandler handler = null;
                handler =
                    (s, e) => EventAsyncPattern.HandleCompletion(tcs, e, () => e.Result, () => xdsService.ExecuteCompleted -= handler);
                xdsService.ExecuteCompleted += handler;

                try {
                    xdsService.ExecuteAsync(requestBytes, productName, compress.GetValueOrDefault(false), tcs);
                }
                catch(Exception ex) {
                    xdsService.ExecuteCompleted -= handler;
                    tcs.TrySetException(ex);
                }
            }

            tcs.Task.Wait();
            Assert.IsTrue(tcs.Task.IsCompleted);
            var xdsResponse = ((byte[])serializer.Deserialize(tcs.Task.Result)).ConvertToXdsResponseDocument();

            Assert.IsNotNull(xdsResponse);
            Assert.IsFalse(xdsResponse.HasError);
            Assert.IsTrue(xdsResponse.Responses.Count > 0);

            xdsService.Dispose();
        }
    }

    [TestFixture]
    public class XmlDataServiceAsyncByWebServiceFixture : XmlDataServiceByWebServiceFixture {
        public override XdsResponseDocument ExecuteXmlDataManager(XdsRequestDocument requestDocument) {
            using(var xdsService = new XmlDataService()) {
                var adapter = XmlDataTool.ResolveXmlDataManagerAdapter(ProductName);

                var tcs = new TaskCompletionSource<byte[]>(null);

                ExecuteCompletedEventHandler handler = null;
                handler =
                    (s, e) => EventAsyncPattern.HandleCompletion(tcs, e, () => e.Result, () => xdsService.ExecuteCompleted -= handler);
                xdsService.ExecuteCompleted += handler;

                try {
                    var requestBytes = adapter.RequestSerializer.Serialize(requestDocument);
                    xdsService.ExecuteAsync(requestBytes, ProductName, false, tcs);
                }
                catch(Exception ex) {
                    xdsService.ExecuteCompleted -= handler;
                    tcs.TrySetException(ex);
                }


                tcs.Task.Wait();
                Assert.IsTrue(tcs.Task.IsCompleted);

                return adapter.ResponseSerializer.Deserialize(tcs.Task.Result);
            }
        }
    }
}