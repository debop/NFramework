using NHibernate.Event;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.NHibernateEx.EventListeners {
    public class ModifyEventListener : IPreInsertEventListener, IPreUpdateEventListener, IPreDeleteEventListener {
        public ModifyEventListener(AuditLogger logger) {
            logger.ShouldNotBeNull("logger");
            Logger = logger;
        }

        public AuditLogger Logger { get; private set; }

        public bool OnPreInsert(PreInsertEvent @event) {
            Logger.Insert(@event.Entity as IStateEntity);
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event) {
            Logger.Update(@event.Entity as IStateEntity);
            return false;
        }

        public bool OnPreDelete(PreDeleteEvent @event) {
            Logger.Delete(@event.Entity as IStateEntity);
            return false;
        }
    }
}