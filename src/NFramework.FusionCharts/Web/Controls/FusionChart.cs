using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// Fusion Chart 용 Web Control 입니다.
    /// Fusion Chart 용 SWF 파일 및 Script 는 <see cref="ResourceFileHandler"/>에 의해 다운로드됩니다.
    /// web.config에 <see cref="ResourceFileHandler"/> Handler를 등록하고 사용해야 합니다. 
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <example>
    /// <code>
    /// // web.config 의 system.web/httpHandlers 에 등록한다.
    /// &lt;add verb="GET" path="FusionResourceFile.axd" type="NSoft.NFramework.FusionCharts.Web.ResourceFileHandler, NSoft.NFramework.FusionCharts" validate="false" /&gt;
    /// 
    /// // web page에 FusionChart 용 control 정의
    /// &lt;%@ Register Assembly="NSoft.NFramework.FusionCharts" Namespace="NSoft.NFramework.FusionCharts.Web" TagPrefix="rcl" %&gt;
    /// 
    /// &lt;rcl:FusionChart ID="dynamicChart" 
    ///                     runat="server" 
    ///                     FileName="Gantt.swf"
    ///                     ChartId="gantt2" 
    ///                     DataUrl="~/Widgets/Gantt/DataProviders/SampleDataProvider.ashx" 
    ///                     Width="800" 
    ///                     Height="500" 
    ///                     DebugMode="false" /&gt;
    /// </code>
    /// </example>
    [DefaultProperty("FileName")]
    [ToolboxData("<{0}:FusionChart runat=server></{0}:FusionChart>")]
    [Serializable]
    public class FusionChart : WebControl {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Fusion Chart Javascript
        /// </summary>
        public static readonly string FusionCharts_Javascript
            = string.Format(@"<script type='text/javascript' src='{0}?File=FusionCharts.js'></script>",
                            HandlerSettings.ResourceFileHandler);

        /// <summary>
        /// Fusion Chart Export to Image / PDF 용 Javascript
        /// </summary>
        public static readonly string FusionChartsExportComponent_Javascript
            = string.Format(@"<script type='text/javascript' src='{0}?File=FusionChartsExportComponent.js'></script>",
                            HandlerSettings.ResourceFileHandler);

        #region << Properties >>

        /// <summary>
        /// Chart Identity
        /// </summary>
        [Category("Chart")]
        [DefaultValue("Chart")]
        public string ChartId {
            get { return ViewState["ChartId"].AsText(); }
            set { ViewState["ChartId"] = value; }
        }

        /// <summary>
        /// Fusion Chart Filepath (예: FileName="line.swf"  )
        /// </summary>
        [Category("Chart")]
        [DefaultValue("Line.swf")]
        public string FileName {
            get { return ViewState["FileName"].AsText(); }
            set { ViewState["FileName"] = value; }
        }

        /// <summary>
        /// URI to retrieve data to display in chart (예: "~/Widgets/Gantt/DataProviders/SampleDataProvider.ashx")
        /// </summary>
        [Category("Chart")]
        [DefaultValue("")]
        public string DataUrl {
            get { return ViewState["DataUrl"].AsText(); }
            set { ViewState["DataUrl"] = value; }
        }

        /// <summary>
        /// Xml format data to display in chart
        /// </summary>
        [Category("Chart")]
        [DefaultValue("")]
        public string DataXml {
            get { return ViewState["DataXml"].AsText(); }
            set { ViewState["DataXml"] = value; }
        }

        /// <summary>
        /// Chart display with DebugMode?
        /// </summary>
        [Category("Chart")]
        [DefaultValue(false)]
        public bool DebugMode {
            get { return ViewState["DebugMode"].AsBool(false); }
            set { ViewState["DebugMode"] = value; }
        }

        /// <summary>
        /// Register chart with javascript
        /// </summary>
        [Category("Chart")]
        [DefaultValue(true)]
        public bool RegisterWithJS {
            get { return ViewState["RegisterWithJS"].AsBool(true); }
            set { ViewState["RegisterWithJS"] = value; }
        }

        /// <summary>
        /// Transparent Chart
        /// </summary>
        [Category("Chart")]
        [DefaultValue(false)]
        public bool Transparent {
            get { return ViewState["Transparent"].AsBool(false); }
            set { ViewState["Transparent"] = value; }
        }

        #endregion

        /// <summary>
        /// Fusion Chart 에 대한 HTML 을 Response에 씁니다.
        /// </summary>
        /// <param name="output">Rendering 대상 HtmlTextWriter</param>
        protected override void RenderContents(HtmlTextWriter output) {
            if(IsDebugEnabled)
                log.Debug(@"FusionChart Control을 HTML로 Rending을 시작합니다... ChartId={0}, FileName={1}", ChartId, FileName);

            output.WriteLine(FusionCharts_Javascript);
            output.WriteLine(FusionChartsExportComponent_Javascript);

            var filename = FileName;
            if(filename.StartsWith("~") || filename.Contains("/") || filename.Contains(@"\"))
                filename = WebTool.GetScriptPath(filename);
            else
                filename = HandlerSettings.ResourceFileHandler + "?File=" + filename;

            if(IsDebugEnabled)
                log.Debug(@"Render FusionChart WebControl. " +
                          @"Chart Filename={0}, DataUrl={1}, DataXml={2}, ChartId={3}, Width={4}, Height={5}, DebugMode={6}, RegisterWithJS={7}, Transparent={8}",
                          filename, DataUrl, DataXml, ChartId, Width, Height, DebugMode, RegisterWithJS, Transparent);

            var html = ChartUtil.RenderChartHtml(filename,
                                                 DataUrl,
                                                 DataXml,
                                                 ChartId,
                                                 Width.ToString(),
                                                 Height.ToString(),
                                                 DebugMode,
                                                 RegisterWithJS,
                                                 Transparent);

            if(IsDebugEnabled)
                log.Debug("Rendering Html= " + html);

            output.WriteLine();
            output.WriteLine(html);
            output.WriteLine();

            if(IsDebugEnabled)
                log.Debug(@"FusionChart Control을 HTML로 Rending을 완료했습니다!!! ChartId={0}, FileName={1}", ChartId, FileName);
        }
    }
}