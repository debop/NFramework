using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NSoft.NFramework.Parallelism.Tools {
    /// <summary>
    /// "Ping" 을 EAP (Event-based Asynchronous Pattern-이벤트 기반 비동기 패턴) 방식의 작업으로 수행하는 Extension Methods입니다.
    /// </summary>
    /// <remarks>
    /// 참고사이트:
    /// <list>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/wewwczdw.aspx</item>
    ///		<item>http://msdn.microsoft.com/ko-kr/library/dd997423.aspx</item>
    /// </list>
    /// </remarks>
    public static class PingAsync {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, int timeout, byte[] buffer, PingOptions options,
                                               object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, timeout, buffer, options, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, int timeout, byte[] buffer, object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, timeout, buffer, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, int timeout, object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, timeout, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, IPAddress address, object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(address, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, int timeout, byte[] buffer, PingOptions options,
                                               object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, timeout, buffer, options, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, int timeout, byte[] buffer,
                                               object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, timeout, buffer, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, int timeout, object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, timeout, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        public static Task<PingReply> SendTask(this Ping ping, string hostNameOrAddress, object userToken = null) {
            return SendTaskCore(ping, userToken, tcs => ping.SendAsync(hostNameOrAddress, tcs));
        }

        /// <summary>
        /// Internet Control Message Protocol (ICMP) echo message 를 비동기적으로 보냅니다.
        /// </summary>
        private static Task<PingReply> SendTaskCore(Ping ping, object userToken, Action<TaskCompletionSource<PingReply>> sendAsync) {
            ping.ShouldNotBeNull("ping");
            sendAsync.ShouldNotBeNull("sendAsync");

            if(IsDebugEnabled)
                log.Debug("Ping 작업을 비동기적으로 수행하는 Task를 생성합니다...");

            var tcs = new TaskCompletionSource<PingReply>(userToken);
            PingCompletedEventHandler handler = null;
            handler = (sender, e) => EventAsyncPattern.HandleCompletion(tcs, e, () => e.Reply, () => ping.PingCompleted -= handler);
            ping.PingCompleted += handler;

            try {
                sendAsync(tcs);
            }
            catch(Exception ex) {
                ping.PingCompleted -= handler;
                tcs.TrySetException(ex);
            }

            return tcs.Task;
        }
    }
}