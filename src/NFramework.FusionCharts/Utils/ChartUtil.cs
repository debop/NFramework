using System;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart 관련 Utility class 입니다.
    /// </summary>
    public static partial class ChartUtil {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static readonly string CHART_CLASS_ID = @"d27cdb6e-ae6d-11cf-96b8-444553540000";

        public static readonly string CHART_CODE_BASE =
            @"http://fpdownload.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=8,0,0,0";

        public static readonly string CHART_PARAM_FMT = "<param name=\"{0}\" value=\"{1}\" />";

        /// <summary>
        /// Chart에 제공할 Data의 URL을 인코딩합니다.
        /// </summary>
        /// <param name="dataUrl">Chart에게 Data를 제공해 줄 URL</param>
        /// <param name="noCacheStr">캐시 사용 여부, True면 캐시를 하지 않도록 한다.</param>
        /// <returns>Encoding된 DataUrl</returns>
        public static string EncodeDataUrl(string dataUrl, bool noCacheStr) {
            dataUrl.ShouldNotBeWhiteSpace("dataUrl");

            if(IsDebugEnabled)
                log.Debug("Encode dataUrl. dataUrl=[{0}], noCacheStr=[{1}]", dataUrl, noCacheStr);

            string encoded = WebTool.GetScriptPath(dataUrl);

            if(noCacheStr) {
                encoded += (dataUrl.IndexOf("?") >= 0) ? "&" : "?";
                encoded += "NfwChartID=" + Guid.NewGuid();
            }

            return System.Web.HttpUtility.UrlEncode(encoded);
        }
    }
}