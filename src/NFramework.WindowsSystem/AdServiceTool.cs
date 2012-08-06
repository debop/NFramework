using System;
using System.DirectoryServices;

namespace NSoft.NFramework.WindowsSystem {
    /// <summary>
    /// Active Directory 관련 Utility Class
    /// </summary>
    public static class AdServiceTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Scheme for LDAP (LDAP://)
        /// </summary>
        public const string DEFAULT_PREFIX = "LDAP://";

        /// <summary>
        /// Active Directory 서버에서 사용자 인증을 수행한다.
        /// </summary>
        /// <param name="adPath">AD 서버 경로 (ex: LDAP://ServerName )</param>
        /// <param name="username">사용자 Id</param>
        /// <param name="password">사용자 비밀번호</param>
        /// <returns>인증 성공 여부</returns>
        public static bool Authenticate(string adPath, string username, string password) {
            return Authenticate(adPath, username, password, AuthenticationTypes.None);
        }

        /// <summary>
        /// Active Directory 서버에서 사용자 인증을 수행한다.
        /// </summary>
        /// <param name="adPath">AD 서버 경로 (ex: LDAP://ServerName )</param>
        /// <param name="username">사용자 Id</param>
        /// <param name="password">사용자 비밀번호</param>
        /// <param name="authType">인증 형식</param>
        /// <returns>인증 성공 여부</returns>
        public static bool Authenticate(string adPath, string username, string password, AuthenticationTypes authType) {
            adPath.ShouldNotBeWhiteSpace("adPath");
            username.ShouldNotBeWhiteSpace("username");

            if(IsDebugEnabled)
                log.Debug("Authenticate with adPath=[{0}], username=[{1}], password=[{2}], authType=[{3}]",
                          adPath, username, password, authType);

            bool result = false;
            DirectoryEntry entry = null;

            try {
                entry = new DirectoryEntry(adPath, username, password, authType);

                var nativeObject = entry.NativeObject;
                result = (nativeObject != null);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("AD DirectoryEntry 조회에 실패했습니다. adPath=[{0}]", adPath);
                    log.Warn(ex);
                }

                result = false;
            }
            finally {
                if(entry != null)
                    entry.Dispose();
            }

            if(IsDebugEnabled)
                log.Debug("Authenticate with adPath=[{0}], username=[{1}], password=[{2}], authType=[{3}], result=[{4}]",
                          adPath, username, password, authType, result);

            return result;
        }
    }
}