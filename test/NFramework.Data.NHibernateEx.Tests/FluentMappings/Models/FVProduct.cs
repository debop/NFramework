using System;

namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    /// <summary>
    /// 
    /// </summary>
    public class FVProduct : VersionedEntity {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual Decimal UnitPrice { get; set; }
    }
}