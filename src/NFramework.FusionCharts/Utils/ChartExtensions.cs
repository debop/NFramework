using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using NSoft.NFramework.FusionCharts.Web;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart 관련 Extension Methods 들입니다.
    /// </summary>
    public static partial class ChartExtensions {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Chart 정보를 FusionChart의 DataXml로 변환해서 반환합니다.
        /// </summary>
        /// <typeparam name="T">Chart 수형</typeparam>
        /// <param name="chart">DataXml을 생성하고자하는 Chart</param>
        /// <param name="encoding">DataXml 의 Encoding 방식 (한글 때문에 무조건 UTF8 이어야 한다.)</param>
        /// <param name="indent">indent 여부</param>
        /// <returns>Chart 정보를 나타내는 DataXml의 xml text</returns>
        public static string GetDataXml<T>(this T chart, Encoding encoding, bool? indent) where T : IChart {
            chart.ShouldNotBeNull("chart");

            if(IsDebugEnabled)
                log.Debug(@"Generate DataXml is starting... chart type={0}, encoding={1}, indent={2}", chart.GetType().FullName,
                          encoding, indent);

            var builder = new StringBuilder(400);

            var xmlSettings = new XmlWriterSettings();

            if(encoding != null && encoding == Encoding.Unicode)
                xmlSettings.Encoding = encoding;
            else
                xmlSettings.OmitXmlDeclaration = true;

            xmlSettings.Indent = indent ?? false;

            if(xmlSettings.Indent)
                xmlSettings.IndentChars = "\t";

            // About ContextMenu 보이기
            chart.SetAboutMenuItemAttribute();

            // using(var writer = new XmlTextWriter(new StringWriter(builder)))
            using(var writer = XmlWriter.Create(new StringWriter(builder), xmlSettings)) {
                writer.WriteStartDocument();
                chart.WriteXmlElement(writer);
                writer.WriteEndDocument();
            }

            if(IsDebugEnabled)
                log.Debug(@"Generate DataXml is finished. chart={0}, encoding={1}, indent={2}", chart.GetType().FullName, encoding,
                          indent);

            return builder.ToString();
        }

        /// <summary>
        /// Chart 정보를 FusionChart의 DataXml로 변환해서 반환합니다.
        /// </summary>
        /// <typeparam name="T">Chart 수형</typeparam>
        /// <param name="chart">DataXml을 생성하고자하는 Chart</param>
        /// <param name="indent">indent 여부</param>
        /// <returns>Chart 정보를 나타내는 DataXml의 xml text</returns>
        public static string GetDataXml<T>(this T chart, bool? indent) where T : IChart {
            return chart.GetDataXml(null, indent);
        }

        /// <summary>
        /// About ContextMenuItem 을 설정합니다.
        /// </summary>
        /// <param name="chart">DataXml을 생성하고자하는 Chart</param>
        private static void SetAboutMenuItemAttribute(this IChart chart) {
            chart.AboutMenuItemAttr.Show = true;
            chart.AboutMenuItemAttr.Label = ConfigTool.GetAppSettings("RealCharts.AboutMenu.Label", @"About RealWeb");
            chart.AboutMenuItemAttr.Link.SetLink(FusionLinkMethod.NewWindow, Assembly.GetExecutingAssembly().GetCompany());
        }

        /// <summary>
        /// Server-side에서 Chart의 Image/PDF를 export 하도록 설정을 합니다.
        /// </summary>
        /// <param name="chart">export 할 chart</param>
        /// <param name="exportHandler">ExportHandler 경로. <see cref="HandlerSettings.ExportHandler"/></param>
        /// <param name="filename">Export할 image/pdf 파일의 이름</param>
        public static void SetExportInServer(this IChart chart, string exportHandler, string filename) {
            if(IsDebugEnabled)
                log.Debug(@"Chart Exporting이 가능하도록 설정합니다. Server-side exporting 후 download를 수행하도록 합니다." +
                          @" exportHander={0}, filename={1}", exportHandler, filename);

            // Server-side Export 후 download 하도록 합니다.
            //
            chart.ExportAttr.Enabled = true;
            chart.ExportAttr.AtClient = false;
            chart.ExportAttr.Action = "download";
            chart.ExportAttr.ShowDialog = true;
            // chart.ExportAttr.TargetWindow = "_blank";  // default is _self

            chart.ExportAttr.Handler = exportHandler.IsNotWhiteSpace()
                                           ? WebTool.GetScriptPath(exportHandler)
                                           : HandlerSettings.ExportHandler;

            chart.ExportAttr.FileName = filename.IsNotWhiteSpace()
                                            ? filename
                                            : "RealWeb_RealCharts_" + Guid.NewGuid().ToString("D");
        }

        /// <summary>
        /// Server-side에서 Chart의 Image/PDF를 export 하도록 설정을 합니다.
        /// </summary>
        /// <param name="chart">export 할 chart</param>
        /// <param name="exportHandler">ExportHandler 경로. <see cref="HandlerSettings.ExportHandler"/></param>
        public static void SetExportInServer(this IChart chart, string exportHandler) {
            chart.SetExportInServer(exportHandler, string.Empty);
        }

        /// <summary>
        /// Server-side에서 Chart의 Image/PDF를 export 하도록 설정을 합니다.
        /// </summary>
        /// <param name="chart">export 할 chart</param>
        public static void SetExportInServer(this IChart chart) {
            chart.SetExportInServer(string.Empty, string.Empty);
        }

        /// <summary>
        /// Fusion Chart를 Rendering 할 때, Chart ActiveX의 Parameter를 설정한다. 
        /// </summary>
        /// <param name="builder">StringBuilder 객체</param>
        /// <param name="name">parameter name to add</param>
        /// <param name="value">parameter value to add</param>
        /// <returns></returns>
        internal static StringBuilder AddParam(this StringBuilder builder, string name, string value) {
            return builder.AppendFormat("<param name=\"{0}\" value=\"{1}\" />", name, value);
        }

        /// <summary>
        /// Fusion Chart를 Rendering 할 때, Chart ActiveX의 Parameter를 설정한다. 
        /// </summary>
        /// <param name="builder">StringBuilder 객체</param>
        /// <param name="name">parameter name to add</param>
        /// <param name="value">parameter value to add</param>
        /// <returns></returns>
        internal static StringBuilder AddParamLine(this StringBuilder builder, string name, string value) {
            return builder.AddParam(name, value).AppendLine();
        }
    }
}