using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class UserMap : ClassMap<User> {
        public UserMap() {
            Table("`User`");

            Id(x => x.Id).GeneratedBy.Native();

            References(x => x.Company).Fetch.Select().LazyLoad();

            Map(x => x.Code).CustomType("AnsiString");
            Map(x => x.Name);

            Map(x => x.Password)
                .CustomType<RijndaelEncryptStringUserType>()
                .Length(MappingContext.MaxAnsiStringLength);

            Map(x => x.Password2)
                .CustomType<AriaEncryptStringUserType>()
                .Length(MappingContext.MaxAnsiStringLength);


            Map(x => x.Data)
                .CustomType<GZipStringUserType>()
                .Length(MappingContext.MaxAnsiStringLength);
            Map(x => x.Blob)
                .CustomType<SevenZipBlobUserType>()
                .Length(MappingContext.MaxAnsiStringLength);

            Map(x => x.ActivePeriod)
                .CustomType<TimeRangeUserType>()
                .Columns.Clear()
                .Columns.Add("ActiveStartTime".AsNamingText())
                .Columns.Add("ActiveEndTime".AsNamingText());

            Map(x => x.ActiveYearWeek)
                .CustomType<YearAndWeekUserType>()
                .Columns.Clear()
                .Columns.Add("ActiveYear".AsNamingText())
                .Columns.Add("ActiveWeek".AsNamingText());

            Component(x => x.PeriodTime,
                      cp => {
                          cp.Map(x => x.Period)
                              .CustomType<TimeRangeUserType>()
                              .Columns.Clear()
                              .Columns.Add("PeriodStartTime".AsNamingText())
                              .Columns.Add("PeriodEndTime".AsNamingText());

                          cp.Map(x => x.YearWeek)
                              .CustomType<YearAndWeekUserType>()
                              .Columns.Clear()
                              .Columns.Add("PeriodYear".AsNamingText())
                              .Columns.Add("PeriodWeek".AsNamingText());
                      });

            HasMany<MetadataValue>(x => x.MetadataMap)
                .Table("UserMeta".AsNamingText())
                .Access.CamelCaseField(Prefix.Underscore)
                .Cascade.AllDeleteOrphan()
                .AsMap<string>("MetaKey".AsNamingText())
                .Component(m => {
                               m.Map(x => x.Value).Column("MetaValue".AsNamingText()).Length(MappingContext.MaxStringLength);
                               m.Map(x => x.ValueType).Column("MetaValueType".AsNamingText());
                           });

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");
        }
    }
}