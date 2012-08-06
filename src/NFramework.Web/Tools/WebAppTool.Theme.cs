using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.Tools
{
    /// <summary>
    /// 웹 Application에서 공통으로 사용할 Utility Class입니다.
    /// </summary>
    public static partial class WebAppTool
    {
        /// <summary>
        /// Client side용 image url를 만든다.
        /// </summary>
        /// <param name="imageFilename"></param>
        /// <returns></returns>
        public static string GetClientImageUrl(string imageFilename)
        {
            return GetImageUrl(imageFilename, string.Empty, false);
        }

        /// <summary>
        /// Theme이 적용된 Image의 Url을 빌드한다.
        /// </summary>
        /// <param name="imageFilename"></param>
        /// <param name="localeKey"></param>
        /// <param name="isRunatServer"></param>
        /// <returns></returns>
        public static string GetImageUrl(string imageFilename, string localeKey = "", bool isRunatServer = true)
        {
            string imageUrl = imageFilename;

            // current page에 theme이 적용되었다면...
            Page currentPage = CurrentPage;

            if(currentPage != null && currentPage.Theme.IsNotWhiteSpace())
            {
                string rootPath = (isRunatServer) ? "~" : WebTool.AppPath;

                imageUrl = string.Format(@"{0}/App_Themes/{1}/Images{2}/{3}",
                                         rootPath,
                                         currentPage.Theme,
                                         localeKey.IsWhiteSpace() ? string.Empty : string.Concat("/", localeKey),
                                         imageFilename);
            }

            return imageUrl;
        }

        /// <summary>
        /// 테마 목록을 반환합니다.
        /// </summary>
        /// <returns></returns>
        public static IList<ThemeItem> GetThemeAssemblyNames()
        {
            var assemblyNames = AppSettings.ThemeAssemblyNames.Split('|');

            return
                assemblyNames
                    .Select(assemblyName => assemblyName.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries))
                    .Where(theme => theme.Length == 2)
                    .Select(theme => new ThemeItem(theme[0], theme[1]))
                    .ToList();
        }
    }
}