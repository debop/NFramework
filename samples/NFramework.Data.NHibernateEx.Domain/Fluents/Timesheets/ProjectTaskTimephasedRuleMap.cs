using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    public class ProjectTaskTimephasedRuleMap : SubclassMap<ProjectTaskTimephasedRule> {
        public ProjectTaskTimephasedRuleMap() {
            DiscriminatorValue("ProjectTask");

            Map(x => x.ProjectId);
            Map(x => x.TaskId);
        }
    }
}