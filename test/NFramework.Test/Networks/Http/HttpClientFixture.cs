using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Networks {
    [TestFixture]
    public class HttpClientFixture : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const string HttpHost = "http://localhost:3500";
        public const string ApplicationRoot = "/NFramework.WebHost";
        public const string NetworksFolder = "/Networks";

        public static readonly string Echo1 = HttpHost + ApplicationRoot + NetworksFolder + "/Echo.aspx";
        public static readonly string Echo2 = HttpHost + ApplicationRoot + NetworksFolder + "/Echo2.aspx";

        public static string[] ScriptPaths = new[]
                                             {
                                                 HttpHost + ApplicationRoot + NetworksFolder + "/Echo.aspx",
                                                 HttpHost + ApplicationRoot + NetworksFolder + "/Echo2.aspx",
                                                 HttpHost + ApplicationRoot + NetworksFolder + "/Echo.ashx",
                                                 HttpHost + ApplicationRoot + NetworksFolder + "/Echo2.ashx"
                                             };

        public const string FileUrl = HttpHost + ApplicationRoot + @"/Documents/PAD.doc";

        private const string PayLoad = "A=123&B=가나다&CD=각하&Name=바보 아냐";

        [Test]
        public void HttpEncoding() {
            string encoded = HtmlTool.UrlEncode(PayLoad, Encoding.UTF8);

            Assert.IsNotEmpty(encoded);
            Assert.AreNotEqual(PayLoad, encoded);

            if(IsDebugEnabled) {
                log.Debug("Payload=" + PayLoad);
                log.Debug("Encoded Payload=" + encoded);
            }
        }

        [Test]
        public void HttpGet() {
            foreach(string script in ScriptPaths) {
                var http = new HttpClient(script + "?" + HtmlTool.UrlEncode(PayLoad));

                var result = http.Get();
                Assert.IsNotEmpty(result, "Result is Empty. script=" + script);

                if(IsDebugEnabled)
                    log.Debug(result.EllipsisChar(80));
            }

            if(IsDebugEnabled)
                log.Debug("============================");

            foreach(string script in ScriptPaths) {
                var httpUtf8 = new HttpClient(script + "?" + HtmlTool.UrlEncode(PayLoad, Encoding.UTF8));

                var result = httpUtf8.Get();
                Assert.IsNotEmpty(result, "Result is Empty. script=" + script);

                if(IsDebugEnabled)
                    log.Debug(result.EllipsisChar(80));
            }
        }

        [Test]
        public void HttpGet2() {
            var http = new HttpClient(Echo2 + "?" + HtmlTool.UrlEncode(PayLoad));
            var result = http.Get();
            Assert.IsNotEmpty(result);
            if(IsDebugEnabled)
                log.Debug(result.EllipsisChar(80));

            http = new HttpClient(Echo1 + "?" + HtmlTool.UrlEncode(PayLoad, Encoding.UTF8));

            result = http.Get();
            Assert.IsNotEmpty(result, "Script=" + http.BaseUriString);
            if(IsDebugEnabled)
                log.Debug(result.EllipsisChar(80));
        }

        [Test]
        public void HttpPost() {
            foreach(string script in ScriptPaths) {
                var http = new HttpClient(script);

                log.Debug("Post(string) = " + http.Post(PayLoad, Encoding.UTF8));
                log.Debug("Post(string) = " + http.Post(PayLoad, Encoding.Default));

                var nvc = new NameValueCollection
                          {
                              { "A", "123" },
                              { "B", "가나다" },
                              { "CD", "각하" },
                              { "Name", "바보 아냐" }
                          };

                log.Debug("Post(NameValueCollection inputs) = " + http.Post(nvc, Encoding.UTF8));
                log.Debug("Post(NameValueCollection inputs) = " + http.Post(nvc, Encoding.Default));
                log.Debug("");
            }
        }

        [Test]
        public void HttpPost2() {
            var list = new NameValueCollection
                       {
                           { "postparam", "리얼웹 가나다 ABC 123 & _ -" }
                       };

            var http = new HttpClient(Echo2);
            var result = http.Post(list, Encoding.Default);

            Assert.IsNotEmpty(result);
            if(IsDebugEnabled)
                log.Debug(result.EllipsisChar(80));
        }

        [Test]
        public void PostString() {
            string result = HttpTool.PostString(Echo1, PayLoad, Encoding.UTF8);
            Assert.IsNotEmpty(result);

            if(IsDebugEnabled)
                log.Debug("PostString result = " + result.EllipsisChar(80));
        }

        [Test]
        public void PostString2() {
            string result = HttpTool.PostString(Echo2, PayLoad, Encoding.UTF8);
            Assert.IsNotEmpty(result);

            if(IsDebugEnabled)
                log.Debug("PostString result = " + result.EllipsisChar(80));
        }

        [Test]
        public void WebClientTest() {
            var uri = new Uri(Echo1);
            using(var client = new WebClient()) {
                // client.Encoding = Encoding.Default;
                client.Encoding = Encoding.UTF8;
                var collection = new NameValueCollection();
                collection.Add("A", "가나다라");
                collection.Add("B", "가나다라");
                collection.Add("CC", "가나다라");
                collection.Add("ZZZ", "가나다라");

                var res = client.UploadValues(uri.AbsoluteUri, "POST", collection);

                Assert.IsNotNull(res);
                Assert.Greater(res.Length, 0);

                if(IsDebugEnabled)
                    log.Debug("Response=" + client.Encoding.GetString(res).EllipsisChar(80));
            }
        }

        [Test]
        public void DownloadData() {
            var http = new HttpClient(Echo1 + "?" + HtmlTool.UrlEncode(PayLoad, Encoding.UTF8));

            var data = http.DownloadData();
            Assert.IsNotNull(data);
            Assert.Greater(data.Length, 0);

            if(IsDebugEnabled)
                log.Debug("Response = " + Encoding.UTF8.GetString(data).EllipsisChar(80));
        }

        [Test]
        public void DownloadFile() {
            var http = new HttpClient();

            http.DownloadFile(FileUrl, @"C:\Temp\File.doc");
        }

        #region << Compression >>

        [Test]
        public void Compression() {
            TestTool.RunTasks(5,
                              () => {
                                  // 압축전송을 제공하는 Web Site의 경우 Stream을 자동 압축/해제를 해준다. (.NET 2.0 이상부터)
                                  //
                                  var http = new HttpClient("http://intra.realweb21.com/Intra/Home/tabid/36/language/ko-KR/Default.aspx");
                                  var result = http.Get();

                                  Assert.IsNotEmpty(result);

                                  if(IsDebugEnabled)
                                      log.Debug("Compression=" + result.EllipsisChar(1024));
                              });
        }

        #endregion
    }
}