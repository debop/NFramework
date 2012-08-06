using NHibernate.Mapping.ByCode.Conformist;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious.InheritanceMappings.TablePerClassHierarchy {
    public class CTeamPlanMap : SubclassMapping<CTeamPlan> {
        public CTeamPlanMap() {
            Lazy(false);

            DiscriminatorValue("Team");

            //! NOTE: SubclassMapping 에서 Subclass의 속성들은 모두 Nullable 이어야 합니다.
            //
            Property(x => x.TeamCode, c => c.NotNullable(false));
            Property(x => x.SubContent, c => c.Length(9999));
        }
    }
}