/**
 *
 * FusionCharts Exporter is an ASP.NET C# script that handles 
 * FusionCharts (since v3.1) Server Side Export feature.
 * This in conjuncture with various export classes would 
 * process FusionCharts Export Data POSTED to it from FusionCharts 
 * and convert the data to image or PDF and subsequently save to the 
 * server or response back as http response to client side as download.
 *
 * This script might be called as the FusionCharts Exporter - main module 
 *
 *    @author FusionCharts
 *    @description FusionCharts Exporter (Server-Side - ASP.NET C#)
 *    @version 2.0 [ 22 February 2009 ]
 *  
 */
/**
 *  ChangeLog / Version History:
 *  ----------------------------
 *
 *   2.0 [ 12 February 2009 ] 
 *       - Integrated with new Export feature of FusionCharts 3.1
 *       - can save to server side directory
 *       - can provide download or open in popup window.
 *       - can report back to chart
 *       - can save as PDF/JPG/PNG/GIF
 *
 *   1.0 [ 16 August 2007 ]
 *       - can process chart data to jpg image and response back to client side as download.
 *
 */
/**
 * Copyright (c) 2009 InfoSoft Global Private Limited. All Rights Reserved
 * 
 */
/**
 *  GENERAL NOTES
 *  -------------
 *
 *  Chart would POST export data (which consists of encoded image data stream,  
 *  width, height, background color and various other export parameters like 
 *  exportFormat, exportFileName, exportAction, exportTargetWindow) to this script. 
 *  
 *  The script would process this data using appropriate resource classes & build 
 *  export binary (PDF/image) 
 *
 *  It either saves the binary as file to a server side directory or push it as
 *  Download to client side.
 *
 *
 *  ISSUES
 *  ------
 *   Q. What if someone wishes to open in the same page where the chart existed as postback
 *      replacing the old page?
 * 
 *   A. Not directly supported using any chart attribute or parameter but can do by
 *      removing/commenting the line containing 'header( content-disposition ...'
 *     
 */
/**
 * 
 *   @requires	FCIMGGenerator  Class to export FusionCharts image data to JPG, PNG, GIF binary
 *   @requires  FCPDFGenerator  Class to export FusionCharts image data to PDF binary
 *
 */

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using NSoft.NFramework.Web.Tools;

namespace NSoft.NFramework.FusionCharts.Web {
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
    [Obsolete("ExportHandler 를 사용하세요.")]
    public class FusionChartExporterHandler : IHttpHandler {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region IHttpHandler Members

        public bool IsReusable {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context) {
            if(IsDebugEnabled)
                log.Debug("Fusion Chart 정보를 Image로 Export 요청을 받았습니다. Request=" + context.Request);

            try {
                Request = context.Request;
                Response = context.Response;
                // Exporting 되는 파일의 경로
                ExporterPath = WebTool.AppPath + "/chartExports/";

                ExportChart();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Fusion Chart를 Exporting 하는데 실패했습니다.", ex);
                throw;
            }
            if(IsDebugEnabled)
                log.Debug("Fusion Chart 정보를 Image로 Export 했습니다.");
        }

        #endregion

        /// <summary>
        /// The main function that handles all Input - Process - Output of this Export Architecture
        /// </summary>
        public virtual void ExportChart() {
            /**
			 * Retrieve export data from POST Request sent by chart
			 * Parse the Request stream into export data readable by this script
			 */
            var exportData = parseExportRequestStream();

            // process export data and get the processed data (image/PDF) to be exported
            var exportObject = exportProcessor(((Hashtable)exportData["parameters"])["exportformat"].ToString(),
                                               exportData["stream"].ToString(), (Hashtable)exportData["meta"]);

            /*
			 * Send the export binary to output module which would either save to a server directory
			 * or send the export file to download. Download terminates the process while
			 * after save the output module sends back export status 
			 */
            var exportedStatus = outputExportObject(exportObject, (Hashtable)exportData["parameters"]);

            // Dispose export object
            exportObject.Close();
            exportObject.Dispose();

            /*
			 * Build Appropriate Export Status and send back to chart by flushing the  
			 * procesed status to http response. This returns status back to chart. 
			 * [ This is not applicable when Download action took place ]
			 */
            flushStatus(exportedStatus, (Hashtable)exportData["meta"]);
        }

        public virtual HttpRequest Request { get; set; }
        public virtual HttpResponse Response { get; set; }

