using System;
using NSoft.NFramework.Reflections;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 메타데이터 값을 표현하는 ValueObject입니다.
    /// </summary>
    [Serializable]
    public class MetadataValue : DataObjectBase, IMetadataValue {
        /// <summary>
        /// 빈 MetadataValue
        /// </summary>
        public static readonly MetadataValue Empty = new MetadataValue(string.Empty);

        /// <summary>
        /// 생성자
        /// </summary>
        public MetadataValue() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="value"></param>
        public MetadataValue(object value) {
            if(value != null) {
                Value = value.ToString();
                ValueType = value.GetType().FullName;
            }
        }

        /// <summary>
        /// 생성자 (<see cref="ObjectMapper.MapProperty{TTarget}(object,System.Func{TTarget},System.Linq.Expressions.Expression{System.Func{TTarget,object}}[])" /> 을 사용하세요)
        /// </summary>
        /// <param name="src">원본 메타데이타</param>
        public MetadataValue(IMetadataValue src) : base(src) {
            //src.ShouldNotBeNull("src");

            //Value = src.Value;
            //ValueType = src.ValueType;
            //Label = src.Label;
            //Description = src.Description;
            //ExAttr = src.ExAttr;
        }

        /// <summary>
        /// 메타데이타 값
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// 메타데이타 값의 형식
        /// </summary>
        public virtual string ValueType { get; set; }

        /// <summary>
        /// 메타데이타 화면 표시명
        /// </summary>
        public virtual string Label { get; set; }

        /// <summary>
        /// 메타데이타 설명
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 메타데이타 추가 속성값
        /// </summary>
        public virtual string ExAttr { get; set; }

        /// <summary>
        /// HashCode 값을 계산합니다.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return HashTool.Compute(Value, ValueType);
        }

        /// <summary>
        /// 현재 개체가 동일한 형식의 다른 개체와 같은지 여부를 나타냅니다.
        /// </summary>
        /// <returns>
        /// 현재 개체가 <paramref name="other"/> 매개 변수와 같으면 true이고, 그렇지 않으면 false입니다.
        /// </returns>
        /// <param name="other">이 개체와 비교할 개체입니다.</param>
        public bool Equals(IMetadataValue other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        /// <summary>
        /// 현재 인스턴스를 문자열로 표현합니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("MetadataValue# Value=[{0}], ValueType=[{1}], Label=[{2}], Description=[{3}], ExAttr=[{4}]",
                                 Value, ValueType, Label, Description, ExAttr);
        }
    }
}