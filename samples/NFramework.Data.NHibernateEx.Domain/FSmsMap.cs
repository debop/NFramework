using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class FSmsMap : ClassMap<FSms> {
        public FSmsMap() {
            Table("FSms");

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Message);

            Map(x => x.JsonData).CustomType<JsonSerializableUserType>().Length(MappingContext.MaxAnsiStringLength);
            Map(x => x.JsonCompressedData).CustomType<JsonCompressedSerializableUserType>().Length(MappingContext.MaxAnsiStringLength);

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");
        }
    }
}