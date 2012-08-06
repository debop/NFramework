using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    public class Movie : FVProduct {
        public virtual string Director { get; set; }

        public virtual IList<ActorRole> Actors { get; set; }
    }
}