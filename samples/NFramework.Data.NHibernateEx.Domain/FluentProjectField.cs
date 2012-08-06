using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Project와 1:1 매핑을 수행합니다.
    /// </summary>
    [Serializable]
    public class FluentProjectField : LocaledEntityBase<long, FluentProjectFieldLocale> {
        protected FluentProjectField() {}

        public FluentProjectField(FluentProject fluentProject) {
            fluentProject.ShouldNotBeNull("FluentProject");

            FluentProject = fluentProject;
            FluentProject.Field = this;
        }

        public virtual FluentProject FluentProject { get; protected set; }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(FluentProject);
        }
    }

    [Serializable]
    public class FluentProjectFieldLocale : DataObjectBase, ILocaleValue {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Description);
        }
    }
}