using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.UserTypeMappings {
    public class UserTypeEntityMapping : ClassMap<UserTypeEntity> {
        public UserTypeEntityMapping() {
            Table("UserTypeEntity");
            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            Id(x => x.Id).GeneratedBy.Native();

            Map(x => x.Name);

            Map(x => x.Password)
                .CustomType<RijndaelEncryptStringUserType>()
                .Length(MappingContext.MaxAnsiStringLength);

            Map(x => x.Password2)
                .CustomType<AriaEncryptStringUserType>()
                .Length(MappingContext.MaxAnsiStringLength);

            Map(x => x.CompressedString)
                .CustomType<GZipStringUserType>()
                .Length(MappingContext.MaxAnsiStringLength);
            Map(x => x.CompressedBlob)
                .CustomType<SevenZipBlobUserType>()
                .Length(MappingContext.MaxAnsiStringLength);

            Map(x => x.ActivePeriod)
                .CustomType<TimeRangeUserType>()
                .Columns.Clear()
                .Columns.Add("ActiveFromDate".AsNamingText())
                .Columns.Add("ActiveToDate".AsNamingText());

            Map(x => x.ActiveYearWeek)
                .CustomType<YearAndWeekUserType>()
                .Columns.Clear()
                .Columns.Add("ActiveYear".AsNamingText())
                .Columns.Add("ActiveWeek".AsNamingText());

            Map(x => x.UpdateTimestamp).CustomType("Timestamp");
        }
    }
}