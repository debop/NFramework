using System;
using NHibernate;
using NHibernate.Criterion;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// 지정된 형식의 Entity에 대한 Loading을 예약한 정보. 
    /// 이 클래스로 예약된 값들은 Current Thread하에서 <see cref="CriteriaBatch"/>를 이용하여 
    /// 한번의 Batch 작업으로 Loading을 수행한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("NHibernate 3.x의 NHibernate.IFutureValue{T}를 사용하세요")]
    public class FutureValue<T> : FutureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _id;
        private readonly FutureValueOptions _options;
        private T _value;

        /// <summary>
        /// Initialize a new instance of FutureValue with identity value to look for.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="options"></param>
        public FutureValue(object id, FutureValueOptions options) {
            id.ShouldNotBeNull("id");

            _id = id;
            _options = options;

            if(IsDebugEnabled)
                log.Debug("CriteriaBatch 를 통해 Entity조회를 예약했습니다. type=[{0}], id=[{1}], options=[{2}]",
                          typeof(T).FullName, id, options);

            var criteriaBatch = FutureBase.Batcher.Add(DetachedCriteria.For<T>().Add(Restrictions.IdEq(id)));

            criteriaBatch.OnRead((T entity) => {
                                     _value = entity;
                                     IsLoaded = true;
                                 });
        }

        /// <summary>
        /// 실제 Loading 할 값 (아직 Loading이 안되었다면 현재까지 예약된 Loading 작업을 배치작업으로 수행한다.
        /// </summary>
        public T Value {
            get {
                if(IsLoaded == false)
                    ExecuteBatchQuery();

                if(_options == FutureValueOptions.ThrowIfNotFound && ReferenceEquals(_value, null))
                    throw new ObjectNotFoundException(_id, typeof(T).FullName);

                return _value;
            }
        }

        /// <summary>
        /// 실제 Loading을 수행해보고, 실제 값이 없다면 기본값을 반환한다.
        /// </summary>
        /// <returns></returns>
        public T GetValueOrDefault() {
            if(IsLoaded == false)
                ExecuteBatchQuery();

            return ReferenceEquals(_value, null) ? default(T) : _value;
        }
    }
}