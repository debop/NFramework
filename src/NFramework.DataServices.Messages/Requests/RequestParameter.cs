using System;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 요청 시, 메소드의 인자에 대한 정보
    /// </summary>
    [Serializable]
    public class RequestParameter : MessageObjectBase {
        public RequestParameter() {}
        public RequestParameter(string name, object value) : this(name, value, null) {}

        public RequestParameter(string name, object value, Type valueType) {
            if(string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name should not be null or white space string.");

            Name = name;
            Value = value;
            ValueType = valueType ?? ((value != null) ? value.GetType() : null);
        }

        /// <summary>
        /// 인자 명
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 인자 값
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 인자의 수형
        /// </summary>
        public Type ValueType { get; set; }

        public override int GetHashCode() {
            return Hasher.Compute(Name);
        }

        public override string ToString() {
            return string.Format("RequestParameter# Name=[{0}], Value=[{1}], ValueType=[{2}]", Name, Value, ValueType);
        }
    }
}