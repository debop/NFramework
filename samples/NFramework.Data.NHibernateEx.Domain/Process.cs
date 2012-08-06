using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class Process : DataEntityBase<Int32> {
        public virtual string Code { get; set; }
        public virtual string Name { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Code);
        }
    }
}