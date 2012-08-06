using System;
using NHibernate;
using NHibernate.Type;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate 의 Query 문의 Parameter를 정보를 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public sealed class NHParameter : NamedParameterBase, INHParameter {
        /// <summary>
        /// Initialize a new instance of NHParameter with a specified parameter name and value
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        public NHParameter(string name, object value) : this(name, value, null) {}

        /// <summary>
        /// Initialize a new instance of NHParameter with a specified parameter name, value, type
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="type">parameter type</param>
        public NHParameter(string name, object value, IType type) : base(name, value) {
            Type = type;

            if(value != null && value.GetType().IsEnum) {
                Value = value.ToString();
                if(type == null)
                    Type = NHibernateUtil.String;
            }
        }

        /// <summary>
        /// NHibernate용 인자의 Type
        /// </summary>
        public IType Type { get; set; }

        /// <summary>
        /// 현재 인스턴스의 내용을 표현하는 문자열을 반환합니다.
        /// </summary>
        public override string ToString() {
            return string.Format("{0}, Type=[{1}]", base.ToString(), Type);
        }
    }
}