        /// <summary>
        /// IMPORTANT: This constant HTTP_URI stores the HTTP reference to 
        /// the folder where exported charts will be saved. 
        /// Please enter the HTTP representation of that folder 
        /// in this constant e.g., http://www.yourdomain.com/images/
        /// 기본 값은 "~/chartExports/"
        /// </summary>
        protected virtual string ExporterPath { get; set; }

        /// <summary>
        /// IMPORTANT: You need to change the location of folder where 
        /// the exported chart images/PDFs will be saved on your 
        /// server. Please specify the path to a folder with 
        /// write permissions in the constant SAVE_PATH below. 
        /// 
        /// Please provide the path as per ASP.NET path conventions. 
        /// You can use relative or  absolute path.
        /// 
        /// Special Cases: 
        ///     '/' means 'wwwroot' directory.
        ///     '. /' ( without the space after .) is the directory where the FCExporter.aspx file recides.
        ///     
        /// Absolute Path :
        /// 
        ///     can be like this : "C:\\myFolders\\myImages" 
        ///     ( Please never use single backslash as that would stop execution of the code instantly)
        ///     or "C:/myFolders/myImages"
        /// 
        ///     You may have a // or \ at end : "C:\\myFolders\\myImages\\"  or "C:/myFolders/myImages/"
        /// 
        ///     You can also have mixed slashes : "C:\\myFolders/myImages" 
        ///     
        /// 
        /// </summary>
        /// directory where the FCExporter.aspx file recides
        private const string SAVE_PATH = "./";

        /// <summary>
        /// IMPORTANT: This constant HTTP_URI stores the HTTP reference to 
        /// the folder where exported charts will be saved. 
        /// Please enter the HTTP representation of that folder 
        /// in this constant e.g., http://www.yourdomain.com/images/
        /// </summary>
        // private const string HTTP_URI = "http://www.yourdomain.com/images/";
        /// <summary>
        /// OVERWRITEFILE sets whether the export handler would overwrite an existing file 
        /// the newly created exported file. If it is set to false the export handler would
        /// not overwrite. In this case, if INTELLIGENTFILENAMING is set to true the handler
        /// would add a suffix to the new file name. The suffix is a randomly generated GUID.
        /// Additionally, you add a timestamp or a random number as additional suffix.
        /// </summary>
        private bool OVERWRITEFILE = false;

        private bool INTELLIGENTFILENAMING = true;
        private string FILESUFFIXFORMAT = "TIMESTAMP"; // // value can be either 'TIMESTAMP' or 'RANDOM'

        /// <summary>
        /// This is a constant list of the MIME types related to each export format this resource handles
        /// The value is semicolon separated key value pair for each format
        /// Each key is the format and value is the MIME type
        /// </summary>
        private const string MIMETYPES = "pdf=application/pdf;jpg=image/jpeg;jpeg=image/jpeg;gif=image/gif;png=image/png";

        /// <summary>
        /// This is a constant list of all the file extensions for the export formats
        /// The value is semicolon separated key value pair for each format
        /// Each key is the format and value is the file extension 
        /// </summary>
        private const string EXTENSIONS = "pdf=pdf;jpg=jpg;jpeg=jpg;gif=gif;png=png";

        /// <summary>
        /// Lists the default exportParameter values taken, if not provided by chart
        /// </summary>
        private const string DEFAULTPARAMS =
            "exportfilename=FusionCharts;exportformat=PDF;exportaction=download;exporttargetwindow=_self";

        /// <summary>
        /// Stores server notices, if any as string [ to be sent back to chart after save ] 
        /// </summary>
        private string notices = "";

        /// <summary>
        /// Whether the export action is download. Default value. Would change as per setting retrieved from chart.
        /// </summary>
        private bool isDownload = true;

        /// <summary>
        /// DOMId of the chart
        /// </summary>
        private string DOMId;

