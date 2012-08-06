using System;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains.Models {
    [Serializable]
    public class ProjectTaskTimeSheet : TimeSheetBase {
        protected ProjectTaskTimeSheet() {}
        public ProjectTaskTimeSheet(Guid projectId, Guid taskId) : this(projectId, taskId, Guid.NewGuid()) {}

        public ProjectTaskTimeSheet(Guid projectId, Guid taskId, Guid id) {
            ProjectId = projectId;
            TaskId = taskId;
            base.Id = id;
        }

        public virtual Guid ProjectId { get; protected set; }

        public virtual Guid TaskId { get; protected set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(ProjectId,
                                    TaskId,
                                    StartTime,
                                    EndTime,
                                    PeriodKind);
        }
    }
}