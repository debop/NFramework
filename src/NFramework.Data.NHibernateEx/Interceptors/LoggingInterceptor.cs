using System;
using System.Collections;
using NHibernate;
using NHibernate.Type;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    /// <summary>
    /// NHibernate Interceptor의 활동을 Trace 레벨로 로그에 기록합니다. (로그 Level이 DEBUG 일때만 가능합니다.)
    /// </summary>
    [Serializable]
    public sealed class LoggingInterceptor : EmptyInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            if(IsDebugEnabled)
                log.Debug("OnLoad... entity=[{0}], id=[{1}], state=[{2}], propertyNames=[{3}], types=[{4}]",
                          entity, id, state, propertyNames, types);

            return false;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState,
                                          string[] propertyNames, IType[] types) {
            if(IsDebugEnabled)
                log.Debug(
                    "OnFlushDirty... entity=[{0}], id=[{1}], currentState=[{2}], previousState=[{3}], propertyNames=[{4}], types=[{5}]",
                    entity, id, currentState.CollectionToString(), previousState.CollectionToString(),
                    propertyNames.CollectionToString(), types.CollectionToString());

            return false;
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            if(IsDebugEnabled)
                log.Debug("OnSave... entity=[{0}], id=[{1}], state=[{2}], propertyNames=[{3}], types=[{4}]",
                          entity, id, state.CollectionToString(), propertyNames.CollectionToString(), types.CollectionToString());
            return false;
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types) {
            if(IsDebugEnabled)
                log.Debug("OnDelete... entity=[{0}], id=[{1}], state=[{2}], propertyNames=[{3}], types=[{4}]",
                          entity, id, state.CollectionToString(), propertyNames.CollectionToString(), types.CollectionToString());
        }

        //public override void OnCollectionRecreate(object collection, object key)
        //{
        //    if (IsDebugEnabled)
        //        log.Debug("OnCollectionRecreate... collection={0}, key={1}", collection, key);
        //}
        //public override void OnCollectionRemove(object collection, object key)
        //{
        //    if (IsDebugEnabled)
        //        log.Debug("OnCollectionRemove... collection={0}, key={1}", collection, key);
        //}
        //public override void OnCollectionUpdate(object collection, object key)
        //{
        //    if (IsDebugEnabled)
        //        log.Debug("OnCollectionUpdate... collection={0}, key={1}", collection, key);
        //}
        public override void PreFlush(ICollection entities) {
            if(IsDebugEnabled)
                log.Debug("PreFlush... entities=[{0}]", entities.CollectionToString());
        }

        public override void PostFlush(ICollection entities) {
            if(IsDebugEnabled)
                log.Debug("PostFlush... entities=[{0}]", entities.CollectionToString());
        }

        public override bool? IsTransient(object entity) {
            if(IsDebugEnabled)
                log.Debug("IsTransient... entity=[{0}]", entity);

            return null;
        }

        public override int[] FindDirty(object entity, object id, object[] currentState, object[] previousState,
                                        string[] propertyNames, IType[] types) {
            if(IsDebugEnabled)
                log.Debug(
                    "FindDirty... entity=[{0}], id=[{1}], currentState=[{2}], previousState=[{3}], propertyNames=[{4}], types=[{5}]",
                    entity, id, currentState.CollectionToString(), previousState.CollectionToString(),
                    propertyNames.CollectionToString(), types.CollectionToString());

            return null;
        }

        public override object Instantiate(string entityName, EntityMode entityMode, object id) {
            if(IsDebugEnabled)
                log.Debug("Instantiate... entityName=[{0}], entityMode=[{1}], id=[{2}]", entityName, entityMode, id);

            return null;
        }

        //public override string GetEntityName(object entity)
        //{
        //    if (IsDebugEnabled)
        //        log.Debug("GetEntityName... entity={0}", entity);
        //    return null;
        //}
        //public override object GetEntity(string entityName, object id)
        //{
        //    if (IsDebugEnabled)
        //        log.Debug("GetEntity... entityName={0}, id={1}", entityName, id);
        //    return null;
        //}
        public override void AfterTransactionBegin(ITransaction tx) {
            if(IsDebugEnabled)
                log.Debug("AfterTransactionBegin... tx=[{0}]", tx);
        }

        public override void BeforeTransactionCompletion(ITransaction tx) {
            if(IsDebugEnabled)
                log.Debug("BeforeTransactionCompletion... tx=[{0}]", tx);
        }

        public override void AfterTransactionCompletion(ITransaction tx) {
            if(IsDebugEnabled)
                log.Debug("AfterTransactionCompletion... tx=[{0}]", tx);
        }

        public override void SetSession(ISession session) {
            if(IsDebugEnabled)
                log.Debug("SetSession... session=[{0}]", session);
        }

        //public override SqlString OnPrepareStatement(SqlString sql)
        //{
        //    if (IsDebugEnabled)
        //        log.Debug("OnPrepareStatement... sql={0}", sql);

        //    return sql;
        //}
    }
}