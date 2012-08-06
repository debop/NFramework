using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NSoft.NFramework.Threading;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.Web {
    // TODO: FusionChart 사에서 제공하는 것인데, Refactoring을 해야 합니다.

    /// <summary>
    /// FusionCharts Exporter is an ASP.NET C# script that handles 
    /// FusionCharts (since v3.1) Server Side Export feature.
    /// This in conjuncture with other resource classses would 
    /// process FusionCharts Export Data POSTED to it from FusionCharts 
    /// and convert the data to an image or a PDF. Subsequently, it would save 
    /// to the server or respond back as an HTTP response to client side as download.
    /// 
    /// This script might be called as the FusionCharts Exporter - main module
    /// </summary>
    /// <example>
    /// <code>
    ///		// web.config의 system.web/httpHandlers 에 추가하면 됩니다.
    ///		&lt;!-- NOTE : Fusion Chart 를 Image로 변환하는 Handler 입니다.--&gt;
    ///		&lt;add verb="*" path="NSoft.NFramework.FusionCharts.Export.axd" type="NSoft.NFramework.FusionCharts.Web.ExportHandler, NSoft.NFramework.FusionCharts" validate="false"/&gt;
    /// 
    /// </code>
    /// </example>
    public class ExportHandler : IHttpHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << Settings >>

        protected static readonly Random rnd = new ThreadSafeRandom();

        public const string SAVE_PATH = "./";

        protected static readonly IDictionary<string, string> ErrorMessages = new Dictionary<string, string>
                                                                              {
                                                                                  { "100", " Insufficient data." },
                                                                                  { "101", "Width/Height is not provided." },
                                                                                  { "102", "Insufficient export paramters." },
                                                                                  { "400", "Bad request." },
                                                                                  { "401", "Unauthorized access." },
                                                                                  { "403", "Directory write access forbidden" },
                                                                                  { "404", "Export resource class not found." }
                                                                              };

        private static readonly IDictionary<string, string> MimeTypes = new Dictionary<string, string>
                                                                        {
                                                                            { "pdf", "application/pdf" },
                                                                            { "jpg", "image/jpeg" },
                                                                            { "jpeg", "image/jpeg" },
                                                                            { "gif", "image/gif" },
                                                                            { "png", "image/png" }
                                                                        };

        private static readonly IDictionary<string, string> Extensions = new Dictionary<string, string>
                                                                         {
                                                                             { "pdf", "pdf" },
                                                                             { "jpg", "jpg" },
                                                                             { "jpeg", "jpg" },
                                                                             { "gif", "gif" },
                                                                             { "png", "png" }
                                                                         };

        private static readonly IDictionary<string, string> DefaultParams = new Dictionary<string, string>
                                                                            {
                                                                                { "exportfilename", "FusionCharts" },
                                                                                { "exportformat", "PDF" },
                                                                                { "exportaction", "download" },
                                                                                { "exporttargetwindow", "_self" }
                                                                            };

        #endregion

        #region << Properties >>

        protected bool OverwriteFile;
        protected bool IntelligentFileNaming = true;

        /// <summary>
        /// value can be either 'TIMESTAMP' or 'RANDOM'
        /// </summary>
        protected string FileSuffixFormat = "TIMESTAMP";

        /// <summary>
        /// Whether the export action is download. default value. Would change as per setting retrieved from chart.
        /// </summary>
        protected bool IsDownload = true;

        private StringBuilder _notices;

        /// <summary>
        /// 알림 정보
        /// </summary>
        protected virtual StringBuilder Notices {
            get { return _notices ?? (_notices = new StringBuilder()); }
        }

        /// <summary>
        /// DOMId of the chart
        /// </summary>
        protected string DOMId;

        /// <summary>
        /// 임시저장할 Exporting 된 파일의 저장경로
        /// </summary>
        protected virtual string ExporterPath { get; set; }

        protected virtual HttpContext CurrentContext { get; set; }
        protected virtual HttpRequest Request { get; set; }
        protected virtual HttpResponse Response { get; set; }

        #endregion

        #region << IHttpHandler >>

        public void ProcessRequest(HttpContext context) {
            if(IsDebugEnabled)
                log.Debug("FusionChart 정보를 Image로 Export 요청을 받았습니다. Request=[{0}]", context.Request.RawUrl);

            try {
                CurrentContext = context;
                Request = context.Request;
                Response = context.Response;

                // Exporting 되는 파일의 경로
                ExporterPath = Path.Combine(WebTool.AppPath, SAVE_PATH);

                ExportChart();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Fusion Chart를 Exporting 하는데 실패했습니다. Request=" + context.Request.RawUrl, ex);
                throw;
            }

            if(IsDebugEnabled)
                log.Debug("Fusion Chart 정보를 Image로 Export 했습니다. Request=" + context.Request.RawUrl);

            // Response.End();
        }

        public bool IsReusable {
            get { return true; }
        }

        #endregion

        /// <summary>
        /// Chart를 Exporting하는 메인 함수입니다.
        /// </summary>
        protected virtual void ExportChart() {
            try {
                // Fusion Chart가 전송한 정보를 파싱합니다.
                var exportData = ParseExportRequestStream();

                // process export data and get the processed data (image/PDF) to be exported
                var exportObject = ExportProcessor(((Hashtable)exportData["parameters"])["exportformat"].ToString(),
                                                   exportData["stream"].ToString(), (Hashtable)exportData["meta"]);

                bool exportedStatus = false;

                if(exportObject != null) {
                    /*
					 * Send the export binary to output module which would either save to a server directory
					 * or send the export file to download. Download terminates the process while
					 * after save the output module sends back export status 
					 */
                    exportedStatus = OutputExportObject(exportObject, (Hashtable)exportData["parameters"]);

                    // Dispose export object
                    exportObject.Close();
                    exportObject.Dispose();
                }

                /*
				 * Build Appropriate Export Status and send back to chart by flushing the  
				 * procesed status to http response. This returns status back to chart. 
				 * [ This is not applicable when Download action took place ]
				 */
                FlushStatus(exportedStatus, (Hashtable)exportData["meta"]);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Chart를 Export하는데 실패했습니다.", ex);
            }
        }

        /// <summary>
        /// Fusion Chart가 전송한 POST 방식의 요청 정보를 해석해서, Hashtable로 반환한다.
        /// 반환되는 Hashtable은 이미지 정보를 담은 'stream', 크기, 색상 등의 정보를 가진 'meta', 부가적인 ExportFormat, exportFileName, exportAction 정보를 담은 'parameters' 라는 키를 가집니다.
        /// </summary>
        /// <returns></returns>
        protected virtual Hashtable ParseExportRequestStream() {
            if(IsDebugEnabled)
                log.Debug("Chart Exporting을 위한 요청정보 파싱을 시작합니다.");

            var exportData = new Hashtable();

            try {
                // string of compressed image data
                exportData["stream"] = Request["stream"];

                // stream 이 제공되지 않는다면, 진행을 멈추고, 예외를 발생시킨다.
                if(Request["stream"].IsWhiteSpace())
                    WriteLogMessage("100", true);

                // Get all export parameters into a Hashtable
                var parameters = ParseParams(Request["parameters"]);
                exportData["parameters"] = parameters;

                // get width and height of the chart
                var meta = new Hashtable();

                meta["width"] = Request["meta_width"];
                // Halt execution on error
                if(Request["meta_width"].IsWhiteSpace())
                    WriteLogMessage("101", true);
                else
                    WriteLogMessage("width=" + Request["meta_width"]);

                meta["height"] = Request["meta_height"];
                if(Request["meta_height"].IsWhiteSpace())
                    WriteLogMessage("101", true);
                else
                    WriteLogMessage("height=" + Request["meta_height"]);

                // Background color of chart
                meta["bgcolor"] = Request["meta_bgColor"];
                if(Request["meta_bgColor"].IsWhiteSpace()) {
                    WriteLogMessage(" Background color not specified. Taking White (FFFFFF) as default background color.");
                    meta["bgcolor"] = "FFFFFF";
                }
                else
                    WriteLogMessage("bgcolor=" + Request["meta_bgColor"]);

                // DOMId of the chart
                meta["DOMId"] = Request["meta_DOMId"].AsText();
                DOMId = meta["DOMId"].ToString();

                exportData["meta"] = meta;
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Chart Exporting을 위한 요청정보를 파싱하는데 실패했습니다.", ex);
                throw;
            }

            if(IsDebugEnabled)
                log.Debug("Chart Exporting을 위한 요청 정보를 파싱했습니다.");

            return exportData;
        }

        /// <summary>
        /// Parse export 'parameters' string into a Hashtable 
        /// Also synchronise default values from defaultparameterValues Hashtable
        /// </summary>
        /// <param name="strParams">A string with parameters (key=value pairs) separated  by | (pipe)</param>
        /// <returns>Hashtable containing parsed key = value pairs.</returns>
        private Hashtable ParseParams(string strParams) {
            var parameters = ToHashtable(strParams, new[] { '|', '=' });

            foreach(var param in DefaultParams) {
                if(parameters.ContainsKey(param.Key) == false)
                    parameters[param.Key] = param.Value;
            }

            IsDownload = (parameters["exportaction"].ToString().ToLower() == "download");

            return parameters;
        }

        /// <summary>
        /// Get Export data from and build the export binary/objct.
        /// </summary>
        /// <param name="strFormat">(string) Export format</param>
        /// <param name="stream">(string) Export image data in FusionCharts compressed format</param>
        /// <param name="meta">{Hastable)Image meta data in keys "width", "heigth" and "bgColor"</param>
        /// <returns></returns>
        private MemoryStream ExportProcessor(string strFormat, string stream, Hashtable meta) {
            if(IsDebugEnabled)
                log.Debug("Chart의 exporting stream을 빌드합니다. strFormat={0}, meta={1}", strFormat, meta);
            strFormat = strFormat.ToLower();

            var exportStream = new MemoryStream();

            switch(strFormat) {
                case "pdf":
                    // Instantiate Export class for PDF, build Binary stream and store in stream object
                    var pdfGenerator = new PDFGenerator(stream, meta["width"].ToString(), meta["height"].ToString(),
                                                        meta["bgcolor"].ToString());
                    exportStream = pdfGenerator.getBinaryStream(strFormat);
                    break;
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    // Instantiate Export class for Images, build Binary stream and store in stream object
                    var imageGenerator = new ImageGenerator(stream, meta["width"].ToString(), meta["height"].ToString(),
                                                            meta["bgcolor"].ToString());
                    exportStream = imageGenerator.getBinaryStream(strFormat);
                    break;

                default:
                    WriteLogMessage(" Invalid Export Format.", true);
                    break;
            }

            return exportStream;
        }

        /// <summary>
        /// Checks whether the export action is download or save.
        /// If action is 'download', send export parameters to 'setupDownload' function.
        /// If action is not-'download', send export parameters to 'setupServer' function.
        /// In either case it gets exportSettings and passes the settings along with 
        /// processed export binary (image/PDF) to the output handler function if the
        /// export settings return a 'ready' flag set to 'true' or 'download'. The export
        /// process would stop here if the action is 'download'. In the other case, 
        /// it gets back success status from output handler function and returns it.
        /// </summary>
        /// <param name="exportStream">Export binary/object in memery stream</param>
        /// <param name="exportParams">Hashtable of export parameters</param>
        /// <returns>Export success status ( filename if success, false if not)</returns>
        private bool OutputExportObject(MemoryStream exportStream, Hashtable exportParams) {
            //pass export paramters and get back export settings as per export action
            var exportActionSettings = (IsDownload ? SetUpDownload(exportParams) : SetUpServer(exportParams));

            // set default export status to true
            bool status = true;

            // filepath returned by server setup would be a string containing the file path
            // where the export file is to be saved.
            // If filepath is a boolean (i.e. false) the server setup must have failed. Hence, terminate process.
            if(exportActionSettings["filepath"] is bool) {
                status = false;
                WriteLogMessage(" Failed to export.", true);
            }
            else {
                // When 'filepath' is a string write the binary to output stream
                try {
                    using(var outStream = (Stream)exportActionSettings["outStream"]) {
                        exportStream.WriteTo(outStream);
                        outStream.Flush();
                    }
                    exportStream.Close();
                }
                catch(ArgumentNullException ae) {
                    WriteLogMessage(" Failed to export. Error:" + ae.Message);
                }
                catch(ObjectDisposedException ode) {
                    WriteLogMessage(" Failed to export. Error:" + ode.Message);
                }
            }
            // This is the response after save action
            // If status remains true return the 'filepath'. Otherwise return false to denote failure.
            return status && exportActionSettings["filepath"].AsBool(false);
        }

        /// <summary>
        /// check server permissions and settings and return ready flag to exportSettings 
        /// </summary>
        /// <param name="exportParams">Various export parameters</param>
        /// <returns>Hashtable containing various export settings</returns>
        private Hashtable SetUpServer(Hashtable exportParams) {
            if(IsDebugEnabled)
                log.Debug("서버 설정을 시작합니다.");

            // get export file name
            string exportfile = exportParams["exportfilename"].AsText();
            // get extension related to specified type
            string ext = exportParams["exportformat"].AsText();

            var result = new Hashtable();

            // set server status to true by default
            result["ready"] = true;

            string path = SAVE_PATH;

            // if path is null set it to folder where FCExporter.aspx is present
            if(path.IsWhiteSpace())
                path = "./";
            path = Regex.Replace(path, @"([^\/]$)", "${1}/");

            try {
                path = HttpContext.Current.Server.MapPath(path);
            }
            catch(HttpException he) {
                if(log.IsErrorEnabled)
                    log.ErrorException("서버 저장소 위치를 찾는데 실패했습니다. Path=" + path, he);

                WriteLogMessage(he.Message);
            }

            if(!Directory.Exists(path))
                WriteLogMessage(" Server Directory does not exists.", true);

            bool dirWritable = IsDirectoryWritable(path);

            result["filepath"] = exportfile + "." + ext;

            var filePath = path + result["filepath"];

            if(File.Exists(filePath) == false) {
                if(dirWritable) {
                    var fs = File.Open(filePath, FileMode.Create, FileAccess.Write);
                    result["outStream"] = fs;
                    return result;
                }

                WriteLogMessage("403", true);
            }

            WriteLogMessage(" file already exists. FilePath=" + filePath);

            // if overwrite is on return with ready flag 
            if(OverwriteFile) {
                // add notice while trying to overwrite
                WriteLogMessage(" Export handler's Overwrite setting is on. Trying to overwrite.");

                if(new FileInfo(filePath).IsReadOnly)
                    WriteLogMessage(" Overwrite forbidden. Readonly file. filePath=" + filePath);

                result["outStream"] = File.Open(filePath, FileMode.Create, FileAccess.Write);
                return result;
            }

            // raise error and halt execution when overwrite is off and intelligent naming is off 
            if(IntelligentFileNaming == false)
                WriteLogMessage(" Export handler's overwrite setting is off. Cannot overwrite. IntelligentFileNaming=" +
                                IntelligentFileNaming, true);

            WriteLogMessage(" Using intelligent naming of file by adding an unique suffix to the exising name.");

            exportfile = exportfile + "_" + GenerateIntelligentFileId();
            result["filepath"] = exportfile + "." + ext;

            // return intelligent file name with ready flag
            // need to check whether the directory is writable to create a new file 
            if(dirWritable) {
                // if directory is writable return with ready flag
                // add new filename notice
                // open the output file in FileStream
                // set the output stream to the FileStream object
                WriteLogMessage(" The filename has changed to " + result["filepath"] + ".");
                result["outStream"] = File.Open(Path.Combine(path, result["filepath"].AsText()), FileMode.Create, FileAccess.Write);

                return result;
            }
            // if not writable halt and raise error
            WriteLogMessage("403", true);

            // in any unknown case the export should not execute	
            result["ready"] = false;
            WriteLogMessage(" Not exported due to unknown reasons.");
            return result;
        }

        /// <summary>
        /// setup download headers and return ready flag in exportSettings 
        /// </summary>
        /// <param name="exportParams">Various export parameters</param>
        /// <returns>Hashtable containing various export settings</returns>
        private Hashtable SetUpDownload(Hashtable exportParams) {
            //get export filename
            string exportFile = exportParams["exportfilename"].ToString();
            //get extension
            string ext = GetExtension(exportParams["exportformat"].ToString());
            //get mime type
            string mime = GetMime(exportParams["exportformat"].ToString());
            // get target window
            string target = exportParams["exporttargetwindow"].ToString().ToLower();

            // set content-type header 
            Response.ContentType = mime;

            // set content-disposition header 
            // when target is _self the type is 'attachment'
            // when target is other than self type is 'inline'
            // NOTE : you can comment this line in order to replace present window (_self) content with the image/PDF  
            Response.AddHeader("Content-Disposition",
                               (target == "_self" ? "attachment" : "inline") + "; filename=\"" + exportFile + "." + ext + "\"");

            // return exportSetting array. 'Ready' key should be set to 'download'
            var result = new Hashtable();
            result["filepath"] = string.Empty;

            // set the output strem to Response stream as the file is going to be downloaded
            result["outStream"] = Response.OutputStream;
            return result;
        }

        /// <summary>
        /// Exporting 상태 정보를 Chart나 Exporting 파일에 씁니다.
        /// It parses the exported status through parser function parseExportedStatus,
        /// builds proper response string using buildResponse function and flushes the response
        /// string to the output stream and terminates the program.
        /// </summary>
        /// <param name="filename">Exporting할 파일명 or 실패시에는 null</param>
        /// <param name="meta">Image의 메타 정보</param>
        /// <param name="msg">부가 메시지</param>
        private void FlushStatus(object filename, Hashtable meta, string msg) {
            Response.Output.Write(BuildResponse(ParseExportedStatus(filename, meta, msg)));
            Response.Flush();

            // 이것을 하게 되면, Thread 예외가 발생한다.
            // Response.End();
        }

        /// <summary>
        /// Exporting 상태 정보를 Chart나 Exporting 파일에 씁니다.
        /// It parses the exported status through parser function parseExportedStatus,
        /// builds proper response string using buildResponse function and flushes the response
        /// string to the output stream and terminates the program.
        /// </summary>
        /// <param name="filename">Exporting할 파일명 or 실패시에는 null</param>
        /// <param name="meta">Image의 메타 정보</param>
        private void FlushStatus(object filename, Hashtable meta) {
            FlushStatus(filename, meta, string.Empty);
        }

        /// <summary>
        /// Parses the exported status and builds an array of export status information. As per
        /// status it builds a status array which contains statusCode (0/1), statusMesage, fileName,
        /// width, height and notice in some cases.
        /// </summary>
        /// <param name="filename">exported status ( false if failed/error, filename as stirng if success)</param>
        /// <param name="meta">Hastable containing meta descriptions of the chart like width, height</param>
        /// <param name="msg">custom message to be added as statusMessage.</param>
        /// <returns></returns>
        private IList<string> ParseExportedStatus(object filename, Hashtable meta, string msg) {
            var arrStatus = new List<string>();

            bool status = filename is string;

            // add notices
            if(Notices.Length > 0)
                arrStatus.Add("notice=" + Notices.ToString().Trim());

            // DOMId of the chart
            arrStatus.Add("DOMId=" + (meta["DOMId"] == null ? DOMId : meta["DOMId"].ToString()));

            // add width and height
            if(meta["width"] == null)
                meta["width"] = 0;
            if(meta["height"] == null)
                meta["height"] = 0;

            arrStatus.Add("height=" + (status ? meta["height"].ToString() : "0"));
            arrStatus.Add("width=" + (status ? meta["width"].ToString() : "0"));

            // add file URI
            arrStatus.Add("fileName=" + (status ? (Regex.Replace(ExporterPath, @"([^\/]$)", "${1}/") + filename) : ""));
            arrStatus.Add("statusMessage=" + (msg.Trim() != "" ? msg.Trim() : (status ? "Success" : "Failure")));
            arrStatus.Add("statusCode=" + (status ? "1" : "0"));

            return arrStatus;
        }

        /// <summary>
        /// Builds response from an array of status information. Joins the array to a string.
        /// Each array element should be a string which is a key=value pair. This array are either joined by 
        /// a & to build a querystring (to pass to chart) or joined by a HTML <BR> to show neat
        /// and clean status informaton in Browser window if download fails at the processing stage. 
        /// </summary>
        /// <param name="arrMsg">Array of string containing status data as [key=value ]</param>
        /// <returns>A string to be written to output stream</returns>
        private string BuildResponse(IEnumerable<string> arrMsg) {
            string msg = string.Join((IsDownload ? "<br/>" : "&"), arrMsg.ToArray());
            return (IsDownload ? "" : "&") + msg;
        }

        private string GenerateIntelligentFileId() {
            string guid = Guid.NewGuid().ToString("D");

            // check FileSuffixFormat Type
            if(FileSuffixFormat.ToLower() == "timestamp")
                guid += "_" + DateTime.Now.ToString("yyyyMMddHHmmssff");
            else
                guid += "_" + rnd.Next();

            return guid;
        }

        /// <summary>
        /// 예외정보를 발생 시킨다.
        /// </summary>
        /// <param name="msg">예외 정보</param>
        /// <param name="halt">중단 여부</param>
        private void WriteLogMessage(string msg, bool halt) {
            if(halt) {
                if(log.IsErrorEnabled)
                    log.Error(msg);
            }
            else {
                if(IsDebugEnabled)
                    log.Debug("Msg=[{0}], halt=[{1}]", msg, halt);
            }

            string errMessage = (msg.IsWhiteSpace() ? "ERROR!" : (ErrorMessages.ContainsKey(msg) ? ErrorMessages[msg] : msg));

            if(halt)
                FlushStatus(false, new Hashtable(), errMessage);
            else
                Notices.Append(errMessage);
        }

        /// <summary>
        /// 예외정보를 발생 시킨다.
        /// </summary>
        /// <param name="msg">예외 정보</param>
        private void WriteLogMessage(string msg) {
            WriteLogMessage(msg, false);
        }

        /// <summary>
        ///  gets file extension checking the export type. 
        /// </summary>
        /// <param name="exportType">(string) export format</param>
        /// <returns>string extension name</returns>
        private static string GetExtension(string exportType) {
            return Extensions.GetValueOrDefault(exportType, exportType);

            //if (EXTENSIONS.ContainsKey(exportType))
            //    return EXTENSIONS[exportType];
            //return exportType;
        }

        /// <summary>
        /// gets mime type for an export type
        /// </summary>
        /// <param name="exportType">Export format</param>
        /// <returns>Mime type as stirng</returns>
        private static string GetMime(string exportType) {
            return MimeTypes.GetValueOrDefault(exportType, exportType);
        }

        /// <summary>
        /// 지정된 경로가 쓰기 가능한 Directory인지 알아본다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static bool IsDirectoryWritable(string path) {
            try {
                var dirInfo = new DirectoryInfo(path);
                return ((dirInfo.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Directory가 쓰기가능하진 알아볼 때 예외가 발생했습니다. Path=" + path, ex);
            }
            return false;
        }

        /// <summary>
        /// Helper function that splits a string containing delimiter separated key value pairs 
        /// into hashtable
        /// </summary>
        /// <param name="str">delimiter separated key value pairs</param>
        /// <param name="delimiters">List of delimiters</param>
        /// <returns></returns>
        private static Hashtable ToHashtable(string str, char[] delimiters) {
            var result = new Hashtable();

            if(str.IsWhiteSpace())
                return result;

            if(delimiters == null || delimiters.Length == 0)
                return result;

            var strArray = StringTool.Split(str, delimiters[0]);

            if(delimiters.Length > 1)
                foreach(var s in strArray) {
                    var values = s.Split(delimiters[1]);
                    if(values.Length >= 2)
                        result[values[0].ToLower()] = values[1];
                }
            return result;
        }

        /// <summary>
        /// Helper function that splits a string containing delimiter separated key value pairs 
        /// into hashtable
        /// </summary>
        /// <param name="str">delimiter separated key value pairs</param>
        /// <returns></returns>
        private static Hashtable ToHashtable(string str) {
            return ToHashtable(str, new[] { ';', '=' });
        }
    }
}