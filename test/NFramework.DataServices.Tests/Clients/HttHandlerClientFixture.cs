using System.Net;
using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.DataServices.Clients {
    [TestFixture]
    public class HttHandlerClientFixture : LocalDataServiceFixture {
        public virtual string ScriptUrlFormat {
            get { return "http://localhost:41110/DataService.axd?Product={0}"; }
        }

        public string GetScriptUrl() {
            return string.Format(ScriptUrlFormat, ProductName);
        }

        public override ResponseMessage RunDataService(RequestMessage requestMessage) {
            using(var client = new WebClient()) {
                var requestBytes = DataServiceTool.ResolveRequestSerializer(ProductName).Serialize(requestMessage);

                var responseText = client.UploadString(GetScriptUrl(), "POST", requestBytes.Base64Encode());

                return DataServiceTool.ResolveResponseSerializer(ProductName).Deserialize(responseText.Base64Decode());
            }
        }

        [Test]
        public void Ping() {
            using(var client = new WebClient()) {
                client.DownloadStringTask(GetScriptUrl()).Result.Should().Not.Be.Null();
            }
        }
    }

    [TestFixture]
    public class HttHandlerAsyncClientFixture : HttHandlerClientFixture {
        public override string ScriptUrlFormat {
            get { return "http://localhost:41110/DataServiceAsync.axd?Product={0}"; }
        }

        public override ResponseMessage RunDataService(RequestMessage requestMessage) {
            using(var client = new WebClient()) {
                var requestBytes = DataServiceTool.ResolveRequestSerializer(ProductName).Serialize(requestMessage);

                var responseText = client.UploadStringTask(GetScriptUrl(), "POST", requestBytes.Base64Encode()).Result;

                return DataServiceTool.ResolveResponseSerializer(ProductName).Deserialize(responseText.Base64Decode());
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