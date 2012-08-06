using System;
using System.Linq.Expressions;
using NHibernate.Impl;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate QueryOver의 정렬 작업 클래스
    /// </summary>
    [Serializable]
    public sealed class NHOrder : INHOrder {
        public static INHOrder Empty = new NHOrder();

        public NHOrder() {}
        public NHOrder(Expression<Func<object>> orderExpr) : this(orderExpr, true) {}

        public NHOrder(Expression<Func<object>> orderExpr, bool ascending) {
            orderExpr.ShouldNotBeNull("orderExpr");

            OrderExpr = orderExpr;
            Ascending = ascending;
        }

        /// <summary>
        /// 정렬을 위한 Lambda Expression
        /// </summary>
        public Expression<Func<object>> OrderExpr { get; set; }

        public bool Ascending { get; set; }

        public override bool Equals(object obj) {
            return (obj != null && obj is INHOrder && GetHashCode().Equals(obj.GetHashCode()));
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0} {1}", OrderExpr.Body, Ascending ? "ASC" : "DESC");
        }
    }

    /// <summary>
    /// NHibernate QueryOver의 정렬 작업 클래스
    /// </summary>
    /// <typeparam name="T">정렬할 정보를 가지는 엔티티의 수형</typeparam>
    public class NHOrder<T> : INHOrder<T> {
        public static INHOrder<T> Empty = new NHOrder<T>();

        public NHOrder() {}
        public NHOrder(Expression<Func<T, object>> orderExpr) : this(orderExpr, true) {}

        public NHOrder(Expression<Func<T, object>> orderExpr, bool ascending) {
            orderExpr.ShouldNotBeNull("orderExpr");

            OrderExpr = orderExpr;
            Ascending = ascending;
        }

        public NHOrder(string propertyName) : this(propertyName, true) {}

        public NHOrder(string propertyName, bool ascending) {
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            OrderExpr = NSoft.NFramework.LinqEx.DynamicExpression.ParseLambda<T, object>(propertyName);
            Ascending = ascending;
        }

        /// <summary>
        /// 정렬을 위한 Lambda Expression (예: u=>u.Name)
        /// </summary>
        public Expression<Func<T, object>> OrderExpr { get; set; }

        /// <summary>
        /// 정렬 방식 (Ascending | Descending)
        /// </summary>
        public bool Ascending { get; set; }

        public override bool Equals(object obj) {
            return (obj != null && obj is INHOrder<T> && GetHashCode().Equals(obj.GetHashCode()));
        }

        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        public override string ToString() {
            return string.Format("{0} {1}", ExpressionProcessor.FindMemberExpression(OrderExpr.Body), Ascending ? "ASC" : "DESC");
        }
    }
}