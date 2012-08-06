using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace NSoft.NFramework.Data.OdpNet.EnterpriseLibrary.Configuration {
    /// <summary>
    /// <para>Represents the package information to use when calling a stored procedure for Oracle.</para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// A package name can be appended to the stored procedure name of a command if the prefix of the stored procedure
    /// matchs the prefix defined. This allows the caller of the stored procedure to use stored procedures
    /// in a more database independent fashion.
    /// </para>
    /// </remarks>
    public class OraclePackageData : NamedConfigurationElement, IOraclePackage {
        private const string prefixProperty = "prefix";

        public OraclePackageData() {
            Prefix = string.Empty;
        }

        public OraclePackageData(string name, string prefix) : base(name) {
            Prefix = prefix;
        }

        /// <summary>
        /// 패키지 접두사
        /// </summary>
        [ConfigurationProperty(prefixProperty, IsRequired = true)]
        public string Prefix {
            get { return (string)this[prefixProperty]; }
            set { this[prefixProperty] = value; }
        }
    }
}