using Microsoft.Win32;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// 32bit OS 용 Registry 사용을 위한 클래스입니다.
    /// </summary>
    public class Registry32 : IRegistryReader {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly RegistryKey _rootKey;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rootKey"></param>
        public Registry32(RegistryKey rootKey) {
            _rootKey = rootKey;
        }

        /// <summary>
        /// 32bit OS Registry의 LocalMachine
        /// </summary>
        public static IRegistryReader LocalMachine {
            get { return new Registry32(Microsoft.Win32.Registry.LocalMachine); }
        }

        /// <summary>
        /// 지정된 서브키의 레지스트리 명에 해당하는 값을 가져온다. 없으면 null을 반환한다.
        /// </summary>
        /// <param name="subKey">레지스트리 서브 키</param>
        /// <param name="name">레지스트리 명</param>
        /// <returns>레지스트리 값, 없으면 null을 반환한다.</returns>
        public string GetValue(string subKey, string name) {
            if(IsDebugEnabled)
                log.Debug("레지스트리 키[{0}]의 Name[{1}]의 값을 얻습니다.", subKey, name);

            using(var client = new RegistryClient(_rootKey, subKey)) {
                var value = client.GetValue(name);

                if(IsDebugEnabled)
                    log.Debug("레지스트리에서 값을 읽었습니다!!! subKey=[{0}], name=[{1}], value=[{2}]", subKey, name, value);

                return (value != null) ? value.ToString() : null;
            }
        }
    }
}