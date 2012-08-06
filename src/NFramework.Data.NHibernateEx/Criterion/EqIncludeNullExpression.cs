using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// 속성이 같은 값을 가지거나 NULL 인 경우를 나타내는 Criterion을 생성합니다.<br/>
    /// 예 : ( Product = 'RealAdmin' or Product is null) 
    /// </summary>
    public sealed class EqIncludeNullExpression : AbstractCriterion {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly IProjection _projection;
        private readonly ICriterion _criterion;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public EqIncludeNullExpression(string propertyName, object value) {
            if(value != null)
                _criterion = Restrictions.Disjunction()
                    .Add(Restrictions.Eq(propertyName, value))
                    .Add(Restrictions.IsNull(propertyName));
            else
                _criterion = Restrictions.IsNull(propertyName);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="value"></param>
        public EqIncludeNullExpression(IProjection projection, object value) {
            _projection = projection;

            if(value != null)
                _criterion = Restrictions.Disjunction()
                    .Add(Restrictions.Eq(projection, value))
                    .Add(Restrictions.IsNull(projection));
            else
                _criterion = Restrictions.IsNull(projection);
        }

        /// <summary>
        /// 현재 인스턴스를 나타내는 문자열을 반환합니다.
        /// </summary>
        public override string ToString() {
            return _criterion.ToString();
        }

        ///<summary>
        /// Render a SqlString for the expression.
        ///</summary>
        ///<returns>
        ///A SqlString that contains a valid Sql fragment.
        ///</returns>
        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
                                              IDictionary<string, IFilter> enabledFilters) {
            var builder = new SqlStringBuilder(32);

            builder
                .Add("(")
                .Add(_criterion.ToSqlString(criteria, criteriaQuery, enabledFilters))
                .Add(")");

            var sqlString = builder.ToSqlString();

            if(IsDebugEnabled)
                log.Debug("Criteria의 생성된 SQL 문자열은 다음과 같습니다. sqlString=[{0}]" + sqlString);

            return sqlString;
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

        /// <summary>
        /// Return all projections used in this criterion
        /// </summary>
        /// <returns>
        /// An array of IProjection used by the Expression.
        /// </returns>
        public override IProjection[] GetProjections() {
            if(_projection != null)
                return new[] { _projection };

            return null;
        }
    }
}