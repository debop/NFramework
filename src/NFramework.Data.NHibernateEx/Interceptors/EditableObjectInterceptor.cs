using System;
using System.ComponentModel;
using NSoft.NFramework.DynamicProxy;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    /// <summary>
    /// 엔티티에 <see cref="IEditableObject"/>를 제공하는 Proxy로 만들어주는 Interceptor입니다.
    /// </summary>
    [Serializable]
    public sealed class EditableObjectInterceptor : ProxyInterceptorBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Proxy가 제공할 대표 Interface의 형식 (예: typeof(INotifyPropertyChanged), typeof(IEditableObjecgt))
        /// </summary>
        public override Type ProxyInterface {
            get { return typeof(IEditableObject); }
        }

        /// <summary>
        /// NOTE: Proxy 생성 시 꼭 Type을 이용하여 Proxy를 생성해야 제대로 됩니다!!! Target Instance 으로 Proxy를 생성하면 예외가 발생합니다.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected override object CreateProxy(Type entityType) {
            return DynamicProxyTool.CreateEditableObject(entityType);
        }
    }
}