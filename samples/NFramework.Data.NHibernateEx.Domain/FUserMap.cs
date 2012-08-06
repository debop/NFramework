using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class FUserMap : ClassMap<FUser> {
        public FUserMap() {
            Table("FUser");
            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            Id(x => x.Id).Column("UserId").GeneratedBy.Native();

            References(x => x.Company)
                .Column("CompanyId")
                .Cascade.SaveUpdate()
                .Fetch.Select()
                .LazyLoad(Laziness.False);

            Map(x => x.Code);
            Map(x => x.Name).Not.Nullable();

            Map(x => x.IsActive);
            Map(x => x.Description).Length(MappingContext.MaxStringLength);
            Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);


            Map(x => x.JsonData)
                .CustomType<JsonSerializableUserType>()
                .Columns.Clear()
                .Columns.Add(new ColumnMapping { Name = "JsonData", Length = 9999 });

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");

            HasMany<FUser.FUserLocale>(x => x.LocaleMap)
                .Table("FUserLocale")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                //.LazyLoad()
                .KeyColumn("UserId")
                .AsMap<CultureUserType>("LocaleKey")
                .Component(loc => {
                               loc.Map(x => x.Name).Column("UserName").Not.Nullable();
                               loc.Map(x => x.Description).Length(MappingContext.MaxStringLength);
                               loc.Map(x => x.ExAttr).Length(MappingContext.MaxStringLength);
                           });

            HasMany<MetadataValue>(x => x.MetadataMap)
                .Table("FUserMeta")
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                //.LazyLoad()
                .KeyColumn("UserId")
                .AsMap<string>("MetaKey")
                .Component(m => {
                               m.Map(x => x.Value).Column("MetaValue").Length(MappingContext.MaxStringLength);
                               m.Map(x => x.ValueType).Column("MetaValueType").Length(MappingContext.MaxStringLength);
                           });
        }

        public override NaturalIdPart<FUser> NaturalId() {
            return new NaturalIdPart<FUser>()
                .Reference(x => x.Company)
                .Reference(x => x.Code);
        }
    }
}