using System;

namespace NSoft.NFramework {
    [Serializable]
    public class Company : ValueObjectBase {
        public string Name { get; set; }
        public string Region { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Region);
        }

        public override string ToString() {
            return string.Format("Company# Name=[{0}], Region=[{1}]", Name, Region);
        }
    }
}