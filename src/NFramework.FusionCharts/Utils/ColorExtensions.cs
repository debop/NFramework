using System.Drawing;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Color와 관련된 Extension Methods를 제공합니다.
    /// </summary>
    public static class ColorExtensions {
        public const string FusionChartColorFormat = @"{0:X2}{1:X2}{2:X2}";

        /// <summary>
        /// <see cref="Color"/> 정보를 HTML Color 형식의 문자열로 반환합니다.
        /// </summary>
        /// <param name="color"></param>
        /// <returns>White는 "#FFFFFF", Black="#000000", Red="#FF0000", Green="#00FF00", Blue="#0000FF"</returns>
        /// <seealso cref="ColorTranslator"/>
        public static string ToHtmlColor(this Color color) {
            return ColorTranslator.ToHtml(color);
        }

        /// <summary>
        /// HTML Color Code를 <see cref="Color"/>로 변환합니다.
        /// </summary>
        /// <param name="htmlColor">Html Color (예: #00FF00)</param>
        /// <returns><see cref="Color"/> 수형</returns>
        /// <seealso cref="ColorTranslator"/>
        public static Color FromHtml(this string htmlColor) {
            if(htmlColor.StartsWith("#") == false)
                htmlColor = "#" + htmlColor;

            return ColorTranslator.FromHtml(htmlColor);
        }

        /// <summary>
        /// RGB 값을 Hex format으로 만든다
        /// </summary>
        /// <param name="color"></param>
        /// <returns>White는 "FFFFFF", Black="000000", Red="FF0000", Green="00FF00", Blue="0000FF"</returns>
        public static string ToHexString(this Color color) {
            return string.Format(FusionChartColorFormat, color.R, color.G, color.B);
        }
    }
}