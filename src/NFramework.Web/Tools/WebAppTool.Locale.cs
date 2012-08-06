using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Tools
{
    /// <summary>
    /// 웹 Application에서 공통으로 사용할 Utility Class입니다.
    /// </summary>
    public static partial class WebAppTool
    {
        /// <summary>
        /// 지역화 목록을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public static IList<LocaleItem> GetLocales()
        {
            try
            {
                var locales = AppSettings.LocaleKeys.Split('|');

                return
                    locales
                        .Select(locale => locale.Split(StringSplitOptions.RemoveEmptyEntries, ','))
                        .Where(theme => theme.Length == 2)
                        .Select(locale => new LocaleItem(locale[0], locale[1]))
                        .ToList();
            }
            catch
            {
                return new List<LocaleItem>();
            }
        }
    }
}