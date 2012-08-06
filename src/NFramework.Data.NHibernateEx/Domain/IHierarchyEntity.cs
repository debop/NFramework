using Iesi.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 조상, 자손 정보를 가지는 엔티티의 인터페이스입니다.
    /// </summary>
    /// <remarks>
    /// 계층구조를 가지는 엔티티의 경우, 부모, 자식 만으로 재귀호출로 조상, 자손을 구할 수 있습니다만, 속도때문에 따로 조상, 자손을 기록할 수 있습니다.
    /// 다만 이렇게되면 부모, 자식에 대한 정보의 추가, 갱신 시 부하가 발생합니다.
    /// </remarks>
    /// <typeparam name="T">엔티티의 수형</typeparam>
    /// <seealso cref="ITreeNodeEntity{T}"/>
    public interface IHierarchyEntity<T> : IDataObject where T : IHierarchyEntity<T> {
        /// <summary>
        /// 조상 엔티티의 집합
        /// </summary>
        ISet<T> Ancestors { get; }

        /// <summary>
        /// 자손 엔티티의 집합
        /// </summary>
        ISet<T> Descendents { get; }
    }
}