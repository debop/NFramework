using System;
using NSoft.NFramework.Serializations;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 다양한 직렬화 방식으로 직렬화 된 객체를 저장하고 로드합니다.
    /// </summary>
    [Serializable]
    public class SerializedObjectEntity : DataEntityBase<Int32> {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ISerializedObject BinarySerialized { get; set; }

        public virtual ISerializedObject BsonSerialized { get; set; }

        public virtual ISerializedObject JsonSerialized { get; set; }

        public virtual ISerializedObject XmlSerialized { get; set; }

        public virtual ISerializedObject SoapSerialized { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Name);
        }
    }
}