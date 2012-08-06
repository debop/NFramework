namespace NSoft.NFramework.Web.UserControls
{
    /// <summary>
    /// 엔티티를 편집하기 위한 UserControl의 기본 인터페이스입니다.
    /// </summary>
    public interface IEntityEditUserControl<T> where T : class
    {
        void Show(T entity, T parent = null);
    }
}