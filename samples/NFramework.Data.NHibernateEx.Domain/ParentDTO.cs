using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class ParentDTO : DataObjectBase {
        protected ParentDTO() {}

        public ParentDTO(string name, int age) {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }

        public int Age { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Age);
        }

        public override string ToString() {
            return string.Format("ParentDTO# Name=[{0}], Age=[{1}]", Name, Age);
        }
    }

    [Serializable]
    public class ParentSummaryDTO : ParentDTO {
        public ParentSummaryDTO() {}

        public ParentSummaryDTO(string name, int age, int childCount) : base(name, age) {
            ChildCount = childCount;
        }

        public int ChildCount { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), ChildCount);
        }

        public override string ToString() {
            return string.Format("ParentSummaryDTO# Name=[{0}], Age=[{1}], ChildCount=[{2}]", Name, Age, ChildCount);
        }
    }
}