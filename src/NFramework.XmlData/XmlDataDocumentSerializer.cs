using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NSoft.NFramework.Xml;
using NSoft.NFramework.XmlData.Messages;

namespace NSoft.NFramework.XmlData {
    /// <summary>
    /// RclXmlDataSet과 이 클래스로부터 상속받은 Class들을 ( <see cref="XdsRequestDocument"/>, <see cref="XdsResponseDocument"/>)
    /// Serialize, Deserialize를 수행한다.
    /// </summary>
    /// <remarks>
    ///	Encoding 방식은 UTF8이 기본이다.
    /// </remarks>
    public static class XmlDataDocumentSerializer {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        //private static readonly Type requestDocumentType = typeof(XdsRequestDocument);
        //private static readonly Type responseDocumentType = typeof(XdsResponseDocument);

        /// <summary>
        /// <see cref="XdsDocumentBase"/> 형식의 객체를 Xml Serializer를 수행한 후, byte array로 변환한다.
        /// </summary>
        /// <param name="xdsDocument"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(this XdsDocumentBase xdsDocument, Encoding enc) {
            enc = enc ?? XmlTool.XmlEncoding;
            byte[] result;

            if(XmlTool.Serialize(xdsDocument, out result))
                return result;

            throw new InvalidOperationException("XdsDocument를 serialize 하지 못했습니다.");
        }

        /// <summary>
        /// <see cref="XdsDocumentBase"/> 형식의 객체를 Xml Serializer를 수행한 후, byte array로 변환한다.
        /// </summary>
        /// <param name="xdsDocument"></param>
        /// <returns></returns>
        public static byte[] ConvertToBytes(this XdsDocumentBase xdsDocument) {
            return ConvertToBytes(xdsDocument, XmlTool.XmlEncoding);
        }

        /// <summary>
        /// 요청문서를 XmlSerializer를 통해 스트림으로 빌드합니다.
        /// </summary>
        /// <param name="requestDocument">요청문서</param>
        /// <param name="enc">Encoding 방식</param>
        /// <returns>요청문서의 Xml 직렬화된 정보를 가진 Stream</returns>
        public static Stream ToStream(this XdsRequestDocument requestDocument, Encoding enc) {
            var stream = new MemoryStream();
            XmlTool.Serialize(requestDocument, stream);

            if(stream.CanSeek && stream.Position != 0)
                stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// 요청 문서를 <see cref="XmlSerializer"/>를 이용해 직렬화를 수행하여 <see cref="XmlDocument"/>로 반환합니다.
        /// </summary>
        /// <param name="requestDocument">요청 문서</param>
        /// <param name="enc">Encoding 방식</param>
        /// <returns>요청문서의 Xml 직렬화된 정보를 가진 <see cref="XmlDocument"/> 인스턴스</returns>
        public static XmlDocument ToXmlDocument(this XdsRequestDocument requestDocument, Encoding enc) {
            XmlDocument document;
            XmlTool.Serialize(requestDocument, out document);

            return document;
        }

        /// <summary>
        /// 응답문서를 XmlSerializer를 통해 스트림으로 빌드합니다.
        /// </summary>
        /// <param name="responseDocument">응답문서</param>
        /// <param name="enc">Encoding 방식</param>
        /// <returns>응답문서의 Xml 직렬화된 정보를 가진 Stream</returns>
        public static Stream ToStream(this XdsResponseDocument responseDocument, Encoding enc) {
            var stream = new MemoryStream();
            XmlTool.Serialize(responseDocument, stream);

            if(stream.CanSeek && stream.Position != 0)
                stream.Position = 0;

            return stream;
        }

        /// <summary>
        /// <see cref="XdsDocumentBase"/>를 상속한 Class의 객체를 Xml Serialize를 수행하여 XDocument 객체로 변환한다.
        /// </summary>
        /// <param name="xds"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XdsDocumentBase xds, Encoding enc) {
            XDocument xdoc;
            if(XmlTool.Serialize(xds, out xdoc))
                return xdoc;

            return new XDocument();
        }

        /// <summary>
        /// 응답 문서를 <see cref="XmlSerializer"/>를 이용해 직렬화를 수행하여 <see cref="XmlDocument"/>로 반환합니다.
        /// </summary>
        /// <param name="responseDocument">응답문서</param>
        /// <param name="enc">Encoding 방식</param>
        /// <returns>요청문서의 Xml 직렬화된 정보를 가진 <see cref="XmlDocument"/> 인스턴스</returns>
        public static XmlDocument ToXmlDocument(this XdsResponseDocument responseDocument, Encoding enc) {
            XmlDocument document;
            if(XmlTool.Serialize(responseDocument, out document))
                return document;

            return new XmlDocument();
        }

        /// <summary>
        /// byte array 정보를 Xml Deserialize를 수행하여 <see cref="XdsRequestDocument"/>로 빌드한다.
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static XdsRequestDocument ConvertToXdsRequestDocument(this byte[] inBytes, Encoding enc) {
            inBytes.ShouldNotBeNull("inBytes");
            return XmlTool.Deserialize<XdsRequestDocument>(inBytes, enc);
        }

        /// <summary>
        /// byte array 정보를 Xml Deserialize를 수행하여 <see cref="XdsRequestDocument"/>로 빌드한다.
        /// </summary>
        /// <param name="inBytes"></param>
        /// <returns></returns>
        public static XdsRequestDocument ConvertToXdsRequestDocument(this byte[] inBytes) {
            return ConvertToXdsRequestDocument(inBytes, XmlTool.XmlEncoding);
        }

        /// <summary>
        /// byte array 정보를 Xml Deserialize를 수행하여 <see cref="XdsResponseDocument"/>로 빌드한다.
        /// </summary>
        /// <param name="inBytes"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static XdsResponseDocument ConvertToXdsResponseDocument(this byte[] inBytes, Encoding enc) {
            inBytes.ShouldNotBeNull("inBytes");
            return XmlTool.Deserialize<XdsResponseDocument>(inBytes, enc);
        }

        /// <summary>
        /// byte array 정보를 Xml Deserialize를 수행하여 <see cref="XdsResponseDocument"/>로 빌드한다.
        /// </summary>
        /// <param name="inBytes"></param>
        /// <returns></returns>
        public static XdsResponseDocument ConvertToXdsResponseDocument(this byte[] inBytes) {
            return ConvertToXdsResponseDocument(inBytes, XmlTool.XmlEncoding);
        }

        /// <summary>
        /// Stream을 Xml Deserialize를 통해 <see cref="XdsRequestDocument"/>로 빌드한다.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="enc">Encoding 방식</param>
        /// <returns>역직렬화된 <see cref="XdsRequestDocument"/> 인스턴스</returns>
        public static XdsRequestDocument ToRequestDocument(this Stream stream, Encoding enc) {
            stream.ShouldNotBeNull("stream");
            return XmlTool.Deserialize<XdsRequestDocument>(stream, enc);
        }

        /// <summary>
        /// Stream을 Xml Deserialize를 통해 <see cref="XdsResponseDocument"/>로 빌드한다.
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="enc">Encoding 방식</param>
        /// <returns>역직렬화된 <see cref="XdsResponseDocument"/> 인스턴스</returns>
        public static XdsResponseDocument ToResponseDocument(this Stream stream, Encoding enc) {
            stream.ShouldNotBeNull("stream");
            return XmlTool.Deserialize<XdsResponseDocument>(stream, enc);
        }
    }
}