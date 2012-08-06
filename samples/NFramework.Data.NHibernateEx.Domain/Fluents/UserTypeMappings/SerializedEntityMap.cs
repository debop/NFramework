using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.UserTypeMappings {
    public class SerializedEntityMap : ClassMap<SerializedEntity> {
        public SerializedEntityMap() {
            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name);
            Map(x => x.Description).Length(MappingContext.MaxStringLength);

            Map(x => x.BinarySerialized)
                .CustomType<SerializedObjectUserType>()
                .Columns.Clear()
                .Columns.Add("BinaryMethod".AsNamingText())
                .Columns.Add("BinaryTypeName".AsNamingText())
                .Columns.Add(new ColumnMapping { Name = "BinaryValue".AsNamingText(), Length = MappingContext.MaxAnsiStringLength });

            Map(x => x.BsonSerialized)
                .CustomType<SerializedObjectUserType>()
                .Columns.Clear()
                .Columns.Add("BsonMethod".AsNamingText())
                .Columns.Add("BsonTypeName".AsNamingText())
                .Columns.Add(new ColumnMapping { Name = "BsonValue".AsNamingText(), Length = MappingContext.MaxAnsiStringLength });

            Map(x => x.JsonSerialized)
                .CustomType<SerializedObjectUserType>()
                .Columns.Clear()
                .Columns.Add("JsonMethod".AsNamingText())
                .Columns.Add("JsonTypeName".AsNamingText())
                .Columns.Add(new ColumnMapping { Name = "JsonValue".AsNamingText(), Length = MappingContext.MaxAnsiStringLength });

            Map(x => x.XmlSerialized)
                .CustomType<SerializedObjectUserType>()
                .Columns.Clear()
                .Columns.Add("XmlMethod".AsNamingText())
                .Columns.Add("XmlTypeName".AsNamingText())
                .Columns.Add(new ColumnMapping { Name = "XmlValue".AsNamingText(), Length = MappingContext.MaxAnsiStringLength });

            Map(x => x.SoapSerialized)
                .CustomType<SerializedObjectUserType>()
                .Columns.Clear()
                .Columns.Add("SoapMethod".AsNamingText())
                .Columns.Add("SoapTypeName".AsNamingText())
                .Columns.Add(new ColumnMapping { Name = "SoapValue".AsNamingText(), Length = MappingContext.MaxAnsiStringLength });

            Map(x => x.UpdateTimestamp);
        }
    }
}