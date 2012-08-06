using System;
using NSoft.NFramework.Json;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Serializations.SerializedObjects {
    /// <summary>
    /// 객체를 JSON 포맷으로 직렬화/역직렬화를 수행합니다.
    /// </summary>
    [Serializable]
    public class JsonSerializedObject : AbstractSerializedObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public JsonSerializedObject() {
            Method = SerializationMethod.Json;
        }

        public JsonSerializedObject(object graph)
            : base(graph) {
            Method = SerializationMethod.Json;
            SerializedValue = JsonTool.SerializeAsText(graph).ToBytes();
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
            return JsonTool.DeserializeFromText(SerializedValue.ToText(), objectType);
        }
    }
}