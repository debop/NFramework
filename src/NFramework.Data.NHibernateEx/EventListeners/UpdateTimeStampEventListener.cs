using System;
using NHibernate.Event;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.EventListeners {
    /// <summary>
    /// 엔티티가 추가되거나 갱신 될 때에, <see cref="IUpdateTimestampedEntity"/> 엔티티라면, UpdateTime 을 갱신합니다.
    /// </summary>
    public sealed class UpdateTimestampEventListener : IPreInsertEventListener, IPreUpdateEventListener {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 현재 시각을 반환한다. UTC 를 원한다면 상속받아, 재정의를 하십시요.
        /// </summary>
        /// <returns></returns>
        public DateTime? GetCurrentTime() {
            return DateTime.Now;
        }

        /// <summary>
        /// 엔티티를 처음 등록할 때, 엔티티의 UpdateTimestamp 속성 값이 없을 때, 현재 시각으로 설정합니다.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public bool OnPreInsert(PreInsertEvent @event) {
            if(IsDebugEnabled)
                log.Debug("엔티티에 대해 PreInsertEvent를 수행합니다...");

            var entity = @event.Entity as IUpdateTimestampedEntity;

            // UpdateTimestamp 값을 사용자가 미리 지정했을 수도 있으므로 값이 없을 때에만 할당한다.
            if(entity != null && entity.UpdateTimestamp.HasValue == false)
                entity.UpdateTimestamp = GetCurrentTime();

            return false;
        }

        /// <summary>
        /// 엔티티를 Update 하기 전에, 엔티티의 UpdateTimestamp 속성 값을 현재 시각으로 설정합니다.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        public bool OnPreUpdate(PreUpdateEvent @event) {
            if(IsDebugEnabled)
                log.Debug("엔티티에 대해 PreUpdateEvent를 수행합니다...");

            var entity = @event.Entity as IUpdateTimestampedEntity;

            if(entity != null)
                entity.UpdateTimestamp = GetCurrentTime();

            return false;
        }
    }
}