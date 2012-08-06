using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// FutureValue의 실제값 조회시의 Entity 존재 여부에 따른 행동방식
    /// </summary>
    public enum FutureValueOptions {
        /// <summary>
        /// <see cref="ISession.Get{T}(object)"/>과 같이 조회한 값이 없다면 Null을 반환.
        /// </summary>
        NullIfNotFound,

        /// <summary>
        /// <see cref="ISession.Load{T}(object)"/>와 같이 조회한 값이 없다면 예외를 발생시킨다.
        /// </summary>
        ThrowIfNotFound
    }
}