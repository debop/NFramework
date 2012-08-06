using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NSoft.NFramework.Web;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Parallelism.Extensions.APM {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class WebRequestExtensionsTestCase : ParallelismFixtureBase {
        public static string[] UriStrings = new[]
                                            {
                                                @"http://debop.egloos.com/",
                                                @"http://www.hani.co.kr/",
                                                @"http://www.daum.net/",
                                                @"http://www.naver.com/"
                                            };

        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        [Test]
        public void GetResponseAsyncTest() {
            var requests = UriStrings.SelectMany<string, HttpWebRequest>(GetAllRequests).ToArray();

            // foreach(var request in requests)
            Parallel.ForEach(requests,
                             request => {
                                 var localRequest = request;

                                 With.TryActionAsync(() => {
                                                         var task = localRequest.GetResponseAsync();

                                                         if(IsDebugEnabled)
                                                             log.Debug("비동기 다운로드를 요청했습니다.");

                                                         task.Wait();
                                                         task.IsCompleted.Should().Be.True();
                                                         task.Result.Should().Not.Be.Null();

                                                         // 다운로드 다 받았으므로...
                                                         Assert.IsTrue(task.IsCompleted);

                                                         if(IsDebugEnabled) {
                                                             // 압축이 되어있을 수 있습니다. 헤더를 보고 알아서 하셔야합니다^^
                                                             log.Debug("비동기 다운로드 완료. Header=[{0}]",
                                                                       task.Result.Headers.CollectionToString());
                                                             log.Debug("비동기 다운로드를 완료했습니다. Download Response=[{0}]",
                                                                       task.Result.GetResponseDataByContentEncoding().ToText(0, 1024));
                                                         }

                                                         task.Result.Close();
                                                     });
                             });
        }

        [Test]
        public void GetRequestStreamAsyncTest() {
            var requests = UriStrings.SelectMany<string, HttpWebRequest>(GetAllRequests).ToArray();

            //foreach(var request in requests)
            Parallel.ForEach(requests,
                             request => {
                                 var localRequest = request;

                                 With.TryActionAsync(() => {
                                                         var task = localRequest.GetResponseStreamAsync();

                                                         if(IsDebugEnabled)
                                                             log.Debug("비동기 다운로드를 요청했습니다.");

                                                         task.Wait();
                                                         task.IsCompleted.Should().Be.True();
                                                         task.Result.Should().Not.Be.Null();

                                                         // 다운로드 다 받았으므로...
                                                         Assert.IsTrue(task.IsCompleted);

                                                         if(IsDebugEnabled)
                                                             log.Debug("비동기 다운로드를 완료했습니다. ResponseStream=" + task.Result.ToText(1024));

                                                         task.Result.Close();
                                                     });
                             });
        }

        [Test]
        public void DownloadDataAsyncTest() {
            var requests = UriStrings.SelectMany<string, HttpWebRequest>(GetAllRequests).ToArray();

            //foreach(var request in requests)
            Parallel.ForEach(requests,
                             request => {
                                 var localRequest = request;

                                 try {
                                     var task = localRequest.DownloadDataAsync();

                                     if(IsDebugEnabled)
                                         log.Debug("비동기 다운로드를 요청했습니다.");

                                     task.Wait();
                                     task.IsCompleted.Should().Be.True();
                                     task.Result.Should().Not.Be.Null();
                                     task.Result.Length.Should().Be.GreaterThan(0);


                                     if(IsDebugEnabled)
                                         log.Debug("비동기 다운로드를 완료했습니다. DownloadData=" + task.Result.ToText(0, 1024));
                                 }
                                 catch(Exception ex) {
                                     if(log.IsWarnEnabled)
                                         log.WarnException("다운로드에 실패했습니다!!!", ex);
                                 }
                             });
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(2,
                              delegate {
                                  GetResponseAsyncTest();
                                  GetRequestStreamAsyncTest();
                                  DownloadDataAsyncTest();
                              });
        }

        private static IEnumerable<HttpWebRequest> GetAllRequests(string uriString) {
            var requests = new List<HttpWebRequest>
                           {
                               WebClientFactory.Instance.CreateHttpRequest(uriString),
                               WebClientFactory.Instance.CreateHttpRequestAsFirefox(uriString),
                               WebClientFactory.Instance.CreateHttpRequestAsChrome(uriString),
                               WebClientFactory.Instance.CreateHttpRequestAsIE8(uriString)
                           };

            return requests;
        }
    }
}