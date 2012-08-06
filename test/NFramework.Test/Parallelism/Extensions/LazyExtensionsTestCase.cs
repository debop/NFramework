using System;
using System.Threading;
using NSoft.NFramework.Parallelism.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.Parallelism.Extensions {
    [Microsoft.Silverlight.Testing.Tag("Parallel")]
    [TestFixture]
    public class LazyExtensionsTestCase : ParallelismFixtureBase {
        private const int LazyValue = 100;

        public static Lazy<int> CreateSampleLazy(int lazyValue) {
            return
                new Lazy<int>(() => {
                                  if(IsDebugEnabled)
                                      log.Debug("Lazy Value를 실제로 생성합니다...");

                                  Thread.Sleep(Rnd.Next(10, 100));

                                  if(IsDebugEnabled)
                                      log.Debug("Lazy Value를 실제로 생성했습니다!!!");

                                  return lazyValue;
                              });
        }

        [Test]
        public void Force_Value() {
            var lazy = CreateSampleLazy(LazyValue);
            Assert.AreEqual(LazyValue, lazy.Force().Value);
        }

        [Test]
        public void CreateValueAsync() {
            var lazy = CreateSampleLazy(LazyValue);
            var task = lazy.GetValueAsync();

            if(IsDebugEnabled)
                log.Debug("Future Pattern 형식으로 Task를 빌드했습니다...");

            Assert.AreEqual(LazyValue, task.Result);

            if(IsDebugEnabled)
                log.Debug("Task 작업이 완료했습니다.");
        }

#if !SILVERLIGHT
        [Test]
        public void CreateValue_By_Asynchronous_Delegate() {
            var lazy = CreateSampleLazy(LazyValue);
            var task = lazy.GetValueTask();

            if(IsDebugEnabled)
                log.Debug("비동기 델리게이트 호출방식의 Task를 빌드했습니다...");

            Assert.AreEqual(LazyValue, task.Result);

            if(IsDebugEnabled)
                log.Debug("비동기 델리게이트 호출방식의 Task가 완료했습니다.");
        }
#endif
    }
}