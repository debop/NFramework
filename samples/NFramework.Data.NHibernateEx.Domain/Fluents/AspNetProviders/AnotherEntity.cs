using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class AnotherEntity : DataEntityBase<int> {
        public virtual string Output { get; set; }
        public virtual string Input { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Output, Input);
        }
    }
}