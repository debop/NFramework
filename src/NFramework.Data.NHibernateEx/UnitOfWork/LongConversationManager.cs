using System;
using System.Collections;
using System.Web;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx {
    internal static class LongConversationManager {
        public const string LongConversationStorageKey = "NSoft.NFramework.Data.NH.LongConversationStorage.Key";
        public const string DefaultLongConversationKey = "NSoft.NFramework.Data.NH.DefaultLongConversation.Key";

        public const string LongConversationRequestKey = "LongConversationKey";

        public static void SaveConversation() {
            var table = new Hashtable();
            IoC.TryResolve<IUnitOfWorkFactory, NHUnitOfWorkFactory>().SaveUnitOfWorkToHashtable(table);
            // IoC.Resolve<IUnitOfWorkFactory>().SaveUnitOfWorkToHashtable(table);

            if(UnitOfWork.LongConversationIsPrivate)
                Conversations.Add(UnitOfWork.CurrentLongConversationId, table);
            else
                Conversations.Add(DefaultLongConversationKey, table);
        }

        public static bool LoadConversation() {
            bool privateConversation;
            var conversation = LoadConversationFromRequest(out privateConversation);

            if(conversation != null) {
                IUnitOfWork UoW;
                Guid? longConversationId;
                IoC.Resolve<IUnitOfWorkFactory>().LoadUnitOfWorkFromHashtable(conversation, out UoW, out longConversationId);

                UnitOfWork.LongConversationIsPrivate = privateConversation;

                UnitOfWork.Current = UoW;
                UnitOfWork.CurrentLongConversationId = longConversationId;
                UnitOfWork.CurrentSession.Reconnect();

                Conversations.Remove(conversation);
                return true;
            }
            return false;
        }

        public static void EndAllConversations() {
            HttpContext.Current.Session[LongConversationStorageKey] = null;
        }

        private static Hashtable LoadConversationFromRequest(out bool privateConversation) {
            Hashtable conversation;
            string keyString = HttpContext.Current.Request.QueryString[LongConversationRequestKey] ??
                               HttpContext.Current.Request.Form[LongConversationRequestKey];

            if(keyString.IsNotWhiteSpace()) {
                var conversationKey = new Guid(keyString);
                conversation = (Hashtable)Conversations[conversationKey];
                if(conversation == null)
                    throw new InvalidOperationException("Attempted to load a specific UnitOfWork that no longer exists.");
                Conversations.Remove(conversationKey);
                privateConversation = true;
            }
            else {
                conversation = (Hashtable)Conversations[DefaultLongConversationKey];
                Conversations.Remove(DefaultLongConversationKey);
                privateConversation = false;
            }
            return conversation;
        }

        private static Hashtable Conversations {
            get {
                var hashtable = (Hashtable)HttpContext.Current.Session[LongConversationStorageKey];
                if(hashtable == null)
                    HttpContext.Current.Session[LongConversationStorageKey] = hashtable = new Hashtable();
                return hashtable;
            }
        }
    }
}