using System.IO;
using System.Web.SessionState;

namespace NSoft.NFramework.Web.Tools {
    public static partial class WebTool {
        /// <summary>
        /// Session 정보를 직렬화합니다. (일반적인 직렬화 방식은 안되고, SessionStateItemCollection 자체의 Serialize를 사용해서 직렬화합니다.)
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static byte[] SerializeSessionState(SessionStateItemCollection items) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 직렬화를 수행합니다...");

            if(items == null)
                return new byte[0];

            using(var ms = new MemoryStream())
            using(var writer = new BinaryWriter(ms)) {
                items.Serialize(writer);
                writer.Close();

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Session 정보를 역직렬화합니다.  (일반적인 역직렬화 방식은 안되고, SessionStateItemCollection 자체의 Deserialize를 사용해서 직렬화합니다.)
        /// </summary>
        /// <param name="serializedItems"></param>
        /// <returns></returns>
        public static SessionStateItemCollection DeserializeSessionState(byte[] serializedItems) {
            if(IsDebugEnabled)
                log.Debug("세션 정보를 역직렬화를 수행합니다...");

            if(serializedItems == null || serializedItems.Length == 0)
                return new SessionStateItemCollection();

            SessionStateItemCollection sessionItems = null;

            using(var ms = new MemoryStream(serializedItems)) {
                if(ms.Length > 0) {
                    var reader = new BinaryReader(ms);
                    sessionItems = SessionStateItemCollection.Deserialize(reader);
                }
            }
            return sessionItems ?? new SessionStateItemCollection();
        }
    }
}