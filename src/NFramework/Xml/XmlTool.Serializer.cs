using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using NSoft.NFramework.IO;

namespace NSoft.NFramework.Xml {
    public static partial class XmlTool {
        /// <summary>
        /// 주어진 객체를 Xml 직렬화하여 Stream 객체에 쓴다.
        /// </summary>
        /// <param name="target">직렬화할 개체</param>
        /// <param name="enc">인코딩 방식</param>
        /// <param name="useIndent">들여쓰기를 이용한 포맷 사용여부</param>
        /// <param name="outStream">직렬화한 정보가 담길 stream</param>
        /// <exception cref="ArgumentNullException">target이 null이거나 stream이 null인 경우</exception>
        public static void Serialize(object target, Stream outStream, Encoding enc, bool useIndent) {
            target.ShouldNotBeNull("target");
            outStream.ShouldNotBeNull("outStream");

            if(IsDebugEnabled)
                log.Debug("Xml 직렬화를 시작합니다... target=[{0}], encoding=[{1}], useIndent=[{2}]", target.GetType(), enc, useIndent);

            try {
                var settings = new XmlWriterSettings
                               {
                                   Encoding = enc,
                                   Indent = useIndent
                               };

                var writer = XmlWriter.Create(outStream, settings);
                var serializer = new XmlSerializer(target.GetType());

                lock(target) {
                    serializer.Serialize(writer, target);
                    writer.Flush();
                }
                writer.Close();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("직렬화에 실패했습니다!!! target=[{0}]", target);
                    log.Error(ex);
                }
                throw;
            }
        }

        /// <summary>
        /// 객체를 Xml 직렬화하여 outStream에 쓴다.
        /// </summary>
        /// <param name="target">직렬화할 객체</param>
        /// <param name="outStream">직렬화된 정보를 쓸 Stream</param>
        /// <param name="enc">Stream에 쓸때 사용할 인코딩 방식</param>
        public static void Serialize(object target, Stream outStream, Encoding enc) {
            Serialize(target, outStream, enc, false);
        }

        /// <summary>
        /// 객체를 Xml 직렬화하여 outStream에 쓴다.
        /// </summary>
        /// <param name="target">직렬화할 객체</param>
        /// <param name="outStream">직렬화된 정보를 쓸 Stream</param>
        /// <param name="useIndent">들여쓰기 사용 여부</param>
        public static void Serialize(object target, Stream outStream, bool useIndent) {
            Serialize(target, outStream, XmlEncoding, useIndent);
        }

        /// <summary>
        /// 객체를 Xml 직렬화하여 outStream에 쓴다.
        /// </summary>
        /// <param name="target">직렬화할 객체</param>
        /// <param name="outStream">직렬화된 정보를 쓸 Stream</param>
        public static void Serialize(object target, Stream outStream) {
            Serialize(target, outStream, false);
        }

        /// <summary>
        /// 객체를 XmlSerialize를 수행하여 Byte 배열로 변환한다.
        /// </summary>
        /// <param name="target">직렬화할 객체</param>
        /// <param name="outBytes">직렬화된 데이타</param>
        public static bool Serialize(object target, out byte[] outBytes) {
            target.ShouldNotBeNull("target");

            using(var ms = new MemoryStream()) {
                Serialize(target, ms);
                ms.SetStreamPosition();

                outBytes = ms.ToArray();
            }
            return true;
        }

        /// <summary>
        /// 객체를 XML 직렬화하여 <see cref="XDocument"/> 객체로 변환한다.
        /// </summary>
        /// <param name="target">직렬화할 객체</param>
        /// <param name="xdoc">직렬화된 객체 정보를 담은 <see cref="XDocument"/>의 인스턴스</param>
        public static bool Serialize(object target, out XDocument xdoc) {
            target.ShouldNotBeNull("target");

            xdoc = null;
            var stream = new MemoryStream();
            {
                Serialize(target, stream, true);
                stream.SetStreamPosition();

                xdoc = XDocument.Load(XmlReader.Create(stream));
            }
            return (xdoc.Root != null);
        }

        /// <summary>
        /// 객체를 직렬화하여 <see cref="XmlDocument"/> 객체로 변환한다.
        /// </summary>
        /// <param name="target">직렬화할 객체</param>
        /// <param name="document">직렬화 정보를 담은 <see cref="XmlDocument"/></param>
        public static bool Serialize(object target, out XmlDocument document) {
            using(var ms = new MemoryStream()) {
                Serialize(target, ms);
                ms.SetStreamPosition();

                document = ms.CreateXmlDocument();
            }
            return (document != null && document.IsValidDocument());
        }

