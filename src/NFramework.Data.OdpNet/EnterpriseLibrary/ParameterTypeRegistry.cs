using System.Collections.Generic;
using System.Data;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.OdpNet.EnterpriseLibrary {
    internal sealed class ParameterTypeRegistry {
        private readonly IDictionary<string, DbType> _parameterTypes = new Dictionary<string, DbType>();

        internal void RegisterParameterType(string parameterName, DbType parameterType) {
            _parameterTypes.AddValue(parameterName, parameterType);
        }

        internal bool HasRegisteredParameterType(string parameterName) {
            return _parameterTypes.ContainsKey(parameterName);
        }

        internal DbType GetRegisteredParameterType(string parameterName) {
            return _parameterTypes.GetValue(parameterName);
        }
    }
}