using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CFileBaseMap : ClassMapping<CFileBase> {
        public CFileBaseMap() {
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => {
                              c.Column("FileId");
                              c.Generator(Generators.Assigned);
                          });

            Property(x => x.Name, c => c.Column("FileName"));
            Property(x => x.Size, c => c.Column("FileSize"));
            Property(x => x.Content, c => {
                                         c.Column("FileContent");
                                         c.Type<SevenZipBlobUserType>();
                                         c.Length(9999);
                                     });
        }
    }
}