using System;
using System.Globalization;
using NSoft.NFramework.ResourceProviders;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// <see cref="IResourceProvider"/>의 값이 다른 리소스 값을 참조할 때, 참조된 값을 Parsing해서 가져올 수 있도록 한다.
    /// </summary>
    /// <example>
    /// Title=RealWeb.Common.Library
    /// Version=3.0
    /// Product=${Title} v${Version}
    /// 
    /// 일때
    /// <code>
    /// providerDecorator = new Parameterized(new ExternalResourceProvider("ExtRes", "Strings"));
    /// string productName = providerDecorator.GetObject("Product", CultureInfo.CurrentUICulture);
    /// 
    /// // productName is RealWeb.Common.Library v3.0
    /// </code>
    /// </example>
    public class ParameterizedResourceProvider : ResourceProviderDecorator {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="inner">resource provider to wrapped</param>
        public ParameterizedResourceProvider(ResourceProviderBase inner) : base(inner) {}

        ///<summary>
        /// 키 및 culture에 대한 리소스 개체를 반환합니다.
        ///</summary>
        ///<returns>
        ///<paramref name="resourceKey" /> 및 <paramref name="culture" />에 대한 리소스 값을 포함하는 <see cref="T:System.Object" />입니다.
        ///</returns>
        ///<param name="resourceKey">
        /// 특정 리소스를 식별하는 키입니다.
        /// </param>
        ///<param name="culture">
        /// 리소스의 지역화된 값을 식별하는 culture입니다.
        ///</param>
        /// <returns>resource value, if not exists, return null</returns>
        public override object GetObject(string resourceKey, CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug(@"리소스 값을 조회합니다. resourceKey=[{0}], culture=[{1}]", resourceKey, culture);

            culture = culture.GetCulture();
            var result = base.GetObject(resourceKey, culture);

            // resource value가 문자열일때만 parameter를 치환한다.
            //
            if(result != null && result is string && ((string)result).Contains(StringResourceTool.PARAM_REF_START))
                result = GetExpanded(resourceKey, culture, (string)result);

            if(IsDebugEnabled)
                log.Debug("리소스 값을 조회했습니다. resourceKey=[{0}], culture=[{1}], value=[{2}]", resourceKey, culture, result);

            return result;
        }

        private string GetExpanded(string resourceKey, CultureInfo culture, string value) {
            if(IsDebugEnabled)
                log.Debug(@"Parameterized된 리소스 값을 조회합니다... resourceKey=[{0}], culture=[{1}], resourceValue=[{2}]",
                          resourceKey, culture, value);

            string result = value;

            int paramPrefixLength = StringResourceTool.PARAM_REF_START.Length;

            while(true) {
                var startIndex = result.IndexOf(StringResourceTool.PARAM_REF_START, 0);
                if(startIndex < 0)
                    break;

                var endIndex = result.IndexOf(StringResourceTool.PARAM_REF_END, startIndex + paramPrefixLength);
                if(endIndex < 0)
                    break;

                string paramKey = result.Substring(startIndex + paramPrefixLength, endIndex - (startIndex + paramPrefixLength));

                if(paramKey.IsNotWhiteSpace()) {
                    if(resourceKey == paramKey)
                        throw new InvalidOperationException("Self reference parameter found. resourceKey=" + resourceKey);

                    // replace ${parameterKey} ==> parameterValue
                    result = result.Replace(StringResourceTool.PARAM_REF_START + paramKey + StringResourceTool.PARAM_REF_END,
                                            GetParameterValue(paramKey, culture));
                }
            }

            return result;
        }

        private string GetParameterValue(string paramKey, CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug("Get Parameter value. paramKey=[{0}], culuture=[{1}]", paramKey, culture);

            if(paramKey.IsWhiteSpace())
                return null;

            string result;

            string resourceFile, resourceKey;
            StringResourceTool.ParseClassKey(paramKey, out resourceFile, out resourceKey);

            // External Resource Provider의 예로 들면
            // 다른 resource file에 있을 시 (resource assembly에는 여러개의 resource file이 존재하고,
            // 다른 resource file의 키값을 조회하기 위해 ${filename|resourceKey} 형태를 취할 수 있다.
            //
            // Resource : ${resourceFile|resourceKey}
            if(resourceFile != StringResourceTool.DEFAULT_ASSEMBLY_NAME)
                result = ResourceProvider.GetString(AssemblyName + StringResourceTool.CLASS_KEY_DELIMITER + resourceFile,
                                                    resourceKey,
                                                    culture);
            else
                result = base.GetObject(paramKey, culture) as string;

            if(IsDebugEnabled)
                log.Debug("Parameter에 해당하는 값을 조회했습니다. paramKey=[{0}], culture=[{1}], paramValue=[{2}]", paramKey, culture, result);

            return result;
        }
    }
}