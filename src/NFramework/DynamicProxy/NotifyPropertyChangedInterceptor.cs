using System;
using System.ComponentModel;
using Castle.DynamicProxy;

namespace NSoft.NFramework.DynamicProxy {
    /// <summary>
    /// 특정 형식을 <see cref="INotifyPropertyChanged"/> 인터페이스를 구현한 것처럼 해주는 Interceptor입니다.
    /// </summary>
    [Serializable]
    public class NotifyPropertyChangedInterceptor : Castle.DynamicProxy.IInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private PropertyChangedEventHandler _propertyChangedHandler;

        /// <summary>
        /// 원본 인스턴스에 대한 작업에 대해 Intercept를 수행합니다.
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation) {
            var methodName = invocation.Method.Name;
            var target = invocation.InvocationTarget ?? invocation.Proxy;

            if(invocation.Method.DeclaringType == typeof(INotifyPropertyChanged)) {
                var propertyChangedEventHandler = invocation.Arguments[0] as PropertyChangedEventHandler;

                if(propertyChangedEventHandler == null)
                    return;

                if(methodName.StartsWith("add_", StringComparison.Ordinal)) {
                    _propertyChangedHandler += propertyChangedEventHandler;

                    if(IsDebugEnabled)
                        log.Debug("Proxy에 INotifyPropertyChanged와 관련된 PropertyChanged 이벤트에 Handler를 추가했습니다.");

                    return;
                }
                if(methodName.StartsWith("remove_", StringComparison.Ordinal)) {
                    With.TryAction(() => { _propertyChangedHandler -= propertyChangedEventHandler; });

                    if(IsDebugEnabled)
                        log.Debug("Proxy에 INotifyPropertyChanged와 관련된 PropertyChanged 이벤트에 Handler를 삭제했습니다.");

                    return;
                }
            }

            //
            // Proxied 된 원본 객체의 메소드를 호출합니다.
            //
            invocation.Proceed();

            if(methodName.StartsWith("set_", StringComparison.Ordinal)) {
                if(_propertyChangedHandler != null) {
                    if(IsDebugEnabled)
                        log.Debug("Interceptor가 객체[{0}].속성[{1}]에 값[{2}]를 설정하고, NotifyPropertyChangedEventHandler를 수행합니다.",
                                  target.GetType().FullName, methodName.Substring(4), invocation.Arguments[0]);

                    OnPropertyChanged(invocation.Proxy ?? invocation.InvocationTarget, methodName.Substring(4));
                }
            }
        }

        protected virtual void OnPropertyChanged(object invocationTarget, string propertyName) {
            var handler = _propertyChangedHandler;
            if(handler != null)
                handler(invocationTarget, new PropertyChangedEventArgs(propertyName));
        }
    }
}