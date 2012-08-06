using System;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Engine;
using NHibernate.Impl;
using NLog;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// NHibernate Criteria 조작을 위한 Utility Class
    /// </summary>
    public static partial class CriteriaTool {
        #region << logger >>

        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="criteria"/>가 사용하는 <see cref="ISessionImplementor"/>을 반환합니다.
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static ISessionImplementor GetSession(this ICriteria criteria) {
            criteria.ShouldNotBeNull("criteria");

            return GetRootCriteria(criteria).Session;
        }

        /// <summary>
        /// Get Root Criteria
        /// </summary>
        public static CriteriaImpl GetRootCriteria(this ICriteria criteria) {
            criteria.ShouldNotBeNull("criteria");

            var impl = criteria as CriteriaImpl;

            if(impl != null)
                return impl;

            return GetRootCriteria(((CriteriaImpl.Subcriteria)criteria).Parent);
        }

        /// <summary>
        /// Get the entity type in criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">criteria의 session이 null일 경우</exception>
        public static Type GetRootType(this ICriteria criteria) {
            criteria.ShouldNotBeNull("criteria");

            var rootType = criteria.GetRootEntityTypeIfAvailable();
            if(rootType != null)
                return rootType;

            var impl = GetRootCriteria(criteria);
            impl.ShouldNotBeNull("impl");
            impl.Session.ShouldNotBeNull("impl.Session");

            var factory = impl.Session.Factory;
            var persister = factory.GetEntityPersister(impl.EntityOrClassName);

            persister.ShouldNotBeNull("Could not find entity named: " + impl.EntityOrClassName);

            return persister.GetMappedClass(EntityMode.Poco);
        }

        /// <summary>
        /// Get the root entity type in criteria
        /// </summary>
        /// <param name="detachedCriteria"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Type GetRootType(this DetachedCriteria detachedCriteria, ISession session) {
            // NOTE : NHibernate 2.1.0 과 2.0.1 GA에서 차이가 나서 그렇다.

            // for NH 2.1.0
            var rootType = detachedCriteria.GetRootEntityTypeIfAvailable();
            // for NH 2.0.1
            // Type rootType = detachedCriteria.CriteriaClass;

            if(rootType != null)
                return rootType;

            var factory = (ISessionFactoryImplementor)session.SessionFactory;

            // for NH 2.1.0
            var persister = factory.GetEntityPersister(detachedCriteria.EntityOrClassName);
            Guard.Assert(persister != null, "Could not find entity named: " + detachedCriteria.EntityOrClassName);

            // for NH 2.0.1
            //IEntityPersister persister = factory.GetEntityPersister(detachedCriteria.CriteriaClass.FullName);
            //Guard.Assert(persister != null, "Could not find entity named: " + detachedCriteria.CriteriaClass.FullName);

            return persister.GetMappedClass(EntityMode.Poco);
        }

        /// <summary>
        /// Between	(상하한을 포함하는 구간의 값을 구한다. 상하한에 대한 구간 검증은 하지 않는다!!!)
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <param name="lo">하한</param>
        /// <param name="hi">상한</param>
        /// <returns></returns>
        public static ICriterion IsBetweenCriterion(this string propertyName, object lo, object hi) {
            propertyName.ShouldNotBeWhiteSpace("propertyName");

            if(lo == null && hi == null)
                throw new InvalidOperationException("Between 을 사용할 상하한 값 모두 null이면 안됩니다.");

            if(IsDebugEnabled)
                log.Debug("Between Criteria를 빌드합니다... propertyName=[{0}], lo=[{1}], hi=[{2}]", propertyName, lo, hi);

            if(lo != null && hi != null)
                return Restrictions.Between(propertyName, lo, hi);

            // lo, hi 값 중 하나가 없다면 
            var result = Restrictions.Conjunction();

            if(lo != null)
                result.Add(Restrictions.Ge(propertyName, lo));

            if(hi != null)
                result.Add(Restrictions.Le(propertyName, hi));

            return result;
        }

        /// <summary>
        /// 지정한 값이 두 속성의 값 범위 안에 있을 때 ( Between 의 반대 개념 ) 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="loPropertyName"></param>
        /// <param name="hiPropertyName"></param>
        /// <returns></returns>
        public static ICriterion IsInRangeCriterion(this object value, string loPropertyName, string hiPropertyName) {
            value.ShouldNotBeNull("value");

            if(IsDebugEnabled)
                log.Debug("지정한 값이 두 속성의 값 범위 안에 있을 검색 조건을 빌드합니다. " +
                          @"value={0}, loPropertyName={1}, hiPropertyName={2}", value, loPropertyName, hiPropertyName);

            return Restrictions.Conjunction()
                .Add(Restrictions.Disjunction()
                         .Add(Restrictions.IsNull(loPropertyName))
                         .Add(Restrictions.Le(loPropertyName, value)))
                .Add(Restrictions.Disjunction()
                         .Add(Restrictions.IsNull(hiPropertyName))
                         .Add(Restrictions.Ge(hiPropertyName, value)));
        }

        /// <summary>
        /// 주어진 기간이 오버랩되는지를 파악하는 Criterion
        /// </summary>
        public static ICriterion IsOverlapCriterion(this ITimePeriod period, string loPropertyName, string hiPropertyName) {
            period.ShouldNotBeNull("range");
            Guard.Assert(period.IsAnytime == false, @"기간이 설정되어 있지 않습니다. 상하한 값 모두 없으므로, 질의어를 만들 필요가 없습니다.");
            loPropertyName.ShouldNotBeWhiteSpace("loProperty");
            hiPropertyName.ShouldNotBeWhiteSpace("hiProperty");

            if(IsDebugEnabled)
                log.Debug("Build IsOverlapCriterion... range={0}, loPropertyName={1}, hiPropertyName={2}",
                          period, loPropertyName, hiPropertyName);

            if(period.HasStart && period.HasEnd) {
                return Restrictions.Disjunction()
                    .Add(period.Start.IsInRangeCriterion(loPropertyName, hiPropertyName))
                    .Add(period.End.IsInRangeCriterion(loPropertyName, hiPropertyName))
                    .Add(loPropertyName.IsBetweenCriterion(period.Start, period.End))
                    .Add(hiPropertyName.IsBetweenCriterion(period.Start, period.End));
            }

            if(period.HasStart) {
                return Restrictions.Disjunction()
                    .Add(period.Start.IsInRangeCriterion(loPropertyName, hiPropertyName))
                    .Add(Restrictions.Ge(loPropertyName, period.Start))
                    .Add(Restrictions.Ge(hiPropertyName, period.Start));
            }

            if(period.HasEnd) {
                return Restrictions.Disjunction()
                    .Add(period.End.IsInRangeCriterion(loPropertyName, hiPropertyName))
                    .Add(Restrictions.Le(loPropertyName, period.End))
                    .Add(Restrictions.Le(hiPropertyName, period.End));
            }

            throw new InvalidOperationException("기간이 Overlap되는지 판단하는 Criterion을 생성하기 위한 조건이 맞지 않습니다.");
        }
    }
}