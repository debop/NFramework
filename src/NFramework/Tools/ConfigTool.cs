using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace NSoft.NFramework.Tools {
    /// <summary>
    /// Application, Web 환경 설정 정보를 제공합니다. 기존 (AppConfig)
    /// </summary>
    /// <example>
    /// <code>
    ///		string companyName = ConfigTool.GetAppSettings{string}("Company", "RealWeb");
    /// </code>
    /// </example>
    public static class ConfigTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 웹 Application 환경인지 판단한다.
        /// </summary>
        public static bool IsWebContext {
            get { return (HttpContext.Current != null); }
        }

        /// <summary>
        /// app/web configuration에 지정된 key에 해당하는 값을 가져온다. 지정한 키가 없다면 defaultValue를 반환한다.
        /// </summary>
        /// <typeparam name="TValue">설정값의 수형</typeparam>
        /// <param name="key">설정 Key</param>
        /// <returns>설정 값</returns>
        public static TValue GetAppSettings<TValue>(string key) {
            return GetAppSettings(key, () => default(TValue));
        }

        /// <summary>
        /// app/web configuration에 지정된 key에 해당하는 값을 가져온다. 지정한 키가 없다면 defaultValue를 반환한다.
        /// </summary>
        /// <typeparam name="TValue">설정값의 수형</typeparam>
        /// <param name="key">설정 Key</param>
        /// <param name="defaultValue">기본 설정값</param>
        /// <returns>설정 값</returns>
        public static TValue GetAppSettings<TValue>(string key, TValue defaultValue) {
            return GetAppSettings(key, () => defaultValue);
        }

        /// <summary>
        /// 환경설정 AppSettings section에 설정된 key에 해당하는 값을 조회합니다. 지정한 키가 없다면, 기본값 생성 함수를 실행하여, 반환합니다.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValueFactory"></param>
        /// <returns></returns>
        public static TValue GetAppSettings<TValue>(string key, Func<TValue> defaultValueFactory) {
            key.ShouldNotBeWhiteSpace("key");
            // defaultValueFactory.ShouldNotBeNull("defaultValueFactory");

            defaultValueFactory = defaultValueFactory ?? (() => default(TValue));

            var value = (IsWebContext)
                            ? WebConfigurationManager.AppSettings[key]
                            : ConfigurationManager.AppSettings[key];

            if(IsDebugEnabled)
                log.Debug("환경설정중에 AppSettings section의 정보를 가져옵니다. key=[{0}], value=[{1}]", key, value);

            var result = Equals(value, null)
                             ? defaultValueFactory()
                             : value.AsValue(defaultValueFactory);

            if(IsDebugEnabled)
                log.Debug("환경설정중에 AppSettings section의 정보를 원하는 형식으로 변환하여 반환합니다. key=[{0}], converted=[{1}]", key, result);

            return result;
        }

        /// <summary>
        /// 지정된 key의 설정이 app/web configuration 파일에 존재하는가?
        /// </summary>
        /// <param name="key">설정 Key</param>
        /// <returns>설정 존재 여부</returns>
        public static bool HasAppSettings(string key) {
            key.ShouldNotBeWhiteSpace("key");

            return (IsWebContext)
                       ? (WebConfigurationManager.AppSettings[key] != null)
                       : (ConfigurationManager.AppSettings[key] != null);
        }

        /// <summary>
        /// AppSettings 컬렉션 전체를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public static NameValueCollection GetAppSettings() {
            return (IsWebContext) ? WebConfigurationManager.AppSettings : ConfigurationManager.AppSettings;
        }

        /// <summary>
        /// 지정된 이름의 <see cref="ConnectionStringSettings"/> 정보를 조회합니다. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ConnectionStringSettings GetConnectionSettings(string name) {
            name.ShouldNotBeWhiteSpace("name");

            if(IsDebugEnabled)
                log.Debug("지정된 이름의 ConnectionStringSettings를 조회합니다. name=[{0}]", name);

            return (IsWebContext)
                       ? WebConfigurationManager.ConnectionStrings[name]
                       : ConfigurationManager.ConnectionStrings[name];
        }

        /// <summary>
        /// 지정된 이름의 <see cref="ConnectionStringSettings"/> 설정이 있는지 검사한다.
        /// </summary>
        /// <param name="name">ConnectionString name</param>
        /// <returns>존재 여부</returns>
        public static bool HasConnectionSettings(string name) {
            name.ShouldNotBeWhiteSpace("name");

            return (IsWebContext)
                       ? (WebConfigurationManager.ConnectionStrings[name] != null)
                       : (ConfigurationManager.ConnectionStrings[name] != null);
        }

        /// <summary>
        /// ConnectionStringSettings 컬렉션을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public static ConnectionStringSettingsCollection GetConnectionStringSettingsCollection() {
            return (IsWebContext)
                       ? WebConfigurationManager.ConnectionStrings
                       : ConfigurationManager.ConnectionStrings;
        }

        /// <summary>
        /// 지정된 이름의 ConnectionString 정보를 가져온다.
        /// </summary>
        /// <param name="name">ConnectionString name</param>
        /// <returns>ConnectionString</returns>
        public static string GetConnectionString(string name) {
            name.ShouldNotBeWhiteSpace("name");

            var settings = GetConnectionSettings(name);

            var connectionString = (settings != null) ? settings.ConnectionString : string.Empty;

            if(IsDebugEnabled)
                log.Debug("로드된 ConnectionString 설정정보: name=[{0}], ConnnectionString=[{1}]", name, connectionString);

            return connectionString;
        }

        /// <summary>
        /// 지정된 이름의 ConnectionString 설정이 있는지 검사한다.
        /// </summary>
        /// <param name="name">ConnectionString name</param>
        /// <returns>존재 여부</returns>
        public static bool HasConnectionString(string name) {
            name.ShouldNotBeWhiteSpace("name");

            return (IsWebContext)
                       ? (WebConfigurationManager.ConnectionStrings[name] != null)
                       : (ConfigurationManager.ConnectionStrings[name] != null);
        }

        /// <summary>
        /// 환경설정에 설정된 ConnectionStrings 섹션의 <see cref="ConnectionStringSettings"/> 의 이름을 가져온다.
        /// </summary>
        /// <returns></returns>
        public static IList<string> GetDatabaseNames() {
            return GetConnectionStringSettings().Select(settings => settings.Name).ToList();
        }

        /// <summary>
        /// 환경설정에 설정된 ConnectionStrings 섹션의 <see cref="ConnectionStringSettings"/> 을 모두 가져온다.
        /// </summary>
        /// <returns></returns>
        public static IList<ConnectionStringSettings> GetConnectionStringSettings() {
            return GetConnectionStringSettingsCollection().Cast<ConnectionStringSettings>().ToList();
        }
    }
}