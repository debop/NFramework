using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    public class ProjectTaskTimeSheetMap : SubclassMap<ProjectTaskTimeSheet> {
        public ProjectTaskTimeSheetMap() {
            DiscriminatorValue("ProjectTask");

            Map(x => x.ProjectId);
            Map(x => x.TaskId);
        }
    }
}