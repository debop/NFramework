using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NSoft.NFramework.LinqEx;

namespace NSoft.NFramework.Data.NHibernateEx.ForTesting {
    /// <summary>
    /// NHibernate 용 Mapping 파일에 대한 정보
    /// </summary>
    public class MappingInfo {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 지정된 Assembly 들로 부터 매핑할 형식들을 찾아낸다.
        /// </summary>
        /// <param name="assemblies">매핑할 어셈블리들</param>
        /// <returns></returns>
        public static MappingInfo From(params Assembly[] assemblies) {
            return new MappingInfo(assemblies);
        }

        /// <summary>
        /// 지정된 Assembly 들로 부터 매핑할 형식들을 찾아낸다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MappingInfo FromAssemblyContaining<T>() {
            return new MappingInfo(new[] { typeof(T).Assembly });
        }

        private readonly List<Assembly> _mappedAssemblies = new List<Assembly>();
        private readonly Dictionary<string, string> _queryLanguageImports = new Dictionary<string, string>();

        private MappingInfo(IEnumerable<Assembly> assemblies) {
            if(assemblies != null)
                _mappedAssemblies.AddRange(assemblies.Distinct());
        }

        /// <summary>
        /// NHibernate 초기 설정자
        /// </summary>
        public INHInitializationAware NHInitializationAware { get; set; }

        /// <summary>
        /// 매핑한 Assebly 들
        /// </summary>
        public Assembly[] MappingAssemblies {
            get { return _mappedAssemblies.ToArray(); }
        }

        /// <summary>
        ///  (Key: Full name of Entity Type, Value = Assembly Name")
        /// </summary>
        public IDictionary<string, string> QueryLanguageImports {
            get { return new Dictionary<string, string>(_queryLanguageImports); }
        }

        /// <summary>
        /// Tell NHibernate about the specified types that are referenced in HQL Queries.
        /// </summary>
        /// <param name="types">추가할 Entity의 타입들</param>
        /// <remarks>
        /// This method makes a simple assumption: the name of a type referenced in HQL is the same as the 
        /// unqalified name of the actual class/struct. If alias names are used, then
        /// register these using <see cref="AddQueryLanguageImport(string,Type)"/>
        /// </remarks>
        /// <returns>Current instance</returns>
        public MappingInfo AddQueryLanguageImport(params Type[] types) {
            if(types != null && types.Length > 0)
                types.RunEach(type => AddQueryLanguageImport(type.Name, type));

            return this;
        }

        /// <summary>
        /// Tell NHibernate about the specified types that are referenced in HQL Queries.
        /// </summary>
        /// <param name="alias">Alias</param>
        /// <param name="type">추가할 Entity 수형</param>
        /// <remarks>
        /// This method makes a simple assumption: the name of a type referenced in HQL is the same as the 
        /// unqalified name of the actual class/struct. 
        /// </remarks>
        /// <returns>Current instance</returns>
        public MappingInfo AddQueryLanguageImport(string alias, Type type) {
            //var qualifiedTypeName = type.AssemblyQualifiedName;
            var qualifiedTypeName = string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
            _queryLanguageImports.Add(alias, qualifiedTypeName);

            if(IsDebugEnabled)
                log.Debug("Add query language... alias=[{0}], type=[{1}]", alias, qualifiedTypeName);

            return this;
        }

        /// <summary>
        /// NHibernate 초기화 수행자를 지정한다.
        /// </summary>
        /// <param name="aware">초기화 수행자</param>
        /// <returns>Current instance</returns>
        public MappingInfo SetNHInitializationAware(INHInitializationAware aware) {
            NHInitializationAware = aware;
            return this;
        }
    }
}