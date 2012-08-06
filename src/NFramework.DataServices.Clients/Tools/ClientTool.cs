using NSoft.NFramework.DataServices.Messages;
using NSoft.NFramework.InversionOfControl;

namespace NSoft.NFramework.DataServices.Clients {
    public static partial class ClientTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        static ClientTool() {
            if(log.IsInfoEnabled)
                log.Info("ClientTool 클래스의 Static 생성자가 호출되었습니다.");
        }

        internal static ISerializer<RequestMessage> ResolveRequestSerializer(string productName) {
            return IoC.Resolve<ISerializer<RequestMessage>>("MessageSerializer." + productName);
        }

        internal static ISerializer<ResponseMessage> ResolveResponseSerializer(string productName) {
            return IoC.Resolve<ISerializer<ResponseMessage>>("MessageSerializer." + productName);
        }
    }
}