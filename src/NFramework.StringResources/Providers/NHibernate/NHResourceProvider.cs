using System.Threading;
using Castle.Core;
using NSoft.NFramework.Data.NHibernateEx;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.StringResources.NHibernate {
    /// <summary>
    /// NHResource Provider by NHibernate
    /// </summary>
    public class NHResourceProvider : ResourceProviderBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly object _syncLock = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resourceName"></param>
        public NHResourceProvider(string resourceName) : this(string.Empty, resourceName) {}

        /// <summary>
        /// Constructor
        /// </summary>
        public NHResourceProvider(string assemblyName, string resourceName)
            : base(assemblyName, resourceName) {
            if(IsDebugEnabled)
                log.Debug("NHResourceProvider created. assemblyName=[{0}], resourceName=[{1}]", assemblyName, resourceName);

            // _repository = IoC.Resolve<INHRepository<NHResource>>() as NHResourceRepository;

            //if(_repository == null)
            //    throw new InvalidOperationException("Database NHResource 정보 접근을 위한 NHResourceRepository 형식 정보를 configuration에 넣지 않았습니다.");
        }

        private INHResourceRepository _repository;

        /// <summary>
        /// Instance of repository
        /// </summary>
        public INHResourceRepository Repository {
            get {
                if(UnitOfWork.IsNotStarted)
                    lock(_syncLock)
                        if(UnitOfWork.IsNotStarted)
                            UnitOfWork.Start();

                if(_repository == null)
                    lock(_syncLock)
                        if(_repository == null) {
                            var repository = IoC.TryResolve<INHResourceRepository>(() => new NHResourceRepository(), true,
                                                                                   LifestyleType.Singleton);
                            Thread.MemoryBarrier();
                            _repository = repository;
                        }

                return _repository;
            }
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
        public override object GetObject(string resourceKey, System.Globalization.CultureInfo culture) {
            if(IsDebugEnabled)
                log.Debug("Get Resource Value. resourceKey=[{0}], culture=[{1}]", resourceKey, culture);

            var resource = Repository.GetResource(AssemblyName, ResourceName, resourceKey);

            if(IsDebugEnabled)
                log.Debug("Get NHResource from Repository. resource=[{0}]", resource);

            culture = culture.GetOrCurrentUICulture();

            if(resource != null)
                return (resource.LocaleMap.ContainsKey(culture))
                           ? resource.LocaleMap[culture].ResourceValue
                           : resource.ResourceValue;

            return null;
        }

        ///<summary>
        /// 소스에서 리소스 값을 읽기 위한 개체를 가져옵니다.
        ///</summary>
        ///<returns>
        /// 현재 리소스 공급자와 관련된 <see cref="T:System.Resources.IResourceReader" />입니다.
        ///</returns>
        public override System.Resources.IResourceReader ResourceReader {
            get {
                var resources = Repository.GetResources(AssemblyName, ResourceName, CultureTool.GetOrCurrentUICulture(null));
                return new NHResourceReader(resources);
            }
        }
    }
}