using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Criterion;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// Future Value를 로딩하기위한 Query들을 예약 보관하고 있다가 한번의 배치작업으로 실행해서, 
    /// 실제로 값을 
    /// Hold the future of a query, when you try to iterate over
    /// a instance of <see cref="FutureQueryOf{TEntity}"/> or access the Results
    /// or TotalCount properties, all the future queries in the current context
    /// (current thread / current request) are executed as a single batch and all
    /// their results are loaded in a single round trip.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [Obsolete("NHibernate 3.x 의 Future를 사용하세요")]
    public class FutureQueryOf<TEntity> : FutureBase, IEnumerable<TEntity> {
        private ICollection<TEntity> _results;
        private int? _totalCount;

        #region << Constructors >>

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="detachedCriteria">criteria to look for</param>
        public FutureQueryOf(DetachedCriteria detachedCriteria) : this(detachedCriteria, FutureQueryOptions.None) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="detachedCriteria">criteria to look for</param>
        /// <param name="firstResult">first result index (start by 0)</param>
        /// <param name="maxResults">maximum result count (if maxResults is 0, retrieve all records from firstResult.)</param>
        public FutureQueryOf(DetachedCriteria detachedCriteria, int firstResult, int maxResults)
            : this(detachedCriteria.SetFirstResult(firstResult).SetMaxResults(maxResults), FutureQueryOptions.WithTotalCount) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="detachedCriteria">criteria to look for</param>
        /// <param name="options">future query option</param>
        public FutureQueryOf(DetachedCriteria detachedCriteria, FutureQueryOptions options) {
            var criteriaBatch = Batcher.Add(detachedCriteria);

            switch(options) {
                case FutureQueryOptions.None:
                    criteriaBatch.OnRead(delegate(ICollection<TEntity> entities) {
                                             _results = entities;
                                             IsLoaded = true;
                                         });
                    break;
                case FutureQueryOptions.WithTotalCount:
                    criteriaBatch.OnRead(delegate(ICollection<TEntity> entities, int count) {
                                             _results = entities;
                                             _totalCount = count;
                                             IsLoaded = true;
                                         });
                    break;
                default:
                    throw new NotSupportedException(options.ToString());
            }
        }

        #endregion

        /// <summary>
        /// 조회한 Entity의 전체 갯수
        /// </summary>
        /// <seealso cref="FutureQueryOptions.WithTotalCount"/>
        public int TotalCount {
            get {
                if(IsLoaded == false)
                    ExecuteBatchQuery();

                if(_totalCount.HasValue == false)
                    throw new InvalidOperationException("The future was not asked to load the total query as well");

                return _totalCount.Value;
            }
        }

        /// <summary>
        /// 조회한 Entity의 collection
        /// </summary>
        public ICollection<TEntity> Results {
            get {
                if(IsLoaded == false)
                    ExecuteBatchQuery();

                return _results;
            }
        }

        /// <summary>
        /// 결과 Entity. (유일하지 않거나, 결과 Entity가 null이면 예외가 발생한다.
        /// </summary>
        /// 
        public TEntity SingleResult {
            get {
                var results = Results;
                return Enumerable.Single(results);
            }
        }

        /// <summary>
        /// Return enumerator for TEntity
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TEntity> GetEnumerator() {
            return Results.GetEnumerator();
        }

        /// <summary>
        /// Return enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)this).GetEnumerator();
        }
    }
}