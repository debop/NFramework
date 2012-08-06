using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// <c>RwEventLog</c>를 이용하여, Windows Event Log를 쉽게 사용할 수 있는 Utility Class 입니다.
    /// </summary>
    /// <example>
    ///     <code>
    ///     // Windows EventLog에 정보를 쓴다. 
    ///     EventLogTool.WriteEntry("Application", "NSoft.NFramework.Test", "Information : EventLogTool Test", EventLogEntryType.Information);
    ///     </code>
    /// </example>
    public static class EventLogTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// <paramref name="logName">로그 이름</paramref>에 해당하는 <see href="EventLog"/> 인스턴스를 반환한다.
        /// </summary>
        /// <param name="logName"></param>
        /// <returns>EventLog 인스턴스</returns>
        public static EventLog GetEventLog(string logName) {
            logName.ShouldNotBeWhiteSpace("logName");

            string source = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
            return GetEventLog(logName, source);
        }

        /// <summary>
        /// 해당 EventLog를 반환한다. Event Source가 없다면 생성한다.
        /// </summary>
        /// <param name="logName">로그 이름</param>
        /// <param name="source">이벤트 로그 엔트리 소스</param>
        /// <returns>해당 EventLog Instance</returns>
        public static EventLog GetEventLog(string logName, string source) {
            logName.ShouldNotBeWhiteSpace("logName");

            return GetEventLog(logName, source, EventLogger.DEFAULT_MACHINE_NAME);
        }

        /// <summary>
        /// 해당 EventLog를 반환한다. Event Source가 없다면 생성한다.
        /// </summary>
        /// <param name="logName">로그 이름</param>
        /// <param name="source">엔트리 소스</param>
        /// <param name="machineName">로그가 있는 컴퓨터</param>
        /// <returns></returns>
        public static EventLog GetEventLog(string logName, string source, string machineName) {
            logName.ShouldNotBeWhiteSpace("logName");

            if(EventLog.SourceExists(source, machineName) == false) {
                var sourceData = new EventSourceCreationData(source, logName)
                                 {
                                     MachineName = machineName
                                 };
                EventLog.CreateEventSource(sourceData);
            }

            return new EventLog(logName, machineName, source)
                   {
                       EnableRaisingEvents = true
                   };
        }

        /// <summary>
        /// 이벤트 로그를 쓴다.
        /// </summary>
        /// <param name="logName">이벤트 로그 명</param>
        /// <param name="source">원본</param>
        /// <param name="message">이벤트 메시지</param>
        /// <param name="type">이벤트 수준(경고, 정보 등)</param>
        /// <param name="eventID">이벤트 ID</param>
        /// <param name="category">이벤트 분류</param>
        /// <param name="rawData"></param>
        public static void WriteEntry(string logName, string source, string message, EventLogEntryType type, int eventID, short category,
                                      byte[] rawData) {
            logName.ShouldNotBeWhiteSpace("logName");

            using(var eventLogger = new EventLogger(logName, EventLogger.DEFAULT_MACHINE_NAME, source)) {
                eventLogger.WriteEntry(message, type, eventID, category, rawData);
            }
        }

        /// <summary>
        /// 이벤트 로그를 쓴다.
        /// </summary>
        /// <param name="logName">이벤트 로그 명</param>
        /// <param name="source">원본</param>
        /// <param name="message">이벤트 메시지</param>
        /// <param name="type">이벤트 수준(경고, 정보 등)</param>
        /// <param name="eventID">이벤트 ID</param>
        /// <param name="category">이벤트 분류</param>
        public static void WriteEntry(string logName, string source, string message, EventLogEntryType type, int eventID, short category) {
            logName.ShouldNotBeWhiteSpace("logName");

            using(var eventLogger = new EventLogger(logName, EventLogger.DEFAULT_MACHINE_NAME, source)) {
                eventLogger.WriteEntry(message, type, eventID, category);
            }
        }

        /// <summary>
        /// 이벤트 로그를 쓴다.
        /// </summary>
        /// <param name="logName">이벤트 로그 명</param>
        /// <param name="source">원본</param>
        /// <param name="message">이벤트 메시지</param>
        /// <param name="type">이벤트 수준(경고, 정보 등)</param>
        /// <param name="eventID">이벤트 ID</param>
        public static void WriteEntry(string logName, string source, string message, EventLogEntryType type, int eventID) {
            logName.ShouldNotBeWhiteSpace("logName");

            using(var eventLogger = new EventLogger(logName, EventLogger.DEFAULT_MACHINE_NAME, source)) {
                eventLogger.WriteEntry(message, type, eventID);
            }
        }

        /// <summary>
        /// 이벤트 로그를 쓴다.
        /// </summary>
        /// <param name="logName">이벤트 로그 명</param>
        /// <param name="source">원본</param>
        /// <param name="message">이벤트 메시지</param>
        /// <param name="type">이벤트 수준(경고, 정보 등)</param>
        public static void WriteEntry(string logName, string source, string message, EventLogEntryType type) {
            logName.ShouldNotBeWhiteSpace("logName");

            using(var eventLogger = new EventLogger(logName, EventLogger.DEFAULT_MACHINE_NAME, source)) {
                eventLogger.WriteEntry(message, type);
            }
        }

        /// <summary>
        /// 이벤트 로그를 쓴다.
        /// </summary>
        /// <param name="message">이벤트 메시지</param>
        /// <param name="type">이벤트 수준(경고, 정보 등)</param>
        public static void WriteEntry(string message, EventLogEntryType type) {
            using(var eventLogger = new EventLogger()) {
                eventLogger.WriteEntry(message, type);
            }
        }

        /// <summary>
        /// EventLogEntry의 생성 시간을 기준으로 검색한다.
        /// </summary>
        /// <param name="eventLog">대성 EventLog 인스턴스</param>
        /// <param name="generatedTime">검색할 기준 시간</param>
        /// <param name="isBefore">기준시간 전/후를 판단하는 옵션</param>
        /// <returns>검색된 로그 엔트리의 컬렉션</returns>
        /// <returns></returns>
        public static IEnumerable<EventLogEntry> FindEntryByGeneratedTime(EventLog eventLog, DateTime generatedTime, bool isBefore) {
            eventLog.ShouldNotBeNull("eventLog");

            foreach(EventLogEntry entry in eventLog.Entries) {
                if((isBefore && entry.TimeGenerated <= generatedTime) ||
                   (!isBefore && entry.TimeGenerated >= generatedTime))
                    yield return entry;
            }
        }

        /// <summary>
        /// EventLogEntry의 생성 시간을 기준으로 검색한다.
        /// </summary>
        /// <param name="logName">로그 엔트리 목록 (예: Application, System)</param>
        /// <param name="source">로그 소스</param>
        /// <param name="generatedTime">검색할 기준 시간</param>
        /// <param name="isBefore">기준시간 전/후를 판단하는 옵션</param>
        /// <returns>검색된 로그 엔트리의 컬렉션</returns>
        public static IEnumerable<EventLogEntry> FindEntryByGeneratedTime(string logName, string source, DateTime generatedTime,
                                                                          bool isBefore) {
            logName.ShouldNotBeWhiteSpace("logName");

            using(var eventLog = GetEventLog(logName, source)) {
                return FindEntryByGeneratedTime(eventLog, generatedTime, isBefore);
            }
        }

        /// <summary>
        /// <c>EventLogEntry</c>의 원본 이름으로 찾기
        /// </summary>
        /// <param name="eventLog">검색 대상 <c>EventLog</c></param>
        /// <param name="source">검색할 <c>EventLogEntry</c>의 원본 이름</param>
        /// <param name="ignoreCase">대소문자 구분 여부</param>
        /// <returns>찾은 <c>EventLogEntry</c>의 컬렉션</returns>
        public static IEnumerable<EventLogEntry> FindEntryBySource(EventLog eventLog, string source, bool ignoreCase) {
            eventLog.ShouldNotBeNull("eventLog");

            return
                eventLog.Entries
                    .Cast<EventLogEntry>()
                    .Where(entry => source.EqualTo(entry.Source));
        }

        /// <summary>
        /// <c>EventLogEntry</c>의 원본 이름으로 찾기
        /// </summary>
        /// <param name="logName"><c>EventLog</c>의 이름</param>
        /// <param name="source">검색할 <c>EventLogEntry</c>의 원본 이름</param>
        /// <param name="ignoreCase">대소문자 구분 여부</param>
        /// <returns>찾은 <c>EventLogEntry</c>의 컬렉션</returns>
        public static IEnumerable<EventLogEntry> FindEntryBySource(string logName, string source, bool ignoreCase) {
            logName.ShouldNotBeWhiteSpace("logName");

            using(EventLog eventLog = GetEventLog(logName, source))
                return FindEntryBySource(eventLog, source, ignoreCase);
        }

        /// <summary>
        /// 지정된 <c>EventLogEntryType</c>(수준)과 같은 <c>EventLogEntry</c>를 검색한다.
        /// </summary>
        /// <param name="eventLog">대상 <c>EventLog</c> 인스턴스</param>
        /// <param name="entryType">검색할 <c>EventLogEntryType</c>(수준)의 값</param>
        /// <returns>찾은 <c>EventLogEntry</c>의 컬렉션</returns>
        public static IEnumerable<EventLogEntry> FindEntryByEntryType(EventLog eventLog, EventLogEntryType entryType) {
            eventLog.ShouldNotBeNull("eventLog");

            return
                eventLog.Entries
                    .Cast<EventLogEntry>()
                    .Where(entry => entry.EntryType == entryType);
        }

        /// <summary>
        /// 지정된 <c>EventLogEntryType</c>(수준)과 같은 <c>EventLogEntry</c>를 검색한다.
        /// </summary>
        /// <param name="logName"><c>EventLog</c>의 이름</param>
        /// <param name="source">검색할 <c>EventLogEntry</c>의 원본 이름</param>
        /// <param name="entryType">검색할 <c>EventLogEntryType</c>(수준)의 값</param>
        /// <returns>찾은 <c>EventLogEntry</c>의 컬렉션</returns>
        public static IEnumerable<EventLogEntry> FindEntryByEntryType(string logName, string source, EventLogEntryType entryType) {
            logName.ShouldNotBeWhiteSpace("logName");

            using(EventLog eventLog = GetEventLog(logName, source))
                return FindEntryByEntryType(eventLog, entryType);
        }

        /// <summary>
        /// 이벤트 로그 크기를 조절한다.
        /// </summary>
        /// <param name="logName">이벤트 로그 이름</param>
        /// <param name="maxSize">Log 크기, KByte 단위</param>
        /// <remarks>레지스트리를 조작할 수 있는 권한이 있어야 한다.</remarks>
        public static void SetLogMaxSize(string logName, int maxSize) {
            logName.ShouldNotBeWhiteSpace("logName");

            var subKey = @"SYSTEM\CurrentControlSet\Services\EventLog\" + logName;

            if(IsDebugEnabled)
                log.Debug("Set Maximum size of Log. logName=[{0}], maxSize=[{1}], subKey=[{2}]", logName, maxSize, subKey);

            using(var reg = new RegistryClient(Microsoft.Win32.Registry.LocalMachine)) {
                reg.SubKeyName = subKey;
                reg.SetValue("MaxSize", maxSize);
            }
        }
    }
}