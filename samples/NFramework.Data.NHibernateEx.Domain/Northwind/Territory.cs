using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Northwind {
    [Serializable]
    public class Territory : AssignedIdEntityBase<string> {
        protected Territory() {}

        public Territory(string id) {
            id.ShouldNotBeWhiteSpace("id");
            Id = id;
        }

        public virtual string Description { get; set; }
        public virtual Region Region { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return (Id ?? string.Empty).GetHashCode();
        }
    }
}