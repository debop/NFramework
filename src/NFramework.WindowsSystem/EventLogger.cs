using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// Windows 의 Event Log 서비스에 Event를 기록하는 Logger입니다.
    /// </summary>
    public class EventLogger : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly string[] SystemDefaultLogNames = new[] { "Application", "Security", "System" };

        /// <summary>
        /// 기본 로그 명
        /// </summary>
        public const string DEFAULT_LOG_NAME = "Application";

        /// <summary>
        /// 기본 머신 이름
        /// </summary>
        public const string DEFAULT_MACHINE_NAME = ".";

        /// <summary>
        /// MS Windows 시스템 Event Log 객체
        /// </summary>
        private EventLog _eventLog;

        /// <summary>
        /// RwEventLog 생성자 (Application Event Log)
        /// </summary>
        public EventLogger() : this(DEFAULT_LOG_NAME) {}

        /// <summary>
        /// RwEventLog 생성자
        /// </summary>
        /// <param name="logName">ex: Application, Security, System 같은 로그 이름</param>
        public EventLogger(string logName) : this(logName, DEFAULT_MACHINE_NAME) {}

        /// <summary>
        /// RwEventLog 생성자
        /// </summary>
        /// <param name="logName">ex: Application, Security, System 같은 로그 이름</param>
        /// <param name="machineName">Computer machine name (ex '.')</param>
        public EventLogger(string logName, string machineName)
            : this(logName, machineName, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name) {}

        /// <summary>
        /// RwEventLog 생성자
        /// </summary>
        /// <param name="logName">ex: Application, Security, System 같은 로그 이름</param>
        /// <param name="machineName">Computer machine name (ex '.')</param>
        /// <param name="source">event source (일반적으로 Assembly 명을 사용한다)</param>
        public EventLogger(string logName, string machineName, string source) {
            if(IsDebugEnabled)
                log.Debug("Create EventLogger with logName=[{0}], machineName=[{1}], source=[{2}]", logName, machineName, source);

            LogName = logName.AsText().Trim();
            MachineName = machineName;

            if(source.IsEmpty())
                source = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;

            Source = source;

            _eventLog = EventLogTool.GetEventLog(LogName, Source, MachineName);

            IsSystemDefault = SystemDefaultLogNames.Any(name => name.Compare(LogName, true) == 0);
        }

        /// <summary>
        /// 이벤트 로그의 이름
        /// </summary>
        public string LogName { get; private set; }

        /// <summary>
        /// 원본 이름
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// 컴퓨터 이름
        /// </summary>
        public string MachineName { get; private set; }

        /// <summary>
        /// 사용 가능 여부
        /// </summary>
        public bool IsOpen {
            get { return (_eventLog != null); }
        }

        /// <summary>
        /// 열려진 EventLog 객체
        /// </summary>
        public EventLog Log {
            get {
                CheckEventLog();
                return _eventLog;
            }
        }

        /// <summary>
        /// Windows System이 제공하는 기본 EventLog 여부
        /// </summary>
        public bool IsSystemDefault { get; private set; }

        /// <summary>
        /// Event Log 객체가 존재하는 지 검사한다. EventLog 객체가 Null이면 ArgumentNullException 을 발생시킨다.
        /// </summary>
        private void CheckEventLog() {
            if(IsOpen == false)
                throw new InvalidOperationException("This EventLog has not been opened or have been closed");
        }

        /// <summary>
        /// EventLog에 항목을 쓴다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="entryType"></param>
        /// <param name="eventID"></param>
        /// <param name="category"></param>
        /// <param name="rawData"></param>
        public void WriteEntry(string message, EventLogEntryType entryType, int eventID, short category, byte[] rawData) {
            Log.WriteEntry(message, entryType, eventID, category, rawData);
        }

        /// <summary>
        /// EventLog에 항목을 쓴다.
        /// </summary>
        /// <param name="message">로그엔트리 정보</param>
        /// <param name="entryType">엔트리 타입</param>
        /// <param name="eventID">이벤트 ID</param>
        /// <param name="category">분류 번호</param>
        public void WriteEntry(string message, EventLogEntryType entryType, int eventID, short category) {
            Log.WriteEntry(message, entryType, eventID, category);
        }

        /// <summary>
        /// EventLog에 항목을 쓴다.
        /// </summary>
        /// <param name="message">로그엔트리 정보</param>
        /// <param name="entryType">엔트리 타입</param>
        /// <param name="eventID">이벤트 ID</param>
        public void WriteEntry(string message, EventLogEntryType entryType, int eventID) {
            Log.WriteEntry(message, entryType, eventID);
        }

        /// <summary>
        /// EventLog에 항목을 쓴다.
        /// </summary>
        /// <param name="message">로그엔트리 정보</param>
        /// <param name="entryType">엔트리 타입</param>
        public void WriteEntry(string message, EventLogEntryType entryType) {
            Log.WriteEntry(message, entryType);
        }

        /// <summary>
        /// Event Log의 모든 항목을 가져온다.
        /// </summary>
        /// <returns>EventLogEntry의 Collection 객체</returns>
        public EventLogEntryCollection GetEntries() {
            return Log.Entries;
        }

        /// <summary>
        /// Event Log에서 모든 항목을 삭제한다.
        /// </summary>
        public void ClearLog() {
            Log.Clear();
        }

        /// <summary>
        /// EventLog 를 닫는다.
        /// </summary>
        public void CloseLog() {
            if(_eventLog != null) {
                _eventLog.Close();
                _eventLog = null;
            }
        }

        /// <summary>
        /// 해당 Log를 삭제하고 로그객체 리소스를 해제한다. 
        /// </summary>
        public void DeleteLog() {
            if(EventLog.SourceExists(Source, MachineName))
                EventLog.DeleteEventSource(Source, MachineName);

            if(!IsSystemDefault) {
                if(EventLog.Exists(LogName, MachineName))
                    EventLog.Delete(LogName, MachineName);
            }

            CloseLog();
        }

        /// <summary>
        /// EventLog 중 생성일 전/후의 Entry를 검색
        /// </summary>
        /// <param name="generatedTime">검색 기준이 되는 생성시간</param>
        /// <param name="isBefore">생성시간 전, 후를 나눔</param>
        /// <returns></returns>
        public IEnumerable<EventLogEntry> FindEntryByTime(DateTime generatedTime, bool isBefore) {
            return EventLogTool.FindEntryByGeneratedTime(Log, generatedTime, isBefore);
        }

        /// <summary>
        /// 지정된 <c>EventLogEntryType</c>(수준)과 같은 <c>EventLogEntry</c>를 검색한다.
        /// </summary>
        /// <param name="entryType">검색할 <c>EventLogEntryType</c>(수준)의 값</param>
        /// <returns>찾은 <c>EventLogEntry</c>의 컬렉션</returns>
        public IEnumerable<EventLogEntry> FindEntryByEntryType(EventLogEntryType entryType) {
            return EventLogTool.FindEntryByEntryType(Log, entryType);
        }

        #region << IDisposable >>

        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// EventLog 닫기
        /// </summary>
        public void Close() {
            Dispose();
        }

        ~EventLogger() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposing
        /// </summary>
        /// <param name="disposing">관리되는 객체를 메모리에서 해제할 것인가</param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                if(_eventLog != null)
                    CloseLog();
            }
        }

        /// <summary>
        /// 인스턴스가 Dispose될 때 호출되는 메소드
        /// </summary>
        public virtual void OnDisposed() {
            if(IsDebugEnabled)
                log.Debug("EventLogger is closed. logName=[{0}]", LogName);
        }

        #endregion
    }
}