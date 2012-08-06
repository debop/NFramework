using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using Castle.DynamicProxy;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.DynamicProxy {
    /// <summary>
    /// 객체에 <see cref="IEditableObject"/> 인터페이스를 제공하는 Proxy를 생성할 때, 제공하는 Interceptor입니다.
    /// </summary>
    [Serializable]
    public class EditableObjectInterceptor : Castle.DynamicProxy.IInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private bool _isEditing;
        private readonly ConcurrentDictionary<string, object> _propertyTempValues = new ConcurrentDictionary<string, object>();

        public void Intercept(IInvocation invocation) {
            var methodName = invocation.Method.Name;
            var target = invocation.InvocationTarget ?? invocation.Proxy;

            // NOTE: _isEditing 의 값 설정 연산의 순서가 상당히 중요하다. 항상 먼저 해주어야 한다.
            switch(methodName) {
                case @"BeginEdit":
                    _isEditing = true;
                    return;

                case @"CancelEdit":
                    _isEditing = false;
                    _propertyTempValues.Clear();
                    return;

                case @"EndEdit":
                    _isEditing = false;
                    AssignValues(target);
                    _propertyTempValues.Clear();
                    return;
            }

            if(_isEditing == false) {
                invocation.Proceed();
                return;
            }

            var isSetProperty = methodName.StartsWith("set_", StringComparison.Ordinal);
            var isGetProperty = methodName.StartsWith("get_", StringComparison.Ordinal);

            //  Proxied 객체의 메소드에 대해서는 처리할 필요가 없다.
            //
            if(isSetProperty == false && isGetProperty == false)
                return;

            var propertyName = methodName.Substring(4);

            if(isSetProperty) {
                if(IsDebugEnabled)
                    log.Debug("편집된 속성 값을 보관합니다. propertyName=[{0}], value=[{1}]", propertyName, invocation.Arguments[0]);

                _propertyTempValues.AddValue(propertyName, invocation.Arguments[0]);
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("속성 값을 반환합니다. 편집된 값이 있다면 편집된 값을 반환하고, 아니면, 원본 객체의 값을 반환합니다.");

                if(_propertyTempValues.ContainsKey(propertyName))
                    invocation.ReturnValue = _propertyTempValues[propertyName];
                else
                    invocation.Proceed();
            }
        }

        private void AssignValues(object target) {
            target.ShouldNotBeNull("target");

            var targetType = target.GetType();

            if(IsDebugEnabled)
                log.Debug("객체의 편집된 값을 최종적으로 적용합니다. Target=[{0}]", targetType.FullName);

            foreach(var propertyEntry in _propertyTempValues) {
                if(IsDebugEnabled)
                    log.Debug("속성 설정: [{0}] = [{1}]", propertyEntry.Key, propertyEntry.Value);
#if !SILVERLIGHT
                Fasterflect.PropertyExtensions.SetPropertyValue(target, propertyEntry.Key, propertyEntry.Value);
#else
				var propertyInfo = targetType.GetProperty(propertyEntry.Key);
				propertyInfo.SetValue(target, propertyEntry.Value, null);
#endif
            }
        }
    }
}