using System.Collections;
using System.Globalization;
using NSoft.NFramework.Data.NHibernateEx;

namespace NSoft.NFramework.StringResources.NHibernate {
    public interface INHResourceRepository : INHRepository<NHResource> {
        /// <summary>
        /// 지정한 부분의 모든 리소스 정보를 반환합니다.
        /// </summary>
        Hashtable GetResources(string application, string section, CultureInfo culture);

        /// <summary>
        /// 지정된 리소스키에 해당하는 <see cref="NHResource"/>정보를 가져온다.
        /// </summary>
        /// <param name="assemblyName">Assembly Name</param>
        /// <param name="sectionName">Section name</param>
        /// <param name="resourceKey">NHResource Key</param>
        /// <returns>NHResource Value를 갖는 Entity</returns>
        NHResource GetResource(string assemblyName, string sectionName, string resourceKey);

        /// <summary>
        /// 지정된 리소스키에 해당하는 <see cref="NHResource"/>정보를 가져온다.
        /// </summary>
        /// <param name="section">Section name or NHResource File name</param>
        /// <param name="resourceKey">NHResource Key</param>
        /// <returns>NHResource Value를 갖는 Entity</returns>
        NHResource GetResource(string section, string resourceKey);

        /// <summary>
        /// 지정된 리소스키에 해당하는 <see cref="NHResource"/>정보를 가져온다.
        /// </summary>
        /// <param name="resourceKey">NHResource Key</param>
        /// <returns>NHResource Value를 갖는 Entity</returns>
        NHResource GetResource(string resourceKey);
    }
}