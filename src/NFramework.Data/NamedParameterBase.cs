using System;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Represents Parameter for Command.Parameters or QueryString parameter or HQL parameter
    /// </summary>
    [Serializable]
    public abstract class NamedParameterBase : INamedParameter {
        /// <summary>
        ///Initialize a new instance of NamedParameterBase with parameter name and value
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        protected NamedParameterBase(string name, object value = null) {
            name.ShouldNotBeWhiteSpace("name");

            Name = name;
            Value = value;
        }

        /// <summary>
        /// Parameter name
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public virtual object Value { get; set; }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(INamedParameter other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is INamedParameter) && Equals((INamedParameter)obj);
        }

        public override int GetHashCode() {
            return HashTool.Compute(Name);
        }

        /// <summary>
        /// 현재 인스턴스의 내용을 표현하는 문자열을 반환합니다.
        /// </summary>
        public override string ToString() {
            return string.Format("{0}# Name=[{1}], Value=[{2}]", GetType().FullName, Name, Value);
        }
    }
}