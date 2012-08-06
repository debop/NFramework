using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// 64bit OS 용 Registry 사용을 위한 클래스입니다.
    /// </summary>
    /// <example>
    /// <code>
    /// // 64bit registry 중 LOCAL_MACHINE에 해당하는 registry key를 반환한다.
    ///	Registry64 reg64 = Registry64.LocalMachine;
    /// 
    /// // registry로부터 값을 얻는다.
    /// string maxSizeValue = reg64.GetValue( @"SOFTWARE\NSoft\NFramework\Test", "MaxSize");
    /// </code>
    /// </example>
    public class Registry64 : IRegistryReader {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private const int KEY_WOW64_64KEY = 0x100;
        private const int KEY_WOW64_32KEY = 0x200;
        private const int READ_RIGHTS = 131097;

        private readonly IntPtr _rootKey;

        /// <summary>
        /// Initialize a new instance of the <see cref="Registry64"/> class
        /// </summary>
        /// <param name="rootKey">The root HKey</param>
        public Registry64(IntPtr rootKey) {
            _rootKey = rootKey;
        }

        ///<summary>
        /// 레지스트리의 서브키의 name에 해당하는 값을 가져온다. 
        ///</summary>
        ///<param name="subKey">레지스트리 서브키찾</param>
        ///<param name="name">찾고자하는 레지스트리 명</param>
        ///<returns></returns>
        public string GetValue(string subKey, string name) {
            if(IsDebugEnabled)
                log.Debug("레지스트리에서 값을 읽습니다... subKey=[{0}], name=[{1}]", subKey, name);

            var openKey = OpenSubKey(_rootKey, subKey);

            try {
                KeyValueInfo keyValueInfo = GetKeyValueInfo(openKey, name);
                var value = GetKeyValueData(openKey, keyValueInfo);

                if(IsDebugEnabled)
                    log.Debug("레지스트리에서 값을 읽었습니다!!! subKey=[{0}], name=[{1}], value=[{2}]", subKey, name, value);

                return value;
            }
            finally {
                RegCloseKey(openKey);
            }
        }

        private static string GetKeyValueData(IntPtr openKey, KeyValueInfo keyValueName) {
            if(IsDebugEnabled)
                log.Debug("레지스트리 키 값을 읽어옵니다... name=[{0}]", keyValueName.Name);

            var keyValue = new StringBuilder(((int)keyValueName.Length) - 1);

            int resultCode = RegQueryValueEx(openKey,
                                             keyValueName.Name,
                                             0,
                                             out keyValueName.Type,
                                             keyValue,
                                             ref keyValueName.Length);

            if(resultCode != 0)
                ThrowException(resultCode);

            return keyValue.ToString();
        }

        private static KeyValueInfo GetKeyValueInfo(IntPtr openKey, string name) {
            if(IsDebugEnabled)
                log.Debug("레지스트리 키의 정보를 읽어옵니다... name=[{0}]", name);

            uint keyType;
            uint keyValueLength = 0u;
            int resultCode = RegQueryValueEx(openKey, name, 0, out keyType, null, ref keyValueLength);

            if(resultCode != 0)
                ThrowException(resultCode);

            return new KeyValueInfo(keyType, keyValueLength, name);
        }

        private static IntPtr OpenSubKey(IntPtr rootKey, string subKey) {
            if(IsDebugEnabled)
                log.Debug("레지스트리 서브키를 엽니다... subKey=[{0}]", subKey);

            IntPtr openKey;

            int resultCode = RegOpenKeyEx(rootKey, subKey, 0, KEY_WOW64_64KEY | READ_RIGHTS, out openKey);

            if(resultCode == 2)
                resultCode = RegOpenKeyEx(rootKey, subKey, 0, KEY_WOW64_32KEY | READ_RIGHTS, out openKey);

            if(resultCode != 0)
                ThrowException(resultCode);

            if(IsDebugEnabled)
                log.Debug("레지스트리 서브키를 열었습니다!!! subKey=" + subKey);

            return openKey;
        }

        private static void ThrowException(int errorCode) {
            if(log.IsErrorEnabled)
                log.Error("레지스트리 작업 중 예외가 발생했습니다. errorCode=[{0}]", errorCode);

            switch(errorCode) {
                case 2:
                    throw new InvalidOperationException("Error 2: Key or value name not found.");
                case 3:
                    throw new InvalidOperationException("Error 3: Path not found.");
                case 5:
                    throw new InvalidOperationException("Error 5: Access is denied.");
                case 6:
                    throw new InvalidOperationException("Error 6: Invalid handle");
                case 9:
                    throw new InvalidOperationException("Error 9: Invalid block");
                case 12:
                    throw new InvalidOperationException("Error 12: Invalid Access");
                default:
                    throw new InvalidOperationException("Error " + errorCode +
                                                        ". Please refer to msdn documention on Winerror.h for further information.");
            }
        }

        /// <summary>
        /// 64bit OS Registry의 HKEY_LOCAL_MACHINE의 값
        /// </summary>
        private static readonly IntPtr HKEY_LOCAL_MACHINE = new IntPtr(-2147483646);

        /// <summary>
        /// 64bit OS Registry의 LocalMachine
        /// </summary>
        public static IRegistryReader LocalMachine {
            get {
                if(IsDebugEnabled)
                    log.Debug("64bit OS용 Registry의 LocalMachine 에 해당하는 Registry Key를 생성합니다.");

                return new Registry64(HKEY_LOCAL_MACHINE);
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern int RegCloseKey(IntPtr hKey);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegOpenKeyEx")]
        private static extern int RegOpenKeyEx(IntPtr hKey, string subKey, uint options, int sam, out IntPtr phkResult);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegQueryValueExW", SetLastError = true)]
        private static extern int RegQueryValueEx(IntPtr hKey, string lpValueName, int lpReserved, out uint lpType, StringBuilder lpData,
                                                  ref uint lpcbData);

        [Serializable]
        private class KeyValueInfo {
            public uint Length;
            public readonly string Name;

#pragma warning disable 219
            public uint Type;
#pragma warning restore 219

            public KeyValueInfo(uint type, uint length, string name) {
                Type = type;
                Length = length;
                Name = name;
            }
        }
    }
}