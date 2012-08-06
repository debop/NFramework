using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace NSoft.NFramework.Web.Tools
{
    /// <summary>
    /// 네트웍크 드라이브 관련 Util
    /// </summary>
    public class NetworkDriveTool
    {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        #region << PIvoke >>

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct NETRESOURCE
        {
            public uint dwScope;
            public uint dwType;
            public uint dwDisplayType;
            public uint dwUsage;
            public string lpLocalName;
            public string lpRemoteName;
            public string lpComment;
            public string lpProvider;
        }

        [DllImport("mpr.dll", CharSet = CharSet.Auto)]
        internal static extern int WNetUseConnection(IntPtr hwndOwner,
                                                   [MarshalAs(UnmanagedType.Struct)] ref NETRESOURCE lpNetResource,
                                                   string lpPassword,
                                                   string lpUserID,
                                                   uint dwFlags,
                                                   StringBuilder lpAccessName,
                                                   ref int lpBufferSize,
                                                   out uint lpResult);

        #endregion

        /// <summary>
        /// 공유 폴더에 대한 네트워크 연결을 만든다.
        /// </summary>
        /// <param name="remoteAccessUri">원격 공유폴더 경로</param>
        /// <param name="remoteUserId">원격 사용자 아이디</param>
        /// <param name="remotePassword">원격 비밀번호</param>
        /// <param name="localDriveName">사용가능한 드라이브 명, NULL일때 시스템이 자동으로 설정한다.</param>
        public static void Connect(string remoteAccessUri, string remoteUserId, string remotePassword, string localDriveName)
        {
            if(IsDebugEnabled)
                log.Debug("네트웍 드라이브를 만든다. remoteAccessUri=[{0}], remoteUserId=[{1}], remotePassword=[{2}], localDriveName=[{3}]",
                          remoteAccessUri, remoteUserId, remotePassword, localDriveName);

            var capacity = 64;
            uint resultFlags;
            const uint flags = 0;

            var lpAccessName = new StringBuilder(capacity);

            var netResource = new NETRESOURCE
                              {
                                  dwType = 1,
                                  lpLocalName = localDriveName,
                                  lpRemoteName = remoteAccessUri,
                                  lpProvider = null
                              };

            // 공유 디스크
            // 로컬 드라이브 지정하지 않음
            var result = WNetUseConnection(IntPtr.Zero,
                                           ref netResource,
                                           remotePassword,
                                           remoteUserId,
                                           flags,
                                           lpAccessName,
                                           ref capacity,
                                           out resultFlags);

            if(IsDebugEnabled)
                log.Debug("네트웍 드라이브 빌드. result=[{0}], lpAccessname=[{1}]", result, lpAccessName.ToString());

            if(result != 0)
                throw new Win32Exception(result);
        }
    }
}