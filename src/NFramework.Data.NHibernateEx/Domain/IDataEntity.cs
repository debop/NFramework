using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// NHibernate 용 Data Entity의 기본 Interface 입니다.
    /// </summary>
    public interface IDataEntity<TId> : IStateEntity, IEquatable<IDataEntity<TId>> {
        /// <summary>
        /// Identity value for Entity
        /// </summary>
        TId Id { get; }
    }
}