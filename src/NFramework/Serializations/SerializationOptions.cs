using System;

namespace NSoft.NFramework.Serializations {
    /// <summary>
    /// 직렬화 옵션
    /// </summary>
    [Serializable]
    public class SerializationOptions : ValueObjectBase {
#if !SILVERLIGHT
        public static SerializationOptions Binary = new SerializationOptions(SerializationMethod.Binary, false, false);
        public static SerializationOptions CompressedBinary = new SerializationOptions(SerializationMethod.Binary, true, false);
        public static SerializationOptions Xml = new SerializationOptions(SerializationMethod.Xml, false, false);
        public static SerializationOptions CompressedXml = new SerializationOptions(SerializationMethod.Xml, true, false);
#endif
        public static SerializationOptions Json = new SerializationOptions(SerializationMethod.Json, false, false);
        public static SerializationOptions Bson = new SerializationOptions(SerializationMethod.Bson, false, false);
        public static SerializationOptions CompressedJson = new SerializationOptions(SerializationMethod.Json, true, false);
        public static SerializationOptions CompressedBson = new SerializationOptions(SerializationMethod.Bson, true, false);

        public SerializationOptions() {
            Method = SerializationMethod.Bson;
            IsCompress = true;
            IsEncrypt = false;
        }

        public SerializationOptions(SerializationMethod method, bool isCompress, bool isEncrypt) {
            Method = method;
            IsCompress = isCompress;
            IsEncrypt = isEncrypt;
        }

        /// <summary>
        /// 직렬화 방법 (JSON, BSON, XML 등)
        /// </summary>
        public SerializationMethod Method { get; set; }

        /// <summary>
        /// 압축 여부
        /// </summary>
        public bool IsCompress { get; set; }

        /// <summary>
        /// 암호화 여부
        /// </summary>
        public bool IsEncrypt { get; set; }

        public bool Equals(SerializationOptions other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is SerializationOptions) && Equals((SerializationOptions)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Method, IsCompress, IsEncrypt);
        }

        public override string ToString() {
            return string.Format("SerializationOptions# Method={0}, IsCompress={1}, IsEncrypt={2}", Method, IsCompress, IsEncrypt);
        }
    }
}