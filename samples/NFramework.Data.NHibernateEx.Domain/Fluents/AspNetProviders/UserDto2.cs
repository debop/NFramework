using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class UserDto2 {
        public virtual DateTime RegisteredAt { get; set; }
        public virtual EnumStoredAsInt32 Enum { get; set; }
    }
}