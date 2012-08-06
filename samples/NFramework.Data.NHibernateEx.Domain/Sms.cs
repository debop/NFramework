namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class Sms : DataEntityBase<long> {
        public virtual string Message { get; set; }
    }
}