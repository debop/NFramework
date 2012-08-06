using System.Text;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts {
    public static partial class ChartUtil {
        /// <summary>
        /// Chart를 HTML Code로 Rendering을 수행합니다.
        /// 이 함수를 수행하려면, ASP.NET Page에 FunsionChart JavaScript class를 포함하고 있어야 합니다.
        /// </summary>
        /// <param name="chartSwf">Chart에 해당하는 SWF 파일명 (전체경로)</param>
        /// <param name="dataUrl">Data 제공을 URL을 통해서 할 경우, dataXml로 할 경우에는 빈 문자열을 지정하면 된다.</param>
        /// <param name="dataXml">Data 제공을 XML로 제공할 경우, Data정보를 나타낸다. dataUrl 방식이라면, 빈문자열을 지정</param>
        /// <param name="chartId">HTML Code 상에서 Chart의 고유 ID</param>
        /// <param name="width">Chart 폭 (pixel 단위 또는 %)</param>
        /// <param name="height">Chart 높이 (pixel 단위 또는 %)</param>
        /// <param name="debugMode">디버그 모드</param>
        /// <param name="registerWithJS">JavaScript에 Chart를 자체적으로 등록할 것인가 여부</param>
        /// <param name="transparent">Chart 투명화 여부</param>
        /// <returns>Chart를 보이기 위한 JavaScript + HTML Code</returns>
        public static string RenderChart(string chartSwf, string dataUrl, string dataXml,
                                         string chartId, string width, string height,
                                         bool? debugMode, bool? registerWithJS,
                                         bool? transparent) {
            return RenderChartInternal(chartSwf,
                                       dataUrl,
                                       dataXml,
                                       chartId,
                                       width,
                                       height,
                                       debugMode ?? false,
                                       registerWithJS ?? false,
                                       transparent ?? false);
        }

        /// <summary>
        /// Chart를 HTML Code로 Rendering을 수행합니다.
        /// 이 함수를 수행하려면, ASP.NET Page에 FunsionChart JavaScript class를 포함하고 있어야 합니다.
        /// </summary>
        /// <param name="chartSwf">Chart에 해당하는 SWF 파일명 (전체경로)</param>
        /// <param name="dataUrl">Data 제공을 URL을 통해서 할 경우, dataXml로 할 경우에는 빈 문자열을 지정하면 된다.</param>
        /// <param name="dataXml">Data 제공을 XML로 제공할 경우, Data정보를 나타낸다. dataUrl 방식이라면, 빈문자열을 지정</param>
        /// <param name="chartId">HTML Code 상에서 Chart의 고유 ID</param>
        /// <param name="width">Chart 폭 (pixel 단위 또는 %)</param>
        /// <param name="height">Chart 높이 (pixel 단위 또는 %)</param>
        /// <returns>Chart를 보이기 위한 JavaScript + HTML Code</returns>
        public static string RenderChart(string chartSwf, string dataUrl, string dataXml,
                                         string chartId, string width, string height) {
            return RenderChart(chartSwf,
                               dataUrl,
                               dataXml,
                               chartId,
                               width,
                               height,
                               null,
                               null,
                               null);
        }

        /// <summary>
        /// Chart를 HTML Code로 Rendering을 수행합니다.
        /// 이 함수를 수행하려면, ASP.NET Page에 FunsionChart JavaScript class를 포함하고 있어야 합니다.
        /// </summary>
        /// <param name="chartSwf">Chart에 해당하는 SWF 파일명 (전체경로)</param>
        /// <param name="dataUrl">Data 제공을 URL을 통해서 할 경우, dataXml로 할 경우에는 빈 문자열을 지정하면 된다.</param>
        /// <param name="dataXml">Data 제공을 XML로 제공할 경우, Data정보를 나타낸다. dataUrl 방식이라면, 빈문자열을 지정</param>
        /// <param name="chartId">HTML Code 상에서 Chart의 고유 ID</param>
        /// <param name="width">Chart 폭 (pixel 단위 또는 %)</param>
        /// <param name="height">Chart 높이 (pixel 단위 또는 %)</param>
        /// <param name="debugMode">디버그 모드</param>
        /// <param name="registerWithJS">JavaScript에 Chart를 자체적으로 등록할 것인가 여부</param>
        /// <param name="transparent">Chart 투명화 여부</param>
        /// <returns>Chart를 보이기 위한 JavaScript + HTML Code</returns>
        private static string RenderChartInternal(string chartSwf, string dataUrl, string dataXml,
                                                  string chartId, string width, string height,
                                                  bool debugMode, bool registerWithJS,
                                                  bool transparent) {
            if(IsDebugEnabled) {
                log.Debug("Render Chart Starting...");
                log.Debug("SWF={0}, dataUrl={1}, dataXml={2}", chartSwf, dataUrl, dataXml.EllipsisChar(100));
                log.Debug("charId={0}, width={1}, height={2}, debugMode={3}, registerWithJS={4}, transparent={5}",
                          chartId, width, height, debugMode, registerWithJS, transparent);
            }

            chartSwf.ShouldNotBeWhiteSpace("chartSwf");
            chartId.ShouldNotBeWhiteSpace("chartId");

            var chartSwfPath = WebTool.GetScriptPath(chartSwf);

            string chartName = "chart_" + chartId;
            var builder = new StringBuilder();

            builder.AppendFormat("<!-- Start Script Block for Chart : {0} -->", chartId).AppendLine();
            builder.AppendFormat("<div id='{0}Div' align='center'>", chartId).AppendLine();
            builder.AppendLine("Chart.");
            builder.AppendLine("</div>");
            builder.AppendLine("<script type=\"text/javascript\">");
            builder.AppendFormat("    var chart_{0} = new FusionCharts(\"{1}\", \"{0}\", \"{2}\", \"{3}\", \"{4}\", \"{5}\" );",
                                 chartId, chartSwfPath, width, height, debugMode.GetHashCode(), registerWithJS.GetHashCode()).
                AppendLine();

            if(dataXml.IsWhiteSpace())
                builder.AppendFormat("    {0}.setDataURL(\"{1}\");", chartName, EncodeDataUrl(dataUrl, true)).AppendLine();
            else
                builder.AppendFormat("    {0}.setDataXML(\"{1}\");", chartName, dataXml).AppendLine();

            if(transparent)
                builder.AppendFormat("    {0}.setTransparent(\"{1}\");", chartName, true).AppendLine();

            builder.AppendFormat("{0}.render(\"{1}Div\");", chartName, chartId).AppendLine();
            builder.AppendLine("</script>");
            builder.AppendFormat("<!-- End script block for Chart {0} -->", chartId).AppendLine();

            return builder.ToString();
        }

        /// <summary>
        /// Chart를 HTML Code로 Rendering을 수행합니다.
        /// 이 함수를 수행하려면, ASP.NET Page에 FunsionChart JavaScript class를 포함하고 있어야 합니다.
        /// </summary>
        /// <param name="chartSwf">Chart에 해당하는 SWF 파일명 (전체경로)</param>
        /// <param name="dataUrl">Data 제공을 URL을 통해서 할 경우, dataXml로 할 경우에는 빈 문자열을 지정하면 된다.</param>
        /// <param name="dataXml">Data 제공을 XML로 제공할 경우, Data정보를 나타낸다. dataUrl 방식이라면, 빈문자열을 지정</param>
        /// <param name="chartId">HTML Code 상에서 Chart의 고유 ID</param>
        /// <param name="width">Chart 폭 (pixel 단위 또는 %)</param>
        /// <param name="height">Chart 높이 (pixel 단위 또는 %)</param>
        /// <returns>Chart를 보이기 위한 JavaScript + HTML Code</returns>
        public static string RenderChartHtml(string chartSwf, string dataUrl, string dataXml,
                                             string chartId, string width, string height) {
            return RenderChartHtml(chartSwf, dataUrl, dataXml, chartId, width, height,
                                   null, null, null);
        }

        /// <summary>
        /// Chart를 HTML Code로 Rendering을 수행합니다.
        /// 이 함수를 수행하려면, ASP.NET Page에 FunsionChart JavaScript class를 포함하고 있어야 합니다.
        /// </summary>
        /// <param name="chartSwf">Chart에 해당하는 SWF 파일명 (전체경로)</param>
        /// <param name="dataUrl">Data 제공을 URL을 통해서 할 경우, dataXml로 할 경우에는 빈 문자열을 지정하면 된다.</param>
        /// <param name="dataXml">Data 제공을 XML로 제공할 경우, Data정보를 나타낸다. dataUrl 방식이라면, 빈문자열을 지정</param>
        /// <param name="chartId">HTML Code 상에서 Chart의 고유 ID</param>
        /// <param name="width">Chart 폭 (pixel 단위 또는 %)</param>
        /// <param name="height">Chart 높이 (pixel 단위 또는 %)</param>
        /// <param name="debugMode">디버그 모드</param>
        /// <param name="registerWithJS">JavaScript에 Chart를 자체적으로 등록할 것인가 여부</param>
        /// <param name="transparent">Chart 투명화 여부</param>
        /// <returns>Chart를 보이기 위한 JavaScript + HTML Code</returns>
        public static string RenderChartHtml(string chartSwf, string dataUrl, string dataXml,
                                             string chartId,
                                             string width, string height,
                                             bool? debugMode, bool? registerWithJS, bool? transparent) {
            return RenderChartHtmlInternal(chartSwf, dataUrl, dataXml, chartId, width, height,
                                           debugMode ?? false, registerWithJS ?? true, transparent ?? false);
        }

        /// <summary>
        /// Chart를 HTML Code로 Rendering을 수행합니다.
        /// 이 함수를 수행하려면, ASP.NET Page에 FunsionChart JavaScript class를 포함하고 있어야 합니다.
        /// </summary>
        /// <param name="chartSwf">Chart에 해당하는 SWF 파일명 (전체경로)</param>
        /// <param name="dataUrl">Data 제공을 URL을 통해서 할 경우, dataXml로 할 경우에는 빈 문자열을 지정하면 된다.</param>
        /// <param name="dataXml">Data 제공을 XML로 제공할 경우, Data정보를 나타낸다. dataUrl 방식이라면, 빈문자열을 지정</param>
        /// <param name="chartId">HTML Code 상에서 Chart의 고유 ID</param>
        /// <param name="width">Chart 폭 (pixel 단위 또는 %)</param>
        /// <param name="height">Chart 높이 (pixel 단위 또는 %)</param>
        /// <param name="debugMode">디버그 모드</param>
        /// <param name="registerWithJS">JavaScript에 Chart를 자체적으로 등록할 것인가 여부</param>
        /// <param name="transparent">Chart 투명화 여부</param>
        /// <returns>Chart를 보이기 위한 JavaScript + HTML Code</returns>
        private static string RenderChartHtmlInternal(string chartSwf, string dataUrl, string dataXml,
                                                      string chartId, string width, string height,
                                                      bool debugMode, bool registerWithJS, bool transparent) {
            var chartSwfPath = WebTool.GetScriptPath(chartSwf);

            if(IsDebugEnabled) {
                log.Debug("Render Chart Starting...");
                log.Debug("SWF={0}, dataUrl={1}, dataXml={2}", chartSwfPath, dataUrl, dataXml.EllipsisChar(100));
                log.Debug("charId={0}, width={1}, height={2}, debugMode={3}, registerWithJS={4}, transparent={5}",
                          chartId, width, height, debugMode, registerWithJS, transparent);
            }

            var flashVariables =
                string.Format("&chartWidth={0}&chartHeight={1}&debugMode={2}&registerWithJS={3}&DOMId={4}",
                              width, height, debugMode.GetHashCode(), registerWithJS.GetHashCode(), chartId);

            flashVariables += dataUrl.IsWhiteSpace()
                                  ? "&dataXml=" + dataXml
                                  : "&dataUrl=" + WebTool.GetScriptPath(dataUrl).UrlEncode();

            var builder = new StringBuilder();

            builder.AppendFormat("<!-- START Code Block for Chart {0} -->", chartId).AppendLine();
            builder.AppendFormat("<div id='{0}Div'>", chartId).AppendLine();
            builder.AppendFormat("<object classid=\"clsid:{0}\" ", CHART_CLASS_ID);
            builder.AppendFormat("codebase=\"{0}\" ", CHART_CODE_BASE);
            builder.AppendFormat(" width=\"{0}\" height=\"{1}\" name=\"{2}\" id=\"{2}\" >", width, height, chartId).AppendLine();

            builder.AddParamLine("allowScriptAccess", "always");
            builder.AddParamLine("movie", chartSwfPath);
            builder.AddParamLine("FlashVars", flashVariables);
            builder.AddParamLine("quality", "high");

            var wmode = string.Empty;
            if(transparent) {
                builder.AppendFormat(CHART_PARAM_FMT, "wmode", "transparent").AppendLine();
                wmode = "wmode=\"transparent\"";
            }

            builder.AppendFormat(
                "<embed src=\"{0}\" FlashVars=\"{1}\" quality=\"high\" width=\"{2}\" height=\"{3}\" name=\"{4}\" id=\"{4}\" allowScriptAccess=\"always\" type=\"application/x-shockwave-flash\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\" {5} />",
                chartSwfPath, flashVariables, width, height, chartId, wmode);
            builder.AppendLine("</object>");
            builder.AppendLine("</div>");
            builder.AppendFormat("<!-- END Code Block for Chart {0} -->", chartId).AppendLine();

            return builder.ToString();
        }
    }
}