using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// License 관련 Utility 클래스
    /// </summary>
    public static class LicenseTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Floating 라이선스를 생성합니다.
        /// 참고 : http://en.wikipedia.org/wiki/Floating_licensing
        /// </summary>
        /// <param name="privateKey">제품의 Private Key</param>
        /// <param name="name">라이선스 명</param>
        /// <param name="publicKey">제품의 Public Key</param>
        /// <returns>Floating License의 XML 문자열</returns>
        public static string GenerateFloatingLicense(string privateKey, string name, string publicKey) {
            if(IsDebugEnabled)
                log.Debug("Floating License를 생성합니다... privateKey=[{0}], name=[{1}], publicKey=[{2}]", privateKey, name, publicKey);

            using(var rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(privateKey);

                var doc = new XmlDocument();
                var licenseElement = doc.CreateElement(LicensingSR.FloatingLicense);
                doc.AppendChild(licenseElement);

                var publicKeyElement = doc.CreateElement(LicensingSR.LicenseServerPublicKey);
                licenseElement.AppendChild(publicKeyElement);
                publicKeyElement.InnerText = publicKey;

                var nameElement = doc.CreateElement(LicensingSR.LicenseName);
                licenseElement.AppendChild(nameElement);
                nameElement.InnerText = name;

                var signatureElement = GetXmlDigitalSignature(doc, rsa);
                doc.FirstChild.AppendChild(doc.ImportNode(signatureElement, true));

                using(var ms = new MemoryStream())
                using(var xw = XmlWriter.Create(ms, new XmlWriterSettings
                                                    {
                                                        Indent = true,
                                                        Encoding = Encoding.UTF8
                                                    })) {
                    doc.Save(xw);
                    ms.Position = 0;
                    return new StreamReader(ms).ReadToEnd();
                }
            }
        }

        internal static XmlElement GetXmlDigitalSignature(XmlDocument doc, AsymmetricAlgorithm key) {
            if(IsDebugEnabled)
                log.Debug("암호화된 Xml 문서를 만듭니다...");

            var signedXml = new SignedXml(doc) { SigningKey = key };
            var reference = new Reference { Uri = string.Empty };

            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(reference);
            signedXml.ComputeSignature();

            return signedXml.GetXml();
        }

        /// <summary>
        /// 새로운 라이선스를 생성합니다.
        /// </summary>
        /// <param name="privateKey">제품의 Private Key</param>
        /// <param name="name">라이선스 소유자 명</param>
        /// <param name="id">라이선스 소유자 Id</param>
        /// <param name="expirationDate">라이선스 유효기간</param>
        /// <param name="attributes">라이선스 속성</param>
        /// <param name="licenseKind">라이선스 종류</param>
        /// <returns>라이선스 내용</returns>
        public static string GenerateLicense(string privateKey, string name, Guid id, DateTime expirationDate,
                                             IDictionary<string, string> attributes, LicenseKind licenseKind) {
            if(IsDebugEnabled)
                log.Debug(
                    "Floating License를 생성합니다... privateKey=[{0}], name=[{1}], id=[{2}], expirationDate=[{3}], attributes=[{4}], licenseKind=[{5}]",
                    name, id, expirationDate, attributes, licenseKind);

            using(var rsa = new RSACryptoServiceProvider()) {
                rsa.FromXmlString(privateKey);
                var doc = CreateDocument(id, name, expirationDate, attributes, licenseKind);

                var signatureElement = GetXmlDigitalSignature(doc, rsa);
                doc.FirstChild.AppendChild(doc.ImportNode(signatureElement, true));

                using(var ms = new MemoryStream())
                using(var xw = XmlWriter.Create(ms, new XmlWriterSettings
                                                    {
                                                        Indent = true,
                                                        Encoding = Encoding.UTF8
                                                    })) {
                    doc.Save(xw);
                    ms.Position = 0;
                    return new StreamReader(ms).ReadToEnd();
                }
            }
        }

        internal static XmlDocument CreateDocument(Guid id, string name, DateTime expirationDate, IDictionary<string, string> attributes,
                                                   LicenseKind licenseKind) {
            var doc = new XmlDocument();

            var license = doc.CreateElement(LicensingSR.License);
            doc.AppendChild(license);

            var idAttr = doc.CreateAttribute(LicensingSR.LicenseId);
            license.Attributes.Append(idAttr);
            idAttr.Value = id.ToString();

            var expirDateAttr = doc.CreateAttribute(LicensingSR.LicenseExpiration);
            license.Attributes.Append(expirDateAttr);
            expirDateAttr.Value = expirationDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture);

            var licenseAttr = doc.CreateAttribute(LicensingSR.LicenseKind);
            license.Attributes.Append(licenseAttr);
            licenseAttr.Value = licenseKind.ToString();

            var nameEl = doc.CreateElement(LicensingSR.LicenseName);
            license.AppendChild(nameEl);
            nameEl.InnerText = name;

            foreach(var attribute in attributes) {
                var attrib = doc.CreateAttribute(attribute.Key);
                attrib.Value = attribute.Value;
                license.Attributes.Append(attrib);
            }

            return doc;
        }
    }
}