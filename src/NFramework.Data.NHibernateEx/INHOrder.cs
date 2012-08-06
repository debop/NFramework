using System;
using System.Linq.Expressions;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate QueryOver의 정렬 작업 클래스
    /// </summary>
    public interface INHOrder {
        /// <summary>
        /// 정렬을 위한 Lambda Expression
        /// </summary>
        Expression<Func<object>> OrderExpr { get; set; }

        /// <summary>
        /// 정렬 방식
        /// </summary>
        bool Ascending { get; set; }
    }

    /// <summary>
    /// NHibernate QueryOver의 정렬 작업 클래스
    /// </summary>
    /// <typeparam name="T">정렬할 정보를 가지는 엔티티의 수형</typeparam>
    public interface INHOrder<T> {
        /// <summary>
        /// 정렬을 위한 Lambda Expression (예: u=>u.Name)
        /// </summary>
        Expression<Func<T, object>> OrderExpr { get; set; }

        /// <summary>
        /// 정렬 방식
        /// </summary>
        bool Ascending { get; set; }
    }
}