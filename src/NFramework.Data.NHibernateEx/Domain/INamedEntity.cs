namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Name 속성을 가진 Entity입니다
    /// </summary>
    /// <remarks>
    /// Name 속성을 가진 Entity들이 많기 때문에, Name으로 찾기 기능 등을 Generics로 일반화시킬 수 있습니다. 
    /// </remarks>
    public interface INamedEntity : IDataObject {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
    }
}