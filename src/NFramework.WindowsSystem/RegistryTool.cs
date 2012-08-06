using System;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// Registry 접근을 위한 Utility Class 입니다.
    /// </summary>
    public static class RegistryTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 레지스트리 Local Machine에서 값을 읽어옵니다.
        /// </summary>
        /// <param name="subKey"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetLocalMachineValue(string subKey, string name) {
            if(IsDebugEnabled)
                log.Debug("레지스트리에서 값을 읽어옵니다. subKey=[{0}], name=[{1}], Is64BitOS=[{2}]", subKey, name, Is64BitOS);

            if(Is64BitOS)
                return Registry64.LocalMachine.GetValue(subKey, name);

            return Registry32.LocalMachine.GetValue(subKey, name);
        }

        /// <summary>
        /// 64bit OS인가 검사한다.
        /// </summary>
        public static bool Is64BitOS {
            get { return (IntPtr.Size == 8); }
        }
    }
}