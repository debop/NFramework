using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CUser : LocaledMetadataEntityBase<Guid, CUserLocale> {
        protected CUser() {}

        public CUser(CCompany company, string code) {
            company.ShouldNotBeNull("company");
            code.ShouldNotBeWhiteSpace("code");

            Company = company;
            Code = code;
        }

        public CCompany Company { get; set; }

        public string Code { get; protected set; }

        public string Name { get; set; }

        public bool? IsActive { get; set; }

        public string Description { get; set; }

        public string ExAttr { get; set; }

        public object JsonData { get; set; }

        public DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Company, Code);
        }
    }

    [Serializable]
    public class CUserLocale : DataObjectBase, ILocaleValue {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ExAttr { get; set; }
    }
}