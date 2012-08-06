using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class SmsMap : ClassMap<Sms> {
        public SmsMap() {
            Table("Sms");

            Cache.Region("NSoft.NFramework").ReadWrite().IncludeAll();

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Message);
        }
    }
}