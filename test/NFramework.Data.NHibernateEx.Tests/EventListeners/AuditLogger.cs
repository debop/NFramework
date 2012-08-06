using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.EventListeners {
    public class AuditLogger {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public void Insert(IStateEntity entity) {
            if(entity != null)
                if(log.IsDebugEnabled)
                    log.Debug("Entity Inserted: {0}={1}", entity.GetType(), entity);
        }

        public void Update(IStateEntity entity) {
            if(entity != null)
                if(log.IsDebugEnabled)
                    log.Debug("Entity Updated: {0}={1}", entity.GetType(), entity);
        }

        public void Delete(IStateEntity entity) {
            if(entity != null)
                if(log.IsDebugEnabled)
                    log.Debug("Entity Deleted: {0}={1}", entity.GetType(), entity);
        }
    }
}