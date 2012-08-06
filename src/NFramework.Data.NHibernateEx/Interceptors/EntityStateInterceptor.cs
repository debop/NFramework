using System;
using NHibernate;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.Interceptors {
    /// <summary>
    /// Entity 상태 정보를 유지하는 관리하는 Interceptor이다. 
    /// </summary>
    /// <remarks>
    /// <see cref="DataEntityBase{TId}"/>나 <see cref="StateEntityBase"/>를 상속받아서 사용할 때,
    /// Entity의 상태(IsTransient, IsSaved) 값을 제대로 제공받으려면, EntityStateInterceptor를 SessionFactory에 등록해 줘야 합니다.
    /// </remarks>
    /// <seealso cref="IStateEntity"/>
    [Serializable]
    public sealed class EntityStateInterceptor : EmptyInterceptor {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// Indicate that the specified object is transient object.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override bool? IsTransient(object entity) {
            bool? result = null;

            if(entity is IStateEntity)
                result = ((IStateEntity)entity).IsTransient;

            return result;
        }

        /// <summary>
        /// occurred when the specified entity is loaded.
        /// </summary>
        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames,
                                    NHibernate.Type.IType[] types) {
            if(entity is IStateEntity)
                return ((IStateEntity)entity).IsSaved = true;

            return false;
        }

        /// <summary>
        /// Occurred when the specified entity is saved.
        /// </summary>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames,
                                    NHibernate.Type.IType[] types) {
            if(entity is IStateEntity)
                return ((IStateEntity)entity).IsSaved = true;

            return false;
        }
    }
}