        /// <summary>
        /// Parses POST stream from chart and builds a Hashtable containing 
        /// export data and parameters in a format readable by other functions.
        ///  The Hashtable contains keys 'stream' (contains encoded 
        /// image data) ; 'meta' ( Hashtable with 'width', 'height' and 'bgColor' keys) ;
        /// and 'parameters' ( Hashtable of all export parameters from chart as keys, like - exportFormat, 
        /// exportFileName, exportAction etc.)
        /// </summary>
        /// <returns>Hashtable of processed export data and parameters.</returns>
        private Hashtable parseExportRequestStream() {
            // store all export data
            var exportData = new Hashtable();

            //String of compressed image data
            exportData["stream"] = Request["stream"];

            //Halt execution  if image stream is not provided.
            if(Request["stream"] == null || Request["stream"].Trim() == "")
                raise_error("100", true);

            //Get all export parameters into a Hastable
            var parameters = parseParams(Request["parameters"]);
            exportData["parameters"] = parameters;

            //get width and height of the chart
            var meta = new Hashtable();

            meta["width"] = Request["meta_width"];
            //Halt execution on error
            if(Request["meta_width"] == null || Request["meta_width"].Trim() == "")
                raise_error("101", true);

            meta["height"] = Request["meta_height"];
            //Halt execution on error
            if(Request["meta_height"] == null || Request["meta_height"].Trim() == "")
                raise_error("101", true);

            //Background color of chart
            meta["bgcolor"] = Request["meta_bgColor"];
            if(Request["meta_bgColor"] == null || Request["meta_bgColor"].Trim() == "") {
                // Send notice if BgColor is not provided
                raise_error(" Background color not specified. Taking White (FFFFFF) as default background color.");
                // Set White as Default Background color
                meta["bgcolor"] = meta["bgcolor"].ToString().Trim() == "" ? "FFFFFF" : meta["bgcolor"];
            }

            // DOMId of the chart
            meta["DOMId"] = Request["meta_DOMId"] ?? "";
            DOMId = meta["DOMId"].ToString();

            exportData["meta"] = meta;

            return exportData;
        }

        /// <summary>
        /// Parse export 'parameters' string into a Hashtable 
        /// Also synchronise default values from defaultparameterValues Hashtable
        /// </summary>
        /// <param name="strParams">A string with parameters (key=value pairs) separated  by | (pipe)</param>
        /// <returns>Hashtable containing parsed key = value pairs.</returns>
        private Hashtable parseParams(string strParams) {
            //default parameter values
            var defaultParameterValues = bang(DEFAULTPARAMS);

            // get parameters
            var parameters = bang(strParams, new[] { '|', '=' });

            // sync with default values
            // iterate through each default parameter value
            foreach(DictionaryEntry param in defaultParameterValues) {
                // if a parameter from the defaultParameterValues Hashtable is not present
                // in the parameters hashtable take the parameter and value from default
                // parameter hashtable and add it to params hashtable
                // This is needed to ensure proper export
                if(parameters[param.Key] == null)
                    parameters[param.Key] = param.Value.ToString();
            }

            // set a global flag which denotes whether the export is download or not
            // this is needed in many a functions 
            isDownload = parameters["exportaction"].ToString().ToLower() == "download";

            // return parameters
            return parameters;
        }

