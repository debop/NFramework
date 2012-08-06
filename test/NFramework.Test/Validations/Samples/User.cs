using System;

namespace NSoft.NFramework.Validations {
    [Serializable]
    public class User : ValueObjectBase {
        public string Id { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }

        public string NickName { get; set; }

        public DateTime? LastAccessTime { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Id);
        }
    }
}