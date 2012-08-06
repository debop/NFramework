using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// 비교할 값이 NULL일 경우에는 IsNull을, 값이 있을 경우에는 Eq 을 수행한다. ( VALUE가 NULL이면 IS NULL, VALUE에 값이 있으면 = 을 쓴다.)
    /// </summary>
    [Serializable]
    public sealed class EqOrNullExpression : AbstractCriterion {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ICriterion _criterion;
        private readonly IProjection _projection;

        // NOTE : value 가 object라는 뜻은 case 에 상관없이 수행해야 한다는 뜻이다. 문자열이면 각 DB에 맞게 알아서 행동 할 것이므로!!!
        //
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public EqOrNullExpression(string propertyName, object value) : this(propertyName, value, false) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <param name="ignoreCase"></param>
        public EqOrNullExpression(string propertyName, object value, bool ignoreCase) {
            if(value == null)
                _criterion = new NullExpression(propertyName);
            else
                _criterion = new SimpleExpression(propertyName, value, "=", ignoreCase);
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="projection"></param>
        /// <param name="value"></param>
        public EqOrNullExpression(IProjection projection, object value) {
            _projection = projection;

            if(Equals(value, null))
                _criterion = new NullExpression(projection);
        }

        /// <summary>
        /// Return string that represent current Expression
        /// </summary>
        public override string ToString() {
            return _criterion.ToString();
        }

        /// <summary>
        /// Return sql string that build with the specified criteria
        /// </summary>
        /// <seealso cref="ICriterion.ToSqlString"/>
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

        /// <summary>
        /// Return Typed Value 
        /// </summary>
        /// <seealso cref="ICriterion.GetTypedValues"/>
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