using System;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.Serializations.SerializedObjects {
    /// <summary>
    /// 객체를 Binary 방식으로 직렬화하여 보관할 수 있도록 합니다.
    /// </summary>
    [Serializable]
    public class BinarySerializedObject : AbstractSerializedObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        [NonSerialized] private static readonly BinarySerializer _serializer = new BinarySerializer();

        public BinarySerializedObject() {
            Method = SerializationMethod.Binary;
        }

        public BinarySerializedObject(object graph)
            : base(graph) {
            Method = SerializationMethod.Binary;
            SerializedValue = _serializer.Serialize(graph);
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

            return _serializer.Deserialize((byte[])SerializedValue);
        }
    }
}