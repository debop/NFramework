using System.Collections.Generic;
using System.Globalization;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// Locale Text를 가지는 Data Object를 표현한다.
    /// </summary>
    /// <typeparam name="TLocale">엔티티의 Locale정보를 나타내는 class</typeparam>
    public interface ILocaledEntity<TLocale> : IStateEntity where TLocale : ILocaleValue {
        /// <summary>
        /// 지역화 정보를 제공하는 기본 속성<br/>
        /// <see cref="NamedIndexer{TResult,TIndex}"/>를 이용하여, Concrete class의 기본 indexer를 사용할 수 있도록 한다.
        /// </summary>
        NamedIndexer<TLocale, CultureInfo> Locales { get; }

        /// <summary>
        /// Localization 을 지원하는 <see cref="CultureInfo"/>의 종류
        /// </summary>
        IList<CultureInfo> LocaleKeys { get; }

        /// <summary>
        /// 지정된 Culture의 Localization 정보를 추가한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="localeEntity"></param>
        void AddLocale(CultureInfo culture, TLocale localeEntity);

        /// <summary>
        /// 지정된 Culture의 Localization 정보를 제거한다.
        /// </summary>
        /// <param name="culture"></param>
        void RemoveLocale(CultureInfo culture);

        /// <summary>
        /// 지정된 Culture의 Localization 정보를 반환한다. 없다면 기본 Entity의 정보를 반환한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        TLocale GetLocaleOrDefault(CultureInfo culture);

        /// <summary>
        /// Current UI Thread에 해당하는 {TLocale} 정보
        /// </summary>
        TLocale CurrentLocale { get; }
    }
}