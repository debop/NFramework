using System;

namespace NSoft.NFramework.LinqEx.LinqTools {
    [Serializable]
    public class TestPet : ValueObjectBase, IEquatable<TestPet> {
        public string Name { get; set; }
        public string Type { get; set; }

        public TestPet() {
            Name = "";
            Type = "";
        }

        public TestPet(int id) {
            switch(id) {
                case 1:
                    Name = "Daisy";
                    Type = "Dog";
                    break;
                case 2:
                    Name = "Tux";
                    Type = "Cat";
                    break;
                default:
                    Name = "";
                    Type = "";
                    break;
            }
        }

        public bool Equals(TestPet other) {
            return other != null && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return HashTool.Compute(Name);
        }
    }
}