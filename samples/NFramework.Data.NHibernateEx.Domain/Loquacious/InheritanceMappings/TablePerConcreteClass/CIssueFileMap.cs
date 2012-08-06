using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CIssueFileMap : UnionSubclassMapping<CIssueFile> {
        public CIssueFileMap() {
            Table("C_ISSUE_FILE");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Property(x => x.IssueCode, c => c.NotNullable(true));
        }
    }
}