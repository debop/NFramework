using System;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Data 관려 Value Object 를 표현하는 가장 기본적인 클래스입니다.
    /// </summary>
    [Serializable]
    public abstract class DataObjectBase : ValueObjectBase, IDataObject {
        /// <summary>
        /// default constructor
        /// </summary>
        protected DataObjectBase() {}

        /// <summary>
        /// copy constructor
        /// </summary>
        /// <param name="src"></param>
        protected DataObjectBase(IDataObject src) {
            ObjectMapper.Map(src, this, true, true, new string[0]);
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public virtual bool Equals(IDataObject other) {
            return (other != null) &&
                   (GetType() == other.GetType()) &&
                   GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// 현재 인스턴스의 상세 내용을 문자열로 나타낸다.
        /// </summary>
        /// <param name="showDetails"></param>
        /// <returns></returns>
        public virtual string ToString(bool showDetails) {
            return showDetails ? this.ObjectToString() : base.ToString();
        }
    }
}