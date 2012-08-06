using System;
using System.Collections.Concurrent;
using System.Linq;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    /// <summary>
    /// 엔티티를 원하는 Proxy로 만들기 위한 NHibernate.Interceptor의 기본 Interceptor입니다.
    /// NOTE: GetEntityName() 과 CreateProxy() 를 재정의하여야 합니다.
    /// </summary>
    [Serializable]
    public abstract class ProxyInterceptorBase : EmptyInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly ConcurrentDictionary<string, Type> _entityTypeCache = new ConcurrentDictionary<string, Type>();
        private static readonly object _syncLock = new object();

        public static Type FindType(string clazz, ISession session, EntityMode entityMode) {
            if(IsDebugEnabled)
                log.Debug("인스턴싱할 엔티티 형식을 찾습니다... clazz=[{0}], entityMode=[{1}]", clazz, entityMode);

            var result = session.SessionFactory.GetEntityType(clazz, entityMode);

            if(result != null) {
                // lock(_syncLock)
                return _entityTypeCache.GetOrAdd(clazz, result);
            }

            foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                result = assembly.GetType(clazz, false, true);

                if(result != null) {
                    return _entityTypeCache.GetOrAdd(clazz, result);
                    //lock(_syncLock)
                    //    _entityTypeCache.AddValue(clazz, result);

                    //return result;
                }
            }

            return _entityTypeCache.GetOrAdd(clazz, result);
        }

        private ISession _session;

        /// <summary>
        /// Proxy가 제공할 대표 Interface의 형식 (예: typeof(INotifyPropertyChanged), typeof(IEditableObjecgt))
        /// </summary>
        public abstract Type ProxyInterface { get; }

        public override void SetSession(ISession session) {
            _session = session;
            base.SetSession(session);
        }

        public override string GetEntityName(object entity) {
            var entityName = entity.GetType().GetInterfaces().Contains(ProxyInterface)
                                 ? entity.GetType().BaseType.FullName
                                 : entity.GetType().FullName;

            if(IsDebugEnabled)
                log.Debug("엔티티의 수형은 [{0}] 이고, EntityName은 [{1}] 입니다.", entity.GetType().FullName, entityName);

            return entityName;
        }

        public override object Instantiate(string clazz, EntityMode entityMode, object id) {
            if(entityMode == EntityMode.Poco) {
                var type = _entityTypeCache.ContainsKey(clazz)
                               ? _entityTypeCache[clazz]
                               : FindType(clazz, _session, entityMode);

                if(type != null) {
                    if(IsDebugEnabled)
                        log.Debug("NHibernate.Interceptor[{0}]가 엔티티[{1}]에 대응되는 Proxy를 생성합니다...", GetType().FullName, type.FullName);

                    var instance = CreateProxy(type);
                    _session.SessionFactory.GetClassMetadata(clazz).SetIdentifier(instance, id, entityMode);
                    return instance;
                }
            }
            return base.Instantiate(clazz, entityMode, id);
        }

        /// <summary>
        /// NOTE: Proxy 생성 시 꼭 Type을 이용하여 Proxy를 생성해야 제대로 됩니다!!! Target Instance 으로 Proxy를 생성하면 예외가 발생합니다.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected abstract object CreateProxy(Type entityType);
    }
}