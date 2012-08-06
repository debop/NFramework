using System.Collections;
using System.Globalization;
using System.Linq;
using NHibernate.Criterion;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources.NHibernate {
    /// <summary>
    /// <see cref="NHResource"/> 를 다루는 Repository
    /// </summary>
    public class NHResourceRepository : NHRepository<NHResource>, INHResourceRepository {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// NHibernate Resource Repository의 캐시를 위한 키의 포맷
        /// </summary>
        public const string KeyFormat = @"{0}|{1}|{2}:{3}";

        /// <summary>
        /// 지정한 부분의 모든 리소스 정보를 반환합니다.
        /// </summary>
        public Hashtable GetResources(string application, string section, CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug(@"Get Resource String... application=[{0}], section=[{1}], culture=[{2}]", application, section, culture);


            var queryOver = BuildDetachedQueryOver(application, section, string.Empty);
            var resources = FindAll(queryOver);

            var map = new Hashtable();


            foreach(var resource in resources) {
                var nhResourceLocale = resource.GetLocaleOrDefault(culture);

                if(nhResourceLocale != null) {
                    if(IsDebugEnabled)
                        log.Debug(@"지정된 Culture에 해당하는 값을 추가합니다. resourceKey=[{0}], resourceValue=[{1}]",
                                  resource.ResourceKey, nhResourceLocale.ResourceValue);

                    map.Add(string.Format(KeyFormat,
                                          application,
                                          StringResourceTool.CLASS_KEY_DELIMITER,
                                          section,
                                          resource.ResourceKey),
                            nhResourceLocale.ResourceValue);
                }
            }

            return map;
        }

        /// <summary>
        /// 지정된 리소스키에 해당하는 <see cref="NHResource"/>정보를 가져온다.
        /// </summary>
        /// <param name="assemblyName">Assembly Name</param>
        /// <param name="sectionName">Section name</param>
        /// <param name="resourceKey">NHResource Key</param>
        /// <returns>NHResource Value를 갖는 Entity</returns>
        public NHResource GetResource(string assemblyName, string sectionName, string resourceKey) {
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            if(IsDebugEnabled)
                log.Debug(@"리소스 정보를 DB로부터 로딩합니다. assemblyName=[{0}], section=[{1}], resourceKey=[{2}]", assemblyName,
                          sectionName, resourceKey);

            //var criteria = CreateDetachedCriteria();

            var queryOver =
                BuildDetachedQueryOver(assemblyName, sectionName, resourceKey)
                    .OrderBy(res => res.AssemblyName).Asc
                    .OrderBy(res => res.Section).Asc;

            // return FindFirst(criteria, orders.ToArray());
            var resources = FindAll(queryOver);

            if(resources.Count == 0)
                return null;

            if(resources.Count == 1)
                return resources[0];

            // 1. NULL이 아닌, 특정 Application, 특정 Section에 관련된 것을 먼저 제공한다.
            var result = resources.FirstOrDefault(r => r.AssemblyName.IsNotWhiteSpace() && r.Section.IsNotWhiteSpace());
            if(result != null)
                return result;

            // 2. 특정 Assembly의 리소스 값을 제공한다.
            result = resources.FirstOrDefault(r => r.AssemblyName.IsNotWhiteSpace());
            if(result != null)
                return result;

            // 3. 기본 리소스의 특정 Section의 리소스 값을 제공한다.
            result = resources.FirstOrDefault(r => r.Section.IsNotWhiteSpace());

            return result ?? null;
        }

        /// <summary>
        /// 지정된 리소스키에 해당하는 <see cref="NHResource"/>정보를 가져온다.
        /// </summary>
        /// <param name="section">Section name or NHResource File name</param>
        /// <param name="resourceKey">NHResource Key</param>
        /// <returns>NHResource Value를 갖는 Entity</returns>
        public NHResource GetResource(string section, string resourceKey) {
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            return GetResource(string.Empty, section, resourceKey);
        }

        /// <summary>
        /// 지정된 리소스키에 해당하는 <see cref="NHResource"/>정보를 가져온다.
        /// </summary>
        /// <param name="resourceKey">NHResource Key</param>
        /// <returns>NHResource Value를 갖는 Entity</returns>
        public NHResource GetResource(string resourceKey) {
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            return GetResource(string.Empty, string.Empty, resourceKey);
        }

        /// <summary>
        /// <see cref="QueryOver{T,T}"/>를 빌드합니다.
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="section"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        protected QueryOver<NHResource, NHResource> BuildDetachedQueryOver(string assemblyName, string section, string resourceKey) {
            var queryOver = CreateQueryOverOf();

            if(assemblyName.IsNotWhiteSpace())
                queryOver.AddWhere(res => res.AssemblyName == assemblyName);

            if(section.IsNotWhiteSpace())
                queryOver.AddWhere(r => r.Section == section);

            if(resourceKey.IsNotWhiteSpace())
                queryOver.AddWhere(res => res.ResourceKey == resourceKey);

            return queryOver;
        }
    }
}