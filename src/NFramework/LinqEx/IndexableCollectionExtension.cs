using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NSoft.NFramework.Collections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// 인덱스를 가지는 컬렉션에 대한 확장 메소드를 제공합니다.
    /// </summary>
    public static class IndexableCollectionExtension {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 인덱싱된 시퀀스에서 일치하는 키를 기준으로 두 시퀀스의 요소를 연관시킵니다. 지정한 IEqualityComparer{TKey}를 사용하여 키를 비교합니다.
        /// </summary>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(
            this IndexableCollection<TOuter> outer,
            IndexableCollection<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector,
            IEqualityComparer<TKey> comparer) {
            outer.ShouldNotBeNull("outer");
            inner.ShouldNotBeNull("inner");
            outerKeySelector.ShouldNotBeNull("outerKeySelector");
            innerKeySelector.ShouldNotBeNull("innerKeySelector");
            resultSelector.ShouldNotBeNull("resultSelector");

            bool hasIndex = false;

            if(innerKeySelector.NodeType == ExpressionType.Lambda &&
               innerKeySelector.Body.NodeType == ExpressionType.MemberAccess &&
               outerKeySelector.NodeType == ExpressionType.Lambda &&
               outerKeySelector.Body.NodeType == ExpressionType.MemberAccess) {
                var memberExpressionInner = (MemberExpression)innerKeySelector.Body;
                var memberExpressionOuter = (MemberExpression)outerKeySelector.Body;
                var innerIndex = new MultiMap<int, TInner>();
                var outerIndex = new MultiMap<int, TOuter>();

                if(inner.PropertyHasIndex(memberExpressionInner.Member.Name) &&
                   outer.PropertyHasIndex(memberExpressionOuter.Member.Name)) {
                    innerIndex = inner.GetIndexByProperty(memberExpressionInner.Member.Name);
                    outerIndex = outer.GetIndexByProperty(memberExpressionOuter.Member.Name);
                    hasIndex = true;
                }

                if(hasIndex) {
                    foreach(int outerKey in outerIndex.Keys) {
                        IList<TOuter> outerGroup = outerIndex[outerKey];
                        IList<TInner> innerGroup;
                        if(innerIndex.TryGetValue(outerKey, out innerGroup)) {
                            // join on the GROUPS based on key result
                            IEnumerable<TInner> inners = innerGroup.AsEnumerable();
                            IEnumerable<TOuter> outers = outerGroup.AsEnumerable();
                            IEnumerable<TResult> results = outers.Join(inners, outerKeySelector.Compile(),
                                                                       innerKeySelector.Compile(),
                                                                       resultSelector, comparer);
                            foreach(TResult resultItem in results)
                                yield return resultItem;
                        }
                    }
                }
            }

            if(hasIndex == false) {
                if(IsDebugEnabled)
                    log.Debug("Index가 없으므로 System.Linq.Enumerable.Join()을 수행합니다. ");

                var inners = inner.AsEnumerable();
                var outers = outer.AsEnumerable();
                var results = outers.Join(inners, outerKeySelector.Compile(),
                                          innerKeySelector.Compile(),
                                          resultSelector, comparer);

                foreach(var resultItem in results)
                    yield return resultItem;
            }
        }

        /// <summary>
        /// 인덱싱된 시퀀스에서 일치하는 키를 기준으로 두 시퀀스의 요소를 연관시킵니다. 지정한 IEqualityComparer{TKey}를 사용하여 키를 비교합니다.
        /// </summary>
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(
            this IndexableCollection<TOuter> outer,
            IndexableCollection<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector,
            Expression<Func<TInner, TKey>> innerKeySelector,
            Func<TOuter, TInner, TResult> resultSelector) {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// 시퀀스로부터, 조건에 맞는 요소만을 필터링을 수행합니다.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="sourceCollection"></param>
        /// <param name="expr"></param>
        /// <returns></returns>
        public static IEnumerable<TSource> Where<TSource>(this IndexableCollection<TSource> sourceCollection,
                                                          Expression<Func<TSource, bool>> expr) {
            sourceCollection.ShouldNotBeNull("sourceCollection");
            expr.ShouldNotBeNull("expr");

            var noIndex = true;

            if(expr.Body.NodeType == ExpressionType.Equal) {
                // Equality is a binary expression
                BinaryExpression binaryExpr = (BinaryExpression)expr.Body;
                // Get aliases for both side
                Expression leftSide = binaryExpr.Left;
                Expression rightSide = binaryExpr.Right;

                var hashRight = GetHashRight(leftSide, rightSide);

                MemberExpression returnExpr;

                if(hashRight.HasValue && HasIndexablePropertyOnLeft(leftSide, sourceCollection, out returnExpr)) {
                    // cast to MemberExpression = it allows us to get the property
                    var propertyExpr = returnExpr;
                    var propertyName = propertyExpr.Member.Name;

                    var sourceIndex = sourceCollection.GetIndexByProperty(propertyName);
                    if(sourceIndex.ContainsKey(hashRight.Value)) {
                        var sourceEnum = sourceIndex[hashRight.Value].AsEnumerable();
                        var results = sourceEnum.Where(expr.Compile());

                        foreach(var resultItem in results)
                            yield return resultItem;
                    }
                    noIndex = false;
                }
            }

            //  지정된 시퀀스에 인덱스가 없다면...
            if(noIndex) {
                if(IsDebugEnabled)
                    log.Debug("Index가 없으므로 System.Linq.Enumerable.Where()을 수행합니다. ");

                var sourceEnum = sourceCollection.AsEnumerable();
                var results = sourceEnum.Where(expr.Compile());

                foreach(var resultItem in results)
                    yield return resultItem;
            }
        }

        private static bool HasIndexablePropertyOnLeft<TSource>(Expression leftSide,
                                                                IndexableCollection<TSource> sourceCollection,
                                                                out MemberExpression memberExpr) {
            memberExpr = null;
            var mex = leftSide as MemberExpression;

            if(leftSide.NodeType == ExpressionType.Call) {
                var call = leftSide as MethodCallExpression;
                if(call != null)
                    if(call.Method.Name.EqualTo("CompareString"))
                        mex = call.Arguments[0] as MemberExpression;
            }

            if(mex == null)
                return false;

            memberExpr = mex;
            return sourceCollection.PropertyHasIndex(mex.Member.Name);
        }

        private static int? GetHashRight(Expression leftSide, Expression rightSide) {
            if(leftSide.NodeType == ExpressionType.Call) {
                var call = leftSide as MethodCallExpression;
                if(call != null)
                    if(call.Method.Name.EqualTo("CompareString")) {
                        LambdaExpression evalRight = Expression.Lambda(call.Arguments[1], null);
                        // compile it, invoke it, and get the resulting hash
                        return (evalRight.Compile().DynamicInvoke(null).GetHashCode());
                    }
            }

            // shortcut constants, don't eval, will be faster
            if(rightSide.NodeType == ExpressionType.Constant)
                return ((ConstantExpression)rightSide).Value.GetHashCode();

            return Expression.Lambda(rightSide, null).Compile().DynamicInvoke(null).GetHashCode();
        }
    }
}