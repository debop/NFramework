using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class COneToOneMainEntity : LocaledMetadataEntityBase<Int32, COneToOneMainEntityLocale>, IUpdateTimestampedEntity {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public COneToOneSubEntity SubEntity { get; set; }

        public DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Code);
        }
    }

    [Serializable]
    public class COneToOneMainEntityLocale : DataObjectBase, ILocaleValue {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}