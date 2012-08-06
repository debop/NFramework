using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CRiskFileMap : UnionSubclassMapping<CRiskFile> {
        public CRiskFileMap() {
            Table("C_RISK_FILE");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Property(x => x.RiskCode, c => c.NotNullable(true));
        }
    }
}