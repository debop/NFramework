using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    /// <summary>
    /// 다양한 Interceptor를 가지며, IoC를 이용하여 다중의 Interceptor를 등록하여 처리할 수 있다. 
    /// </summary>
    /// <remarks>
    /// <see cref="InterceptorDecorator"/>와는 처리 방식이 약간 다르다.
    /// </remarks>
    /// <example>
    /// IoC 환경설정에서 아래와 같이 여러개의 Interceptor를 MultipleInterceptor에 등록하고, 
    /// SessionFactory에서 Session을 열때 MultipleInterceptor를 사용하게 되면, 등록된 모든 Interceptor를 이용할 수 있다. 
    /// (단 항상 MultipleInterceptor를 제일 먼저 정의해 줘야 한다.)
    /// <code>
    /// &lt;component id="NHibernate.Interceptor"
    /// 		   service="NHibernate.IInterceptor, NHibernate"
    /// 		   type="NSoft.NFramework.Data.NHibernateEx.Interceptors.MultipleInterceptor, NSoft.NFramework.Data.NHibernateEx"&gt;
    /// 	&lt;parameters&gt;
    /// 		&lt;interceptors&gt;
    /// 			&lt;array type="NHibernate.IInterceptor, NHibernate"&gt;
    /// 				&lt;item&gt;${EntityStateInterceptor}&lt;/item&gt;
    /// 				&lt;item&gt;${LoggingInterceptor}&lt;/item&gt;
    /// 			&lt;/array&gt;
    /// 		&lt;/interceptors&gt;
    /// 	&lt;/parameters&gt;
    /// &lt;/component&gt;
    /// 
    /// &lt;component id="EntityStateInterceptor"
    /// 				   service="NHibernate.IInterceptor, NHibernate"
    /// 				   type="NSoft.NFramework.Data.NHibernateEx.Interceptors.EntityStateInterceptor, NSoft.NFramework.Data.NHibernateEx"&gt;
    /// &lt;/component&gt;
    /// 
    /// &lt;component id="LoggingInterceptor"
    /// 		   service="NHibernate.IInterceptor, NHibernate"
    /// 		   type="NSoft.NFramework.Data.NHibernateEx.Interceptors.LoggingInterceptor, NSoft.NFramework.Data.NHibernateEx"&gt;
    /// &lt;/component>
    /// </code>
    /// </example>
    [Serializable]
    public class MultipleInterceptor : List<IInterceptor>, IInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="interceptors"></param>
        public MultipleInterceptor(IEnumerable<IInterceptor> interceptors) : base(interceptors) {
            if(IsDebugEnabled) {
                log.Debug("NHibernate용 MultipleInterceptor가 생성되었습니다. 등록된 Interceptor는 다음과 같습니다.");
                var sb = new StringBuilder().AppendLine();

                ForEach(interceptor => sb.Append(interceptor.GetType().AssemblyQualifiedName).AppendLine());
                log.Debug("등록된 Interceptors:" + sb.ToString());
            }
        }

        public virtual bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            ForEach(interceptor => interceptor.OnLoad(entity, id, state, propertyNames, types));
            return false;
        }

        public virtual bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
                                         string[] propertyNames, IType[] types) {
            ForEach(interceptor => interceptor.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types));
            return false;
        }

        public virtual bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            ForEach(interceptor => interceptor.OnSave(entity, id, state, propertyNames, types));
            return false;
        }

        public virtual void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            ForEach(interceptor => interceptor.OnDelete(entity, id, state, propertyNames, types));
        }

        public virtual void OnCollectionRecreate(object collection, object key) {
            //ForEach(interceptor => interceptor.OnCollectionRecreate(collection, key));
        }

        public virtual void OnCollectionRemove(object collection, object key) {
            //ForEach(interceptor => interceptor.OnCollectionRemove(collection, key));
        }

        public virtual void OnCollectionUpdate(object collection, object key) {
            //ForEach(interceptor => interceptor.OnCollectionUpdate(collection, key));
        }

        public virtual void PreFlush(ICollection entities) {
            ForEach(interceptor => interceptor.PreFlush(entities));
        }

        public virtual void PostFlush(ICollection entities) {
            ForEach(interceptor => interceptor.PostFlush(entities));
        }

        public virtual bool? IsTransient(object entity) {
            foreach(var interceptor in this) {
                var isTransient = interceptor.IsTransient(entity);
                if(isTransient.HasValue)
                    return isTransient;
            }

            return null;
        }

        public virtual int[] FindDirty(object entity, object id, object[] currentState, object[] previousState,
                                       string[] propertyNames, IType[] types) {
            foreach(var interceptor in this) {
                var indexes = interceptor.FindDirty(entity, id, currentState, previousState, propertyNames, types);
                if(indexes != null)
                    return indexes;
            }

            return null;
        }

        public virtual object Instantiate(string entityName, EntityMode entityMode, object id) {
            foreach(var interceptor in this) {
                var obj = interceptor.Instantiate(entityName, entityMode, id);
                if(obj != null)
                    return obj;
            }
            return null;
        }

        public virtual string GetEntityName(object entity) {
            //foreach (var interceptor in this)
            //{
            //    var name = interceptor.GetEntityName(entity);
            //    if (name != null)
            //        return name;
            //}
            return null;
        }

        public virtual object GetEntity(string entityName, object id) {
            //foreach (var interceptor in this)
            //{
            //    var obj = interceptor.GetEntity(entityName, id);
            //    if (obj != null)
            //        return obj;
            //}
            return null;
        }

        public virtual void AfterTransactionBegin(ITransaction tx) {
            ForEach(interceptor => interceptor.AfterTransactionBegin(tx));
        }

        public virtual void BeforeTransactionCompletion(ITransaction tx) {
            ForEach(interceptor => interceptor.BeforeTransactionCompletion(tx));
        }

        public virtual void AfterTransactionCompletion(ITransaction tx) {
            ForEach(interceptor => interceptor.AfterTransactionCompletion(tx));
        }

        public virtual void SetSession(ISession session) {
            ForEach(interceptor => interceptor.SetSession(session));
        }

        public virtual SqlString OnPrepareStatement(SqlString sql) {
            //foreach (var interceptor in this)
            //{
            //    var sqlString = interceptor.OnPrepareStatement(sql);
            //    if (sqlString != null)
            //        return sqlString;
            //}
            return sql;
        }
    }
}