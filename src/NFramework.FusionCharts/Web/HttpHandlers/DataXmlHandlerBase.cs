using System;
using System.Web;
using System.Web.Services;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.HttpHandlers;

namespace NSoft.NFramework.FusionCharts.Web {
    /// <summary>
    /// Fusion Chart의 DataXml 정보를 제공하는 기본 HttpHandler입니다. 이 클래스를 상속받아 <see cref="BuildFusionChart"/> 메소드만 구현하면 됩니다.
    /// </summary>
    [WebService(Namespace = "http://ws.realweb21.com")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public abstract class DataXmlHandlerBase : AbstractHttpAsyncHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// HttpContext의 요청정보를 바탕으로 HttpHandler의 실제 작업을 처리하는 메소드입니다.
        /// </summary>
        /// <param name="context"></param>
        protected override void DoProcessRequest(HttpContext context) {
            base.DoProcessRequest(context);

            if(IsDebugEnabled)
                log.Debug(@"Start FusionChart DataXml Generation... Request.RawUrl=[{0}]", context.Request.RawUrl);

            try {
                CurrentContext = context;
                Request = context.Request;
                Response = context.Response;

                var chart = BuildFusionChart();

                Guard.Assert(chart != null, "Chart를 빌드하는데 실패했습니다. BuildFusionChart() 메소드가 null을 반환했습니다.");

                var dataXml = chart.GetDataXml(Response.ContentEncoding, false);

                WriteDataXml(dataXml);

                if(IsDebugEnabled)
                    log.Debug("Finish FusionChart DataXml Generation...");
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Error occurred in populating FusionChart DataXml!!! Request.RawUrl=[{0}]", context.Request.RawUrl);
                    log.Error(ex);
                }

                throw;
            }
        }

        /// <summary>
        /// Current HttpContext (비동기방식에서 사용할 수 없을 수 있기 때문에 전달받은 Context를 사용한다)
        /// </summary>
        public HttpContext CurrentContext { get; protected set; }

        /// <summary>
        /// Current Response
        /// </summary>
        public HttpResponse Response { get; protected set; }

        /// <summary>
        /// Current Request
        /// </summary>
        public HttpRequest Request { get; protected set; }

        /// <summary>
        /// 원하는 Chart를 빌드합니다.
        /// </summary>
        public abstract IChart BuildFusionChart();

        /// <summary>
        /// Chart의 Xml 정보를 HttpResponse 객체에 쓴다.
        /// </summary>
        /// <param name="dataXml">Chart의 Xml 정보</param>
        protected virtual void WriteDataXml(string dataXml) {
            if(IsDebugEnabled)
                log.Debug(@"Chart 정보를 응답스트림에 쓰기 시작합니다. 응답할 Chart Xml=[{0}]", dataXml.EllipsisChar(1024));

            Response.Clear();
            Response.ContentType = @"text/xml";
            Response.Expires = -1;

            byte[] output = ArrayTool.Combine(StringTool.MultiBytesPrefixBytes, Response.ContentEncoding.GetBytes(dataXml));

            // 압축을 지원하고, 압축할 만큼 크기가 크다면...
            var needCompress = CanCompression && dataXml.Length > CompressionThreshold;

            if(needCompress) {
                // 압축했을 때에, 압축 방법을 응답헤더에 꼭 붙여줘야 Client에서 알아먹겠죠?
                Response.AppendHeader(@"Content-Encoding", CompressionKind.ToString().ToLower());
                output = Compressor.Compress(output);
            }

            Response.AppendHeader(@"Content-Length", output.Length.ToString());
            Response.OutputStream.Write(output, 0, output.Length);
            Response.Flush();

            if(IsDebugEnabled)
                log.Debug(@"Chart 정보를 응답스트림에 쓰기를 완료했습니다!!!");
        }

        /// <summary>
        /// 환경설정에 정의된 옵션에 따라 Chart 의 속성을 설정할 수 있다.
        /// 기본적으로는 Export Image에 대한 환경설정을 수행한다.
        /// </summary>
        /// <param name="chart"></param>
        protected virtual void SetChartAttributes(IChart chart) {
            // var exportEnabled = ConfigTool.GetAppSettings(HandlerSettings.ExportEnabledAppKey, true);
            var exportPath = ConfigTool.GetAppSettings(HandlerSettings.ExportPathAppKey, HandlerSettings.ExportHandler);

            if(IsDebugEnabled)
                log.Debug("Settings export Image/PDF attributes, FusionCharts.Export.Path=[{0}]", exportPath);

            // if (exportEnabled)
            //{
            chart.SetExportInServer(exportPath);
            //}
        }
    }
}