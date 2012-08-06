using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace NSoft.NFramework.ResourceProviders {
    [Serializable]
    public abstract class ResourceProviderBase : IResourceProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly ResourceManager _rm;

        protected ResourceProviderBase(Assembly resourceAssembly) {
            resourceAssembly.ShouldNotBeNull("resourceAssembly");

            string assemblyName = resourceAssembly.GetName().Name;

            var assemblyResourceNames = new List<string>(resourceAssembly.GetManifestResourceNames());
            var possibleResourceNames = new List<string>();
            possibleResourceNames.Add(assemblyName);

            // VS.NET 2005 이상부터 자동생성되는 Resource에 대한 경로
            possibleResourceNames.Add(assemblyName + @".Properties.Resources");

            foreach(var resourceName in possibleResourceNames) {
                if(assemblyResourceNames.Contains(resourceName + ".resources")) {
                    _rm = new ResourceManager(resourceName, resourceAssembly);
                    break;
                }
            }

            Guard.Assert(() => _rm != null, "Embedded 된 리소스 파일을 찾지 못했습니다. assemblyName=[{0}]", assemblyName);
        }

        public virtual ResourceManager ResourceManager {
            get { return _rm; }
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 리소스 문자열을 얻습니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual string GetString(string name) {
            var result = _rm.GetString(name);

            if(result == null)
                if(log.IsWarnEnabled)
                    log.Warn("리소스[{0}]에서 [{1}]에 해당하는 리소스 정보를 찾지 못했습니다.", _rm.BaseName, name);

            return result;
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 리소스 문자열을 얻어, string.Format()을 수행하여 반환합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public virtual string GetString(string name, object[] values) {
            var result = GetString(name);

            if(values == null || values.Length == 0)
                return result;

            return string.Format(CultureInfo.CurrentCulture, result, values);
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 리소스 객체를 얻습니다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual object GetObject(string name) {
            var result = _rm.GetObject(name);

            if(result == null)
                if(log.IsWarnEnabled)
                    log.Warn("리소스[{0}]에서 [{1}]에 해당하는 리소스 정보를 찾지 못했습니다.", _rm.BaseName, name);

            return result;
        }

        /// <summary>
        /// <paramref name="name"/>에 해당하는 객체를 얻습니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual T GetObject<T>(string name) {
            var result = GetObject(name);

            if(result != null && (result is T) == false)
                if(log.IsWarnEnabled)
                    log.Warn("리소스 명[{0}]의 값의 수형은 [{1}]입니다. [{2}]이 아닙니다.", name, result.GetType(), typeof(T));

            return result.AsValue<T>();
        }
    }
}