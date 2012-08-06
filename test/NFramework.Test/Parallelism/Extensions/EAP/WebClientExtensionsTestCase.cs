using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NSoft.NFramework.IO;
using NSoft.NFramework.Parallelism.Tools;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NSoft.NFramework.Web;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions.EAP {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class WebClientExtensionsTestCase : ParallelismFixtureBase {
        public static string[] UriStrings = new[]
                                            {
                                                @"http://debop.egloos.com/",
                                                @"http://www.hani.co.kr/",
                                                @"http://www.daum.net/",
                                                @"http://www.naver.com/"
                                            };

        private const string Address = @"http://debop.egloos.com";

        private static readonly Encoding DefaultEncoding = Encoding.UTF8;

        [Test]
        public void DownloadStringTaskTest() {
            Parallel.ForEach(GetAllWebClients(),
                             client => {
                                 foreach(var task in UriStrings.Select(uri => client.DownloadStringTask(uri))) {
                                     if(IsDebugEnabled)
                                         log.Debug("비동기 다운로드를 요청했습니다.");

                                     // Wait()를 하나 Result를 조회하나 아직 결과가 없을 때에는 기다리게 됩니다.
                                     Assert.IsNotEmpty(task.Result.AsText());
                                     Assert.IsTrue(task.Result.Length > 0);

                                     // 다운로드 다 받았으므로...
                                     Assert.IsFalse(client.IsBusy);

                                     if(IsDebugEnabled)
                                         log.Debug("비동기 다운로드를 완료했습니다. DownloadString=" + task.Result.EllipsisChar(1024));
                                 }
                             });
        }

#if !SILVERLIGHT

        [Test]
        public void DownloadDataTaskTest() {
            Parallel.ForEach(GetAllWebClients(),
                             client => {
                                 var aClient = client;
                                 foreach(var task in UriStrings.Select(uri => aClient.DownloadDataTask(uri))) {
                                     With.TryActionAsync(() => {
                                                             if(IsDebugEnabled)
                                                                 log.Debug("비동기 다운로드를 요청했습니다.");

                                                             Assert.IsNotNull(task.Result);
                                                             Assert.IsTrue(task.Result.Length > 0);

                                                             // 다운로드 다 받았으므로...
                                                             Assert.IsFalse(aClient.IsBusy);

                                                             if(IsDebugEnabled)
                                                                 log.Debug("비동기 다운로드를 완료했습니다. DownloadData=" +
                                                                           task.Result.ToText(DefaultEncoding).EllipsisChar(1024));
                                                         });
                                 }
                             });
        }

        [Test]
        public void DownloadFileTaskTest() {
            Parallel.ForEach(GetAllWebClients(),
                             client => {
                                 var aClient = client;

                                 foreach(var uriString in UriStrings) {
                                     var filename =
                                         FileTool.GetTempFileName("NSoft.NFramework_WebClient_" + "_" +
                                                                  Thread.CurrentThread.ManagedThreadId + "_");
                                     using(var task = aClient.DownloadFileTask(uriString, filename)) {
                                         if(IsDebugEnabled)
                                             log.Debug("비동기 다운로드를 요청했습니다. 파일에 저장될 것입니다. file=" + filename);

                                         // Task가 끝날 때까지 기다립니다.
                                         task.Wait();
                                         Assert.IsFalse(aClient.IsBusy);

                                         Assert.IsTrue(task.IsCompleted);
                                     }

                                     var fi = new FileInfo(filename);
                                     Assert.IsTrue(fi.Exists);
                                     Assert.IsTrue(fi.Length > 0);
                                 }
                             });
        }

        [Test]
        public void OpenReadTaskTest() {
            Parallel.ForEach(GetAllWebClients(),
                             client => {
                                 var aClient = client;
                                 foreach(var task in UriStrings.Select(uri => aClient.OpenReadTask(uri))) {
                                     using(var stream = task.Result) {
                                         Assert.IsNotNull(stream);

                                         var content = stream.ToText(DefaultEncoding);

                                         // var content = StringTool.ToString(stream, DefaultEncoding);
                                         // Assert.IsNotEmpty(content);

                                         if(IsDebugEnabled)
                                             log.Debug("OpenRead Result=" + content.EllipsisChar(1024));
                                     }
                                 }
                             });
        }

        [Test]
        public void ThreadTest() {
            TestTool.RunTasks(2,
                              DownloadDataTaskTest,
                              DownloadStringTaskTest,
                              DownloadFileTaskTest,
                              OpenReadTaskTest);
        }
#endif

        private static IEnumerable<WebClient> GetAllWebClients() {
            return
                new List<WebClient>
                {
                    WebClientFactory.Instance.CreateWebClient(),
                    WebClientFactory.Instance.CreateWebClientAsFirefox(),
                    WebClientFactory.Instance.CreateWebClientAsChrome(),
                    WebClientFactory.Instance.CreateWebClientAsIE8()
                };
        }
    }
}