using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// 지정된 <see cref="ICriteria"/>에 어떤 부가적인 처리를 위한 Delegate입니다.
    /// </summary>
    /// <param name="criteria">부가 처리해야 할 대상</param>
    /// <returns>부가 처리가 된 <see cref="ICriteria"/></returns>
    public delegate ICriteria ProcessCriteriaDelegate(ICriteria criteria);
}