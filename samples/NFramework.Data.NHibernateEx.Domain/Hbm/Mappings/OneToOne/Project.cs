using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    /// <summary>
    /// 프로젝트
    /// </summary>
    [Serializable]
    public class Project : LocaledEntityBase<int, ProjectLocale> {
        protected Project() {
            ProjectField = new ProjectField(this);
        }

        public Project(string code) : this() {
            code.ShouldNotBeNull("code");
            Code = code;
        }

        public virtual string Code { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual ProjectField ProjectField { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Code);
        }
    }

    [Serializable]
    public class ProjectLocale : DataObjectBase, ILocaleValue {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description);
        }
    }
}