using System.IO;
using System.Runtime.Serialization.Formatters.Soap;

namespace NSoft.NFramework.Serializations.SerializedObjects {
    public class SoapSerializedObject : AbstractSerializedObject {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        private static readonly SoapFormatter _formatter = new SoapFormatter();

        public SoapSerializedObject() {
            Method = SerializationMethod.Soap;
        }

        public SoapSerializedObject(object graph)
            : base(graph) {
            Method = SerializationMethod.Soap;
            using(var stream = new MemoryStream()) {
                SerializerTool.Serialize(graph, stream, _formatter);
                SerializedValue = stream.ToArray();
            }
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

            using(var stream = new MemoryStream((byte[])SerializedValue))
                return SerializerTool.Deserialize<object>(stream, _formatter);
        }
    }
}