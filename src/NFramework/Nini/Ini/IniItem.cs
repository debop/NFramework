namespace NSoft.NFramework.Nini.Ini {
    public class IniItem : ValueObjectBase {
        protected internal IniItem(string name, string value, IniType type, string comment) {
            Name = name;
            Value = value;
            Type = type;
            Comment = comment;
        }

        public IniType Type { get; set; }

        public string Name { get; private set; }

        public string Value { get; set; }

        public string Comment { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Type, Name);
        }
    }
}