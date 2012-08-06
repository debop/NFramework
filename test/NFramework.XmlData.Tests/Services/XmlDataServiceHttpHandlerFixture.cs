using System;
using System.Net;
using NSoft.NFramework.Networks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NSoft.NFramework.Xml;
using NSoft.NFramework.XmlData.Messages;
using NSoft.NFramework.XmlData.Web;
using NUnit.Framework;

namespace NSoft.NFramework.XmlData.Services {
    [TestFixture]
    public class XmlDataServiceHttpHandlerFixture : AbstractXmlDataManagerFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly string ScriptPath = ConfigTool.GetAppSettings("XmlManager.Url", string.Empty) + "/XmlManagerAsync.axd";

        private static string BuildScriptPathParameters(string scriptPath, bool compress, bool security) {
            var result = scriptPath;
            var isFirst = true;

            if(compress) {
                result = result + (isFirst ? "?" : "&") + HttpParams.Compress + "=" + compress.GetHashCode();
                isFirst = false;
            }
            if(security) {
                result = result + (isFirst ? "?" : "&") + HttpParams.Security + "=" + security.GetHashCode();
                isFirst = false;
            }
            return result;
        }

        public override XdsResponseDocument ExecuteXmlDataManager(XdsRequestDocument requestDocument) {
            var responseXml = XmlHttpTool.PostXml(ScriptPath, requestDocument.ToXmlDocument(XmlTool.XmlEncoding));
            return XmlTool.Deserialize<XdsResponseDocument>(responseXml);
        }

        [TestCase(false, false)]
        public void OpenQueryAsync(bool compress, bool security) {
            var path = BuildScriptPathParameters(ScriptPath, compress, security);
            using(new OperationTimer("OpenQuery to " + ScriptPath, false)) {
                var requestDoc = CreateRequestDocument();

                requestDoc.AddQuery("SELECT * FROM Orders", XmlDataResponseKind.DataSet);

                VerifyXmlDataServiceAsync(path, requestDoc, compress, security);
            }
        }

        public void VerifyXmlDataService(string scriptPath, XdsRequestDocument requestDocument, bool? compress, bool? security) {
            var serializer = GetSerializer(compress, security);

            var requestBytes = serializer.Serialize(requestDocument.ConvertToBytes());

            // NOTE : SILVERLIGHT에서는 UploadDataAsync 메소드를 지원하지 않는다. Silverlight에서는 WebService나 WCF를 사용해야 한다.
            var webClient = new WebClient();
            var responseBytes = webClient.UploadData(scriptPath, requestBytes);

            var xdsResponse = ((byte[])serializer.Deserialize(responseBytes)).ConvertToXdsResponseDocument();
            Assert.IsNotNull(xdsResponse);
            Assert.IsFalse(xdsResponse.HasError);
        }

        public void VerifyXmlDataServiceAsync(string scriptPath, XdsRequestDocument requestDocument, bool? compress, bool? security) {
            var serializer = GetSerializer(compress, security);

            var requestBytes = serializer.Serialize(requestDocument.ConvertToBytes());

            var webClient = new WebClient();

            var uploadTask = webClient.UploadDataTask(new Uri(scriptPath), "POST", requestBytes);
            uploadTask.Wait();

            var responseBytes = uploadTask.Result;
            var xdsResponse = ((byte[])serializer.Deserialize(responseBytes)).ConvertToXdsResponseDocument();

            Assert.IsNotNull(xdsResponse);
            Assert.IsFalse(xdsResponse.HasError);
        }
    }

    [TestFixture]
    public class AsyncXmlDataServiceHttpHandlerFixture : XmlDataServiceHttpHandlerFixture {
        protected override XdsRequestDocument CreateRequestDocument() {
            return new XdsRequestDocument
                   {
                       Transaction = false,
                       IsParallelToolecute = true
                   };
        }
    }
}