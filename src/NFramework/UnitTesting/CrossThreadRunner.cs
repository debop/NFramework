using System;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;

namespace NSoft.NFramework.UnitTesting {
    /// <summary>
    /// Multi-Thread 환경하에서 Test를 하기 위한 보조 클래스이다.
    /// </summary>
    /// <remarks>
    /// Multi-Thread 환경에서 Test를 하고자 할 때 
    /// Test하고자 하는 함수를 <see cref="ThreadStart"/> 로 Wrapping 하여 
    /// <see cref="CrossThreadRunner"/>를 생성시킨다.
    /// 그후 <see cref="CrossThreadRunner"/>.RunEach() 을 호출하면 Test가 이루어진다.
    /// </remarks>
    /// <example>
    ///     <code>
    ///     CrossThreadTestRunner threadRunner = new CrossThreadTestRunner(new ThreadStart(doThreadWork));
    ///     threadRunner.RunEach();
    ///     </code>
    /// </example>
    public class CrossThreadRunner {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private readonly ThreadStart _userDelegate;
        private Exception _lastException;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="userDelegate">Thread 안에서 수행할 메소드</param>
        public CrossThreadRunner(ThreadStart userDelegate) {
            userDelegate.ShouldNotBeNull("userDelegate");

            _userDelegate = userDelegate;
            _lastException = null;
        }

        /// <summary>
        /// 쓰레드 하에서 수행
        /// </summary>
        public void Run() {
            var thread = new Thread(MultiThreadedWorker);
            thread.Start();
            thread.Join();

            if(_lastException != null)
                ThrowExceptionPreservingStack(_lastException);
        }

        /// <summary>
        /// ThreadStart를 try-catch 구문 안에서 실행시킨다.
        /// </summary>
        private void MultiThreadedWorker() {
            try {
                _userDelegate.DynamicInvoke();
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("예외가 발생했습니다.", ex);

                _lastException = ex;
            }
        }

#if !SILVERLIGHT
        [ReflectionPermission(SecurityAction.Demand)]
#endif
        private static void ThrowExceptionPreservingStack(Exception exception) {
            var remoteStackTracingString
                = typeof(Exception).GetField("_remoteStackTracingString", BindingFlags.Instance | BindingFlags.NonPublic);
            remoteStackTracingString.SetValue(exception, exception.StackTrace + Environment.NewLine);
            throw exception;
        }
    }
}