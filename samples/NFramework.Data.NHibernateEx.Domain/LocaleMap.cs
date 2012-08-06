using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class LocaleMap : ClassMap<Locale> {
        public LocaleMap() {
            Id(x => x.Id).Column("LocaleKey").CustomType<CultureUserType>().GeneratedBy.Assigned();

            Map(x => x.Name).Column("LocaleName").Length(255);
        }
    }
}