using System;
using NSoft.NFramework.Serializations;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.UserTypeMappings {
    [Serializable]
    public class SerializedEntity : DataEntityBase<Int32> {
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