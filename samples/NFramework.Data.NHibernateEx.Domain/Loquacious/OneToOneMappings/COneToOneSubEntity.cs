using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class COneToOneSubEntity : LocaledMetadataEntityBase<Int32, COneToOneSubEntityLocale>, IUpdateTimestampedEntity {
        public COneToOneMainEntity MainEntity { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(MainEntity, Code);
        }
    }

    [Serializable]
    public class COneToOneSubEntityLocale : DataObjectBase, ILocaleValue {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}