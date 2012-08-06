using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    [Serializable]
    public class ProjectTaskTimephasedRule : TimephasedRuleBase {
        protected ProjectTaskTimephasedRule() {}
        public ProjectTaskTimephasedRule(Guid projectId, Guid taskId) : this(projectId, taskId, Guid.NewGuid()) {}

        public ProjectTaskTimephasedRule(Guid projectId, Guid taskId, Guid id) {
            ProjectId = projectId;
            TaskId = taskId;
            Id = id;
        }

        public virtual Guid ProjectId { get; set; }
        public virtual Guid TaskId { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(base.GetHashCode(), TaskId);
        }

        public override string ToString() {
            return string.Format("ProjectTaskTimephasedRule#TaskId=[{0}], TimeRange=[{1}], WeightValue=[{2}]", TaskId, TimeRange,
                                 WeightValue);
        }
    }
}