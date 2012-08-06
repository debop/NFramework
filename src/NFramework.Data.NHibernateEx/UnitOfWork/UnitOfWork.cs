using System;
using System.Data;
using Castle.Core;
using NHibernate;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// Utility class for Unit Of Work
    /// </summary>
    public static class UnitOfWork {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Thread-safe하게 Current UnitOfWork를 보관하기 위한 키
        /// </summary>
        internal const string CurrentUnitOfWorkKey = "NSoft.NFramework.Data.NH.CurrentUnitOfWork.Key";

        /// <summary>
        /// Thread-safe하게 Long-term conversation 수행하기 위해 필요한 Identity 를 저장하기 위한 키
        /// </summary>
        internal const string CurrentLongConversationIdKey = "NSoft.NFramework.Data.NH.CurrentLongConversationId.Key";

        internal const string CurrentLongPrivateKey = "NSoft.NFramework.Data.NH.CurrentLongPrivate.Key";

        private static IUnitOfWork _globalNonThreadSafeUnitOfWork;
        private static readonly object _syncLock = new object();

        /// <summary>
        /// ASP.NET Session을 이용하여 다중 Page Request에 대해 Transaction이 가능하도록 합니다.
        /// 단 실제 Database Transaction과는 달리 Session을 닫지 않는 다는 뜻입니다.
        /// </summary>
        public static void StartLongConversation() {
            Guard.Assert(InLongConversation == false, @"이미 Long Conversion을 시작했습니다.");

            if(IsDebugEnabled)
                log.Debug("Long conversation을 시작합니다.");

            Local.Data[CurrentLongConversationIdKey] = Guid.NewGuid();
        }

        /// <summary>
        /// Signals the start of an application/user transaction that spans multiple page requests, 
        /// but is not loaded without explicitly specifying the conversation key.
        /// </summary>
        /// <remarks>
        /// Used in conjunction with <see cref="UnitOfWorkHttpApplication"/>, will ensure that the current UoW
        /// (see <see cref="Current"/>) is kept intact across multiple page requests. Review the <see cref="LongConversationManager"/> for details.
        /// <para>
        /// Note: This method does not start a physical database transaction.
        /// </para>
        /// </remarks>
        public static Guid StartPrivateConversation() {
            Guard.Assert(InLongConversation == false, "이미 Long Conversion을 시작했습니다.");

            if(IsDebugEnabled)
                log.Debug("Private Long conversation을 시작합니다.");

            LongConversationIsPrivate = true;
            return (Guid)(Local.Data[CurrentLongConversationIdKey] = Guid.NewGuid());
        }

        /// <summary>
        /// Long conversation 을 종료시킵니다.
        /// </summary>
        public static void EndLongConversation() {
            if(IsDebugEnabled)
                log.Debug("Long conversation을 종료합니다.");

            Local.Data[CurrentLongConversationIdKey] = null;
        }

        /// <summary>
        /// Long-term transaction 중인지
        /// </summary>
        public static bool InLongConversation {
            get { return CurrentLongConversationId != null; }
        }

        /// <summary>
        /// Unit Of Work 가 시작되었는지 
        /// </summary>
        public static bool IsStarted {
            get {
                lock(_syncLock) {
                    if(_globalNonThreadSafeUnitOfWork != null)
                        return true;

                    return Local.Data[CurrentUnitOfWorkKey] != null;
                }
            }
        }

        /// <summary>
        /// Unit Of Work가 시작되지 않았는지
        /// </summary>
        public static bool IsNotStarted {
            get { return (IsStarted == false); }
        }

        /// <summary>
        /// 활성화중인 Long conversation identity 값
        /// </summary>
        public static Guid? CurrentLongConversationId {
            get { return Local.Data[CurrentLongConversationIdKey] as Guid?; }
            internal set { Local.Data[CurrentLongConversationIdKey] = value; }
        }

        /// <summary>
        /// LongConversation이 Private mode인가?
        /// </summary>
        public static bool LongConversationIsPrivate {
            get { return Local.Data[CurrentLongPrivateKey] != null && ((bool)Local.Data[CurrentLongPrivateKey]); }
            internal set { Local.Data[CurrentLongPrivateKey] = value; }
        }

        /// <summary>
        /// 지정된 session factory name을 사용하는 session을 current session으로 설정한다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IDisposable SetCurrentSessionName(string name) {
            return UnitOfWorkFactory.SetCurrentSessionName(name);
        }

        /// <summary>
        /// NOT Thread-safe!!!
        /// using 구문을 이용하여 작업을 처리할 때 편리하도록 제공한다.
        /// </summary>
        /// <param name="global"></param>
        /// <returns></returns>
        public static IDisposable RegisterGlobalUnitOfWork(IUnitOfWork global) {
            if(IsDebugEnabled)
                log.Debug("전역 UnitOfWork를 등록합니다. global=[{0}]", global);

            _globalNonThreadSafeUnitOfWork = global;

            return new DisposableAction(delegate { _globalNonThreadSafeUnitOfWork = null; });
        }

        /// <summary>
        /// 새로운 Unit Of Work을 시작합니다.
        /// </summary>
        /// <param name="nestingOptions"></param>
        /// <returns></returns>
        public static IUnitOfWork Start(UnitOfWorkNestingOptions nestingOptions) {
            return Start(null, nestingOptions);
        }

        /// <summary>
        /// 새로운 Unit Of Work을 시작합니다.
        /// </summary>
        /// <returns></returns>
        public static IUnitOfWork Start() {
            return Start(null, UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
        }

        /// <summary>
        /// 새로운 Unit Of Work을 시작합니다.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static IUnitOfWork Start(IDbConnection connection) {
            return Start(connection, UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork);
        }

        /// <summary>
        /// 새로운 Unit Of Work을 시작합니다.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="nestingOptions"></param>
        /// <returns></returns>
        public static IUnitOfWork Start(IDbConnection connection, UnitOfWorkNestingOptions nestingOptions) {
            if(IsDebugEnabled)
                log.Debug("새로운 Unit Of Work 를 시작합니다. connection=[{0}], nestingOptions=[{1}]", connection, nestingOptions);

            if(_globalNonThreadSafeUnitOfWork != null)
                return _globalNonThreadSafeUnitOfWork;

            var existing = Local.Data[CurrentUnitOfWorkKey] as IUnitOfWorkImplementor;

            if(nestingOptions == UnitOfWorkNestingOptions.ReturnExistingOrCreateUnitOfWork && existing != null) {
                if(IsDebugEnabled)
                    log.Debug("기존 IUnitOfWork가 존재하므로, 사용횟수만 증가시키고, 기존 인스턴스를 사용합니다.");

                existing.IncrementUsages();
                return existing;
            }

            if(IsDebugEnabled)
                log.Debug("Castle.Windsor IoC를 통해 IUnitOfWorkFactory와 IUnitOfWork를 생성합니다.");

            // NOTE : SQLite 메모리 DB에서는 현재 사용하고 있는 DB의 Connection을 이용해야 한다.
            //
            if(existing != null && connection == null && !(existing is NHMultipleUnitOfWorkImplementor))
                // connection = ((ISessionFactoryImplementor) UnitOfWorkFactory.SessionFactory).ConnectionProvider.GetConnection();
                connection = existing.Session.Connection;

            Current = UnitOfWorkFactory.Create(connection, existing);

            if(IsDebugEnabled)
                log.Debug("새로운 UnitOfWork 를 시작했습니다. connection=[{0}]", connection);

            return Current;
        }

        /// <summary>
        /// 현재 실행중인 UnitOfWork를 끝냅니다.
        /// </summary>
        public static void Stop() {
            if(IsStarted && Current != null) {
                Current.Dispose();
                Current = null;

                if(IsDebugEnabled)
                    log.Debug("현재 실행 중인 UnitOfWork 를 완료했습니다.");
            }
        }

        /// <summary>
        /// 현재 실행중인 UnitOfWork를 끝냅니다.
        /// </summary>
        /// <param name="needFlushing">Session에 보관된 내용을 Flushing을 할 것인지 여부</param>
        public static void Stop(bool needFlushing) {
            if(IsDebugEnabled)
                log.Debug("현재 실행중인 UnitOfWork를 중지합니다... needFlushing=[{0}]", needFlushing);

            if(IsStarted && Current != null) {
                if(needFlushing) {
                    try {
                        if(IsDebugEnabled)
                            log.Debug("현 UnitOfWork 의 Flush 작업을 시작합니다...");

                        Current.Flush();

                        if(IsDebugEnabled)
                            log.Debug("현 UnitOfWork 의 Flush 작업을 완료했습니다!!!");
                    }
                    catch(Exception ex) {
                        if(log.IsErrorEnabled) {
                            log.Error("UnitOfWork.Flush 중에 예외가 발생했습니다.");
                            log.Error(ex);
                        }
                    }
                }

                Current.Dispose();
                Current = null;

                if(IsDebugEnabled)
                    log.Debug("현재 실행 중인 UnitOfWork 를 끝냈습니다.");
            }
        }

        /// <summary>
        /// Current unit of work instance
        /// </summary>
        public static IUnitOfWork Current {
            get {
                Guard.Assert(IsStarted,
                             @"Unit Of Work 가 시작되지 않았습니다!!! " +
                             @"Unit Of Work를 사용하시려면, 사욘전에 UnitOfWork.Start()를 호출해주셔야합니다!!!");

                lock(_syncLock)
                    return _globalNonThreadSafeUnitOfWork ?? (IUnitOfWork)Local.Data[CurrentUnitOfWorkKey];
            }

            internal set {
                //lock(_syncLock)
                Local.Data[CurrentUnitOfWorkKey] = value;
            }
        }

        /// <summary>
        /// Current SessionFactory
        /// </summary>
        public static ISessionFactory CurrentSessionFactory {
            get { return UnitOfWorkFactory.SessionFactory; }
        }

        /// <summary>
        /// Current session
        /// </summary>
        public static ISession CurrentSession {
            get { return UnitOfWorkFactory.CurrentSession; }
            set { UnitOfWorkFactory.CurrentSession = value; }
        }

        private static IUnitOfWorkFactory _unitOfWorkFactory;

        /// <summary>
        /// NOTE : Current UnitOfWorkFactory (여러 Container에서 작업시에 UnitOfWorkFactory가 변경될 수 있으므로, 꼭 IoC를 통해서 Resolve해야 한다.
        /// </summary>
        public static IUnitOfWorkFactory UnitOfWorkFactory {
            get {
                // NOTE: IUnitOfWorkFactory는 hibernate configuration file 정보를 알아야 하므로, 환경설정을 하지 않았다면, 기본적으로 hibernate.cfg.xml 을 사용한다.

                if(_unitOfWorkFactory == null)
                    lock(_syncLock)
                        if(_unitOfWorkFactory == null) {
                            if(IsDebugEnabled)
                                log.Debug("UnitOfWorkFactory를 IoC에서 Resolve 합니다....");

                            var factory = IoC.TryResolve<IUnitOfWorkFactory, NHUnitOfWorkFactory>(LifestyleType.Singleton);
                            System.Threading.Thread.MemoryBarrier();
                            _unitOfWorkFactory = factory;

                            if(log.IsInfoEnabled)
                                log.Info("UnitOfWorkFactory를 IoC에서 로드하는데 성공했습니다!!!");
                        }

                return _unitOfWorkFactory;
            }
        }

        /// <summary>
        /// 지정된 Entity 형식이 매핑된 현재 Session을 반환한다.
        /// </summary>
        /// <param name="typeOfEntity"></param>
        /// <returns></returns>
        public static ISession GetCurrentSessionFor(Type typeOfEntity) {
            if(UnitOfWorkFactory is NHMultipleUnitOfWorkFactory)
                return UnitOfWorkFactory.GetCurrentSessionFor(typeOfEntity);

            return CurrentSession;
        }

        /// <summary>
        /// 지정된 Session factory name으로 부터 현재 세션 객체를 가져온다.
        /// </summary>
        /// <param name="name">Factory name</param>
        /// <returns></returns>
        public static ISession GetCurrentSessionFor(string name) {
            return UnitOfWorkFactory.GetCurrentSessionFor(name);
        }

        /// <summary>
        /// 지정된 Entity 형식을 해당 Session에 매핑시킨다.
        /// </summary>
        public static void SetCurrentSession(Type typeOfEntity, ISession session) {
            UnitOfWorkFactory.SetCurrentSession(typeOfEntity, session);
        }

        /// <summary>
        /// called internally to clear the current Unit Of Work and move to the previous one.
        /// </summary>
        /// <param name="unitOfWork"></param>
        public static void DisposeUnitOfWork(IUnitOfWorkImplementor unitOfWork) {
            if(IsDebugEnabled)
                log.Debug("UnitOfWork 를 종료시킵니다. 종료되는 UnitOfWork의 Previous를 Current UnitOfWork로 교체합니다.");

            Current = (unitOfWork != null) ? unitOfWork.Previous : null;
        }

        /// <summary>
        /// 여러 Database에 작업 시 기존 UnitOfWorkFactory를 초기화 해줍니다.
        /// </summary>
        public static void DisposeUnitOfWorkFactory() {
            if(_unitOfWorkFactory != null) {
                if(log.IsInfoEnabled)
                    log.Info("UnitOfWorkFactory를 초기화합니다. 여러 DB에 대한 작업 시에 UnitOfWorkFactory를 초기화 해야 합니다.");

                _unitOfWorkFactory = null;
            }
        }

        /// <summary>
        /// HQL 문장을 제공하는 Provider
        /// </summary>
        public static IIniQueryProvider QueryProvider {
            get { return UnitOfWorkFactory.QueryProvider; }
        }
    }
}