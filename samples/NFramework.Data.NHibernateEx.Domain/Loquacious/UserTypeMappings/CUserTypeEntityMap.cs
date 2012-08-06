using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CUserTypeEntityMap : ClassMapping<CUserTypeEntity> {
        public CUserTypeEntityMap() {
            Table("CUserTypeEntity");
            Lazy(false);
            DynamicInsert(true);
            DynamicUpdate(true);

            Id(x => x.Id, c => c.Generator(Generators.Native));

            Property(x => x.Name);

            Property(x => x.Password, c => {
                                          c.Type<RijndaelEncryptStringUserType>();
                                          c.Length(9999);
                                      });
            Property(x => x.Password2, c => {
                                           c.Type<AriaEncryptStringUserType>();
                                           c.Length(9999);
                                       });
            Property(x => x.CompressedString, c => {
                                                  c.Type<GZipStringUserType>();
                                                  c.Length(9999);
                                              });
            Property(x => x.CompressedBlob, c => {
                                                c.Type<SevenZipBlobUserType>();
                                                c.Length(9999);
                                            });

            Property(x => x.ActivePeriod, c => {
                                              c.Type<TimeRangeUserType>();
                                              c.Columns(cm => cm.Name("ACTIVE_FROM_DATE"),
                                                        cm => cm.Name("ACTIVE_TO_DATE"));
                                          });

            Property(x => x.ActiveYearWeek, c => {
                                                c.Type<YearAndWeekUserType>();
                                                c.Columns(cm => cm.Name("ACTIVE_YEAR"),
                                                          cm => cm.Name("ACTIVE_WEEK"));
                                            });
        }
    }
}