using System;

namespace NSoft.NFramework.Data {
    public static class QueryProviderTool {
        /// <summary>
        /// <paramref name="provider"/> 에서 <paramref name="key"/> 에 해당하는 값을 로드합니다. 해당 key가 없으면 <paramref name="defaultValue"/>를 반환합니다.
        /// </summary>
        public static string GetQueryString(this IIniQueryProvider provider, string key, string defaultValue) {
            return GetQueryString(provider, key, () => defaultValue);
        }

        /// <summary>
        /// <paramref name="provider"/> 에서 <paramref name="key"/> 에 해당하는 값을 로드합니다. 해당 key가 없으면 <paramref name="defaultFactory"/>를 실행하여 반환합니다.
        /// </summary>
        public static string GetQueryString(this IIniQueryProvider provider, string key, Func<string> defaultFactory = null) {
            string query;

            if(provider.TryGetQuery(key, out query))
                return query;

            return (defaultFactory != null) ? defaultFactory() : string.Empty;
        }

        /// <summary>
        /// <paramref name="provider"/> 에서 [section, queryName]에 해당하는 값을 로드합니다. 해당 key가 없으면 <paramref name="defaultValue"/>를 반환합니다.
        /// </summary>
        public static string GetQueryString(this IIniQueryProvider provider, string section, string queryName, string defaultValue) {
            return GetQueryString(provider, section, queryName, () => defaultValue);
        }

        /// <summary>
        /// <paramref name="provider"/> 에서 [section, queryName]에 해당하는 값을 로드합니다. 해당 key가 없으면 <paramref name="defaultFactory"/>를 실행하여 반환합니다.
        /// </summary>
        public static string GetQueryString(this IIniQueryProvider provider, string section, string queryName,
                                            Func<string> defaultFactory) {
            string query;

            if(provider.TryGetQuery(section, queryName, out query))
                return query;

            return (defaultFactory != null) ? defaultFactory() : string.Empty;
        }
    }
}