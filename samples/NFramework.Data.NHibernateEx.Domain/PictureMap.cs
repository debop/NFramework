using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class PictureMap : ClassMap<Picture> {
        public PictureMap() {
            Table("Picture");

            DynamicInsert();
            DynamicUpdate();

            // NOTE: 1:1 매핑에서 Detail 부분임.
            Id(x => x.Id).GeneratedBy.Foreign("NHEmployee");

            // NOTE: 1:1 매핑에서 Detail 부분임.
            HasOne(x => x.NhEmployee).Constrained().Cascade.All();

            Map(x => x.Sign).Length(1024);
            Map(x => x.Stamp).Length(1024);
        }
    }
}