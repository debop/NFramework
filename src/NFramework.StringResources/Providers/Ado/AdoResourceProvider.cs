using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using System.Threading;
using NSoft.NFramework.Data;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources {
    /// <summary>
    /// Database에 저장된 String Resource 정보를 <see cref="AdoRepository"/>를 이용하여 제공합니다.
    /// </summary>
    public class AdoResourceProvider : ResourceProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// Default TableName for StringResources ('StringResources')
        /// </summary>
        public const string DefaultStringResourceTableName = "StringResources";

        /// <summary>
        /// Default assembly name ('DEFAULT')
        /// </summary>
        public const string DefaultAssemblyName = "DEFAULT";

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="databaseName">database connection string name</param>
        /// <param name="tableName">table name</param>
        /// <param name="assemblyName">string resource table name</param>
        /// <param name="resourceName">section name</param>
        public AdoResourceProvider(string resourceName, string assemblyName = DefaultAssemblyName, string databaseName = null,
                                   string tableName = null)
            : base(assemblyName, resourceName) {
            DatabaseName = databaseName ?? AdoTool.DefaultDatabaseName;
            TableName = tableName ?? DefaultStringResourceTableName;

            if(IsDebugEnabled)
                log.Debug(@"새로운 AdoResourceProvider를 생성했습니다. DatabaseName=[{0}], TableName=[{1}]", DatabaseName, TableName);
        }

        /// <summary>
        /// culture : (resource key: resource value)
        /// </summary>
        private readonly IDictionary<CultureInfo, Hashtable> _resourcesCache = new Dictionary<CultureInfo, Hashtable>();

        /// <summary>
        /// Database name
        /// </summary>
        public string DatabaseName { get; protected set; }

        /// <summary>
        /// Table name for String Resources
        /// </summary>
        public string TableName { get; protected set; }

        private AdoResourceRepository _repository;

        /// <summary>
        /// Resource Repository
        /// </summary>
        public AdoResourceRepository Repository {
            get {
                if(_repository == null)
                    lock(_syncLock)
                        if(_repository == null) {
                            var repository = new AdoResourceRepository(ResourceName, AssemblyName, DatabaseName, TableName);
                            Thread.MemoryBarrier();
                            _repository = repository;
                        }

                return _repository;
            }
        }

        ///<summary>
        /// 소스에서 리소스 값을 읽기 위한 개체를 가져옵니다.
        ///</summary>
        ///<returns>
        /// 현재 리소스 공급자와 관련된 <see cref="T:System.Resources.IResourceReader" />입니다.
        ///</returns>
        public override IResourceReader ResourceReader {
            get { return new AdoResourceReader(Repository.GetResources(CultureTool.GetOrCurrentUICulture(null))); }
        }

        ///<summary>
        /// 키 및 culture에 대한 리소스 개체를 반환합니다.
        ///</summary>
        ///<returns>
        ///<paramref name="resourceKey" /> 및 <paramref name="culture" />에 대한 리소스 값을 포함하는 <see cref="T:System.Object" />입니다.
        ///</returns>
        ///<param name="resourceKey">
        /// 특정 리소스를 식별하는 키입니다.
        /// </param>
        ///<param name="culture">
        /// 리소스의 지역화된 값을 식별하는 culture입니다.
        ///</param>
        /// <returns>resource value, if not exists, return null</returns>
        public override object GetObject(string resourceKey, CultureInfo culture) {
            resourceKey.ShouldNotBeWhiteSpace("resourceKey");

            if(IsDebugEnabled)
                log.Debug("Get Resource value... resourceKey=[{0}], culture=[{1}], AssemblyName=[{2}], ResourceName=[{3}]",
                          resourceKey, culture, AssemblyName, ResourceName);

            culture = culture.GetCulture();

            // 캐시 확인
            Hashtable resourceValues = null;

            if(_resourcesCache.TryGetValue(culture, out resourceValues)) {
                if(resourceValues.ContainsKey(resourceKey))
                    return resourceValues[resourceKey];
            }

            // DB에서 해당 정보 조회
            var result = Repository.GetResource(culture, resourceKey);

            // 캐시에 저장
            lock(_syncLock) {
                if(_resourcesCache.TryGetValue(culture, out resourceValues) == false) {
                    resourceValues = new Hashtable();
                    _resourcesCache.Add(culture, resourceValues);
                }

                // NOTE: MultiThread 상태에서 lock이 걸려 있어도, resourceValues에 기존 key가 있을 경우에는 넣지 않는다!!!
                //
                if(resourceValues.ContainsKey(resourceKey) == false)
                    resourceValues.Add(resourceKey, result);
            }

            return result;
        }

        /// <summary>
        /// 현재 인스턴스의 정보를 문자열로 표현합니다.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0}, DatabaseName=[{1}], TableName=[{2}]", base.ToString(), DatabaseName, TableName);
        }
    }
}