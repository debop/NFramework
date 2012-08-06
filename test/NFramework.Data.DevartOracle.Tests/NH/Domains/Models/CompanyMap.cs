using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains.Models {
    public class CompanyMap : ClassMap<Company> {
        public CompanyMap() {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Code).CustomType("AnsiString").Length(32);
            Map(x => x.Name);

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");
        }

        public override NaturalIdPart<Company> NaturalId() {
            return base.NaturalId().Property(x => x.Code);
        }
    }
}