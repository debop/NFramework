using System;
using System.Linq;
using System.Threading.Tasks;
using NSoft.NFramework.LinqEx;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class HtmlAsyncCacheTestCase : ParallelismFixtureBase {
#if !SILVERLIGHT
        private static readonly string[] _urlStrings = new[]
                                                       {
                                                           @"http://www.naver.com/",
                                                           @"http://www.daum.net",
                                                           @"http://debop.egloos.com",
                                                           @"http://m.kr.yahoo.com/",
                                                           @"http://www.amazon.com/"
                                                       };

        [ThreadStatic] private HtmlAsyncCache _htmlCache = new HtmlAsyncCache();

        [Test]
        public void Can_Cache_Html_Threads() {
            TestTool.RunTasks(4,
                              () => {
                                  _htmlCache = new HtmlAsyncCache();
                                  Can_Cache_Html();
                              });
        }

        [Test]
        public void Can_Cache_Html() {
            // 첫번째 다운로드는 모두 기다려서 캐시에 저장되게 합니다.
            //
            using(new OperationTimer("First Load", false)) {
                var tasks = _urlStrings.Select(addr => _htmlCache.GetValue(new Uri(addr))).ToArray();

                Task.WaitAll(tasks);

                tasks.RunEach(task => Assert.IsNotEmpty(task.Result));
            }

            // 캐시에 저장되어 있으므로 무지 빠를 것입니다.
            //
            using(new OperationTimer("Second Load", false)) {
                var tasks = _urlStrings.Select(addr => _htmlCache.GetValue(new Uri(addr))).ToArray();

                Task.WaitAll(tasks);

                tasks.RunEach(task => Assert.IsNotEmpty(task.Result));
            }
        }
#endif
    }
}