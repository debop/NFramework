using System;
using System.IO;
using System.Xml.Serialization;
using NSoft.NFramework.IO;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Request Document for Xml Data Service
    /// </summary>
    [XmlRoot("RWXMLDS")]
    [Serializable]
    public class XdsRequestDocument : XdsDocumentBase {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        [NonSerialized] private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Initialize a new instance of XdsRequestDocument.
        /// </summary>
        public XdsRequestDocument()
            : base(XmlDataDirectionKind.Request) {
            if(IsDebugEnabled)
                log.Debug("Initialize a new instance of XdsRequestDocument.");
        }

        #region << Properties >>

        private readonly XdsRequestItemCollection _requests = new XdsRequestItemCollection();

        /// <summary>
        /// 단위 요청 정보의 컬렉션
        /// </summary>
        [XmlElement("REQUEST")]
        public XdsRequestItemCollection Requests {
            get { return _requests; }
        }

        /// <summary>
        /// Indexer
        /// </summary>
        public XdsRequestItem this[int index] {
            get { return Requests[index]; }
        }

        #endregion

        #region << Public methods >>

        /// <summary>
        /// Add simple query string to execute with response type and paging
        /// </summary>
        /// <param name="query">simple query string</param>
        /// <param name="responseKind">response type</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start from 1)</param>
        /// <returns>request index</returns>
        public int AddQuery(string query, XmlDataResponseKind responseKind, int pageSize, int pageNo) {
            return Requests.AddQuery(query, responseKind, pageSize, pageNo);
        }

        /// <summary>
        /// Add simple query string to execute with response type.
        /// </summary>
        /// <param name="query">simple query string</param>
        /// <param name="responseKind">response type</param>
        /// <returns>request index</returns>
        public int AddQuery(string query, XmlDataResponseKind responseKind) {
            return Requests.AddQuery(query, responseKind);
        }

        /// <summary>
        /// Add simple query string to execute.
        /// </summary>
        /// <param name="query">simple query string</param>
        /// <returns>request index</returns>
        public int AddQuery(string query) {
            return Requests.AddQuery(query);
        }

        /// <summary>
        /// Add procedure to execute with <see cref="XmlDataResponseKind"/>	and paging
        /// </summary>
        /// <param name="spName">procedure name</param>
        /// <param name="responseKind">response type</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start from 1)</param>
        /// <returns>request index</returns>
        public int AddStoredProc(string spName, XmlDataResponseKind responseKind, int pageSize, int pageNo) {
            return Requests.AddStoredProc(spName, responseKind, pageSize, pageNo);
        }

        /// <summary>
        /// Add procedure to execute with <see cref="XmlDataResponseKind"/>
        /// </summary>
        /// <param name="spName">procedure name</param>
        /// <param name="responseKind">response type</param>
        /// <returns>request index</returns>
        public int AddStoredProc(string spName, XmlDataResponseKind responseKind) {
            return Requests.AddStoredProc(spName, responseKind);
        }

        /// <summary>
        /// Add procedure to execute
        /// </summary>
        /// <param name="spName">procedure name</param>
        /// <returns>request index</returns>
        public int AddStoredProc(string spName) {
            return Requests.AddStoredProc(spName);
        }

        /// <summary>
        /// Add method name to execute
        /// </summary>
        /// <param name="method">method name</param>
        /// <param name="responseKind">response type</param>
        /// <param name="pageSize">page size</param>
        /// <param name="pageNo">page number (start from 1)</param>
        /// <returns>request index</returns>
        public int AddMethod(string method, XmlDataResponseKind responseKind, int pageSize, int pageNo) {
            return Requests.AddMethod(method, responseKind, pageSize, pageNo);
        }

        /// <summary>
        /// Add method name to execute
        /// </summary>
        /// <param name="method">method name</param>
        /// <param name="responseKind">response type</param>
        /// <returns>request index</returns>
        public int AddMethod(string method, XmlDataResponseKind responseKind) {
            return Requests.AddMethod(method, responseKind);
        }

        /// <summary>
        /// Add method name to execute
        /// </summary>
        /// <param name="method">method name</param>
        /// <returns>request index</returns>
        public int AddMethod(string method) {
            return Requests.AddMethod(method);
        }

        #endregion

        #region << Static Methods >>

        /// <summary>
        /// Create a new instance of <see cref="XdsRequestDocument"/> from the specified file.
        /// </summary>
        /// <param name="filename">full file path</param>
        /// <returns>instance of <see cref="XdsRequestDocument"/></returns>
        public static XdsRequestDocument LoadFromFile(string filename) {
            if(IsDebugEnabled)
                log.Debug("Load from file to build request document. filename=[{0}]", filename);

            if(File.Exists(filename) == false)
                throw new FileNotFoundException("File not found.", filename);

            XdsRequestDocument result = null;
            using(var stream = FileTool.GetBufferedFileStream(filename, FileOpenMode.Read)) {
                result = XmlTool.Deserialize<XdsRequestDocument>(stream);
            }
            if(IsDebugEnabled)
                log.Debug("Load from file and build XdsRequestDocument is finished.");

            return result;
        }

        /// <summary>
        /// Create a new instance of <see cref="XdsRequestDocument"/> from the specified xml string.
        /// </summary>
        /// <param name="xml">xml string</param>
        /// <returns>instance of <see cref="XdsRequestDocument"/></returns>
        public static XdsRequestDocument Load(string xml) {
            xml.ShouldNotBeWhiteSpace("xml");

            if(IsDebugEnabled)
                log.Debug("Build XdsRequestDocument with the specified xml string. xml=[{0}]", xml.EllipsisChar(80));

            XdsRequestDocument result = null;

            var data = xml.ToBytes(XmlTool.XmlEncoding);
            result = XmlTool.Deserialize<XdsRequestDocument>(data);

            return result;
        }

        #endregion
    }
}