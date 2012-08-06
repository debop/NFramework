using System;
using System.Collections.Generic;
using System.Globalization;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    /// <summary>
    /// 지역화 정보를 가지는 엔티티입니다.
    /// </summary>
    /// <typeparam name="TId">엔티티 Id의 수형</typeparam>
    /// <typeparam name="TLocale">엔티티의 Locale class의 수형</typeparam>
    [Serializable]
    public abstract class LocaledEntityBase<TId, TLocale> : DataEntityBase<TId>, ILocaledEntity<TLocale> where TLocale : ILocaleValue {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private IDictionary<CultureInfo, TLocale> _localeMap;
        private TLocale _defaultLocale;

        /// <summary>
        /// LocaledEntity의 HBM과 연결되는 속성입니다.
        /// </summary>
        public virtual IDictionary<CultureInfo, TLocale> LocaleMap {
            get { return _localeMap ?? (_localeMap = new Dictionary<CultureInfo, TLocale>()); }
            set { _localeMap = value; }
        }

        /// <summary>
        /// Localization 정보가 없을 때, 기본 정보
        /// </summary>
        public virtual TLocale DefaultLocale {
            get {
                if(Equals(_defaultLocale, default(TLocale)))
                    _defaultLocale = CreateDefaultLocale();

                return _defaultLocale;
            }
        }

        /// <summary>
        /// 기본 Localization Object를 생성합니다.
        /// </summary>
        /// <returns></returns>
        protected virtual TLocale CreateDefaultLocale() {
            if(IsDebugEnabled)
                log.Debug("기본 Locale 정보가 없습니다. 엔티티 [{0}] 속성으로 기본 Locale [{1}] 정보를 생성합니다...",
                          GetType().Name, typeof(TLocale).Name);

            try {
                return this.MapProperty(() => ActivatorTool.CreateInstance<TLocale>(), MapPropertyOptions.Safety);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("기본 Locale 정보를 엔티티의 속성으로부터 생성하는데 실패했습니다!!!", ex);

                throw;
            }
        }

        #region << ILocaledEntity<TLocale> >>

        private NamedIndexer<TLocale, CultureInfo> _localeIndexer;

        /// <summary>
        /// Localization 정보
        /// </summary>
        public virtual NamedIndexer<TLocale, CultureInfo> Locales {
            get { return _localeIndexer ?? (_localeIndexer = new NamedIndexer<TLocale, CultureInfo>(GetLocaleOrDefault, AddLocale)); }
        }

        /// <summary>
        /// Localization 을 지원하는 <see cref="CultureInfo"/>의 종류
        /// </summary>
        public virtual IList<CultureInfo> LocaleKeys {
            get { return new List<CultureInfo>(LocaleMap.Keys); }
        }

        /// <summary>
        /// 지정된 Culture의 Localization 정보를 추가한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="locale"></param>
        public virtual void AddLocale(CultureInfo culture, TLocale locale) {
            LocaleMap.AddValue(culture, locale);
        }

        /// <summary>
        /// 지정된 Culture의 Localization 정보를 제거한다.
        /// </summary>
        /// <param name="culture"></param>
        public virtual void RemoveLocale(CultureInfo culture) {
            if(LocaleMap.ContainsKey(culture))
                LocaleMap.Remove(culture);
        }

        /// <summary>
        /// 지정된 Culture의 Localization 정보를 반환한다. 없다면 기본 Entity의 정보를 반환한다.
        /// </summary>
        /// <param name="culture"></param>
        /// <returns></returns>
        public virtual TLocale GetLocaleOrDefault(CultureInfo culture) {
            if(_localeMap == null || _localeMap.Count == 0 || culture.IsNullCulture())
                return DefaultLocale;

            if(LocaleMap.ContainsKey(culture))
                return LocaleMap[culture];

            return GetLocaleOrDefault(culture.Parent);
        }

        /// <summary>
        /// Current UI Thread에 해당하는 {TLocale} 정보
        /// </summary>
        public virtual TLocale CurrentLocale {
            get { return GetLocaleOrDefault(CultureInfo.CurrentCulture); }
        }

        #endregion
    }
}