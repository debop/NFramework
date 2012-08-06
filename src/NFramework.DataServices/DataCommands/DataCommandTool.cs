using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Json;

namespace NSoft.NFramework.DataServices.DataCommands {
    /// <summary>
    /// DataCommand 관련 Helper Class
    /// </summary>
    public static class DataCommandTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        private static IList<IDataCommand> _dataCommands;

        internal static IList<IDataCommand> DataCommands {
            get {
                if(_dataCommands == null)
                    lock(_syncLock)
                        if(_dataCommands == null) {
                            var dataCommands = IoC.ResolveAll<IDataCommand>();
                            Thread.MemoryBarrier();
                            _dataCommands = dataCommands;
                        }
                return _dataCommands;
            }
        }

        /// <summary>
        /// 지정한 메소드에 해당하는 <see cref="IDataCommand"/> 인스턴스를 IoC로부터 Resolve합니다.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IDataCommand GetCommand(string method) {
            method.ShouldNotBeWhiteSpace("method");

            var commandName = method + "Command";
            if(IsDebugEnabled)
                log.Debug("Command를 로드합니다... method=[{0}], commandName=[{1}]", method, commandName);

            return DataCommands.FirstOrDefault(c => c.GetType().FullName.EndsWith(commandName));
        }

        public static string BuildResultReposonse(string method, object value) {
            return JsonTool.SerializeAsText(new
                                            {
                                                Method = method,
                                                Value = value
                                            });
        }
    }
}