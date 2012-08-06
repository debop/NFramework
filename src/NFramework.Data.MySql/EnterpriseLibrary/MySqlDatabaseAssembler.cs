using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Configuration;

namespace NSoft.NFramework.Data.MySql.EnterpriseLibrary {
    /// <summary>
    /// Represents the process to build an instance of <see cref="MySqlDatabase"/> described by configuration information.
    /// </summary>
    public class MySqlDatabaseAssembler : IDatabaseAssembler {
        /// <summary>
        /// Builds an instance of the concrete subtype of <see cref="T:Microsoft.Practices.EnterpriseLibrary.Data.Database"/> the receiver knows how to build, based on 
        /// the provided connection string and any configuration information that might be contained by the 
        /// <paramref name="configurationSource"/>.
        /// </summary>
        /// <param name="name">The name for the new database instance.</param><param name="connectionStringSettings">The connection string for the new database instance.</param>
        /// <param name="configurationSource">The source for any additional configuration information.</param>
        /// <returns>
        /// The new database instance.
        /// </returns>
        public Database Assemble(string name, ConnectionStringSettings connectionStringSettings,
                                 IConfigurationSource configurationSource) {
            return new MySqlDatabase(connectionStringSettings.ConnectionString);
        }
    }
}