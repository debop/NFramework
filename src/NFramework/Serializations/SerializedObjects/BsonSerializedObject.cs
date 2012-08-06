using System;
using NSoft.NFramework.Json;

namespace NSoft.NFramework.Serializations.SerializedObjects {
    /// <summary>
    /// 객체를 BSON 직렬화하여 보관할 수 있도록 합니다.
    /// </summary>
    [Serializable]
    public class BsonSerializedObject : AbstractSerializedObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public BsonSerializedObject() {
            Method = SerializationMethod.Bson;
        }

        public BsonSerializedObject(object graph)
            : base(graph) {
            Method = SerializationMethod.Bson;
            SerializedValue = JsonTool.SerializeAsBytes(graph);
        }

        /// <summary>
        /// 역직렬화를 통해, 객체를 반환한다.
        /// </summary>
        /// <returns></returns>
        public override object GetDeserializedObject() {
            if(SerializedValue == null)
                return null;

            if(log.IsDebugEnabled)
                log.Debug("직렬화 객체 (수형[{0}])에 대해 [{1}] 역직렬화를 수행합니다... ", ObjectTypeName, Method);

            var objectType = Type.GetType(ObjectTypeName, true, true);
            return JsonTool.DeserializeFromBytes((byte[])SerializedValue, objectType);
        }
    }
}