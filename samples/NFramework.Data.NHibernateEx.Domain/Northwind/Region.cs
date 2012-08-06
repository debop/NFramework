using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Region : DataEntityBase<int> {
        protected Region() {}

        public Region(string description) {
            Description = description;
        }

        public virtual string Description { get; protected set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(Description);
        }
    }
}