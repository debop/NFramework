using System.Runtime.Remoting.Messaging;
using System.Threading;
using NUnit.Framework;

namespace NSoft.NFramework.Threading {
    [TestFixture]
    public class ExecutionContextCase : AbstractFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        private static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        [Test]
        public void ExecutionContext_SuppressFlow_Test() {
            // Main Thread의 Logical call context 에 Data를 저장합니다.
            CallContext.LogicalSetData("Name", "배성혁");

            ThreadPool.QueueUserWorkItem(_ => Assert.IsNotNull(CallContext.LogicalGetData("Name")));

            // Main Thread의 실행 컨텍스트를 중지시킨다.
            ExecutionContext.SuppressFlow();

            // 실행 컨텍스트가 중지된 상태에스는 스레드 풀에서 호출 컨텍스트의 논리적 데이터를 조회하지 못합니다.
            //
            ThreadPool.QueueUserWorkItem(_ => Assert.IsNull(CallContext.LogicalGetData("Name")));


            ExecutionContext.RestoreFlow();

            // 실행 컨텍스트가 다시 시작되었으므로, 조회가 가능하다.
            ThreadPool.QueueUserWorkItem(_ => Assert.IsNotNull(CallContext.LogicalGetData("Name")));
        }

        [Test]
        public void Cooperative_Cancellation() {
            var cts = new CancellationTokenSource();

            cts.Token.Register(() => log.Debug("Cancellation Token에 Register를 이용하여, 취소 시 정리작업을 수행하면 됩니다."), true);
            cts.Token.Register(() => log.Debug("동시 다발적인 작업!!!"));

            ThreadPool.QueueUserWorkItem(_ => {
                                             for(var i = 0; i < 1000; i++) {
                                                 if(cts.Token.IsCancellationRequested) {
                                                     log.Debug("Counting is cancelled");
                                                     return;
                                                 }

                                                 if(IsDebugEnabled)
                                                     log.Debug("Count = " + i);

                                                 Thread.Sleep(2);
                                             }
                                             log.Debug("Count is done.");
                                         });

            log.Debug("Run counting...");

            Thread.Sleep(1000);

            cts.Cancel();

            log.Debug("Waiting cancelling...");
            cts.Token.WaitHandle.WaitOne();

            log.Debug("Cancellation is success");
        }

        [Test]
        public void Linked_Cancellation_Token() {
            var cts1 = new CancellationTokenSource();
            cts1.Token.Register(() => log.Debug("cts1 canceled"));

            var cts2 = new CancellationTokenSource();
            cts2.Token.Register(() => log.Debug("cts2 canceled"));

            // 두 개의 CancellationToken을 링크시켰습니다.
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
            linkedCts.Token.Register(() => log.Debug("linedCts canceled"));

            // cts2 만 cancel 시켰습니다!!!
            cts2.Cancel();

            if(IsDebugEnabled)
                log.Debug("cts1 canceled={0}, cts2 canceled={1}, linkedCts canceled={2}",
                          cts1.IsCancellationRequested, cts2.IsCancellationRequested, linkedCts.IsCancellationRequested);

            // cts2 Token을 가지는 linkedCts도 취소 요청을 받아 취소됩니다.
            Assert.IsTrue(linkedCts.IsCancellationRequested);

            // cts1은 직접적인 취소를 요청 받은게 아니므로, 취소요청을 받지 않았습니다.
            Assert.IsFalse(cts1.IsCancellationRequested);

            // cts2는 당근 취소 요청을 받았습니다.
            Assert.IsTrue(cts2.IsCancellationRequested);
        }
    }
}