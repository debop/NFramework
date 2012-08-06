using FluentNHibernate.Mapping;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains.Models {
    public class ProjectTaskTimeSheetMap : SubclassMap<ProjectTaskTimeSheet> {
        public ProjectTaskTimeSheetMap() {
            DiscriminatorValue("ProjectTask");

            Map(x => x.ProjectId);
            Map(x => x.TaskId);
        }
    }
}