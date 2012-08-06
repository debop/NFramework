using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace NSoft.NFramework.Licensing {
    /// <summary>
    /// 라이선스 서버 구현체입니다.
    /// ServiceBehavior 를 Singleton 이면서 <see cref="ConcurrencyMode.Single"/>을 지정하여, 멀티스레드에 대해 안정정으로 작동할 수 있도록 했습니다.
    /// 다만 대량의 요청에 대해서는 부하가 걸릴 수 있습니다.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class LicensingService : ILicensingService {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly List<LicenseValidator> _availableLicenses = new List<LicenseValidator>();

        private readonly Dictionary<string, KeyValuePair<DateTime, LicenseValidator>> _leasedLicenses =
            new Dictionary<string, KeyValuePair<DateTime, LicenseValidator>>();

        private readonly string _state;

        /// <summary>
        /// 생성자
        /// </summary>
        public LicensingService() {
            if(log.IsInfoEnabled)
                log.Info("LicensingService 인스턴스를 생성합니다...");

            if(SoftwarePublicKey == null)
                throw new InvalidOperationException("LicensingService를 시작하기 전에 SoftwarePublicKey 값이 설정되어 있어야 합니다.");

            if(string.IsNullOrEmpty(LicenseServerPrivateKey))
                throw new InvalidOperationException("LicensingService를 시작하기 전에 LicenseServerPrivateKey 값이 설정되어 있어야 합니다.");

            var licensesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Licenses");
            _state = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "licenseServer.State");

            if(log.IsInfoEnabled) {
                log.Info("License Directory=[{0}]", licensesDirectory);
                log.Info("License State Directory = [{0}]", _state);
            }

            EnsureLicenseDirectoryExists(licensesDirectory);
            ReadAvailableLicenses(licensesDirectory);

            ReadInitialState();

            if(log.IsInfoEnabled)
                log.Info("LicensingService 인스턴스 생성을 완료했습니다!!!");
        }

        /// <summary>
        /// 제품의 공개 키
        /// </summary>
        public static string SoftwarePublicKey { get; set; }

        /// <summary>
        /// 라이선스 서버의 Private Key (비밀번호)
        /// </summary>
        public static string LicenseServerPrivateKey { get; set; }

        private static void EnsureLicenseDirectoryExists(string licensesDirectory) {
            if(string.IsNullOrEmpty(licensesDirectory))
                throw new ArgumentNullException("licensesDirectory");

            if(IsDebugEnabled)
                log.Debug("라이선스 디렉토리가 존재하는지 확인합니다. licenseDirectory=[{0}]", licensesDirectory);

            if(Directory.Exists(licensesDirectory) == false) {
                try {
                    Directory.CreateDirectory(licensesDirectory);

                    if(IsDebugEnabled)
                        log.Debug("새로운 라이선스 디렉토리를 생성했습니다. licenseDirectory=[{0}]", licensesDirectory);
                }
                catch(Exception ex) {
                    throw new DirectoryNotFoundException("라이선스 디렉토리를 찾을 수 없습니다. licenseDirectory=" + licensesDirectory, ex);
                }
            }
        }

        private void ReadAvailableLicenses(string licensesDirectory) {
            if(string.IsNullOrEmpty(licensesDirectory))
                throw new ArgumentNullException("licensesDirectory");

            if(IsDebugEnabled)
                log.Debug("유효한 라이선스 파일들을 읽습니다. licenseDirectory=[{0}], 라이선스 파일=*.xml", licensesDirectory);

            foreach(var license in Directory.GetFiles(licensesDirectory, "*.xml")) {
                var set = new HashSet<Guid>();
                var validator = new LicenseValidator(SoftwarePublicKey, license)
                                {
                                    DisableFloatingLicenses = true
                                };
                try {
                    validator.AssertValidLicense();
                    if(IsDebugEnabled)
                        log.Debug("Found license for [{0}] of type [{1}]", validator.Name, validator.LicenseKind);

                    var isNewLicense = validator.LicenseKind == LicenseKind.Standard && set.Add(validator.UserId);
                    if(isNewLicense) {
                        _availableLicenses.Add(validator);
                        if(IsDebugEnabled)
                            log.Debug("라이선스를 접수했습니다. 라이선스 소유자 명=[{0}], 소유자 Id=[{1}]", validator.Name, validator.UserId);
                    }
                }
                catch(Exception ex) {
                    if(log.IsWarnEnabled)
                        log.WarnException("라이선스 검증에 실패했습니다. license=" + license, ex);
                    continue;
                }
            }
        }

        private void ReadInitialState() {
            if(IsDebugEnabled)
                log.Debug("파일 [{0}] 의 내용을 읽습니다.", _state);

            try {
                using(var file = new FileStream(_state, FileMode.OpenOrCreate, FileAccess.ReadWrite)) {
                    ReadState(file);
                }
            }
            catch(AccessViolationException ave) {
                var msg = string.Format("파일[{0}]를 read/write 모드로 열 수 없습니다. 파일에 read/write 접근 권한을 주시기 바랍니다.", _state);
                throw new AccessViolationException(msg, ave);
            }
            catch(Exception ex) {
                throw new InvalidOperationException("파일을 인식할 수 없습니다. 파일=" + _state, ex);
            }
        }

        private void ReadState(Stream stream) {
            if(IsDebugEnabled)
                log.Debug("라이선스 파일로부터 정보를 읽어드립니다...");

            try {
                using(var reader = new BinaryReader(stream)) {
                    while(true) {
                        var identifier = reader.ReadString();
                        var time = DateTime.FromBinary(reader.ReadInt64());
                        var userId = new Guid(reader.ReadBytes(16));

                        if(IsDebugEnabled)
                            log.Debug("읽은 정보... identifier=[{0}], time=[{1}], userId=[{2}]", identifier, time, userId);

                        var licenseValidator = _availableLicenses.FirstOrDefault(x => x.UserId == userId);
                        if(licenseValidator == null) {
                            if(IsDebugEnabled)
                                log.Debug("유효한 LicenseValidator가 없습니다. 다음 정보를 읽습니다...");
                            continue;
                        }

                        _leasedLicenses[identifier] = new KeyValuePair<DateTime, LicenseValidator>(time, licenseValidator);
                        _availableLicenses.Remove(licenseValidator);
                    }
                }
            }
            catch(EndOfStreamException ex) {
                if(log.IsWarnEnabled)
                    log.WarnException("스트림의 끝에 도달했습니다. 예외를 무시합니다.", ex);
            }
        }

        private void WriteState(Stream stream) {
            if(IsDebugEnabled)
                log.Debug("라이선스 파일에 라이선스 정보를 씁니다...");

            using(var writer = new BinaryWriter(stream)) {
                foreach(var pair in _leasedLicenses) {
                    writer.Write(pair.Key); // identifier
                    writer.Write(pair.Value.Key.ToBinary()); // time
                    writer.Write(pair.Value.Value.UserId.ToByteArray()); // userId
                }
                writer.Flush();
                stream.Flush();
            }
        }

        /// <summary>
        /// 사용자에게 만료된 Floating 라이선스를 제공합니다.
        /// </summary>
        /// <param name="machine">machine name</param>
        /// <param name="user">user name</param>
        /// <param name="id">Id of the license holder</param>
        /// <returns></returns>
        public string LeaseLicense(string machine, string user, Guid id) {
            KeyValuePair<DateTime, LicenseValidator> value;
            var identifier = string.Format("{0}\\{1} :{2}", machine, user, id);

            if(_leasedLicenses.TryGetValue(identifier, out value)) {
                if(IsDebugEnabled)
                    log.Debug("id [{0}] 가 이미 있습니다, 만료기간을 연장합니다.", id);
                var licenseValidator = value.Value;
                return GenerateLicenseAndRenewLease(identifier, id, licenseValidator, value.Value.LicenseAttributes);
            }
            if(_availableLicenses.Count > 0) {
                var availableLicense = _availableLicenses.Last();
                _availableLicenses.RemoveAt(_availableLicenses.Count - 1);

                if(IsDebugEnabled)
                    log.Debug("유효한 라이선스를 찾았습니다.");
                return GenerateLicenseAndRenewLease(identifier, id, availableLicense, availableLicense.LicenseAttributes);
            }

            foreach(var kvp in _leasedLicenses) {
                if((DateTime.UtcNow - kvp.Value.Key).TotalMinutes < 45)
                    continue;

                _leasedLicenses.Remove(kvp.Key);
                if(IsDebugEnabled)
                    log.Debug("만료된 라이선스를 찾았습니다. 라이선스를 임대합니다... id=[{0}]", kvp.Key);

                return GenerateLicenseAndRenewLease(identifier, id, kvp.Value.Value, kvp.Value.Value.LicenseAttributes);
            }

            if(IsDebugEnabled)
                log.Debug("임대할 라이선스를 찾지 못했습니다.");
            return null;
        }

        private string GenerateLicenseAndRenewLease(string identifier, Guid id, LicenseValidator licenseValidator,
                                                    IDictionary<string, string> attributes) {
            _leasedLicenses[identifier] = new KeyValuePair<DateTime, LicenseValidator>(DateTime.UtcNow.AddMinutes(30), licenseValidator);
            using(var file = new FileStream(_state, FileMode.Create, FileAccess.ReadWrite)) {
                WriteState(file);
            }
            return GenerateLicense(id, licenseValidator, attributes);
        }

        private static string GenerateLicense(Guid id, LicenseValidator validator, IDictionary<string, string> attributes) {
            return
                new LicenseGenerator(LicenseServerPrivateKey)
                    .Generate(validator.Name,
                              id,
                              DateTime.UtcNow.AddMinutes(45),
                              attributes,
                              LicenseKind.Floating);
        }
    }
}