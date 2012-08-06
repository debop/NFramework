using System.Collections;
using FluentNHibernate.Testing;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// FluentNHibernate 의 <see cref="PersistenceSpecification{T}"/>를 사용하기 쉽게 해주는 Utility Class 입니다.
    /// </summary>
    public static class PersistenceSpecificationTool {
        private static readonly ValueObjectEqualityComparer _entityEqualityComparer = new ValueObjectEqualityComparer();

        /// <summary>
        /// 지정된 Session을 이용하여, 엔티티의 매핑을 테스트할 수 있는 <see cref="PersistenceSpecification{T}"/>를 생성해 반환한다.
        /// </summary>
        /// <typeparam name="T">테스트할 엔티티의 수형</typeparam>
        /// <param name="session">NHibernate Session</param>
        /// <returns><see cref="PersistenceSpecification{T}"/> 인스턴스</returns>
        public static PersistenceSpecification<T> Specification<T>(this ISession session) where T : IDataObject {
            return new PersistenceSpecification<T>(session, _entityEqualityComparer);
        }

        /// <summary>
        /// 지정된 Session을 이용하여, 엔티티의 매핑을 테스트할 수 있는 <see cref="PersistenceSpecification{T}"/>를 생성해 반환한다.
        /// </summary>
        /// <typeparam name="T">테스트할 엔티티의 수형</typeparam>
        /// <param name="session">NHibernate Session</param>
        /// <param name="entityEqualityComparer">엔티티의 속성들의 비교자</param>
        /// <returns><see cref="PersistenceSpecification{T}"/> 인스턴스</returns>
        public static PersistenceSpecification<T> Specification<T>(this ISession session, IEqualityComparer entityEqualityComparer)
            where T : IDataObject {
            return new PersistenceSpecification<T>(session, entityEqualityComparer);
        }
    }
}