using System;
using NSoft.NFramework.Serializations.SerializedObjects;

namespace NSoft.NFramework.Serializations {
    public static class SerializedObjectTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static ISerializedObject Serialize(this object graph, SerializationMethod method) {
            if(IsDebugEnabled)
                log.Debug("객체를 직렬화하여 SerializedObject를 생성합니다. method=[{0}], graph=[{1}]", method, graph);

            switch(method) {
                case SerializationMethod.Binary:
                    return new BinarySerializedObject(graph);

                case SerializationMethod.Bson:
                    return new BsonSerializedObject(graph);

                case SerializationMethod.Json:
                    return new JsonSerializedObject(graph);

                case SerializationMethod.Xml:
                    return new XmlSerializedObject(graph);

                case SerializationMethod.Soap:
                    return new SoapSerializedObject(graph);

                default:
                    throw new NotSupportedException(string.Format("지원하지 않는 직렬화 방법입니다. method=[{0}]", method));
            }
        }

        public static object Deserialize(this ISerializedObject serializedObject) {
            return serializedObject.GetDeserializedObject();
        }

        /// <summary>
        /// 지정된 직렬화 정보들을 가지고 <see cref="ISerializedObject"/>의 Concrete Class의 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="typename"></param>
        /// <param name="serializedValue"></param>
        /// <returns></returns>
        public static ISerializedObject Create(SerializationMethod method, string typename, byte[] serializedValue) {
            if(IsDebugEnabled)
                log.Debug("SerializedObject를 생성합니다. method=[{0]}, typename=[{1}], serializedValue=[{2}]", method, typename,
                          serializedValue);

            switch(method) {
                case SerializationMethod.Binary:
                    return new BinarySerializedObject
                           {
                               Method = method,
                               ObjectTypeName = typename,
                               SerializedValue = (serializedValue != null) ? (byte[])serializedValue.Clone() : null
                           };

                case SerializationMethod.Bson:
                    return new BsonSerializedObject
                           {
                               Method = method,
                               ObjectTypeName = typename,
                               SerializedValue = (serializedValue != null) ? (byte[])serializedValue.Clone() : null
                           };

                case SerializationMethod.Json:
                    return new JsonSerializedObject
                           {
                               Method = method,
                               ObjectTypeName = typename,
                               SerializedValue = (serializedValue != null) ? (byte[])serializedValue.Clone() : null
                           };

                case SerializationMethod.Xml:
                    return new XmlSerializedObject
                           {
                               Method = method,
                               ObjectTypeName = typename,
                               SerializedValue = (serializedValue != null) ? (byte[])serializedValue.Clone() : null
                           };

                case SerializationMethod.Soap:
                    return new SoapSerializedObject
                           {
                               Method = method,
                               ObjectTypeName = typename,
                               SerializedValue = (serializedValue != null) ? (byte[])serializedValue.Clone() : null
                           };

                default:
                    throw new NotSupportedException(string.Format("지원하지 않는 직렬화 방법입니다. method=[{0}]", method));
            }
        }

        public static ISerializedObject DeepCopy(this ISerializedObject serializedObject) {
            serializedObject.ShouldNotBeNull("serializedObject");
            return Create(serializedObject.Method, serializedObject.ObjectTypeName, serializedObject.SerializedValue.DeepCopy());
        }
    }
}