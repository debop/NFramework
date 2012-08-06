using NSoft.NFramework.StringResources;

namespace NSoft.NFramework.Web.Tools
{
    /// <summary>
    /// 웹 Application에서 공통으로 사용할 Utility Class입니다.
    /// </summary>
    public static partial class WebAppTool
    {
        /// <summary>
        /// Global Resource String을 로드합니다.
        /// </summary>
        /// <param name="classKey">리소스 ClassKey</param>
        /// <param name="resourceKey">리소스 ResourceKey</param>
        /// <param name="defaultValue">리소스 기본값</param>
        /// <returns></returns>
        public static string GetGlobalResourceString(string classKey, string resourceKey, string defaultValue = "")
        {
            classKey.ShouldNotBeWhiteSpace("classKey");
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            if(IsDebugEnabled)
                log.Debug("Get Local Resource... classKey=[{0}], resourceKey=[{1}], defaultValue=[{2}]",
                          classKey, resourceKey, defaultValue);

            var resourceStr =
                WebResourceExpressionBuilder
                    .GetGlobalResourceObject(classKey, resourceKey)
                    .AsText(defaultValue);

            if(IsDebugEnabled)
                log.Debug("Local Resource string =[{0}]", resourceStr);

            return resourceStr;
        }

        /// <summary>
        /// Local Resource String을 로드합니다.
        /// </summary>
        /// <param name="classKey">리소스 ClassKey</param>
        /// <param name="resourceKey">리소스 ResourceKey</param>
        /// <param name="defaultValue">리소스 기본값</param>
        /// <returns></returns>
        public static string GetLocalResourceString(string classKey, string resourceKey, string defaultValue = "")
        {
            classKey.ShouldNotBeWhiteSpace("classKey");
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            if(IsDebugEnabled)
                log.Debug("Get Local Resource... classKey=[{0}], resourceKey=[{1}], defaultValue=[{2}]",
                          classKey, resourceKey, defaultValue);

            var resourceStr =
                WebResourceExpressionBuilder
                    .GetLocalResourceObject(classKey, resourceKey)
                    .AsText(defaultValue);

            if(IsDebugEnabled)
                log.Debug("Local Resource string =[{0}]", resourceStr);


            return resourceStr;
        }
    }
}