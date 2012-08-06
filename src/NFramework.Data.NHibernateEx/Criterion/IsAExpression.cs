using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// Entity의 속성이 특정 형식인지를 판단하는 Restriction
    /// </summary>
    /// <remarks>
    /// 실제 HQL문으로 생성시 "Customer isa (Northwind.DataObjects.Customer)" 와 같이 사용된다.
    /// </remarks>
    [Obsolete("사용하지 마세요.")]
    public class IsAExpression : AbstractCriterion {
        private readonly Type _entityClass;
        private readonly string _propertyNameOrAlias;

        /// <summary>
        /// Initialize a new instance of IsAExpression with the specified type for verifying
        /// </summary>
        /// <param name="entityClass">type for verifying</param>
        public IsAExpression(Type entityClass) : this(string.Empty, entityClass) {}

        /// <summary>
        /// Initialize a new instance of IsAExpression with the specified type for verifing, and property name alias
        /// </summary>
        /// <param name="propertyNameOrAlias">alias of property name</param>
        /// <param name="entityClass">type for verifying</param>
        public IsAExpression(string propertyNameOrAlias, Type entityClass) {
            _propertyNameOrAlias = propertyNameOrAlias;
            _entityClass = entityClass;
        }

        ///<summary>
        /// HQL 문을 생성한다.
        ///</summary>
        ///<exception cref="QueryException"></exception>
        public override SqlString ToSqlString(ICriteria criteria, ICriteriaQuery criteriaQuery,
                                              IDictionary<string, IFilter> enabledFilters) {
            string alias;
            Type targetType;
            ICriteria aliasedCriteria;

            if(_propertyNameOrAlias.IsWhiteSpace()) {
                alias = criteriaQuery.GetSQLAlias(criteria);
                string entityName = criteriaQuery.GetEntityName(criteria);
                targetType = criteriaQuery.Factory.GetEntityPersister(entityName).GetMappedClass(EntityMode.Poco);
            }
            else if((aliasedCriteria = criteria.GetCriteriaByAlias(_propertyNameOrAlias)) != null) {
                alias = criteriaQuery.GetSQLAlias(aliasedCriteria);
                string entityName = criteriaQuery.GetEntityName(aliasedCriteria);
                targetType = criteriaQuery.Factory.GetEntityPersister(entityName).GetMappedClass(EntityMode.Poco);
            }
            else {
                alias = criteriaQuery.GetSQLAlias(criteria, _propertyNameOrAlias);
                var type = criteriaQuery.GetTypeUsingProjection(criteria, _propertyNameOrAlias);

                if(!type.IsEntityType) {
                    throw new QueryException("Only entities can be used with an IsAExpression");
                }

                targetType = type.ReturnedClass;
            }

            if(!targetType.IsAssignableFrom(_entityClass)) {
                return new SqlString("1=0");
            }

            var queryable = ObtainQueryable(criteriaQuery);
            var condition = queryable.WhereJoinFragment(alias, true, true);

            if(condition.IndexOfCaseInsensitive(" and ") == 0) {
                condition = condition.Substring(5);
            }

            return (condition.Length > 0) ? condition : new SqlString("1=1");
        }

        ///<summary>
        ///</summary>
        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery) {
            return EmptyTypes;
        }

        /// <summary>
        /// HQL 문장을 반환한다.
        /// </summary>
        public override string ToString() {
            return _propertyNameOrAlias + " isa (" + _entityClass.FullName + ')';
        }

        /// <summary>
        /// 새로운 IsAExpression 인스턴스를 생성합니다.
        /// </summary>
        public static IsAExpression Create(Type entityClass) {
            return new IsAExpression(entityClass);
        }

        /// <summary>
        /// 새로운 IsAExpression 인스턴스를 생성합니다.
        /// </summary>
        public static IsAExpression Create<T>() {
            return new IsAExpression(typeof(T));
        }

        /// <summary>
        /// 새로운 IsAExpression 인스턴스를 생성합니다.
        /// </summary>
        public static IsAExpression Create(string propertyNameOrAlias, Type entityClass) {
            return new IsAExpression(propertyNameOrAlias, entityClass);
        }

        /// <summary>
        /// 새로운 IsAExpression 인스턴스를 생성합니다.
        /// </summary>
        public static IsAExpression Create<T>(string propertyNameOrAlias) {
            return new IsAExpression(propertyNameOrAlias, typeof(T));
        }

        private IQueryable ObtainQueryable(ICriteriaQuery criteriaQuery) {
            var queryable = criteriaQuery.Factory.GetEntityPersister(_entityClass.FullName) as IQueryable;

            if(queryable == null) {
                //queryable = SessionFactoryHelper.FindQueryableUsingImports(criteriaQuery.Factory, _entityClass.FullName);
                // queryable = SessionFactoryHelper.FindQueryableUsingImports(_entityClass.FullName);
            }

            return queryable;
        }

        private static readonly TypedValue[] EmptyTypes = new TypedValue[0];

        /// <summary>
        /// Return all projections used in this criterion
        /// </summary>
        /// <returns>
        /// An array of IProjection used by the Expression.
        /// </returns>
        public override IProjection[] GetProjections() {
            return null;
        }
    }
}