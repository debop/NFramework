using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NSoft.NFramework.LinqEx {
    /// <summary>
    /// An IQueryable wrapper that allows us to visit the query's expression tree just before LINQ to SQL gets to it.
    /// This is based on the excellent work of Tomas Petricek: http://tomasp.net/blog/linq-expand.aspx
    /// </summary>
    public class ExpandableQuery<T> : IOrderedQueryable<T> {
        private readonly ExpandableQueryProvider<T> _provider;
        private readonly IQueryable<T> _inner;

        internal IQueryable<T> InnerQuery {
            get { return _inner; }
        }

        // Original query, that we're wrapping

        internal ExpandableQuery(IQueryable<T> inner) {
            _inner = inner;
            _provider = new ExpandableQueryProvider<T>(this);
        }

        Expression IQueryable.Expression {
            get { return _inner.Expression; }
        }

        Type IQueryable.ElementType {
            get { return typeof(T); }
        }

        IQueryProvider IQueryable.Provider {
            get { return _provider; }
        }

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<T> GetEnumerator() {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _inner.GetEnumerator();
        }

        /// <summary>
        /// Return string represents object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return _inner.ToString();
        }
    }
}