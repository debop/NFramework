using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CJoinEntityMap : ClassMapping<CJoinEntity> {
        public CJoinEntityMap() {
            Table("CJoinEntity");
            Lazy(true);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => c.Generator(Generators.Native));

            Property(x => x.Name);

            Join("CJoinEntityDetail",
                 jm => {
                     jm.Property(x => x.NickName);
                     jm.Property(x => x.Description, c => c.Length(9999));
                 });
        }
    }
}