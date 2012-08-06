using System;
using NSoft.NFramework.Data.NHibernateEx.Domain;

namespace NSoft.NFramework.Data.DevartOracle.NH.Domains.Models {
    /// <summary>
    /// Project와 1:1 매핑을 수행합니다.
    /// </summary>
    [Serializable]
    public class ProjectField : LocaledEntityBase<long, ProjectFieldLocale> {
        protected ProjectField() {}

        public ProjectField(Project project) {
            project.ShouldNotBeNull("project");

            Project = project;
            Project.Field = this;
        }

        public virtual Project Project { get; protected set; }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        protected override ProjectFieldLocale CreateDefaultLocale() {
            return new ProjectFieldLocale
                   {
                       Name = Name,
                       Description = Description
                   };
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Project);
        }
    }

    public class ProjectFieldLocale : DataObjectBase, ILocaleValue {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description);
        }
    }
}