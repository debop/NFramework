using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace NSoft.NFramework.Data.OdpNet.EnterpriseLibrary.Configuration {
    /// <summary>
    /// Oracle 용 configuration section
    /// </summary>
    public class OracleConnectionSettings : SerializableConfigurationSection {
        /// <summary>
        /// The section name ( "oracleConnectionSettings" )
        /// </summary>
        public const string SectionName = "oracleConnectionSettings";

        private const string OracleConnectionDataCollectionProperty = "";

        [ConfigurationProperty(OracleConnectionDataCollectionProperty, IsRequired = false, IsDefaultCollection = true)]
        public NamedElementCollection<OracleConnectionData> OracleConnectionsData {
            get { return (NamedElementCollection<OracleConnectionData>)base[OracleConnectionDataCollectionProperty]; }
        }

        public static OracleConnectionSettings GetSettings(IConfigurationSource configurationSource) {
            return configurationSource.GetSection(SectionName) as OracleConnectionSettings;
        }
    }
}