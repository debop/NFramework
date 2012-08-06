using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace NSoft.NFramework.Data.OdpNet.EnterpriseLibrary.Configuration {
    /// <summary>
    /// Oracle 전용 Connection 정보
    /// </summary>
    public class OracleConnectionData : NamedConfigurationElement {
        private const string PackagesProperty = "packages";

        public OracleConnectionData() {}

        [ConfigurationProperty(PackagesProperty, IsRequired = true)]
        public NamedElementCollection<OraclePackageData> Packages {
            get { return (NamedElementCollection<OraclePackageData>)base[PackagesProperty]; }
        }
    }
}