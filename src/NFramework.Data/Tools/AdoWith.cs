using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace NSoft.NFramework.Data {
    public static partial class AdoWith {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const TransactionScopeOption TransactionScopeOptionForTesting = TransactionScopeOption.RequiresNew;

        /// <summary>
        /// Database 작업을 테스트하기 위한 Utility 함수이다.<br/>
        /// 지정된 테스트용 DB 작업이 실제 Database에서 실행은 되지만 Transaction이 Commit이 되지 않으므로, 테스트시에 유용한다.
        /// </summary>
        /// <param name="actionToTest">테스트할 Action</param>
        /// <param name="options">TransactionScopeOption</param>
        /// <example>
        /// // ForTesting 의 anonymous method가 실행되지만 실제 DB에 commit 되지는 않습니다.
        /// <code>
        /// var originalCount = Convert.ToInt32(AdoRepository.ExecuteScalar(CountString));
        /// AdoWith.ForTesting(delegate
        ///		{
        ///			AdoRepository.ExecuteNonQuery(InsertString);
        ///			AdoRepository.ExecuteNonQuery(InsertString2);
        ///			var count = Convert.ToInt32(AdoRepository.ExecuteScalar(CountString));
        ///	
        ///			Assert.AreEqual(originalCount + 2, count);
        ///		});
        /// 
        /// var rollbackCount = Convert.ToInt32(AdoRepository.ExecuteScalar(CountString));
        /// Assert.AreEqual(originalCount, rollbackCount);
        /// </code>
        /// </example>
        public static void ForTesting(this Action actionToTest, TransactionScopeOption options = TransactionScopeOption.RequiresNew) {
            actionToTest.ShouldNotBeNull("actionToTest");

            using(var scope = AdoTool.CreateTransactionScope(options)) {
                actionToTest();

                // NOTE: 테스트를 위해 수행한 것이므로, scope.Complete()를 호출하여 Transaciton을 Commit 하지 않는다. (즉 Rollback 한다)
            }

            if(IsDebugEnabled)
                log.Debug("Execute {0}() for testing, and rollback!!!", actionToTest.Method.Name);
        }

        /// <summary>
        /// Database 작업을 테스트하기 위한 Utility 함수이다.<br/>
        /// 지정된 테스트용 DB 작업이 실제 Database에서 실행은 되지만 Transaction이 Commit이 되지 않으므로, 테스트시에 유용한다.
        /// </summary>
        /// <remarks>
        /// Database 작업에 대해, 실제 DB에 적용은 되지 않고, 테스트만을 하기 원할 때 사용합니다.
        /// </remarks>
        /// <param name="actionsToTest">테스트할 Action 시퀀스</param>
        /// <param name="isolationLevel">Transaction isolation level</param>
        /// <example>
        /// <code>
        /// // ForTesting 의 anonymous method가 실행되지만 실제 DB에 commit 되지는 않습니다.
        /// IList{Action} actions = new List{Action}();
        /// 
        /// actions.Add(DeleteActionTest);
        /// actions.Add(InsertActionTest);
        /// actions.Add(DeleteActionTest);
        /// actions.Add(InsertActionTest);
        /// 
        /// var originalCount = TotalCount();
        /// 
        /// actions.ForTesting(AdoTool.AdoIsolationLevel);
        /// 
        /// Assert.AreEqual(originalCount, TotalCount());
        /// </code>
        /// </example>
        public static void ForTesting(this IEnumerable<Action> actionsToTest, IsolationLevel isolationLevel) {
            actionsToTest.ShouldNotBeNull("actions");

            using(var scope = AdoTool.CreateTransactionScope(TransactionScopeOptionForTesting, isolationLevel)) {
                foreach(var action in actionsToTest.Where(action => action != null)) {
                    if(IsDebugEnabled)
                        log.Debug("Execute {0}() for testing...", action.Method.Name);

                    action();
                }

                //! NOTE: 테스트를 위해 수행한 것이므로, scope.Complete()를 호출하여 Transaciton을 Commit 하지 않는다. (즉 Rollback 한다)
            }

            if(IsDebugEnabled)
                log.Debug("Execute {0} actions for testing and rollback.", actionsToTest.Count());
        }

        /// <summary>
        /// Database 작업을 테스트하기 위한 Utility 함수이다.<br/>
        /// 지정된 테스트용 DB 작업이 실제 Database에서 실행은 되지만 Transaction이 Commit이 되지 않으므로, 테스트시에 유용한다.
        /// </summary>
        /// <remarks>
        /// Database 작업에 대해, 실제 DB에 적용은 되지 않고, 테스트만을 하기 원할 때 사용합니다.
        /// </remarks>
        /// <param name="actionsToTest">테스트할 Action 시퀀스</param>
        /// <example>
        /// <code>
        /// // ForTesting 의 anonymous method가 실행되지만 실제 DB에 commit 되지는 않습니다.
        /// IList{Action} actions = new List{Action}();
        /// 
        /// actions.Add(DeleteActionTest);
        /// actions.Add(InsertActionTest);
        /// actions.Add(DeleteActionTest);
        /// actions.Add(InsertActionTest);
        /// 
        /// var originalCount = TotalCount();
        /// 
        /// actions.ForTesting(AdoTool.AdoIsolationLevel);
        /// 
        /// Assert.AreEqual(originalCount, TotalCount());
        /// </code>
        /// </example>
        public static void ForTesting(this IEnumerable<Action> actionsToTest) {
            ForTesting(actionsToTest, AdoTool.AdoIsolationLevel);
        }

        /// <summary>
        /// Database 작업을 테스트하기 위한 Utility 함수이다.<br/>
        /// 지정된 테스트용 DB 작업이 실제 Database에서 실행은 되지만 Transaction이 Commit이 되지 않으므로, 테스트시에 유용한다.
        /// </summary>
        /// <remarks>
        /// Database 작업에 대해, 실제 DB에 적용은 되지 않고, 테스트만을 하기 원할 때 사용합니다.
        /// </remarks>
        /// <param name="isolationLevel">Transaction isolation level</param>
        /// <param name="actionsToTest">테스트할 <see cref="Action"/> 시퀀스</param>
        /// <example>
        /// <code>
        /// // ForTesting 의 anonymous method가 실행되지만 실제 DB에 commit 되지는 않습니다.
        /// [Test]
        /// [ThreadedRepeat(3)]
        /// public void MultipleActionsForTesting2()
        /// {
        /// 	var originalCount = TotalCount();
        /// 
        /// 	AdoWith.ForTesting(AdoTool.AdoIsolationLevel,
        /// 	                   DeleteActionTest,
        /// 	                   InsertActionTest,
        /// 	                   DeleteActionTest,
        /// 	                   InsertActionTest);
        /// 
        /// 	Assert.AreEqual(originalCount, TotalCount());
        /// }
        /// </code>
        /// </example>
        public static void ForTesting(IsolationLevel isolationLevel, params Action[] actionsToTest) {
            actionsToTest.ShouldNotBeEmpty<Action>("actionsToTest");

            actionsToTest.ForTesting(isolationLevel);
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        /// <param name="scopeOption">TransactionScopeOption 값</param>
        /// <param name="isolationLevel">Transaction 격리수준</param>
        /// <param name="actionsToExecute">TransactionScope 안에서 실행할 Action(s).</param>
        /// <example>
        ///	<code>
        ///		// 한 Tx안에서 3개의 Action 를 수행합니다.
        ///		With.TransactionScope(TransactionScopeOption.Required,
        ///                           System.Transactions.IsolationLevel.ReadCommited,
        ///                           FindAll_By_DetachedCriteria, 
        ///                           FindAll_By_Criterion, 
        ///                           FindAll_By_Example);
        /// </code>
        /// </example>
        public static void TransactionScope(TransactionScopeOption scopeOption,
                                            IsolationLevel isolationLevel,
                                            params Action[] actionsToExecute) {
            if(IsDebugEnabled)
                log.Debug("TransactionScope 안에서, 지정한 Action 들을 실행하기 위해 준비합니다.");

            if(actionsToExecute == null) {
                if(IsDebugEnabled)
                    log.Debug("실행할 Action이 제공되지 않아 반환합니다.");

                return;
            }

            int dtcCount = 0;
            TransactionManager.DistributedTransactionStarted += delegate { dtcCount++; };

            using(var txScope = AdoTool.CreateTransactionScope(scopeOption, isolationLevel)) {
                foreach(Action action in actionsToExecute)
                    if(action != null)
                        action();

                txScope.Complete();
            }

            if(IsDebugEnabled)
                log.Debug("TransactionScope 내에서 Action 들을 실행에 성공했습니다. DTC count=[{0}]", dtcCount);
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        /// <param name="scopeOption">TransactionScopeOption 값</param>
        /// <param name="actionsToExecute">TransactionScope 안에서 실행할 Action(s).</param>
        /// <example>
        ///	<code>
        ///		// 한 Tx안에서 3개의 Action 를 수행합니다.
        ///		With.TransactionScope(TransactionScopeOption.Required,
        ///                           System.Transactions.IsolationLevel.ReadCommited,
        ///                           FindAll_By_DetachedCriteria, 
        ///                           FindAll_By_Criterion, 
        ///                           FindAll_By_Example);
        /// </code>
        /// </example>
        public static void TransactionScope(TransactionScopeOption scopeOption, params Action[] actionsToExecute) {
            TransactionScope(scopeOption, AdoTool.AdoIsolationLevel, actionsToExecute);
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        /// <param name="isolationLevel">Transaction 격리수준</param>
        /// <param name="actionsToExecute">TransactionScope 안에서 실행할 Action(s).</param>
        /// <example>
        ///	<code>
        ///		// 한 Tx안에서 3개의 Action 를 수행합니다.
        ///		With.TransactionScope(TransactionScopeOption.Required,
        ///                           System.Transactions.IsolationLevel.ReadCommited,
        ///                           FindAll_By_DetachedCriteria, 
        ///                           FindAll_By_Criterion, 
        ///                           FindAll_By_Example);
        /// </code>
        /// </example>
        public static void TransactionScope(IsolationLevel isolationLevel, params Action[] actionsToExecute) {
            TransactionScope(TransactionScopeOption.Required, isolationLevel, actionsToExecute);
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        /// <param name="actionsToExecute">TransactionScope 안에서 실행할 Action(s).</param>
        /// <example>
        ///	<code>
        ///		// 한 Tx안에서 3개의 Action 를 수행합니다.
        ///		With.TransactionScope(TransactionScopeOption.Required,
        ///                           System.Transactions.IsolationLevel.ReadCommited,
        ///                           FindAll_By_DetachedCriteria, 
        ///                           FindAll_By_Criterion, 
        ///                           FindAll_By_Example);
        /// </code>
        /// </example>
        public static void TransactionScope(params Action[] actionsToExecute) {
            TransactionScope(AdoTool.AdoIsolationLevel, actionsToExecute);
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        /// <param name="actionToExecute">실행할 Action</param>
        /// <example>
        ///	<code>
        ///		// 한 Tx안에서 3개의 Action 를 수행합니다.
        ///		With.TransactionScope(TransactionScopeOption.Required,
        ///                           System.Transactions.IsolationLevel.ReadCommited,
        ///                           FindAll_By_DetachedCriteria, 
        ///                           FindAll_By_Criterion, 
        ///                           FindAll_By_Example);
        /// </code>
        /// </example>
        public static void TransactionScope(Action actionToExecute) {
            actionToExecute.ShouldNotBeNull("actionToExecute");

            using(var txScope = AdoTool.CreateTransactionScope()) {
                actionToExecute();
                txScope.Complete();
            }
        }
    }
}