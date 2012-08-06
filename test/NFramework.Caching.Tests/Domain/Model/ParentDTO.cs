using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Caching.Domain.Model {
    [Serializable]
    public class ParentDTO : ValueObjectBase {
        public ParentDTO(string name, int age) {
            Name = name;
            Age = age;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        private IList<ChildDTO> _children;

        public IList<ChildDTO> Children {
            get { return _children ?? (_children = new List<ChildDTO>()); }
            set { _children = value; }
        }

        public override int GetHashCode() {
            return HashTool.Compute(Name);
        }

        public override string ToString() {
            return string.Format("ParentDTO# Id=[{0}], Name=[{1}], Age=[{2}]", Id, Name, Age);
        }
    }

    [Serializable]
    public class ParentSummaryDTO : ParentDTO {
        public ParentSummaryDTO(string name, int age, int childCount)
            : base(name, age) {
            ChildCount = childCount;
        }

        public int ChildCount { get; set; }
    }
}