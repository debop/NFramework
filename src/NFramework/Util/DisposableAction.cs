using System;

namespace NSoft.NFramework {
    /// <summary>
    /// Context 처리시에 더 직관적인 문법을 사용하기 위한 Class입니다.
    /// Java의 anonymous class를 흉내낸 것으로서, 
    /// 이 클래스의 인스턴스가 Dispose 될 때, 어떤 Method를 호출하여 처리하도록 하기 위해서 사용된다.
    /// </summary>
    /// <remarks>
    /// try / finally 구문과 유사하지만, exception 발생시에는 disposable이 수행되지 않는다.
    /// </remarks>
    /// <example>
    /// <code>
    /// // using 구문 안의 code가 수행 된 후 action이 수행되도록 한다. (try / finally 구문과 유사 - exception 발생 시에는 action이 수행되지 않는다.)
    /// bool calledAtDisposing = false;
    /// using (DisposableAction action = new DisposableAction(delegate { calledAtDisposing = true; }))
    /// {
    ///		// some codes executed before execute action.
    /// 	Thread.Sleep(50);
    /// }
    /// 
    /// Assert.IsTrue(calledAtDisposing);   // calledAtDisposing is true at action is disposing
    /// </code>
    /// </example>
    [Serializable]
    public class DisposableAction : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="action">dispose 시에 수행할 action</param>
        public DisposableAction(Action action) {
            action.ShouldNotBeNull("action");
            Action = action;
        }

        /// <summary>
        /// 현재 인스턴스가 Dispose될 때 실행할 Action
        /// </summary>
        protected Action Action { get; private set; }

        /// <summary>
        /// 메모리 해제 여부
        /// </summary>
        public bool IsDisposed { get; protected set; }

        ///<summary>
        /// Allows an <see cref="T:System.Object" /> to attempt to free resources and perform other cleanup operations before the <see cref="T:System.Object" /> is reclaimed by garbage collection.
        ///</summary>
        ~DisposableAction() {
            Dispose(false);
        }

        ///<summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">need to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                try {
                    if(IsDebugEnabled)
                        log.Debug("Disposing 단계에서 지정된 Action을 호출합니다...");

                    // 지정된 Action을 인스턴스가 Dispose 될 때 수행한다.
                    //
                    if(Action != null)
                        Action();

                    if(IsDebugEnabled)
                        log.Debug("Disposing 단계의 지정된 Action을 수행했습니다!!!");
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled) {
                        log.Warn("DisposableAction 인스턴스를 Disposing시에  Action 호출 시 예외가 발생했습니다. 예외는 무시됩니다.");
                        log.Warn(ex);
                    }
                }
            }
            IsDisposed = true;
        }
    }
}