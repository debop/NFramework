using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Join Mapping 에 대한 예제입니다.
    /// </summary>
    [Serializable]
    public class Person : DataEntityBase<Int32> {
        public virtual string Name { get; set; }

        public virtual string City { get; set; }

        public virtual string Country { get; set; }

        public virtual string Zip { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name);
        }
    }
}