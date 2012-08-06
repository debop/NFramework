using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    /// <summary>
    /// Project와 1:1 매핑을 수행합니다.
    /// </summary>
    [Serializable]
    public class ProjectField : LocaledEntityBase<int, ProjectFieldLocale> {
        protected ProjectField() {}

        public ProjectField(Project project) {
            project.ShouldNotBeNull("project");

            Project = project;
            Project.ProjectField = this;
        }

        /// <summary>
        /// 프로젝트
        /// </summary>
        public virtual Project Project { get; protected set; }

        /// <summary>
        /// 필드 명
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 필드 값
        /// </summary>
        public virtual string Value { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 최종 갱신일
        /// </summary>
        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Project);
        }
    }

    [Serializable]
    public class ProjectFieldLocale : DataObjectBase, ILocaleValue {
        /// <summary>
        /// 필드 명
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 설명
        /// </summary>
        public virtual string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description);
        }
    }
}