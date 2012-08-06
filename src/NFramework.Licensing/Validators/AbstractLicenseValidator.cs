using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.ServiceModel;
using System.Threading;
using System.Xml;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 검증자 기본 클래스
    /// </summary>
    public abstract class AbstractLicenseValidator : ILicenseValidator {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        protected AbstractLicenseValidator(string publicKey) {
            _nextLeaseTimer = new Timer(LeaseLicenseAgain);
            _publicKey = publicKey;
        }

        protected AbstractLicenseValidator(string publicKey, string licenseServerUrl, Guid clientId) {
            _nextLeaseTimer = new Timer(LeaseLicenseAgain);
            _publicKey = publicKey;
            _licenseServerUrl = licenseServerUrl;
            _clientId = clientId;
        }

        /// <summary>
        /// Standard Time servers
        /// </summary>
        protected readonly string[] TimeServers = new[]
                                                  {
                                                      "time.kriss.re.kr",
                                                      "time2.kriss.re.kr",
                                                      "ntp.postech.co.kr",
                                                      "ntp.ewha.or.kr",
                                                      "time.ewha.or.kr",
                                                      "time.nist.gov",
                                                      "time-nw.nist.gov",
                                                      "time-a.nist.gov",
                                                      "time-b.nist.gov",
                                                      "time-a.timefreq.bldrdoc.gov",
                                                      "time-b.timefreq.bldrdoc.gov",
                                                      "time-c.timefreq.bldrdoc.gov",
                                                      "utcnist.colorado.edu",
                                                      "nist1.datum.com",
                                                      "nist1.dc.certifiedtime.com",
                                                      "nist1.nyc.certifiedtime.com",
                                                      "nist1.sjc.certifiedtime.com"
                                                  };

        private readonly string _licenseServerUrl;
        private readonly Guid _clientId;
        private readonly string _publicKey;
        private Timer _nextLeaseTimer;
        private bool _disableFutureCheckes;
        private bool _currentlyValidatingSubscriptionLicense;

        public event Action<InvalidationKind> LicenseInvalidated;

        /// <summary>
        /// 라이선스 갱신일자
        /// </summary>
        public DateTime ExpirationDate { get; private set; }

        /// <summary>
        /// 등록 서비스의 Endpoint 정보
        /// </summary>
        public string SubscriptionEndpoint { get; set; }

        /// <summary>
        /// 라이선스 종류
        /// </summary>
        public LicenseKind LicenseKind { get; private set; }

        /// <summary>
        /// 라이선스 소유자 Id
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// 라이선스 소유자 명
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Floating License 불가 여부
        /// </summary>
        public bool DisableFloatingLicenses { get; set; }

        private IDictionary<string, string> _licenseAttributes;

        /// <summary>
        /// 라이선스 추가 속성 정보
        /// </summary>
        public IDictionary<string, string> LicenseAttributes {
            get { return _licenseAttributes ?? (_licenseAttributes = new Dictionary<string, string>()); }
            protected set { _licenseAttributes = value; }
        }

        /// <summary>
        /// 라이선스 정보
        /// </summary>
        protected abstract string License { get; set; }

        private void LeaseLicenseAgain(object state) {
            if(HasExistingLicense())
                return;

            RaiseLicenseInvalidated();
        }

        private void RaiseLicenseInvalidated() {
            var licenseInvalidated = LicenseInvalidated;

            if(licenseInvalidated == null)
                throw new InvalidOperationException(
                    "License was invalidated. but there is no one subscribe to the LicenseInvalidated event");

            licenseInvalidated(GetInvalidationKind());
        }

        private InvalidationKind GetInvalidationKind() {
            return LicenseKind == LicenseKind.Floating
                       ? InvalidationKind.CannotGetNewLicense
                       : InvalidationKind.TimeExpired;
        }

        /// <summary>
        /// 로드된 라이선스를 검증합니다.
        /// </summary>
        public virtual void AssertValidLicense() {
            LicenseAttributes.Clear();

            if(HasExistingLicense())
                return;

            if(log.IsWarnEnabled)
                log.Warn("Could not validatte existing license:\r\n{0}", License);

            throw new LicenseNotFoundException();
        }

        private bool HasExistingLicense() {
            try {
                if(TryLoadingLicenseValuesFromValidatedXml() == false) {
                    if(log.IsWarnEnabled)
                        log.Warn("Fail validating license:\r\n{0}", License);
                    return false;
                }

                if(log.IsInfoEnabled)
                    log.Info("License expiration date is [{0}]", ExpirationDate);

                var result = (LicenseKind == LicenseKind.Subscription)
                                 ? ValidateSubscription()
                                 : DateTime.UtcNow < ExpirationDate;

                if(result)
                    ValidateUsingNetworkTime();
                else
                    throw new LicenseExpiredException("Expiration Date: " + ExpirationDate);

                return true;
            }
            catch(LicensingException) {
                throw;
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("라이선스 정보를 찾는데, 예외가 발생했습니다.", ex);

                return false;
            }
        }

        private bool ValidateSubscription() {
            if((ExpirationDate - DateTime.UtcNow).TotalDays > 4)
                return true;

            if(_currentlyValidatingSubscriptionLicense)
                return DateTime.UtcNow < ExpirationDate;

            if(SubscriptionEndpoint == null)
                throw new InvalidOperationException("Subscription endpoints are not supported for this license validator");

            try {
                TryGettingNewLeaseSubscription();
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("Could not re-lease subscription license.", ex);
            }

            return ValidateWithoutUsingSubscriptionLeasing();
        }

        private bool ValidateWithoutUsingSubscriptionLeasing() {
            _currentlyValidatingSubscriptionLicense = true;

            try {
                return HasExistingLicense();
            }
            finally {
                _currentlyValidatingSubscriptionLicense = false;
            }
        }

        private void TryGettingNewLeaseSubscription() {
            var service =
                ChannelFactory<ISubscriptionLicensingService>
                    .CreateChannel(new BasicHttpBinding(),
                                   new EndpointAddress(SubscriptionEndpoint));

            try {
                var newLicense = service.LeaseLicense(License);
                TryOverwritingWithNewLicense(newLicense);
            }
            finally {
                var communicationObject = service as ICommunicationObject;

                if(communicationObject != null) {
                    try {
                        communicationObject.Close(TimeSpan.FromMilliseconds(200));
                    }
                    catch {
                        communicationObject.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// 라이선스 파일을 로드합니다.
        /// </summary>
        /// <param name="newLicense"></param>
        /// <returns></returns>
        protected bool TryOverwritingWithNewLicense(string newLicense) {
            if(string.IsNullOrEmpty(newLicense))
                return false;

            try {
                var xdoc = new XmlDocument();
                xdoc.LoadXml(newLicense);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled)
                    log.ErrorException("New license is not valid XML", ex);

                return false;
            }

            License = newLicense;
            return true;
        }

        private void ValidateUsingNetworkTime() {
            if(NetworkInterface.GetIsNetworkAvailable() == false)
                return;

            var sntp = new SntpClient(TimeServers);

            sntp.BeginGetDate(time => {
                                  if(time > ExpirationDate)
                                      RaiseLicenseInvalidated();
                              },
                              () => {
                                  // ignore
                              });
        }

        /// <summary>
        /// 기존 라이선스 정보를 제거합니다.
        /// </summary>
        public virtual void RemoveExistingLicense() {
            // Nothing to do
        }

        /// <summary>
        /// 유효한 라이선스 파일로부터 라이선스 정보를 로드합니다.
        /// </summary>
        /// <returns></returns>
        public bool TryLoadingLicenseValuesFromValidatedXml() {
            try {
                var doc = new XmlDocument();
                doc.LoadXml(License);

                if(TryGetVaidateDocument(_publicKey, doc) == false) {
                    if(log.IsWarnEnabled)
                        log.Warn("지정한 XML 정보로 라이선스 유효성을 검증할 수 없습니다.{0}{1}", Environment.NewLine, License);

                    return false;
                }

                if(doc.FirstChild == null) {
                    if(log.IsWarnEnabled)
                        log.Warn("XML 문서의 첫번째 자식 노드를 찾을 수 없습니다.{0}{1}", Environment.NewLine, License);

                    return false;
                }

                if(doc.SelectSingleNode("/" + LicensingSR.FloatingLicense) != null) {
                    var node =
                        doc.SelectSingleNode("/" + LicensingSR.FloatingLicense + "/" + LicensingSR.LicenseServerPublicKey + "/text()");
                    if(node == null) {
                        var msg = string.Format("라이선스가 유효하지 않습니다. floating license가 license server public key:{0}{1}",
                                                Environment.NewLine, License);
                        if(log.IsWarnEnabled)
                            log.Warn(msg);

                        throw new InvalidOperationException(msg);
                    }
                    return ValidateFloatingLicense(node.InnerText);
                }

                var result = ValidateXmlDocumentLicense(doc);
                if(result && _disableFutureCheckes == false) {
                    _nextLeaseTimer.Change(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
                }

                return result;
            }
            catch(LicensingException) {
                throw;
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("올바른 라이선스가 아닙니다.", ex);

                return false;
            }
        }

        private bool ValidateFloatingLicense(string publicKeyOfFloatingLicense) {
            if(DisableFloatingLicenses) {
                if(log.IsWarnEnabled)
                    log.Warn("Floating 라이선스는 사용할 수 없습니다.");

                return false;
            }

            if(string.IsNullOrEmpty(_licenseServerUrl)) {
                if(log.IsWarnEnabled)
                    log.Warn("License Server Url 이 지정되지 않았습니다.");
                throw new InvalidOperationException("Floating license를 찾았습니다. 하지만 licenseServerUrl이 지정되지 않았습니다.");
            }

            var success = false;
            var licensingService = ChannelFactory<ILicensingService>.CreateChannel(new WSHttpBinding(),
                                                                                   new EndpointAddress(_licenseServerUrl));

            try {
                var leasedLicense = licensingService.LeaseLicense(Environment.NewLine,
                                                                  Environment.UserName,
                                                                  _clientId);

                ((ICommunicationObject)licensingService).Close();
                success = true;

                if(leasedLicense == null) {
                    var msg = string.Format("서버로부터 NULL 응답이 왔습니다.서버=[{0}]", _licenseServerUrl);
                    if(log.IsWarnEnabled)
                        log.Warn(msg);

                    throw new FloatingLicenseNotAvailableException(msg);
                }

                var doc = new XmlDocument();
                doc.LoadXml(leasedLicense);


                if(TryGetVaidateDocument(publicKeyOfFloatingLicense, doc) == false) {
                    var msg = string.Format("응답받은 라이선스가 유효하지 않습니다.서버=[{0}]", _licenseServerUrl);
                    if(log.IsWarnEnabled)
                        log.Warn(msg);

                    throw new FloatingLicenseNotAvailableException(msg);
                }

                var validLicense = ValidateXmlDocumentLicense(doc);
                if(validLicense) {
                    var time = ExpirationDate.AddMinutes(-5) - DateTime.UtcNow;

                    if(IsDebugEnabled)
                        log.Debug("라이선스를 [{0}]애 갱신할 것입니다.", time);

                    if(_disableFutureCheckes == false)
                        _nextLeaseTimer.Change(time, time);
                }
                return validLicense;
            }
            finally {
                if(success == false && licensingService != null)
                    ((ICommunicationObject)licensingService).Abort();
            }
        }

        internal bool ValidateXmlDocumentLicense(XmlDocument doc) {
            XmlNode idNode = doc.SelectSingleNode(LicensingSR.LicenseIdAttribute);

            if(idNode == null) {
                if(log.IsWarnEnabled)
                    log.Warn("License Id를 찾지 못했습니다.{0}{1}", Environment.NewLine, License);

                return false;
            }

            UserId = new Guid(idNode.Value);

            XmlNode expirationNode = doc.SelectSingleNode(LicensingSR.LicenseExpirationAttribute);

            if(expirationNode == null) {
                if(log.IsWarnEnabled)
                    log.Warn("License의 Expiration를 찾지 못했습니다.{0}{1}", Environment.NewLine, License);

                return false;
            }

            ExpirationDate = DateTime.Parse(expirationNode.Value, CultureInfo.InvariantCulture);

            var licenseTypeNode = doc.SelectSingleNode(LicensingSR.LicenseKindAttribute);
            if(licenseTypeNode == null) {
                if(log.IsWarnEnabled)
                    log.Warn("License 종류를 찾지 못했습니다.{0}{1}", Environment.NewLine, License);

                return false;
            }

            LicenseKind = (LicenseKind)Enum.Parse(typeof(LicenseKind), licenseTypeNode.Value);

            var nameNode = doc.SelectSingleNode(LicensingSR.LicenseNameElement);
            if(nameNode == null) {
                if(log.IsWarnEnabled)
                    log.Warn("License Name를 찾지 못했습니다.{0}{1}", Environment.NewLine, License);

                return false;
            }

            Name = nameNode.Value;

            var license = doc.SelectSingleNode("/license");
            foreach(XmlAttribute attr in license.Attributes) {
                if(attr.Name == "type" || attr.Name == "expiration" || attr.Name == "id")
                    continue;

                LicenseAttributes[attr.Name] = attr.Value;
            }

            return true;
        }

        private bool TryGetVaidateDocument(string licensePublicKey, XmlDocument doc) {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(licensePublicKey);

            var nsMgr = new XmlNamespaceManager(doc.NameTable);
            nsMgr.AddNamespace("sig", "http://www.w3.org/2000/09/xmldsig#");

            var signedXml = new SignedXml(doc);
            var sig = (XmlElement)doc.SelectSingleNode("//sig:Signature", nsMgr);

            if(sig == null) {
                if(log.IsWarnEnabled)
                    log.Warn("License에서 signature node를 찾지 못했습니다.{0}{1}", Environment.NewLine, License);

                return false;
            }
            signedXml.LoadXml(sig);

            return signedXml.CheckSignature(rsa);
        }

        public void DisableFutureChecks() {
            _disableFutureCheckes = true;

            if(_nextLeaseTimer != null) {
                _nextLeaseTimer.Dispose();
                _nextLeaseTimer = null;
            }
        }
    }
}