        /// <summary>
        /// 지정된 Stream을 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="stream">직렬화 정보를 가진 Stream</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, Stream stream) {
            return Deserialize(targetType, stream, XmlEncoding);
        }

        /// <summary>
        /// 지정된 Stream을 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="stream">직렬화 정보를 가진 Stream</param>
        /// <param name="enc">XML 인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, Stream stream, Encoding enc) {
            stream.ShouldNotBeNull("stream");

            if(IsDebugEnabled)
                log.Debug("Deserialize stream... stream=[{0}], enc=[{1}]", stream, enc);

            stream.SetStreamPosition(0);

            var serializer = new XmlSerializer(targetType);
            return serializer.Deserialize(new StreamReader(stream));
        }

        /// <summary>
        /// byte array를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="inBytes">직렬화 정보를 가진 byte array.</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, byte[] inBytes) {
            return Deserialize(targetType, inBytes, XmlEncoding);
        }

        /// <summary>
        /// byte array를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="inBytes">직렬화 정보를 가진 byte array.</param>
        /// <param name="enc">XML 인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, byte[] inBytes, Encoding enc) {
            using(var ms = new MemoryStream(inBytes)) {
                return Deserialize(targetType, ms, enc);
            }
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="xdoc">직렬화 정보를 가진 <see cref="XDocument"/>인스턴스</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, XDocument xdoc) {
            return Deserialize(targetType, xdoc, XmlEncoding);
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="xdoc">직렬화 정보를 가진 <see cref="XDocument"/>인스턴스</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, XDocument xdoc, Encoding enc) {
            var settings = new XmlWriterSettings
                           {
                               Encoding = enc ?? XmlEncoding
                           };

            using(var stream = new MemoryStream())
            using(var writer = XmlWriter.Create(stream, settings)) {
                xdoc.Save(writer);
                writer.Flush();
                return Deserialize(targetType, stream);
            }
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="document">직렬화 정보를 가진 XmlDocument 인스턴스</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, XmlDocument document) {
            return Deserialize(targetType, document, XmlEncoding);
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <param name="targetType">역직렬화해서 만들 객체의 수형</param>
        /// <param name="document">직렬화 정보를 가진 XmlDocument 인스턴스</param>
        /// <param name="enc">XML 인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static object Deserialize(Type targetType, XmlDocument document, Encoding enc) {
            document.CheckDocument();

            using(var ms = new MemoryStream()) {
                document.Save(ms);
                return Deserialize(targetType, ms, enc);
            }
        }

        //! ========================================================

        /// <summary>
        /// 지정된 Stream을 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="stream">직렬화 정보를 가진 Stream</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(Stream stream) {
            return Deserialize<T>(stream, XmlEncoding);
        }

        /// <summary>
        /// 지정된 Stream을 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="stream">직렬화 정보를 가진 Stream</param>
        /// <param name="enc">XML 인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(Stream stream, Encoding enc) {
            return (T)Deserialize(typeof(T), stream, enc);
        }

        /// <summary>
        /// byte array를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="inBytes">직렬화 정보를 가진 byte array.</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(byte[] inBytes) {
            return Deserialize<T>(inBytes, XmlEncoding);
        }

        /// <summary>
        /// byte array를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="inBytes">직렬화 정보를 가진 byte array.</param>
        /// <param name="enc">XML 인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(byte[] inBytes, Encoding enc) {
            using(var ms = new MemoryStream(inBytes)) {
                return Deserialize<T>(ms, enc);
            }
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="xdoc">직렬화 정보를 가진 <see cref="XDocument"/>인스턴스</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(XDocument xdoc) {
            return Deserialize<T>(xdoc, XmlEncoding);
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="xdoc">직렬화 정보를 가진 <see cref="XDocument"/>인스턴스</param>
        /// <param name="enc">인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(XDocument xdoc, Encoding enc) {
            var settings = new XmlWriterSettings
                           {
                               Encoding = enc ?? XmlEncoding
                           };

            using(var stream = new MemoryStream())
            using(var writer = XmlWriter.Create(stream, settings)) {
                xdoc.Save(writer);
                writer.Flush();
                return Deserialize<T>(stream);
            }
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="document">직렬화 정보를 가진 XmlDocument 인스턴스</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(XmlDocument document) {
            return Deserialize<T>(document, XmlEncoding);
        }

        /// <summary>
        /// XmlDocument 를 읽어서 Xml Deserialize를 수행한다.
        /// </summary>
        /// <typeparam name="T">역직렬화해서 만들 객체의 수형</typeparam>
        /// <param name="document">직렬화 정보를 가진 XmlDocument 인스턴스</param>
        /// <param name="enc">XML 인코딩 방식</param>
        /// <returns>역직렬화된 객체</returns>
        public static T Deserialize<T>(XmlDocument document, Encoding enc) {
            document.CheckDocument();

            using(var ms = new MemoryStream()) {
                document.Save(ms);
                return Deserialize<T>(ms, enc);
            }
        }
    }
}