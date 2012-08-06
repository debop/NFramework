using System;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Xml;

namespace NSoft.NFramework.XmlData.Messages {

    #region << XmlDataSetBase의 XML 포맷 >>

    /*
   <?xml version="1.0" encoding="utf-8"?>
   <RWXMLDS direction="0" tranx="1" cnn="connection string" 
			isolation="4096" cursor="3" commandTimeout="90"
			xmlns:dt="urn:schemas-microsoft-com:datatypes">
	  <!-- dir   : 0 (Request), 1(Response) -->
	  <!-- tranx : 0 (None),    1 (Need Transaction) -->
	  ....
		
		<ERRORS/>
   </RWXMLDS>   
*/

    #endregion

    /// <summary>
    /// RealWeb XML Data 통신을 위한 기본 형식이다.
    /// XmlHttp 통신을 통해 서버와 통신을 하면서 요청 정보 (Database Query or StoredProcedure)를 전달하고,
    /// 그 결과를 받는 일종의 XML-RPC 의 Database 특화 버전이라고 할 수 있다.
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "RWXMLDS")]
    public abstract class XdsDocumentBase {
        #region << logger >>

        [NonSerialized] private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        [NonSerialized] private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        protected XdsDocumentBase() : this(XmlDataDirectionKind.Request) {}

        /// <summary>
        /// Initialize a new instance of <see cref="XdsDocumentBase"/> with direction
        /// </summary>
        /// <param name="direction">direction of this instance.</param>
        protected XdsDocumentBase(XmlDataDirectionKind direction) {
            ConnectionString = string.Empty;
            Transaction = false;
            CommandTimeout = 90;

            IsolationLevel = IsolationLevel.ReadCommitted;
            CursorLocation = 3;

            Direction = direction;
            IsParallelToolecute = false;
        }

        /// <summary>
        /// Direction of Document
        /// </summary>
        [XmlAttribute("dir")]
        public int Dir {
            get { return Direction.ToString("D").AsInt(Direction.GetHashCode()); }
            set { Direction = value.AsEnum(XmlDataDirectionKind.Request); }
        }

        /// <summary>
        /// Document Direction
        /// </summary>
        [XmlAttribute("direction")]
        public XmlDataDirectionKind Direction { get; set; }

        /// <summary>
        /// Connection string 정보
        /// </summary>
        [XmlAttribute("cnn")]
        public string ConnectionString { get; set; }

        /// <summary>
        /// ADO.NET Command 수행 제한 시간
        /// </summary>
        [XmlAttribute("commandTimeout")]
        public int CommandTimeout { get; set; }

        /// <summary>
        /// Need Transaction? (if 0: no transaction, else need transaction)
        /// </summary>
        [XmlAttribute("tranx")]
        public int tranx {
            get { return (Transaction) ? 1 : 0; }
            set { Transaction = (value != 0); }
        }

        /// <summary>
        /// Need Transaction?
        /// </summary>
        [XmlAttribute("transaction")]
        public bool Transaction { get; set; }

        /// <summary>
        /// ADO.NET Recordset Cursor 종류
        /// </summary>
        [XmlAttribute("cursor")]
        public int CursorLocation { get; set; }

        /// <summary>
        /// 격리 수준
        /// </summary>
        [XmlAttribute("isolationLevel")]
        public IsolationLevel IsolationLevel { get; set; }

        /// <summary>
        /// Isolation Level Hashcode.
        /// </summary>
        [XmlAttribute("isolation")]
        public int IsoLevel {
            get { return IsolationLevel.ToString("D").AsInt(IsolationLevel.GetHashCode()); }
            set { IsolationLevel = value.AsEnum(IsolationLevel.ReadCommitted); }
        }

        /// <summary>
        /// 비동기적 수행 벼부
        /// </summary>
        [XmlAttribute("parallel")]
        public int parallel {
            get { return (IsParallelToolecute) ? 1 : 0; }
            set { IsParallelToolecute = (value != 0); }
        }

        /// <summary>
        /// 비동기적 수행 여부
        /// </summary>
        [XmlAttribute("isParallelToolecute")]
        public bool IsParallelToolecute { get; set; }

        private XdsErrorCollection _errors;

        /// <summary>
        /// Error information
        /// </summary>
        [XmlArray("ERRORS")]
        [XmlArrayItem("ERROR", typeof(XdsError))]
        public XdsErrorCollection Errors {
            get { return _errors ?? (_errors = new XdsErrorCollection()); }
            set { _errors = value; }
        }

        /// <summary>
        /// 문서 내용을 Xml Text로 변환한다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return ToString(XmlTool.XmlEncoding);
        }

        /// <summary>
        /// 문서 내용을 Xml Text로 변환한다.
        /// </summary>
        /// <param name="enc">Encoding 방식</param>
        /// <returns>Xml Text</returns>
        public string ToString(System.Text.Encoding enc) {
            using(var ms = new MemoryStream()) {
                XmlTool.Serialize(this, ms);
                ms.Position = 0;

                return ms.ToText(XmlTool.XmlEncoding);
                // return StringTool.ToString((Stream)((Object)ms), XmlConst.XmlEncoding);
            }
        }

        /// <summary>
        /// Save content of current object to the specified file.
        /// </summary>
        /// <param name="filename">Full path of file to save.</param>
        public virtual void Save(string filename) {
            if(IsDebugEnabled)
                log.Debug("현 Document를 파일로 저장합니다... filename=[{0}]", filename);

            var xs = new XmlSerializer(GetType());
            using(TextWriter tw = new StreamWriter(filename)) {
                xs.Serialize(tw, this);
            }

            if(IsDebugEnabled)
                log.Debug("현 Document를 파일로 저장하는데 성공했습니다. filename=[{0}]", filename);
        }
    }
}