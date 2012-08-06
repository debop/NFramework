using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Domain.Model {
    /// <summary>
    /// Factory Master Information
    /// </summary>
    [Serializable]
    public class FactoryMaster {
        /// <summary>
        /// Factory Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Factory Name
        /// </summary>
        public string Name { get; set; }
    }
}