using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace NSoft.NFramework.Diagnostics {
    public static class ProcessTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 시스템에서 실행중인 모든 프로세스에 대해, 사용하지 않는 WorkingSet 메모리를 OS에게 반환하도록 합니다.
        /// </summary>
        /// <param name="excludeThisProcess">현재 프로세스를 제외할 것인가 여부 (기본값은 제외)</param>
        /// <param name="excludeProcessNames">메모리 반환을 하지 않을 프로세스 명의 컬렉션</param>
        public static void TrimAllProcessMemory(bool excludeThisProcess = true, string[] excludeProcessNames = null) {
            if(log.IsInfoEnabled)
                log.Info("컴퓨터의 모든 프로세스에 대해 사용하지 않는 메모리를 OS에 반환하도록 합니다...");

            var currentProcess = Process.GetCurrentProcess();

            Parallel.ForEach(Process.GetProcesses(),
                             process => {
                                 if(excludeThisProcess && (process.ProcessName == currentProcess.ProcessName))
                                     return;

                                 if(excludeProcessNames != null &&
                                    excludeProcessNames.Any(procName => procName == process.ProcessName))
                                     return;

                                 TrimProcessMemory(process);
                             });
        }

        /// <summary>
        /// 지정된 프로세스의 사용하지 않는 WorkingSet 메모리를 OS에게 반환하도록 합니다.
        /// </summary>
        /// <param name="process">메모리 해제를 할 프로세스</param>
        /// <returns>메모리 해제 여부</returns>
        public static bool TrimProcessMemory(Process process) {
            if(IsDebugEnabled)
                log.Debug("프로세스의 Working Memory 중에 사용하지 않는 부분을 OS에 반환하도록 합니다.");

            if(process == null)
                return false;

            bool _result;

            try {
                long oldWorkingSet64 = process.WorkingSet64;
                _result = EmptyWorkingSet((long)process.Handle);

                var targetProcess = Process.GetProcessById(process.Id);

                if(_result) {
                    if(log.IsInfoEnabled)
                        log.Info("프로세스[{0}]의 WorkingSet 메모리를 비웠습니다. 기존=[{1}], 현재=[{2}], 반환값=[{3}]",
                                 process.ProcessName, oldWorkingSet64, targetProcess.WorkingSet64,
                                 oldWorkingSet64 - targetProcess.WorkingSet64);
                }
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("Process[{0}]의 메모리를 정리하는데 예외가 발생했습니다...", process.ProcessName);
                    log.Warn(ex);
                }
                _result = false;
            }

            return _result;
        }

        [DllImport("psapi")]
        private static extern bool EmptyWorkingSet(long hProcess);
    }
}