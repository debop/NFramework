using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Win32;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// Windows Registry 를 조작하는 Class
    /// </summary>
    public class RegistryClient : IDisposable {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Registry Root Key
        /// </summary>
        private RegistryKey _rootKey;

        /// <summary>
        /// Sub Key name
        /// </summary>
        private string _subKeyName;

        /// <summary>
        /// Registry Root Key : Microsoft.Win32.Registry.LocalMachine 이 기본이다.
        /// </summary>
        public virtual RegistryKey RootKey {
            get { return _rootKey; }
            set {
                value.ShouldNotBeNull("RootKey");
                _rootKey = value;
            }
        }

        /// <summary>
        /// Registry의 이용하고자 하는 SubKey Name
        /// </summary>
        public virtual string SubKeyName {
            get { return _subKeyName; }
            set {
                value.ShouldNotBeWhiteSpace("SubKeyName");

                // 앞뒤의 Backslash를 없앤다.
                _subKeyName = value.Trim('\\');
            }
        }

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public RegistryClient() : this(Registry.LocalMachine) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rootKey">레지스트리 키</param>
        public RegistryClient(RegistryKey rootKey) : this(rootKey, string.Empty) {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="rootKey">레지스트리 키</param>
        /// <param name="subKeyName">서브 키 이름</param>
        public RegistryClient(RegistryKey rootKey, string subKeyName) {
            _rootKey = rootKey;
            _subKeyName = subKeyName;

            if(IsDebugEnabled)
                log.Debug("Create RegistryClient with rootKey=[{0}], subKeyName=[{1}]", rootKey, subKeyName);
        }

        #region << IDisposable >>

        public bool IsDisposed { get; protected set; }

        /// <summary>
        /// Registry 사용을 마친다.
        /// </summary>
        public virtual void Close() {
            Dispose();
        }

        ~RegistryClient() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 실제 Disposing하는 메소드
        /// </summary>
        /// <param name="disposing">관리되는 객체를 Disposing할 것인가.</param>
        protected virtual void Dispose(bool disposing) {
            if(IsDisposed)
                return;

            if(disposing) {
                With.TryAction(() => {
                                   if(_rootKey != null) {
                                       if(IsDebugEnabled)
                                           log.Debug("Registry which rootkey=[{0}] is CLOSED.", _rootKey);

                                       _rootKey.Close();
                                       _rootKey = null;
                                   }
                               });
            }
        }

        /// <summary>
        /// 인스턴스가 Dispose 되면 호출된다.
        /// </summary>
        public void OnDisposed() {
            if(IsDebugEnabled)
                log.Debug("RegistryClient is closed.");
        }

        #endregion

        /// <summary>
        /// RootKey에서 새로운 서브키를 생성한다.
        /// </summary>
        /// <param name="subKeyName">경로</param>
        /// <returns>생성된 레지스트리 키 인스턴스, 사용후 꼭 Close 를 해야 한다.</returns>
        protected RegistryKey CreateSubKey(string subKeyName) {
            if(IsDebugEnabled)
                log.Debug("Create Sub key. subKeyName=[{0}]", subKeyName);

            return _rootKey.CreateSubKey(subKeyName);
        }

        /// <summary>
        /// 현재 열려있는 서브키에서 지정된 이름의 값을 읽어온다.
        /// </summary>
        /// <param name="name">이름</param>
        /// <returns>실패시에는 null 값 반환</returns>
        public object GetValue(string name) {
            return GetValue(_subKeyName, name);
        }

        /// <summary>
        /// 지정된 서브키에서 지정된 이름의 값을 읽어온다.
        /// </summary>
        /// <param name="subKeyName">레지스트리 키 명</param>
        /// <param name="name">이름</param>
        /// <returns>실패시에는 null 값 반환</returns>
        public object GetValue(string subKeyName, string name) {
            if(IsDebugEnabled)
                log.Debug("레지스트리에서 값을 읽습니다... subKeyName=[{0}], name=[{1}]", subKeyName, name);

            object value = null;

            if(subKeyName.IsNotWhiteSpace()) {
                using(RegistryKey subKey = RootKey.CreateSubKey(subKeyName)) {
                    if(subKey != null)
                        value = subKey.GetValue(name);
                }
            }

            if(IsDebugEnabled)
                log.Debug("레지스트리에서 값을 읽었습니다!!! subKeyName=[{0}], name=[{1}], value=[{2}]", subKeyName, name, value);

            return value;
        }

        /// <summary>
        /// 현재 서브 키에서 지정된 이름의 값을 가져온다. 실패시에는 지정된 기본값을 반환한다.
        /// </summary>
        /// <typeparam name="T">값의 타입</typeparam>
        /// <param name="name">이름</param>
        /// <param name="defaultValue">기본값</param>
        /// <returns>실패시에는 defaultValue를 반환한다.</returns>
        public T GetValueDef<T>(string name, T defaultValue) {
            return GetValueDef(_subKeyName, name, defaultValue);
        }

        /// <summary>
        /// 지정된 서브키에서 지정된 이름의 값을 가져온다. 실패시에는 지정된 기본값을 반환한다.
        /// </summary>
        /// <typeparam name="T">값의 타입</typeparam>
        /// <param name="subKeyName">서브 키 이름</param>
        /// <param name="name">이름</param>
        /// <param name="defaultValue">기본값</param>
        /// <returns>실패시에는 defaultValue를 반환한다.</returns>
        public T GetValueDef<T>(string subKeyName, string name, T defaultValue) {
            return GetValue(subKeyName, name).AsValue(defaultValue);
            // return ConvertTool.DefValue(GetValue(subKeyName, name), defaultValue);
        }

        /// <summary>
        /// 현재 열려진 서브 키에 있는 모든 이름-값 을 가져온다.
        /// </summary>
        /// <returns><c>System.Collections.Specialized.HybridDictionary</c> 객체를 반환한다.</returns>
        public IDictionary GetValues() {
            return GetValues(_subKeyName);
        }

        /// <summary>
        /// 지정된 서브 키에 있는 모든 이름-값 을 가져온다.
        /// </summary>
        /// <param name="subKeyName">서브 키 이름</param>
        /// <returns><c>System.Collections.Specialized.HybridDictionary</c> 객체를 반환한다.</returns>
        public IDictionary GetValues(string subKeyName) {
            return GetValues(subKeyName, null);
        }

        /// <summary>
        /// 지정된 서브 키에 있는 모든 이름-값 을 가져온다.
        /// </summary>
        /// <param name="subKeyName">서브 키 이름</param>
        /// <param name="defaultValue">기본 값</param>
        /// <returns><c>System.Collections.Specialized.HybridDictionary</c> 객체를 반환한다.</returns>
        public IDictionary GetValues(string subKeyName, object defaultValue) {
            if(IsDebugEnabled)
                log.Debug("지정된 서브 키에 있는 모든 Key-Value Dictionary를 가져옵니다... subKeyName=[{0}], defaultValue=[{1}]",
                          subKeyName, defaultValue);

            var dic = new HybridDictionary();

            if(subKeyName.IsNotEmpty()) {
                using(RegistryKey key = CreateSubKey(subKeyName)) {
                    foreach(string name in key.GetValueNames())
                        dic.Add(name, key.GetValue(name, defaultValue));
                }
            }

            return dic;
        }

        /// <summary>
        /// 지정된 RootKey/SubKeyName에 있는 name 값에 data를 설정한다.
        /// </summary>
        /// <param name="name">이름</param>
        /// <param name="value">데이타 값</param>
        /// <example>
        ///	<code>
        ///		using( DisposableRegistry reg = new DisposableRegistry(Registry.LocalMachine, REG_KEY_SUBKEY) )	
        ///		{
        ///			reg.SetValue(REG_VALUE_NAME1, 34);
        ///			reg.SetValue(REG_VALUE_NAME2, "RealWeb.Common.Win32.DisposableRegistry, RealWeb.Common");
        ///			reg.SetValue(REG_VALUE_NAME3, "가나다라마바사아");
        ///
        ///			Console.WriteLine(REG_VALUE_NAME1 + " = " + reg.GetValue(REG_VALUE_NAME1, 0).ToString());
        ///			Console.WriteLine(REG_VALUE_NAME2 + " = " + reg.GetValue(REG_VALUE_NAME2, "").ToString());
        ///			Console.WriteLine(REG_VALUE_NAME3 + " = " + reg.GetValue(REG_VALUE_NAME3, "").ToString());
        ///		}
        ///	</code>
        /// </example>
        public void SetValue(string name, object value) {
            SetValue(_subKeyName, name, value);
        }

        /// <summary>
        /// 지정된 RootKey/subKeyName에 있는 name 값에 data를 설정한다.
        /// </summary>
        /// <param name="subKeyName">Registry Key</param>
        /// <param name="name">Registry name</param>
        /// <param name="value">Registry value</param>
        /// <example>
        ///	<code>
        ///		using( DisposableRegistry reg = new DisposableRegistry(Registry.LocalMachine, REG_KEY_SUBKEY) )	
        ///		{
        ///			reg.SetValue(REG_VALUE_NAME1, 34);
        ///			reg.SetValue(REG_VALUE_NAME2, "RealWeb.Common.Win32.DisposableRegistry, RealWeb.Common");
        ///			reg.SetValue(REG_VALUE_NAME3, "가나다라마바사아");
        ///
        ///			Console.WriteLine(REG_VALUE_NAME1 + " = " + reg.GetValue(REG_VALUE_NAME1, 0).ToString());
        ///			Console.WriteLine(REG_VALUE_NAME2 + " = " + reg.GetValue(REG_VALUE_NAME2, "").ToString());
        ///			Console.WriteLine(REG_VALUE_NAME3 + " = " + reg.GetValue(REG_VALUE_NAME3, "").ToString());
        ///		}
        ///	</code>
        /// </example>
        public void SetValue(string subKeyName, string name, object value) {
            subKeyName.ShouldNotBeWhiteSpace("subKeyName");

            if(IsDebugEnabled)
                log.Debug("레지스트리에 값을 설정합니다. subKeyName=[{0}], name=[{1}], value=[{2}]", subKeyName, name, value);

            using(RegistryKey key = CreateSubKey(subKeyName)) {
                if(key != null)
                    key.SetValue(name, value);
            }
        }

        /// <summary>
        /// RootKey/SubkeyName 에 Collection에 있는 이름-값 쌍을 넣는다.
        /// </summary>
        /// <param name="dictionary"><c>IDictionary</c>를 구현한 객체</param>
        /// <example>
        ///	<code>
        ///		Hashtable hash = new Hashtabhe();
        ///		for(int i=0; i &lt; 5; i++)
        ///		{
        ///			string val = "Value " + i.ToString();
        ///			hash.Add("Name " + i.ToString(), val);
        ///		}
        ///		
        ///		using(DisposableRegistry reg = new DisposableRegistry(Microsoft.Win32.Registry.LocalMachine))
        ///		{
        ///			reg.SubKeyName = @"SOFTWARE\NSoft\Common\Test";
        ///			
        ///			reg.SetValues(hash);
        ///		}
        ///	</code>
        /// </example>
        public void SetValues(IDictionary dictionary) {
            SetValues(_subKeyName, dictionary);
        }

        /// <summary>
        /// 지정된 RootKey/subkeyName 에 Collection에 있는 이름-Data값 쌍을 저장한다.
        /// </summary>
        /// <param name="subKeyName">서브 키 이름</param>
        /// <param name="nameValues">저장할 정보 객체 <c>IDictionary</c> 형태여아 한다.</param>
        public void SetValues(string subKeyName, IDictionary nameValues) {
            subKeyName.ShouldNotBeWhiteSpace("subKeyName");

            if(nameValues == null)
                return;

            if(IsDebugEnabled)
                log.Debug("레지스트리 서브키에 Name-Value 쌍의 Dictionary를 모두 저장합니다... subKeyName=[{0}], nameValues=[{1}]",
                          subKeyName, nameValues.DictionaryToString());

            if(nameValues.Count == 0)
                return;

            using(RegistryKey key = CreateSubKey(subKeyName)) {
                if(key != null) {
                    foreach(DictionaryEntry de in nameValues)
                        key.SetValue(de.Key.ToString(), de.Value);
                }
            }
        }

        /// <summary>
        /// 현재 지정된 서브키를 삭제한다.
        /// </summary>
        public void DeleteSubKey() {
            DeleteSubKey(_subKeyName);
        }

        /// <summary>
        /// 현재 지정된 서브키 및 하위의 모든 서브 키들을 재귀적으로 삭제한다.
        /// </summary>
        /// <param name="deep">서브키 밑의 모든 하위 서브 키를 삭제할 지 여부</param>
        public void DeleteSubKey(bool deep) {
            DeleteSubKey(_subKeyName, deep);
        }

        /// <summary>
        /// 지정된 서브 키를 삭제한다.
        /// </summary>
        /// <param name="subKeyName"></param>
        public void DeleteSubKey(string subKeyName) {
            DeleteSubKey(subKeyName, false);
        }

        /// <summary>
        /// 지정된 서브키와 자식 서브키들을 삭제한다.
        /// </summary>
        /// <param name="subKeyName">서브 키 이름</param>
        /// <param name="deep">자식 하위키를 재귀적으로 삭제할지 여부</param>
        public void DeleteSubKey(string subKeyName, bool deep) {
            subKeyName.ShouldNotBeWhiteSpace("subKeyName");

            if(IsDebugEnabled)
                log.Debug("지정한 서브키를 삭제합니다... subKeyName=[{0}], deep=[{1}]", subKeyName, deep);

            using(RegistryKey key = CreateSubKey(subKeyName)) {
                if(deep)
                    key.DeleteSubKeyTree(string.Empty);
                else
                    key.DeleteSubKey(string.Empty, false);
            }

            if(IsDebugEnabled)
                log.Debug("지정한 서브키를 삭제했습니다!!! subKeyName=[{0}], deep=[{1}]", subKeyName, deep);
        }

        /// <summary>
        /// 현재 열려진 서브키에서 지정된 이름의 값을 제거한다.
        /// </summary>
        /// <param name="name">이름</param>
        public void DeleteValue(string name) {
            DeleteValue(_subKeyName, name);
        }

        /// <summary>
        /// 지정된 하위키의 지정된 이름의 값을 제거한다.
        /// </summary>
        /// <param name="subKeyName"></param>
        /// <param name="name"></param>
        public void DeleteValue(string subKeyName, string name) {
            subKeyName.ShouldNotBeWhiteSpace("subKeyName");

            if(IsDebugEnabled)
                log.Debug("레지스트리의 이름을 삭제합니다... subKeyName=[{0}], name=[{1}]", subKeyName, name);

            using(RegistryKey key = CreateSubKey(subKeyName)) {
                key.DeleteValue(name, false);
            }
        }

        /// <summary>
        /// 현재 열려진 서브키에서 해당되는 이름들의 값을 삭제한다.
        /// </summary>
        /// <param name="names"></param>
        public void DeleteValues(params string[] names) {
            DeleteValues(_subKeyName, names);
        }

        /// <summary>
        /// 지정된 서브키에서 해당되는 이름의 값을 삭제한다.
        /// </summary>
        /// <param name="subKeyName"></param>
        /// <param name="names"></param>
        public void DeleteValues(string subKeyName, params string[] names) {
            subKeyName.ShouldNotBeWhiteSpace("subKeyName");

            if(names == null || names.Length == 0)
                return;

            if(IsDebugEnabled)
                log.Debug("해당 서브키의 여러 이름을 한꺼번에 삭제합니다. subKeyName=[{0}], names=[{1}]", subKeyName, names.CollectionToString());

            using(var key = CreateSubKey(subKeyName)) {
                foreach(string name in names)
                    key.DeleteValue(name, false);
            }
        }

        /// <summary>
        /// 서브키를 액세스 할 수 있는지 여부를 검사한다.
        /// </summary>
        /// <param name="writable">true이면 읽기/쓰기여부, false이면 읽기 가능한지 여부</param>
        /// <returns></returns>
        public bool CanAccess(bool writable) {
            return CanAccess(_subKeyName, writable);
        }

        /// <summary>
        /// 지정된 서브키를 액세스 할 수 있는지 여부를 검사한다.
        /// </summary>
        /// <param name="subKeyName"></param>
        /// <param name="writable">true이면 읽기/쓰기여부, false이면 읽기 가능한지 여부</param>
        /// <returns></returns>
        public bool CanAccess(string subKeyName, bool writable) {
            subKeyName.ShouldNotBeWhiteSpace("subKeyName");

            if(IsDebugEnabled)
                log.Debug("지정된 서브키를 액세스 할 수 있는지 여부를 검사한다... subKeyName=[{0}], writable=[{1}]",
                          subKeyName, writable);

            using(RegistryKey key = _rootKey.OpenSubKey(subKeyName, writable)) {
                if(IsDebugEnabled)
                    log.Debug("Access to The sub key is [{0}]", (key != null));

                return (key != null);
            }
        }

        /// <summary>
        /// 서브키를 지정된 읽기 모드로 연다.
        /// </summary>
        /// <returns>레지스트리 키 객체</returns>
        public RegistryKey OpenSubKey() {
            return OpenSubKey(_subKeyName, false);
        }

        /// <summary>
        /// 서브키를 지정된 접근 모드 (읽기, 읽기/쓰기) 로 연다.
        /// </summary>
        /// <param name="writable">true이면 읽기/쓰기여부, false이면 읽기 가능한지 여부</param>
        /// <returns>레지스트리 키 객체</returns>
        public RegistryKey OpenSubKey(bool writable) {
            return OpenSubKey(_subKeyName, writable);
        }

        /// <summary>
        /// 지정된 서브키를 읽기모드로 연다.
        /// </summary>
        /// <param name="subKeyName">서브 키 이름</param>
        /// <returns>레지스트리 키 객체</returns>
        public RegistryKey OpenSubKey(string subKeyName) {
            return OpenSubKey(subKeyName, false);
        }

        /// <summary>
        /// 지정된 서브키를 지정된 접근 모드 (읽기, 읽기/쓰기) 로 연다.
        /// </summary>
        /// <param name="subKeyName">서브 키 이름</param>
        /// <param name="writable">true이면 읽기/쓰기여부, false이면 읽기 가능한지 여부</param>
        /// <returns>레지스트리 키 객체</returns>
        public RegistryKey OpenSubKey(string subKeyName, bool writable) {
            if(IsDebugEnabled)
                log.Debug("지정된 서브키를 지정된 접근 모드로 연다... subKeyName=[{0}], writable=[{1}]",
                          subKeyName, writable);

            if(subKeyName.IsNotEmpty())
                return RootKey.OpenSubKey(subKeyName, writable);

            return null;
        }

        /// <summary>
        /// SubKey에 해당하는 모든 Key 이름을 얻는다.
        /// </summary>
        /// <returns>문자열 1차원 배열, 실패시 길이가 0인 문자열 배열을 반환한다.</returns>
        public string[] GetSubKeyNames() {
            return GetSubKeyNames(_subKeyName);
        }

        /// <summary>
        /// 지정된 키의 하위에 있는 모든 Sub Key의 이름을 반환한다.
        /// </summary>
        /// <param name="keyName">키 이름</param>
        /// <returns>성공시 문자열 1차원 배열, 실패시 길이가 0인 문자열 배열을 반환한다.</returns>
        public string[] GetSubKeyNames(string keyName) {
            if(IsDebugEnabled)
                log.Debug("지정된 키의 하위에 있는 모든 Sub Key의 이름을 반환한다... keyName=[{0}]", keyName);

            if(keyName.IsNotEmpty())
                using(RegistryKey key = RootKey.OpenSubKey(keyName))
                    if(key != null)
                        return key.GetSubKeyNames();

            return new string[0];
        }

        /// <summary>
        /// SubKey에 해당하는 모든 값의 이름을 얻는다.
        /// </summary>
        /// <returns>문자열 1차원 배열, 실패시 길이가 0인 문자열 배열을 반환한다.</returns>
        public string[] GetValueNames() {
            return GetValueNames(_subKeyName);
        }

        /// <summary>
        /// 지정된 키와 관련된 모든 값 이름이 포함된 문자열의 배열을 검색합니다.   
        /// </summary>
        /// <param name="subKeyName">원하는 Key Name</param>
        /// <returns>문자열 1차원 배열, 실패시 길이가 0인 문자열 배열을 반환한다.</returns>
        public string[] GetValueNames(string subKeyName) {
            subKeyName.ShouldNotBeWhiteSpace("subKeyName");

            if(IsDebugEnabled)
                log.Debug("Get value names. subKeyName=[{0}]", subKeyName);

            using(RegistryKey key = CreateSubKey(subKeyName)) {
                var valueNames = (key != null) ? key.GetValueNames() : new string[0];

                if(IsDebugEnabled)
                    log.Debug("ValueNames in subKeyName [{0}] is [{1}]", subKeyName, valueNames.CollectionToString());

                return valueNames;
            }
        }

        /// <summary>
        /// RootKey\SubKeyName\ 하위 Key의 갯수를 반환
        /// </summary>
        /// <returns>갯수 반환, 실패시 0 반환</returns>
        public int GetSubKeyCount() {
            return GetSubKeyCount(_subKeyName);
        }

        /// <summary>
        /// RootKey/keyName/ 하위의 Key의 갯수를 반환
        /// </summary>
        /// <param name="keyName">찾고자하는 Registry Key Name</param>
        /// <returns>지정된 서브키 하위의 SubKey 갯수 반환, 실패시 0 반환</returns>
        public int GetSubKeyCount(string keyName) {
            keyName.ShouldNotBeWhiteSpace("keyName");

            if(IsDebugEnabled)
                log.Debug("Get Sub Key Count. keyName=[{0}]", keyName);

            using(RegistryKey key = CreateSubKey(keyName)) {
                var count = (key != null) ? key.SubKeyCount : 0;

                if(IsDebugEnabled)
                    log.Debug("서브키 하위의 Key 갯수를 구합니다. keyName=[{0}], count=[{1}]", keyName, count);

                return count;
            }
        }
    }
}