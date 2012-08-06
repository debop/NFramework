using System;
using System.Data.Common;
using System.Transactions;

namespace NSoft.NFramework.Data {
    public static partial class AdoTool {
        /// <summary>
        /// ADO.NET Transaction 시의 기본 격리 수준 (= <see cref="System.Transactions.IsolationLevel.Unspecified"/>)
        /// </summary>
        public static readonly IsolationLevel AdoIsolationLevel = IsolationLevel.ReadCommitted;

        /// <summary>
        /// 지정한 Command를 현재 활성화된 Transaction에 참여시킵니다.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="cmd"></param>
        public static void EnlistCommandToActiveTransaction(this IAdoRepository repository, DbCommand cmd) {
            cmd.ShouldNotBeNull("cmd");

            if(repository.IsActiveTransaction) {
                if(IsDebugEnabled)
                    log.Debug("DbCommand 객체를 현재 활성화된 Active Transaction에 참가시킵니다...");

                cmd.Connection = repository.ActiveTransaction.Connection;
                cmd.Transaction = repository.ActiveTransaction;
            }
        }

        /// <summary>
        /// 새로운 TransactionScope 를 생성한다.
        /// </summary>
        /// <param name="scopeOption">TransactionScopeOption 값</param>
        /// <param name="isolationLevel">Transaction 격리수준</param>
        /// <returns>Instance of new <see cref="System.Transactions.TransactionScope"/></returns>
        /// <example>
        /// <code>
        /// using(TransactionScope txScope = AdoTool.CreateTransactionScope(TransactionScopeOption.Required, System.Transactions.IsolationLevel.ReadCommitted))
        /// {
        ///		// database operations...
        /// 
        ///		txScope.Commit();
        /// }
        /// </code>
        /// </example>
        public static TransactionScope CreateTransactionScope(TransactionScopeOption scopeOption = TransactionScopeOption.Required,
                                                              IsolationLevel isolationLevel = IsolationLevel.ReadCommitted) {
            if(IsDebugEnabled)
                log.Debug("새로운 TransactionScope를 생성합니다... TransactionScopeOption=[{0}], IsolationLevel=[{1}]", scopeOption,
                          isolationLevel);

            var txOptions = new TransactionOptions { IsolationLevel = isolationLevel };
            return new TransactionScope(scopeOption, txOptions);
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
        ///		AdoTool.UseTransactionScope(TransactionScopeOption.Required,
        ///                                  IsolationLevel.ReadCommited,
        ///                                  FindAll_By_DetachedCriteria, 
        ///                                  FindAll_By_Criterion, 
        ///                                  FindAll_By_Example);
        /// </code>
        /// </example>
        public static void UseTransactionScope(TransactionScopeOption scopeOption,
                                               IsolationLevel isolationLevel,
                                               params Action[] actionsToExecute) {
            if(IsDebugEnabled)
                log.Debug("TransactionScope 안에서, 지정한 Action 들을 실행하기 위해 준비합니다... scopeOption=[{0}], isolationLevel=[{1}]", scopeOption,
                          isolationLevel);

            if(actionsToExecute == null) {
                if(IsDebugEnabled)
                    log.Debug("실행할 Action이 제공되지 않아 중단합니다!!!");

                return;
            }

            using(var txScope = CreateTransactionScope(scopeOption, isolationLevel)) {
                foreach(Action action in actionsToExecute)
                    action();

                txScope.Complete();
            }

            if(IsDebugEnabled)
                log.Debug("Transaction Scope 내에서 Action 들을 실행에 성공했습니다!!!");
        }

        /// <summary>
        /// 지정된 Action을 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        public static void UseTransactionScope(Action actionToExecute) {
            using(var txScope = CreateTransactionScope()) {
                actionToExecute();
                txScope.Complete();
            }
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        public static void UseTransactionScope(TransactionScopeOption scopeOption, params Action[] actionsToExecute) {
            UseTransactionScope(scopeOption, AdoIsolationLevel, actionsToExecute);
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        public static void UseTransactionScope(IsolationLevel isolationLevel, params Action[] actionsToExecute) {
            UseTransactionScope(TransactionScopeOption.Required, isolationLevel, actionsToExecute);
        }

        /// <summary>
        /// 지정된 Action 들을 하나의 Transaction Scope로 묶어서 처리합니다.
        /// </summary>
        public static void UseTransactionScope(params Action[] actionsToExecute) {
            UseTransactionScope(AdoIsolationLevel, actionsToExecute);
        }
    }
}