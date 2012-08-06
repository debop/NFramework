using System;

namespace NSoft.NFramework.Caching.Domain.Model {
    [Serializable]
    public class ChildDTO : ValueObjectBase {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ParentDTO Parent { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Id);
        }

        public override string ToString() {
            return string.Format("ChildDTO# Id=[{0}], Name=[{1}], Description=[{2}], Parent=[{3}]", Id, Name, Description, Parent);
        }
    }
}