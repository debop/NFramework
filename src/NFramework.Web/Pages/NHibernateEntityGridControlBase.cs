using System;
using NSoft.NFramework.Data;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Reflections;
using NHibernate.Criterion;

namespace RCL.Web.Pages
{
    /// <summary>
    /// NHibernate용 엔티티를 Grid에 표시하고, 관리하는 웹 UserControl의 기본 페이지입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NHibernateEntityGridControlBase<T> : EntityGridControlBase<T> where T : class, IDataObject
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정한 엔티티의 ID를 가진 NHibernate용 엔티티들을 삭제합니다.
        /// </summary>
        /// <param name="entityIds">삭제할 엔티티의 Identifier의 컬렉션</param>
        protected override void RemoveEntitiesById(System.Collections.ICollection entityIds)
        {
            if(IsDebugEnabled)
                log.Debug(@"엔티티[{0}] 정보를 삭제합니다... 삭제할 엔티티 ID={1}", ConcreteType.Name, entityIds.CollectionToString());

            IUnitOfWorkTransaction transaction = null;
            try
            {
                transaction = UnitOfWork.Current.BeginTransaction();

                var criteria = DetachedCriteria.For<T>().AddIn(@"Id", entityIds);
                Repository<T>.DeleteAll(criteria);

                transaction.Commit();

                if(IsDebugEnabled)
                    log.Debug(@"엔티티[{0}] 정보 삭제에 성공했습니다.. 삭제된 엔티티 ID={1}", ConcreteType.Name, entityIds.CollectionToString());
            }
            catch(Exception ex)
            {
                if(log.IsErrorEnabled)
                    log.ErrorException(string.Format(@"엔티티[{0}] 삭제시 예외가 발생했습니다.", ConcreteType.Name), ex);

                if(transaction != null)
                    transaction.Rollback();
            }
        }
    }
}