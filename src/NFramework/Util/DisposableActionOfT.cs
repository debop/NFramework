using System;

namespace NSoft.NFramework {
    /// <summary>
    /// Context 처리시에 더 직관적인 문법을 사용하기 위한 Class입니다.
    /// Java의 anonymous class를 흉내낸 것으로서, 
    /// 이 클래스의 인스턴스가 Dispose 될 때, 어떤 Method를 호출하여 처리하도록 하기 위해서 사용된다.
    /// </summary>
    /// <remarks>
    /// 주의할 점은 try / finally 구문과 유사하지만, exception 발생 시에는 disposable이 수행되지 않는다.
    /// </remarks>
    /// <example>
    /// <code>
    /// // using 구문 안의 code가 수행 된 후 action이 수행되도록 한다.
    /// 
    /// int expected = 4543;
    /// int actual = 0;
    /// 
    /// DisposableAction{int} action = new DisposableAction{int}(delegate(int i) { actual = i; }, expected);
    /// 
    /// Assert.AreNotEqual(actual, expected);   // before disposing
    /// 
    /// action.Dispose();
    /// 
    /// Assert.AreEqual(actual, expected);      // after dispoing
    /// </code>
    /// </example>
    [Serializable]
    public class DisposableAction<T> : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="action">Disposing 시에 호출되는 Action</param>
        /// <param name="val">Action 호출시 인자</param>
        public DisposableAction(Action<T> action, T val) {
            action.ShouldNotBeNull("action");

            Action = action;
            Value = val;
        }

        /// <summary>
        /// Disposing시에 호출되는 Action 대리자의 인자
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// Disposing 시에 호출될 Action
        /// </summary>
        protected Action<T> Action { get; private set; }

        #region IDisposable Members

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
                    if(Action != null)
                        Action(Value);

                    if(IsDebugEnabled)
                        log.Debug("Disposing 단계의 지정된 Action을 수행했습니다!!!");
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled)
                        log.WarnException("DisposableAction 인스턴스를 Disposing시에  Action 호출 시 예외가 발생했습니다!!!", ex);

                    // NOTE: 예외는 무시합니다. ==> 어차피 Disposing 되는 것이고, Action 내부에서 예외에 대한 처리를 수행할 것이기 때문이다.
                }
            }
            IsDisposed = true;
        }

        #endregion
    }
}