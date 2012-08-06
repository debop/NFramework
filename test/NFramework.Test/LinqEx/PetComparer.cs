using System.Collections.Generic;

namespace NSoft.NFramework.LinqEx.LinqTools {
    public class PetComparer : IEqualityComparer<TestPet> {
        public bool Equals(TestPet x, TestPet y) {
            if(x == null && y == null)
                return true;
            if(x == null || y == null)
                return false;
            return GetHashCode(x) == GetHashCode(y);
        }

        public int GetHashCode(TestPet obj) {
            return (obj != null) ? HashTool.Compute(obj.Name) : 0;
        }
    }
}