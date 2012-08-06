using System;
using System.Collections;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    /// <summary>
    /// Decorator 패턴을 이용하여 여러개의 Interceptor를 Chain 방식으로 호출할 수 있도록 하였다. <see cref="MultipleInterceptor"/> 와는 다른 효과를 볼 수 있다.
    /// </summary>
    [Serializable]
    public class InterceptorDecorator : IInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public IInterceptor InnerInterceptor { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inner"></param>
        public InterceptorDecorator(IInterceptor inner) {
            InnerInterceptor = inner ?? new EmptyInterceptor();
        }

        public virtual bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            return InnerInterceptor.OnLoad(entity, id, state, propertyNames, types);
        }

        public virtual bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
                                         string[] propertyNames,
                                         IType[] types) {
            return InnerInterceptor.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public virtual bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            return InnerInterceptor.OnSave(entity, id, state, propertyNames, types);
        }

        public virtual void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            InnerInterceptor.OnDelete(entity, id, state, propertyNames, types);
        }

        public virtual void OnCollectionRecreate(object collection, object key) {
            InnerInterceptor.OnCollectionRecreate(collection, key);
        }

        public virtual void OnCollectionRemove(object collection, object key) {
            InnerInterceptor.OnCollectionRemove(collection, key);
        }

        public virtual void OnCollectionUpdate(object collection, object key) {
            InnerInterceptor.OnCollectionUpdate(collection, key);
        }

        public virtual void PreFlush(ICollection entities) {
            InnerInterceptor.PreFlush(entities);
        }

        public virtual void PostFlush(ICollection entities) {
            InnerInterceptor.PostFlush(entities);
        }

        public virtual bool? IsTransient(object entity) {
            return InnerInterceptor.IsTransient(entity);
        }

        public virtual int[] FindDirty(object entity, object id, object[] currentState, object[] previousState,
                                       string[] propertyNames,
                                       IType[] types) {
            return InnerInterceptor.FindDirty(entity, id, currentState, previousState, propertyNames, types);
        }

        public virtual object Instantiate(string entityName, EntityMode entityMode, object id) {
            return InnerInterceptor.Instantiate(entityName, entityMode, id);
        }

        public virtual string GetEntityName(object entity) {
            return InnerInterceptor.GetEntityName(entity);
        }

        public virtual object GetEntity(string entityName, object id) {
            return InnerInterceptor.GetEntity(entityName, id);
        }

        public virtual void AfterTransactionBegin(ITransaction tx) {
            InnerInterceptor.AfterTransactionBegin(tx);
        }

        public virtual void BeforeTransactionCompletion(ITransaction tx) {
            InnerInterceptor.BeforeTransactionCompletion(tx);
        }

        public virtual void AfterTransactionCompletion(ITransaction tx) {
            InnerInterceptor.AfterTransactionCompletion(tx);
        }

        public virtual void SetSession(ISession session) {
            InnerInterceptor.SetSession(session);
        }

        public virtual SqlString OnPrepareStatement(SqlString sql) {
            return InnerInterceptor.OnPrepareStatement(sql);
        }
    }
}