namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// FusionCharts 용 HttpHandler의 경로를 정의한 상수를 제공합니다. web.config에 같은 내용을 정의해야 합니다.
    /// </summary>
    public static class HandlerSettings {
        /// <summary>
        /// NSoft.NFramework.FusionCharts.dll 에 포함된 리소스 파일인 FusionCharts 용 javascript, swf 파일을 얻기 위한 HttpHandler의 경로입니다.
        /// </summary>
        /// <example>
        /// <code>
        /// // IIS 6.0 이하 classic 의 web.config/system.web/httpHandlers section에 다음을 추가해야 합니다.
        /// &lt;add verb="*" path="NSoft.NFramework.FusionCharts.ResourceFile.axd" type="NSoft.NFramework.FusionCharts.Web.ResourceFileHandler, NSoft.NFramework.FusionCharts" /&gt;
        /// 
        /// // IIS 7.0 이상 web.config/system.webServer/httpHandlers section에 다음을 추가해야 합니다.
        /// &lt;add name="NSoft.NFramework_FusionCharts_ResourceFile_axd" verb="*" path="NSoft.NFramework.FusionCharts.ResourceFile.axd" type="NSoft.NFramework.FusionCharts.Web.ResourceFileHandler, NSoft.NFramework.FusionCharts" /&gt;
        /// 
        /// // 코드상에서
        /// // NSoft.NFramework.FusionCharts.ResourceFile.axd?File=FusionCharts.js
        /// ResourceFileHandler + "?File=" + filename;
        /// </code>
        /// </example>
        public const string ResourceFileHandler = "NSoft.NFramework.FusionCharts.ResourceFile.axd";

        /// <summary>
        /// Fusion Chart 의 이미지를 JPG, PNG 등의 이미지 파일이나 PDF로 변환하는 작업을 수행하는 Handler의 경로입니다.
        /// </summary>
        /// <example>
        /// <code>
        /// // IIS 6.0 이하 classic 의 web.config/system.web/httpHandlers section에 다음을 추가해야 합니다.
        /// &lt;add verb="*" path="NSoft.NFramework.FusionCharts.Export.axd" type="NSoft.NFramework.FusionCharts.Web.ExportHandler, NSoft.NFramework.FusionCharts" validate="false"/&gt;
        /// 
        /// // IIS 7.0 이상 web.config/system.webServer/httpHandlers section에 다음을 추가해야 합니다.
        /// &lt;add name="NSoft.NFramework_FusionCharts_Export_axd" verb="*" path="NSoft.NFramework.FusionCharts.Export.axd" type="NSoft.NFramework.FusionCharts.Web.ExportHandler, NSoft.NFramework.FusionCharts"/&gt;
        /// 
        /// // 코드상에서
        /// chart.ExportAttr.ExportHandler =  HandlerPath.ExportHandler
        /// </code>
        /// </example>
        public const string ExportHandler = "NSoft.NFramework.FusionCharts.Export.axd";

        /// <summary>
        /// FusionChart를 Image/PDF 로 Export 할 수 있는지 설정한 환경설정 키
        /// </summary>
        public const string ExportEnabledAppKey = "FusionCharts.Export.Enabled";

        /// <summary>
        /// FusionChart를 Image/PDF로 Export를 담당하는 Handler의 경로를 지정한 환경설정 키
        /// </summary>
        public const string ExportPathAppKey = "FusionCharts.Export.Path";
    }
}