namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate용 Parameter 정보
    /// </summary>
    public interface INHParameter : INamedParameter {
        /// <summary>
        /// NHibernate용 인자의 Type
        /// </summary>
        NHibernate.Type.IType Type { get; set; }
    }
}