using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Criterion;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// Batch processing for collection of Criteria using NHibernate 2.0
    /// </summary>
    /// <remarks>
    /// IMultiCriteria를 이용하여, Batch 작업을 수행한다. 수행시간을 절약할 수 있고, Transaction 경계를 삼을 수 있어서 좋다.
    /// </remarks>
    /// <example>
    /// <code>
    /// // simple use CriteriaBatch
    /// ICollection&lt;SMS&gt; loadedMsgs = null;
    /// 
    /// new CriteriaBatch(_session)
    /// 	.Add(DetachedCriteria.For&lt;SMS&gt;, Order.Asc("Id"))
    /// 	.OnRead(delegate(ICollection&lt;SMS&gt; msgs) { loadedMsgs = msgs; })
    /// 	.Execute();
    /// 
    /// Assert.IsNotNull(loadedMsgs);
    /// 
    /// foreach (SMS msg in loadedMsgs)
    /// 	Console.WriteLine(msg.ObjectToString());
    /// </code>
    /// <code>
    /// 
    /// // 1. 모든 엔티티 목록을 가져오면서, 갯수도 계산한다.
    /// // 2. 페이지 기능을 이용하여 하나의 엔티티만 가져온다.
    /// ICollection&lt;SMS&gt; loadedMsgs = null;
    /// int msgCount = 0;
    /// SMS loadedMsg = null;
    /// 
    /// new CriteriaBatch(_session)
    /// 	.Add(DetachedCriteria.For&lt;SMS&gt;(), Order.Asc("Id"))
    /// 	.OnRead(delegate(ICollection&lt;SMS&gt; msgs, int count)
    /// 	        	{
    /// 	        		loadedMsgs = msgs;
    /// 	        		msgCount = count;
    /// 	        	})
    /// 	.Add(DetachedCriteria.For&lt;SMS&gt;())
    /// 	.Paging(0, 1)
    /// 	.OnRead(delegate(SMS msg) { loadedMsg = msg; })
    /// 	.Execute();
    /// 
    /// Assert.IsNotNull(loadedMsgs);
    /// Assert.AreEqual(1, msgCount);
    /// Assert.IsNotNull(loadedMsg);
    /// 
    /// </code>
    /// </example>
    public class CriteriaBatch {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ISession _session;
        private DetachedCriteria _currentCriteria;
        private int _currentIndex = -1;

        private readonly List<Action<IList>> _collectionDelegates = new List<Action<IList>>();
        private readonly List<Action<object>> _uniqueResultDelegate = new List<Action<object>>();
        private readonly List<Action<IList, int>> _collectionAndCountDelegate = new List<Action<IList, int>>();
        private readonly List<DetachedCriteria> _criteriaList = new List<DetachedCriteria>();

        /// <summary>
        /// Criteria 실행시에 발생하는 이벤트
        /// </summary>
        public static event ProcessCriteriaDelegate ProcessCriteria;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CriteriaBatch() {}

        /// <summary>
        /// Initialize a new instance of CriteriaBatch with NHibernate session.
        /// </summary>
        /// <param name="session">Instance of ISession to execute criteria</param>
        public CriteriaBatch(ISession session) {
            if(IsDebugEnabled)
                log.Debug("Create new instance of CriteriaBatch with session.");

            _session = session;
        }

        /// <summary>
        /// 지정된 Criteria를 Criteria Batch에 추가한다.
        /// </summary>
        /// <param name="criteria">criteria to look for</param>
        /// <param name="orders">order by</param>
        /// <returns>Current object</returns>
        public CriteriaBatch Add(DetachedCriteria criteria, params Order[] orders) {
            criteria.ShouldNotBeNull("criteria");

            if(orders != null)
                foreach(Order order in orders)
                    criteria.AddOrder(order);

            return Add(criteria);
        }

        /// <summary>
        /// 지정된 Criteria를 Criteri Batch 정보에 추가한다.
        /// </summary>
        /// <param name="criteria">criteria to add.</param>
        /// <returns>Current instance of CriteriaBatch</returns>
        public virtual CriteriaBatch Add(DetachedCriteria criteria) {
            criteria.ShouldNotBeNull("criteria");

            _currentIndex++;
            _criteriaList.Add(criteria);
            _currentCriteria = criteria;
            _collectionDelegates.Add(null);
            _uniqueResultDelegate.Add(null);
            _collectionAndCountDelegate.Add(null);

            return this;
        }

        /// <summary>
        /// 하나의 Entity 정보를 읽을 때 지정된 readAction을 수행하도록 설정한다.
        /// </summary>
        /// <typeparam name="T">Entity 수형</typeparam>
        /// <param name="readAction">Entity Loading시 실행할 Action</param>
        /// <returns>Current instance of CriteriaBatch</returns>
        public CriteriaBatch OnRead<T>(Action<T> readAction) {
            readAction.ShouldNotBeNull("readAction");

            _uniqueResultDelegate[_currentIndex] = (obj => readAction((T)obj));
            return this;
        }

        /// <summary>
        /// 복수의 Entity 정보를 읽을 때 지정된 readAction을 수행하도록 설정한다.
        /// </summary>
        /// <typeparam name="T">Entity 수형</typeparam>
        /// <param name="readAction">Entity Loading시 실행할 Action</param>
        /// <returns>Current instance of CriteriaBatch</returns>
        public CriteriaBatch OnRead<T>(Action<ICollection<T>, int> readAction) {
            readAction.ShouldNotBeNull("readAction");

            _collectionAndCountDelegate[_currentIndex] =
                ((list, count) => readAction(list.Cast<T>().ToList(), count));

            // 결과 Set의 RowCount를 세는 Criteria를 추가한다.
            // for NHibernate 2.0
            Add(CriteriaTransformer.TransformToRowCount(_currentCriteria));
            // for NHibernate 1.2
            // Add(_currentCriteria.SetProjection(Projections.RowCount()));
            return this;
        }

        /// <summary>
        /// 복수의 Entity 정보를 읽을 때 지정된 readAction을 수행하도록 설정한다.
        /// </summary>
        /// <typeparam name="T">Entity 수형</typeparam>
        /// <param name="readAction">Entity Loading시 실행할 Action</param>
        /// <returns>Current instance of CriteriaBatch</returns>
        public CriteriaBatch OnRead<T>(Action<ICollection<T>> readAction) {
            readAction.ShouldNotBeNull("readAction");

            _collectionDelegates[_currentIndex] = (list => readAction(list.Cast<T>().ToList()));

            return this;
        }

        /// <summary>
        /// 등록된 Batch 작업을 실행한다. (Session이 정의되어 있지 않다면, <see cref="UnitOfWork.CurrentSession"/>을 사용한다.)
        /// </summary>
        /// <returns>실행 결과</returns>
        public virtual IList Execute() {
            return Execute(_session ?? UnitOfWork.CurrentSession);
        }

        /// <summary>
        /// 지정된 Session에 대해 등록된 Criteria를 Batch 방식으로 수행한다.
        /// </summary>
        /// <param name="theSession">Criteria를 수행한 Session</param>
        /// <returns>실행 결과</returns>
        public virtual IList Execute(ISession theSession) {
            if(_criteriaList.Count == 0)
                return new ArrayList();

            var multiCriteria = theSession.CreateMultiCriteria();

            foreach(DetachedCriteria detachedCriteria in _criteriaList)
                multiCriteria.Add(CreateCriteria(theSession, detachedCriteria));

            var list = multiCriteria.List();
            int results = list.Count;

            for(int i = 0; i < results; i++) {
                if(_collectionDelegates[i] != null)
                    _collectionDelegates[i]((IList)list[i]);

                if(_uniqueResultDelegate[i] != null) {
                    var single = ((IEnumerable)list[i]).SingleUnsafe();
                    _uniqueResultDelegate[i](single);
                }
                if(_collectionAndCountDelegate[i] != null) {
                    var queryList = (IList)list[i];
                    int count = Convert.ToInt32(((IEnumerable)list[i + 1]).SingleUnsafe());
                    _collectionAndCountDelegate[i](queryList, count);
                    i += 1;
                }
            }

            return list;
        }

        /// <summary>
        /// Current Criteria의 Paging 설정을 수행한다.
        /// </summary>
        /// <param name="firstResult">결과 Set의 첫번째 인덱스</param>
        /// <param name="maxResults">결과 Set의 최대 레코드 수 (0 이하이면 설정하지 않는다.)</param>
        /// <returns>Current instance of CriteriaBatch</returns>
        public CriteriaBatch Paging(int firstResult, int maxResults) {
            if(firstResult >= 0)
                _currentCriteria.SetFirstResult(firstResult);

            if(maxResults > 0)
                _currentCriteria.SetMaxResults(maxResults);

            return this;
        }

        /// <summary>
        /// 세션에 사용될 ICriteria를 생성한다.
        /// </summary>
        /// <param name="theSession"></param>
        /// <param name="detachedCriteria"></param>
        /// <returns></returns>
        private static ICriteria CreateCriteria(ISession theSession, DetachedCriteria detachedCriteria) {
            theSession.ShouldNotBeNull("theSession");
            detachedCriteria.ShouldNotBeNull("detachedCriteria");

            var criteria = detachedCriteria.GetExecutableCriteria(theSession);

            var processDelegate = ProcessCriteria;
            if(processDelegate != null) {
                foreach(ProcessCriteriaDelegate process in processDelegate.GetInvocationList())
                    criteria = process(criteria);
            }
            return criteria;
        }
    }
}