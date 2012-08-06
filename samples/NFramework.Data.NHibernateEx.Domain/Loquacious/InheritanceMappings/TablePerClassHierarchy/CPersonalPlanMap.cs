using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    public class CPersonalPlanMap : SubclassMapping<CPersonalPlan> {
        public CPersonalPlanMap() {
            Lazy(true);
            DiscriminatorValue("Personal");

            //! NOTE: SubclassMapping 에서 Subclass의 속성들은 모두 Nullable 이어야 합니다.
            //
            Property(x => x.UserCode, c => c.NotNullable(false));
            Property(x => x.ResourceTime);
        }
    }
}