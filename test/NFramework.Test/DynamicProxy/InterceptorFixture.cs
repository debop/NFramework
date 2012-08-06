using System;
using Castle.DynamicProxy;
using NUnit.Framework;

namespace NSoft.NFramework.DynamicProxy {
    /// <summary>
    /// Castle 의 DynamicProxy를 이용하여, 여러가지 작업을 할 수 있음을 보여준다.
    /// </summary>
    [TestFixture]
    public class InterceptorFixture {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        [Test]
        public void CanCreateInterfaceProxyWithTarget() {
            var generator = new ProxyGenerator();

            var proxy = (IMyInterface)generator.CreateInterfaceProxyWithTarget(typeof(IMyInterface),
                                                                               new MyInterfaceImpl(),
                                                                               new MyInterceptor());

            Assert.IsNotNull(proxy);
            Assert.IsInstanceOf<IMyInterface>(proxy);
            Assert.AreEqual(10, proxy.Calc(5, 5));
        }

        public class MyInterceptor : StandardInterceptor {
            protected override void PreProceed(IInvocation invocation) {
                base.PreProceed(invocation);
                if(IsDebugEnabled)
                    log.Debug("PreProceed. invocation method=" + invocation.Method.Name);
            }

            protected override void PostProceed(IInvocation invocation) {
                base.PostProceed(invocation);
                if(IsDebugEnabled)
                    log.Debug("PostProceed. invocation method=" + invocation.Method.Name);
            }
        }

        public interface IMyInterface {
            int Calc(int x, int y);
            int Calc(int x, int y, int z, Single k);
        }

        public class MyInterfaceImpl : IMyInterface {
            public int Calc(int x, int y) {
                return x + y;
            }

            public int Calc(int x, int y, int z, float k) {
                return x + y + z + (int)k;
            }
        }
    }
}