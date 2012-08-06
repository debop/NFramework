using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class UserComponent {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
        public UserComponent2 OtherComponent { get; set; }
    }
}