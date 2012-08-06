using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NSoft.NFramework {
    /// <summary>
    /// Stack Trace 정보를 가져오는 Helper Class
    /// </summary>
    public static class StackTracer {
        internal const string STACK_TRACE_CALL = "System.Environment.get_StackTrace()";
        private static readonly Regex StackRegex = new Regex(@"\s.*(\sin\s.*:line\s\d*)?", RegexOptions.Compiled);

        private static readonly object _syncLock = new object();

        /// <summary>
        /// 이 함수 호출전까지의 StackTrace 정보를 제공한다.
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentStackTraceInfo() {
            lock(_syncLock)
                return GetStackTraceInfo(Environment.StackTrace);
        }

        /// <summary>
        /// 지정된 StackTrace 정보중에 StackTrace 정보를 얻기 위한 작업을 제거한
        /// 실제 코드상에서 일어난 정보만을 추려서 반환한다.
        /// </summary>
        /// <param name="currentStackTrace"></param>
        /// <returns></returns>
        public static string GetStackTraceInfo(string currentStackTrace) {
            int postOfStackTraceCall = currentStackTrace.IndexOf(STACK_TRACE_CALL, StringComparison.Ordinal);
            return (currentStackTrace.Substring(postOfStackTraceCall + STACK_TRACE_CALL.Length));
        }

        /// <summary>
        /// 현재 StackTrace 정보의 갯수를 반환한다.
        /// </summary>
        /// <returns></returns>
        public static int GetCurrentStackTraceDepth() {
            //lock(_syncLock)
            return GetStackTraceDepth(Environment.StackTrace);
        }

        /// <summary>
        /// 현재 StackTrace 정보의 갯수를 반환한다.
        /// </summary>
        /// <param name="currentStackTrace"></param>
        /// <returns></returns>
        public static int GetStackTraceDepth(string currentStackTrace) {
            var stackTraceInfo = GetStackTraceInfo(currentStackTrace);
            var methodCallMatches = StackRegex.Matches(stackTraceInfo);

            return methodCallMatches.Count;
        }

        /// <summary>
        /// 현재 StackTrace 정보중에 Method 호출한 부분만을 추출하여 문자열 배열로 반환한다.
        /// </summary>
        /// <returns></returns>
        public static ICollection<string> GetCurrentStackTraces() {
            //lock(_syncLock)
            return GetStackTraces(Environment.StackTrace);
        }

        /// <summary>
        /// StackTrace 정보중에 Method 호출한 부분만을 추출하여 문자열 배열로 반환한다.
        /// </summary>
        /// <param name="currentStackTrace"></param>
        /// <returns></returns>
        public static ICollection<string> GetStackTraces(string currentStackTrace) {
            string stackTraceInfo = GetStackTraceInfo(currentStackTrace);
            var methodCallMatches = StackRegex.Matches(stackTraceInfo);

            // return list.ToArray();
            return (methodCallMatches.Cast<Match>().Select(m => m.Value.Trim('\r', '\n'))).ToList();
        }
    }
}