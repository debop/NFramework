using System;
using System.Linq.Expressions;
using NHibernate;
using NHibernate.Criterion;

namespace NSoft.NFramework.Data.NHibernateEx {
    public static partial class NHTool {
        /// <summary>
        /// <paramref name="queryOver"/> 인스턴스를 <paramref name="session"/>에서 수행할 수 있는 <see cref="IQueryOver{T,T}"/> 를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="session"></param>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        public static IQueryOver<T, T> GetExecutableQueryOver<T>(this ISession session, QueryOver<T> queryOver) {
            session.ShouldNotBeNull("session");
            queryOver.ShouldNotBeNull("queryOver");

            var result = queryOver.GetExecutableQueryOver(session);

            return result;
        }

        /// <summary>
        /// <paramref name="queryOver"/>를 <paramref name="statelessSession"/>에서 수행할 수 있는 <see cref="IQueryOver{T,T}"/> 를 생성합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="statelessSession"></param>
        /// <param name="queryOver"></param>
        /// <returns></returns>
        public static IQueryOver<T, T> GetExecutableQueryOver<T>(this IStatelessSession statelessSession, QueryOver<T> queryOver) {
            statelessSession.ShouldNotBeNull("statelessSession");
            queryOver.ShouldNotBeNull("queryOver");

            return queryOver.GetExecutableQueryOver(statelessSession);
        }

        /// <summary>
        /// 지정된 람다 식을 이용하여 <see cref="QueryOver{T,T}"/> 인스턴스에 조건절을 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static QueryOver<T, T> CreateQueryOverOf<T>(params Expression<Func<T, bool>>[] expressions) {
            var queryOver = QueryOver.Of<T>();

            foreach(var expr in expressions)
                queryOver.AddWhere(expr);

            return queryOver;
        }

        /// <summary>
        /// 지정된 람다 식을 이용하여 <see cref="QueryOver{T,T}"/> 인스턴스에 조건절을 추가합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expressions"></param>
        /// <returns></returns>
        public static QueryOver<T, T> CreateQueryOverOf<T>(params Expression<Func<bool>>[] expressions) {
            var queryOver = QueryOver.Of<T>();

            foreach(var expr in expressions)
                queryOver.AddWhere(expr);

            return queryOver;
        }
    }
}