        /// <summary>
        /// Get Export data from and build the export binary/objct.
        /// </summary>
        /// <param name="strFormat">(string) Export format</param>
        /// <param name="stream">(string) Export image data in FusionCharts compressed format</param>
        /// <param name="meta">{Hastable)Image meta data in keys "width", "heigth" and "bgColor"</param>
        /// <returns></returns>
        private MemoryStream exportProcessor(string strFormat, string stream, Hashtable meta) {
            strFormat = strFormat.ToLower();
            // initilize memeory stream object to store output bytes
            var exportObjectStream = new MemoryStream();

            // Handle Export class as per export format
            switch(strFormat) {
                case "pdf":
                    // Instantiate Export class for PDF, build Binary stream and store in stream object
                    var PDFGEN = new PDFGenerator(stream, meta["width"].ToString(), meta["height"].ToString(),
                                                  meta["bgcolor"].ToString());
                    exportObjectStream = PDFGEN.getBinaryStream(strFormat);
                    break;
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    // Instantiate Export class for Images, build Binary stream and store in stream object
                    var IMGGEN = new ImageGenerator(stream, meta["width"].ToString(), meta["height"].ToString(),
                                                    meta["bgcolor"].ToString());
                    exportObjectStream = IMGGEN.getBinaryStream(strFormat);
                    break;
                default:
                    // In case the format is not recognized
                    raise_error(" Invalid Export Format.", true);
                    break;
            }

            return exportObjectStream;
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
        /// <param name="exportObj">Export binary/object in memery stream</param>
        /// <param name="exportParams">Hashtable of export parameters</param>
        /// <returns>Export success status ( filename if success, false if not)</returns>
        private object outputExportObject(MemoryStream exportObj, Hashtable exportParams) {
            //pass export paramters and get back export settings as per export action
            var exportActionSettings = (isDownload ? setupDownload(exportParams) : setupServer(exportParams));

            // set default export status to true
            bool status = true;

            // filepath returned by server setup would be a string containing the file path
            // where the export file is to be saved.
            // If filepath is a boolean (i.e. false) the server setup must have failed. Hence, terminate process.
            if(exportActionSettings["filepath"] is bool) {
                status = false;
                raise_error(" Failed to export.", true);
            }
            else {
                // When 'filepath' is a sting write the binary to output stream
                try {
                    // Write export binary stream to output stream
                    var outStream = (Stream)exportActionSettings["outStream"];
                    exportObj.WriteTo(outStream);
                    outStream.Flush();
                    outStream.Close();
                    exportObj.Close();
                }
                catch(ArgumentNullException e) {
                    raise_error(" Failed to export. Error:" + e.Message);
                    status = false;
                }
                catch(ObjectDisposedException e) {
                    raise_error(" Failed to export. Error:" + e.Message);
                    status = false;
                }

                if(isDownload) {
                    // If 'download'- terminate imediately
                    // As nothing is to be written to response now.
                    // Response.End();
                }
            }

            // This is the response after save action
            // If status remains true return the 'filepath'. Otherwise return false to denote failure.
            return (status ? exportActionSettings["filepath"] : false);
        }

        /// <summary>
        /// Flushes exported status message/or any status message to the chart or the output stream.
        /// It parses the exported status through parser function parseExportedStatus,
        /// builds proper response string using buildResponse function and flushes the response
        /// string to the output stream and terminates the program.
        /// </summary>
        /// <param name="filename">Name of the exported file or false on failure</param>
        /// <param name="meta">Image's meta data</param>
        /// <param name="msg">Additional messages</param>
        private void flushStatus(object filename, Hashtable meta, string msg) {
            // Process and flush message to response stream and terminate
            Response.Output.Write(buildResponse(parseExportedStatus(filename, meta, msg)));
            Response.Flush();
            // Response.End();
        }

        /// <summary>
        /// Flushes exported status message/or any status message to the chart or the output stream.
        /// It parses the exported status through parser function parseExportedStatus,
        /// builds proper response string using buildResponse function and flushes the response
        /// string to the output stream and terminates the program.
        /// </summary>
        /// <param name="filename">Name of the exported file or false on failure</param>
        /// <param name="meta">Image's meta data</param>
        private void flushStatus(object filename, Hashtable meta) {
            flushStatus(filename, meta, string.Empty);
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
        private ArrayList parseExportedStatus(object filename, IDictionary meta, string msg) {
            var arrStatus = new ArrayList();
            // get status
            bool status = (filename is string);

            // add notices 
            if(notices.Trim() != "")
                arrStatus.Add("notice=" + notices.Trim());

            // DOMId of the chart
            arrStatus.Add("DOMId=" + (meta["DOMId"] == null ? DOMId : meta["DOMId"].ToString()));

            // add width and height
            // provide 0 as width and height on failure	
            if(meta["width"] == null)
                meta["width"] = "0";
            if(meta["height"] == null)
                meta["height"] = "0";
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
        /// a & to build a querystring (to pass to chart) or joined by a HTML <BR/> to show neat
        /// and clean status informaton in Browser window if download fails at the processing stage. 
        /// </summary>
        /// <param name="arrMsg">Array of string containing status data as [key=value ]</param>
        /// <returns>A string to be written to output stream</returns>
        private string buildResponse(ArrayList arrMsg) {
            // Join export status array elements into querystring key-value pairs in case of 'save' action
            // or separate with <BR> in case of 'download' action. This would make the imformation readable in browser window.
            string msg = isDownload ? "" : "&";
            msg += string.Join((isDownload ? "<br>" : "&"), (string[])arrMsg.ToArray(typeof(string)));
            return msg;
        }

        /// <summary>
        /// Finds if a directory is writable
        /// </summary>
        /// <param name="path">String Path</param>
        /// <returns></returns>
        private bool isDirectoryWritable(string path) {
            var info = new DirectoryInfo(path);
            return (info.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly;
        }

        /// <summary>
        /// check server permissions and settings and return ready flag to exportSettings 
        /// </summary>
        /// <param name="exportParams">Various export parameters</param>
        /// <returns>Hashtable containing various export settings</returns>
        private Hashtable setupServer(Hashtable exportParams) {
            //get export file name
            string exportFile = exportParams["exportfilename"].ToString();
            // get extension related to specified type 
            string ext = getExtension(exportParams["exportformat"].ToString());

            var retServerStatus = new Hashtable();

            //set server status to true by default
            retServerStatus["ready"] = true;

            // Open a FileStream to be used as outpur stream when the file would be saved
            FileStream fos;

            // process SAVE_PATH : the path where export file would be saved
            // add a / at the end of path if / is absent at the end

            string path = SAVE_PATH;
            // if path is null set it to folder where FCExporter.aspx is present
            if(path.Trim() == "")
                path = "./";
            path = Regex.Replace(path, @"([^\/]$)", "${1}/");

            try {
                // check if the path is relative if so assign the actual path to path
                path = HttpContext.Current.Server.MapPath(path);
            }
            catch(HttpException e) {
                if(log.IsErrorEnabled)
                    log.ErrorException("서버 설정중에 예외가 발생했습니다.", e);
                raise_error(e.Message);
            }

            // check whether directory exists
            // raise error and halt execution if directory does not exists
            if(!Directory.Exists(path))
                raise_error(" Server Directory does not exist.", true);

            // check if directory is writable or not
            bool dirWritable = isDirectoryWritable(path);

            // build filepath
            retServerStatus["filepath"] = exportFile + "." + ext;

            // check whether file exists
            if(!File.Exists(path + retServerStatus["filepath"])) {
                // need to create a new file if does not exists
                // need to check whether the directory is writable to create a new file  
                if(dirWritable) {
                    // if directory is writable return with ready flag

                    // open the output file in FileStream
                    fos = File.Open(path + retServerStatus["filepath"], FileMode.Create, FileAccess.Write);

                    // set the output stream to the FileStream object
                    retServerStatus["outStream"] = fos;
                    return retServerStatus;
                }

                // if not writable halt and raise error
                raise_error("403", true);
            }

            // add notice that file exists 
            raise_error(" File already exists.");

            //if overwrite is on return with ready flag 
            if(OVERWRITEFILE) {
                // add notice while trying to overwrite
                raise_error(" Export handler's Overwrite setting is on. Trying to overwrite.");

                // see whether the existing file is writable
                // if not halt raising error message
                if((new FileInfo(path + retServerStatus["filepath"])).IsReadOnly)
                    raise_error(" Overwrite forbidden. File cannot be overwritten.", true);

                // if writable return with ready flag 
                // open the output file in FileStream
                // set the output stream to the FileStream object
                fos = File.Open(path + retServerStatus["filepath"], FileMode.Create, FileAccess.Write);
                retServerStatus["outStream"] = fos;
                return retServerStatus;
            }

            // raise error and halt execution when overwrite is off and intelligent naming is off 
            if(!INTELLIGENTFILENAMING) {
                raise_error(" Export handler's Overwrite setting is off. Cannot overwrite.", true);
            }

            raise_error(" Using intelligent naming of file by adding an unique suffix to the exising name.");
            // Intelligent naming 
            // generate new filename with additional suffix
            exportFile = exportFile + "_" + generateIntelligentFileId();
            retServerStatus["filepath"] = exportFile + "." + ext;

            // return intelligent file name with ready flag
            // need to check whether the directory is writable to create a new file  
            if(dirWritable) {
                // if directory is writable return with ready flag
                // add new filename notice
                // open the output file in FileStream
                // set the output stream to the FileStream object
                raise_error(" The filename has changed to " + retServerStatus["filepath"] + ".");
                fos = File.Open(path + retServerStatus["filepath"], FileMode.Create, FileAccess.Write);

                // set the output stream to the FileStream object
                retServerStatus["outStream"] = fos;
                return retServerStatus;
            }

            // if not writable halt and raise error
            raise_error("403", true);

            // in any unknown case the export should not execute	
            retServerStatus["ready"] = false;
            raise_error(" Not exported due to unknown reasons.");
            return retServerStatus;
        }

        /// <summary>
        /// setup download headers and return ready flag in exportSettings 
        /// </summary>
        /// <param name="exportParams">Various export parameters</param>
        /// <returns>Hashtable containing various export settings</returns>
        private Hashtable setupDownload(Hashtable exportParams) {
            //get export filename
            string exportFile = exportParams["exportfilename"].ToString();
            //get extension
            string ext = getExtension(exportParams["exportformat"].ToString());
            //get mime type
            string mime = getMime(exportParams["exportformat"].ToString());
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
            var retStatus = new Hashtable();
            retStatus["filepath"] = "";

            // set the output strem to Response stream as the file is going to be downloaded
            retStatus["outStream"] = Response.OutputStream;
            return retStatus;
        }

        /// <summary>
        ///  gets file extension checking the export type. 
        /// </summary>
        /// <param name="exportType">(string) export format</param>
        /// <returns>string extension name</returns>
        private string getExtension(string exportType) {
            // get a Hashtable array of [type=> extension] 
            // from EXTENSIONS constant 
            var extensionList = bang(EXTENSIONS);
            exportType = exportType.ToLower();

            // if extension type is present in $extensionList return it, otherwise return the type 
            return (extensionList[exportType].ToString());
        }

        /// <summary>
        /// gets mime type for an export type
        /// </summary>
        /// <param name="exportType">Export format</param>
        /// <returns>Mime type as stirng</returns>
        private string getMime(string exportType) {
            // get a Hashtable array of [type=> extension] 
            // from MIMETYPES constant 
            var mimelist = bang(MIMETYPES);
            var ext = getExtension(exportType);

            // get mime type asociated to extension
            return mimelist[ext].AsText();
            //var mime = mimelist[ext].ToString();
            //return mime;
        }

        /// <summary>
        /// generates a file suffix for a existing file name to apply smart file naming 
        /// </summary>
        /// <returns>a string containing GUID and random number /timestamp</returns>
        private string generateIntelligentFileId() {
            // Generate Guid
            var guid = Guid.NewGuid().ToString("D");

            // check FILESUFFIXFORMAT type 
            if(FILESUFFIXFORMAT.ToLower() == "timestamp") {
                // Add time stamp with file name
                guid += "_" + DateTime.Now.ToString("ddMMyyyyHHmmssff");
            }
            else {
                // Add Random Number with fileName
                guid += "_" + (new Random()).Next();
            }

            return guid;
        }

        /// <summary>
        /// Helper function that splits a string containing delimiter separated key value pairs 
        /// into hashtable
        /// </summary>
        /// <param name="str">delimiter separated key value pairs</param>
        /// <param name="delimiterList">List of delimiters</param>
        /// <returns></returns>
        private static Hashtable bang(string str, char[] delimiterList) {
            var retArray = new Hashtable();
            // split string as per first delimiter
            if(str == null || str.Trim() == "")
                return retArray;
            var tmpArray = str.Split(delimiterList[0]);

            // iterate through each element of split string
            for(var i = 0; i < tmpArray.Length; i++) {
                // split each element as per second delimiter
                var tmp2Array = tmpArray[i].Split(delimiterList[1]);

                if(tmp2Array.Length >= 2) {
                    // if the secondary split creats at-least 2 array elements
                    // make the fisrt element as the key and the second as the value
                    // of the resulting array
                    retArray[tmp2Array[0].ToLower()] = tmp2Array[1];
                }
            }
            return retArray;
        }

        private Hashtable bang(string str) {
            return bang(str, new[] { ';', '=' });
        }

        private void raise_error(string msg) {
            raise_error(msg, false);
        }

        /// <summary>
        /// Error reporter function that has a list of error messages. It can terminate the execution
        /// and send successStatus=0 along with a error message. It can also append notice to a global variable
        /// and continue execution of the program. 
        /// </summary>
        /// <param name="msg">error code as Integer (referring to the index of the errMessages
        /// array containing list of error messages) OR, it can be a string containing the error message/notice</param>
        /// <param name="halt">Whether to halt execution</param>
        private void raise_error(string msg, bool halt) {
            var errMessages = new Hashtable();

            //list of defined error messages
            errMessages["100"] = " Insufficient data.";
            errMessages["101"] = " Width/height not provided.";
            errMessages["102"] = " Insufficient export parameters.";
            errMessages["400"] = " Bad request.";
            errMessages["401"] = " Unauthorized access.";
            errMessages["403"] = " Directory write access forbidden.";
            errMessages["404"] = " Export Resource class not found.";

            // Find whether error message is passed in msg or it is a custom error string.
            string err_message = ((msg == null || msg.Trim() == "")
                                      ? "ERROR!"
                                      : (errMessages[msg] == null ? msg : errMessages[msg].ToString())
                                 );

            // Halt executon after flushing the error message to response (if halt is true)
            if(halt) {
                flushStatus(false, new Hashtable(), err_message);
            }
                // add error to notices global variable
            else {
                notices += err_message;
            }
        }
    }
}