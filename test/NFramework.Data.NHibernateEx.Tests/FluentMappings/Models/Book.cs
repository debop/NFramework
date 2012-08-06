namespace NSoft.NFramework.Data.NHibernateEx.FluentMappings {
    public class Book : FVProduct {
        public virtual string Author { get; set; }

        public virtual string ISBN { get; set; }
    }
}