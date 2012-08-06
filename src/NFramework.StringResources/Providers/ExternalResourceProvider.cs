namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// External Resource Assembly로부터 Resource 정보를 제공하는 Provider입니다.
    /// </summary>
    public class ExternalResourceProvider : LocalResourceProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #endregion

        /// <summary>
        /// default constructor
        /// </summary>
        protected ExternalResourceProvider() {}

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="assemblyName">External resource assembly name</param>
        /// <param name="resourceName">Resource filename in external resource assembly</param>
        public ExternalResourceProvider(string assemblyName, string resourceName) : base(assemblyName, resourceName) {}
    }
}