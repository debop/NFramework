using System;
using System.Linq;
using NSoft.NFramework.Tools;
using NSoft.NFramework.UnitTesting;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.DataStructures.Async {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class FutureWebCacheTestCase : ParallelismFixtureBase {
#if !SILVERLIGHT

        private static readonly string[] _urlStrings = new[]
                                                       {
                                                           @"http://www.naver.com/",
                                                           @"http://www.daum.net/",
                                                           @"http://debop.egloos.com/",
                                                           //@"http://m.kr.yahoo.com/",
                                                           //@"http://www.amazon.com/"
                                                       };

        [ThreadStatic] private readonly FutureWebCache _futureWebCache = new FutureWebCache();

        [Test]
        public void Can_Cache_WebContents() {
            // 첫번째 다운로드는 모두 기다려서 캐시에 저장되게 합니다.
            //
            using(new OperationTimer("First Load", false)) {
                var contents = _urlStrings.Select(addr => _futureWebCache.GetValue(new Uri(addr))).ToArray();
                Assert.AreEqual(_urlStrings.Length, contents.Length);
                Assert.IsTrue(contents.All(content => content.IsNotWhiteSpace()));
            }

            // 캐시에 저장되어 있으므로 무지 빠를 것입니다.
            //
            using(new OperationTimer("Second Load", false)) {
                var contents = _urlStrings.Select(addr => _futureWebCache.GetValue(new Uri(addr))).ToArray();

                Assert.AreEqual(_urlStrings.Length, contents.Length);
                Assert.IsTrue(contents.All(content => content.IsNotWhiteSpace()));
            }
        }
#endif
    }
}