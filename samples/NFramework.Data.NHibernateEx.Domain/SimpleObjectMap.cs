using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class SimpleObjectMap : ClassMap<SimpleObject> {
        public SimpleObjectMap() {
            Table("SimpleObject");

            Cache.Region("NSoft.NFramework").ReadWrite().IncludeAll();

            Id(x => x.Id).Column("SimpleObjectId").GeneratedBy.Assigned();

            Version(x => x.ConcurrencyId).UnsavedValue("-1");

            Map(x => x.TwoCharactersMax).Length(2);
        }
    }
}