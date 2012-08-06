using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

#pragma warning disable 1591

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// 값이 NULL인 경우를 포함해서,  매칭되는 검색, ((A LIKE P0) OR (A IS NULL)) <br/>
    /// 예: 예 : ( Product = 'Real%' or Product is null) 
    /// </summary>
    public class InsensitiveLikeIncludeNullExpression : AbstractCriterion {
        private readonly IProjection _projection;
        private readonly ICriterion _criterion;

        public InsensitiveLikeIncludeNullExpression(string propertyName, string value, MatchMode matchMode) {
            if(value != null)
                _criterion = Restrictions.Disjunction()
                    .Add(Restrictions.InsensitiveLike(propertyName, value, matchMode))
                    .Add(Restrictions.IsNull(propertyName));
            else
                _criterion = Restrictions.IsNull(propertyName);
        }

        public InsensitiveLikeIncludeNullExpression(string propertyName, object value) {
            if(value != null)
                _criterion = Restrictions.Disjunction()
                    .Add(Restrictions.InsensitiveLike(propertyName, value))
                    .Add(Restrictions.IsNull(propertyName));
            else
                _criterion = Restrictions.IsNull(propertyName);
        }

        public InsensitiveLikeIncludeNullExpression(IProjection projection, string value, MatchMode matchMode) {
            _projection = projection;

            if(value != null)
                _criterion = Restrictions.Disjunction()
                    .Add(Restrictions.InsensitiveLike(projection, value, matchMode))
                    .Add(Restrictions.IsNull(projection));
            else
                _criterion = Restrictions.IsNull(projection);
        }

        public InsensitiveLikeIncludeNullExpression(IProjection projection, object value) {
            _projection = projection;

            if(value != null)
                _criterion = Restrictions.Disjunction()
                    .Add(Restrictions.InsensitiveLike(projection, value))
                    .Add(Restrictions.IsNull(projection));
            else
                _criterion = Restrictions.IsNull(projection);
        }

        public override string ToString() {
            return _criterion.ToString();
        }

        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
                                              IDictionary<string, IFilter> enabledFilters) {
            var builder = new SqlStringBuilder();

            builder
                .Add("(")
                .Add(_criterion.ToSqlString(criteria, criteriaQuery, enabledFilters))
                .Add(")");

            return builder.ToSqlString();
        }

        ///<summary>
        /// Return typed values for all parameters in the rendered SQL fragment
        ///</summary>
        ///<returns>
        ///An array of TypedValues for the Expression.
        ///</returns>
        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery) {
            return _criterion.GetTypedValues(criteria, criteriaQuery);
        }

        public override IProjection[] GetProjections() {
            if(_projection != null)
                return new[] { _projection };

            return null;
        }
    }
}

#pragma warning restore 1591