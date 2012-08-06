using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CSerializedEntityMap : ClassMapping<CSerializedEntity> {
        public CSerializedEntityMap() {
            Table("CSerializedEntity");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => c.Generator(Generators.Native));

            Property(x => x.Name, c => c.NotNullable(true));
            Property(x => x.Description, c => c.Length(9999));

            Property(x => x.BinarySerialized,
                     pm => {
                         pm.Type<SerializedObjectUserType>();
                         pm.Columns(cm => cm.Name("BinaryMethod"),
                                    cm => cm.Name("BinaryTypeName"),
                                    cm => {
                                        cm.Name("BinaryValue");
                                        cm.Length(9999);
                                    });
                     });

            Property(x => x.BsonSerialized,
                     pm => {
                         pm.Type<SerializedObjectUserType>();
                         pm.Columns(cm => cm.Name("BsonMethod"),
                                    cm => cm.Name("BsonTypeName"),
                                    cm => {
                                        cm.Name("BsonValue");
                                        cm.Length(9999);
                                    });
                     });

            Property(x => x.JsonSerialized,
                     pm => {
                         pm.Type<SerializedObjectUserType>();
                         pm.Columns(cm => cm.Name("JsonMethod"),
                                    cm => cm.Name("JsonTypeName"),
                                    cm => {
                                        cm.Name("JsonValue");
                                        cm.Length(9999);
                                    });
                     });

            Property(x => x.XmlSerialized,
                     pm => {
                         pm.Type<SerializedObjectUserType>();
                         pm.Columns(cm => cm.Name("XmlMethod"),
                                    cm => cm.Name("XmlTypeName"),
                                    cm => {
                                        cm.Name("XmlValue");
                                        cm.Length(9999);
                                    });
                     });

            Property(x => x.SoapSerialized,
                     pm => {
                         pm.Type<SerializedObjectUserType>();
                         pm.Columns(cm => cm.Name("SoapMethod"),
                                    cm => cm.Name("SoapTypeName"),
                                    cm => {
                                        cm.Name("SoapValue");
                                        cm.Length(9999);
                                    });
                     });

            Property(x => x.UpdateTimestamp, c => c.Type(NHibernateUtil.Timestamp));
        }
    }
}