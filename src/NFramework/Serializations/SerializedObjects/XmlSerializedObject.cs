using System;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.Serializations.SerializedObjects {
    /// <summary>
    /// 객체를 XML 직렬화하여 보관할 수 있도록 합니다.
    /// </summary>
    [Serializable]
    public class XmlSerializedObject : AbstractSerializedObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        public XmlSerializedObject() {
            Method = SerializationMethod.Xml;
        }

        public XmlSerializedObject(object graph)
            : base(graph) {
            Method = SerializationMethod.Xml;

            byte[] value;
            SerializedValue = XmlTool.Serialize(graph, out value) ? value : null;
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
            return XmlTool.Deserialize(objectType, SerializedValue);
        }
    }
}