using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerClassHierarchy {
    /// <summary>
    /// Table Per Class Hierarchy (subclass) 매핑
    /// </summary>
    public class PlanBaseMapping : ClassMap<PlanBase> {
        public PlanBaseMapping() {
            Table("PLAN_SUBCLASS");
            DynamicInsert();
            DynamicUpdate();
            LazyLoad();

            DiscriminateSubClassesOnColumn("PlanKind");

            Id(x => x.Id).Column("PlanId").GeneratedBy.Native();

            Map(x => x.Title).Length(255).Not.Nullable();
            Map(x => x.Content).Length(MappingContext.MaxStringLength);

            Map(x => x.ReporterId).Not.Nullable();
            Map(x => x.ReportDate).Nullable();
        }
    }

    public class TeamPlanMapping : SubclassMap<TeamPlan> {
        public TeamPlanMapping() {
            LazyLoad();
            DiscriminatorValue("Team");

            Map(x => x.TeamCode);
            Map(x => x.SubContent).Length(MappingContext.MaxStringLength);
        }
    }

    public class PersonalPlanMapping : SubclassMap<PersonalPlan> {
        public PersonalPlanMapping() {
            LazyLoad();
            DiscriminatorValue("Personal");

            Map(x => x.ResourceTime);
        }
    }
}