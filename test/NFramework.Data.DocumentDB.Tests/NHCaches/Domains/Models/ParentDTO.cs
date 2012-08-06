using System;

namespace NSoft.NFramework.Data.MongoDB.NHCaches.Domains.Models {
    [Serializable]
    public class ParentDTO {
        public ParentDTO(string name, int age) {
            Name = name;
            Age = age;
        }

        public string Name { get; set; }
        public int Age { get; set; }
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