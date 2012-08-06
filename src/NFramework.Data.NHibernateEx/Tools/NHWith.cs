using System;
using System.ComponentModel;
using System.Data;
using NHibernate;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Helper class for Database operation with NHibernate 
    /// </summary>
    public static partial class NHWith {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Caching >>

        #region << Caching Class >>

        /// <summary>
        /// Caching
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static class Caching {
            /// <summary>
            /// caching key
            /// </summary>
            public static string CachingKey = "rcl.data.nh.with.caching";

            /// <summary>
            /// cache region key
            /// </summary>
            public static string CachingRegionKey = "rcl.data.nh.with.caching.region";

            /// <summary>
            /// force cache refresh key
            /// </summary>
            public static string ForceCacheRefreshKey = "rcl.data.nh.with.caching.refresh";

            /// <summary>
            /// Cache를 갱신해야 하는지 여부
            /// </summary>
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool ShouldForceCacheRefresh {
                get { return true.Equals(Local.Data[ForceCacheRefreshKey]); }
            }

            /// <summary>
            /// 캐시 사용 가능 여부
            /// </summary>
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static bool Enabled {
                get { return true.Equals(Local.Data[CachingKey]); }
            }

            /// <summary>
            /// 현 캐시의 영역을 나타내는 문자열
            /// </summary>
            [Browsable(false)]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static string CurrentCacheRegion {
                get { return (string)Local.Data[CachingRegionKey]; }
            }

            /// <summary>
            /// 지정한 캐시 영역에서 IQuery를 캐시한 정보를 모두 제거한다.
            /// </summary>
            /// <param name="region"></param>
            /// <seealso cref="ISessionFactory.EvictQueries(string)"/>
            public static void ClearQueryCacheRegion(string region) {
                UnitOfWork.CurrentSession
                    .GetSessionImplementation()
                    .Factory
                    .EvictQueries(region);
            }
        }

        #endregion

        /// <summary>
        /// 반환받는 <see cref="DisposableAction"/>이 Dispose될 때 강제적으로 Cache를 Refresh합니다.
        /// </summary>
        public static IDisposable ForceCacheRefresh() {
            Local.Data[Caching.ForceCacheRefreshKey] = true;
            return new DisposableAction(delegate { Local.Data[Caching.ForceCacheRefreshKey] = null; });
        }

        /// <summary>
        /// 임시로 Query에 대한 Cache를 적용할 수 있도록 합니다.
        /// </summary>
        public static IDisposable TemporaryQueryCache() {
            if(IsDebugEnabled)
                log.Debug("Start temporary query chach...");

            string regionId = Guid.NewGuid().ToString();
            var cache = QueryCache(regionId);

            return new DisposableAction(delegate {
                                            if(IsDebugEnabled)
                                                log.Debug("release tempolary cache.");

                                            cache.Dispose();
                                            Caching.ClearQueryCacheRegion(regionId);
                                        });
        }

        /// <summary>
        /// <seealso cref="QueryCache(string)"/>
        /// </summary>
        public static IDisposable QueryCache() {
            return QueryCache(null);
        }

        /// <summary>
        /// 이 함수를 호출할 때에는 Caching이 되도록 하는데, 
        /// 이 함수의 반환 객체가 disposing 될 때에는 그 전 설정으로 복귀한다.
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        /// <example>
        /// <code>
        /// using(NHWith.QueryCache("rcl"))   // set query caching enabled to true, and region to "rcl"
        /// {
        ///		//
        ///		// some nhibernate code
        ///		// 이 영역에서만 "rcl" 캐시 영역에 IQuery가 캐시된다. 
        ///		//
        /// } // reset query caching enabled and region value
        /// </code>
        /// </example>
        public static IDisposable QueryCache(string region) {
            if(IsDebugEnabled)
                log.Debug("새로운 영역의 QueryCache를 시작합니다. Region=[{0}]", region);

            // 보관용
            var prevCachingEnabled = Local.Data[Caching.CachingKey];
            var prevCachingRegion = Local.Data[Caching.CachingRegionKey];

            if(IsDebugEnabled)
                log.Debug("Set new cache settings for IQuery. Cache enabled=[{0}], Cache Region=[{1}]", true, region);

            // Stack에 Push한다.
            Local.Data[Caching.CachingKey] = true;
            Local.Data[Caching.CachingRegionKey] = region;

            // Disposinge될 때, 보관된 정보로 복구할 수 있도록 한다. (Stack에서 Pop하는 방식)
            return new DisposableAction(() => {
                                            if(IsDebugEnabled)
                                                log.Debug("Restore previous cache settings. Cache Enabled=[{0}], Region=[{1}]",
                                                          prevCachingEnabled, prevCachingRegion);

                                            Local.Data[Caching.CachingKey] = prevCachingEnabled;
                                            Local.Data[Caching.CachingRegionKey] = prevCachingRegion;
                                        });
        }

        #endregion

        #region << Stateless Session >>

        /// <summary>
        /// IStatelessSession을 사용한 DB 처리를 수행할 때 사용합니다.
        /// </summary>
        /// <param name="action"></param>
        /// <remarks>
        ///	SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.
        /// </remarks>
        public static void StatelessSession(Action<IStatelessSession> action) {
            StatelessSession(UnitOfWork.CurrentSession, action);
        }

        /// <summary>
        /// IStatelessSession을 사용한 DB 처리를 수행할 때 사용합니다. (주의: SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.)
        /// </summary>
        /// <param name="session">현재 사용중인 세션</param>
        /// <param name="actionToUpdateExecute">현재 사용중인 세션의 Connection을 이용하여 IStatelessSession을 만들고 그 StatelessSession하에서 실행할 Action</param>
        /// <remarks>
        ///	NOTE: SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.
        /// </remarks>
        public static void StatelessSession(ISession session, Action<IStatelessSession> actionToUpdateExecute) {
            session.ShouldNotBeNull("session");
            actionToUpdateExecute.ShouldNotBeNull("actionToUpdateExecute");

            if(IsDebugEnabled)
                log.Debug("지정된 세션으로부터 StatelessSession을 생성하고 Transaction 하에서 지정된 action을 수행합니다. session=[{0}]", session);

            using(var stateless = CreateStatelessSession(session)) {
                var tx = stateless.BeginTransaction();
                try {
                    actionToUpdateExecute(stateless);
                    tx.Commit();
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled) {
                        log.Error("Transaction 하의 StatelessSession 작업이 실패했습니다. Rollback합니다.");
                        log.Error(ex);
                    }

                    if(tx != null)
                        tx.Rollback();

                    throw;
                }
            }
        }

        /// <summary>
        /// IStatelessSession을 사용한 DB 처리를 수행할 때 사용합니다. (주의: SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.)
        /// </summary>
        /// <param name="connection">현재 사용중인 Connection</param>
        /// <param name="actionToUpdateExecute">현재 사용중인 세션의 Connection을 이용하여 IStatelessSession을 만들고 그 StatelessSession하에서 실행할 Action</param>
        /// <remarks>
        ///	NOTE: SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.
        /// </remarks>
        public static void StatelessSession(IDbConnection connection, Action<IStatelessSession> actionToUpdateExecute) {
            connection.ShouldNotBeNull("connection");
            actionToUpdateExecute.ShouldNotBeNull("actionToUpdateExecute");

            if(IsDebugEnabled)
                log.Debug("지정된 connection으로부터 StatelessSession을 생성하고 Transaction 하에서 지정된 action을 수행합니다. connection=[{0}]", connection);

            using(var stateless = CreateStatelessSession(connection)) {
                var tx = stateless.BeginTransaction();
                try {
                    actionToUpdateExecute(stateless);
                    tx.Commit();
                }
                catch(Exception ex) {
                    if(log.IsErrorEnabled) {
                        log.Error("Transaction 하의 작업이 실패했습니다. Rollback합니다.");
                        log.Error(ex);
                    }

                    if(tx != null)
                        With.TryAction(() => tx.Rollback());

                    throw;
                }
            }
        }

        /// <summary>
        /// IStatelessSession을 사용한 Data 조회 시에만 사용하세요. (주의: SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.)
        /// </summary>
        /// <param name="actionToUpdateExecute"></param>
        public static void StatelessSessionNoTransaction(Action<IStatelessSession> actionToUpdateExecute) {
            actionToUpdateExecute.ShouldNotBeNull("actionToUpdateExecute");
            StatelessSessionNoTransaction(UnitOfWork.CurrentSession, actionToUpdateExecute);
        }

        /// <summary>
        /// IStatelessSession을 사용한 Data 조회 시에만 사용하세요. (주의: SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.)
        /// </summary>
        /// <param name="session">NHibernate session</param>
        /// <param name="actionToUpdateExecute">IStatelessSession을 이용하여 수행하는 메소드</param>
        public static void StatelessSessionNoTransaction(ISession session, Action<IStatelessSession> actionToUpdateExecute) {
            session.ShouldNotBeNull("session");
            actionToUpdateExecute.ShouldNotBeNull("actionToUpdateExecute");

            using(var stateless = CreateStatelessSession(session)) {
                actionToUpdateExecute(stateless);
            }
        }

        /// <summary>
        /// IStatelessSession을 사용한 Data 조회 시에만 사용하세요. (주의: SQLite의 Memory DB에서는 기존 Session과 다른 DB를 바라볼 수 있다. 이때는 예외가 발생한다.)
        /// </summary>
        /// <param name="connection">ADO.NET Connection</param>
        /// <param name="actionToUpdateExecute">IStatelessSession을 이용하여 수행하는 메소드</param>
        public static void StatelessSessionNoTransaction(IDbConnection connection, Action<IStatelessSession> actionToUpdateExecute) {
            connection.ShouldNotBeNull("connection");
            actionToUpdateExecute.ShouldNotBeNull("actionToUpdateExecute");

            using(var stateless = CreateStatelessSession(connection)) {
                actionToUpdateExecute(stateless);
            }
        }

        private static IStatelessSession CreateStatelessSession(ISession session) {
            session.ShouldNotBeNull("session");
            return session.SessionFactory.OpenStatelessSession();
        }

        private static IStatelessSession CreateStatelessSession(IDbConnection connection) {
            return UnitOfWork.CurrentSessionFactory.OpenStatelessSession(connection);
        }

        #endregion

        #region << Transaction >>

        /// <summary>
        /// 지정된 <see cref="Action"/>를 Transaction 하에서 수행한다.
        /// </summary>
        /// <param name="transactionalAction">수행할 Action</param>
        public static void Transaction(Action transactionalAction) {
            Transaction(IsolationLevel.ReadCommitted, transactionalAction);
        }

        /// <summary>
        /// 지정된 <see cref="Action"/>를 지정된 격리수준의 Transaction 하에서 수행한다.
        /// </summary>
        public static void Transaction(System.Data.IsolationLevel isolationLevel, Action transactionalAction) {
            Transaction(isolationLevel, transactionalAction, UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
        }

        /// <summary>
        /// 지정된 <see cref="Action"/>를 지정된 격리수준의 Transaction 하에서 수행한다.
        /// </summary>
        /// <param name="isolationLevel">격리수준</param>
        /// <param name="transactionalAction">Action to execute under Transaction</param>
        /// <param name="nestingOptions">UnitOfWork Nesting option.</param>
        public static void Transaction(IsolationLevel isolationLevel,
                                       Action transactionalAction,
                                       UnitOfWorkNestingOptions nestingOptions) {
            transactionalAction.ShouldNotBeNull("transactionalAction");

            if(IsDebugEnabled)
                log.Debug("Execute the specified action under transaction... " +
                          "isolationLevel=[{0}], transactionalAction=[{1}], nestingOptions=[{2}]",
                          isolationLevel, transactionalAction, nestingOptions);

            using(UnitOfWork.Start(nestingOptions)) {
                // if we are already in a transaction, don't start a new one
                if(UnitOfWork.Current.IsInActiveTransaction) {
                    if(IsDebugEnabled)
                        log.Debug("활성화된 기존 Transaction에 참여합니다.");

                    transactionalAction();
                }
                else {
                    if(IsDebugEnabled)
                        log.Debug("새로운 NHibernate.ITransaction을 생성합니다.");

                    var tx = UnitOfWork.Current.BeginTransaction(isolationLevel);

                    try {
                        transactionalAction();

                        tx.Commit();

                        if(IsDebugEnabled)
                            log.Debug("Transactional Action을 수행하고, Transaction.Commit을 수행하였습니다.");
                    }
                    catch(Exception ex) {
                        if(log.IsErrorEnabled) {
                            log.Error("Fail to execute transactional action. rollback transaction.");
                            log.Error(ex);
                        }

                        tx.Rollback();
                        throw;
                    }
                    finally {
                        if(tx != null) {
                            tx.Dispose();

                            if(IsDebugEnabled)
                                log.Debug("Dispose current transaction.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 지정된 함수를 현재 UnitOfWork의 Transaction 하에서 수행합니다.
        /// </summary>
        /// <typeparam name="T">Return Type of Function to execute</typeparam>
        /// <param name="transactionalFunc">Function to execute under Transaction</param> 
        /// <returns>함수 수행 결과</returns>
        public static T Transaction<T>(Func<T> transactionalFunc) {
            return Transaction(transactionalFunc, IsolationLevel.ReadCommitted);
        }

        /// <summary>
        ///  지정된 함수를 현재 UnitOfWork의 Transaction 하에서 수행합니다.
        /// </summary>
        /// <typeparam name="T">Return Type of Function to execute</typeparam>
        /// <param name="transactionalFunc">Function to execute under Transaction</param> 
        /// <param name="isolationLevel">격리수준</param>
        /// <returns>함수 수행 결과</returns>
        public static T Transaction<T>(Func<T> transactionalFunc, IsolationLevel isolationLevel) {
            return Transaction(transactionalFunc, isolationLevel, UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
        }

        /// <summary>
        ///  지정된 함수를 현재 UnitOfWork의 Transaction 하에서 수행합니다.
        /// </summary>
        /// <typeparam name="T">Return Type of Function to execute</typeparam>
        /// <param name="transactionalFunc">Function to execute under Transaction</param> 
        /// <param name="isolationLevel">격리수준</param>
        /// <param name="nestingOptions">UnitOfWork Nesting option.</param>
        /// <returns>함수 수행 결과</returns>
        public static T Transaction<T>(Func<T> transactionalFunc, IsolationLevel isolationLevel, UnitOfWorkNestingOptions nestingOptions) {
            transactionalFunc.ShouldNotBeNull("transactionalFunc");

            if(IsDebugEnabled)
                log.Debug("Execute the specified action under transaction... " +
                          "transactionalFunc=[{0}], isolationLevel=[{1}], nestingOptions=[{2}]",
                          transactionalFunc, isolationLevel, nestingOptions);

            // 기존 UnitOfWork에 참여할 수도 있고, 새로운 UnitOfWork를 생성할 수도 있습니다.
            //
            using(UnitOfWork.Start(nestingOptions)) {
                // 기존 Transaction 이 없다면, 새로운 Transaction 에서 작업합니다.
                //
                if(UnitOfWork.Current.IsInActiveTransaction == false) {
                    if(IsDebugEnabled)
                        log.Debug("새로운 NHibernate.ITransaction을 생성합니다.");

                    var tx = UnitOfWork.Current.BeginTransaction(isolationLevel);

                    try {
                        var result = transactionalFunc();
                        tx.Commit();

                        if(IsDebugEnabled)
                            log.Debug("Transactional Function을 수행하고, Transaction.Commit을 수행하였습니다!!!");

                        return result;
                    }
                    catch(Exception ex) {
                        if(log.IsErrorEnabled) {
                            log.Error("Fail to execute transactional action. rollback transaction.");
                            log.Error(ex);
                        }

                        tx.Rollback();
                        throw;
                    }
                    finally {
                        if(tx != null) {
                            tx.Dispose();

                            if(IsDebugEnabled)
                                log.Debug("Dispose current transaction.");
                        }
                    }
                }

                if(IsDebugEnabled)
                    log.Debug("활성화된 기존 Transaction에 참여하여, 실행합니다.");

                return transactionalFunc.Invoke();
            }
        }

        #endregion
    }
}