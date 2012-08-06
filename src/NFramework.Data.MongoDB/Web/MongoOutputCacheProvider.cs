//! .NET 4.0 이상에서만 OutputCache Provider 를 사용할 수 있습니다.

using System;
using System.Threading.Tasks;
using System.Web.Caching;
using Castle.Core;
using MongoDB.Driver.Builders;
using NSoft.NFramework.InversionOfControl;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.MongoDB.Web {
    /// <summary>
    /// .NET 4.0 이상에서 ASP.NET Page의 OutputCache를 MongoDB에 저장합니다.
    /// 참고:
    ///      http://www.4guysfromrolla.com/articles/061610-1.aspx
    ///      http://weblogs.asp.net/gunnarpeipman/archive/2009/11/19/asp-net-4-0-writing-custom-output-cache-providers.aspx
    ///      http://weblogs.asp.net/scottgu/archive/2010/01/27/extensible-output-caching-with-asp-net-4-vs-2010-and-net-4-0-series.aspx
    /// </summary>
    /// <example>
    /// <code>
    /// <system.web>
    ///		<compilation debug="true" targetFramework="4.0"/>
    ///		<caching>
    ///			<outputCache defaultProvider="MongoOutputCacheProvider">
    ///				<providers>
    ///					<add name="MongoOutputCacheProvider" type="NSoft.NFramework.Data.MongoDB.Web.MongoOutputCacheProvider, NSoft.NFramework.Data.MongoDB"/>
    ///				</providers>
    ///			</outputCache>
    ///		</caching>
    /// </system.web>
    /// </code>
    /// </example>
    public class MongoOutputCacheProvider : OutputCacheProvider {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public MongoOutputCacheProvider() {}

        public const string ConnectionString = @"server=localhost;database=AspOutputCache;safe=true";

        public static IMongoRepository GetRepository() {
            IMongoRepository repository;
            if(IoC.IsInitialized)
                repository = IoC.TryResolve<IMongoRepository>(() => new MongoRepositoryImpl(ConnectionString),
                                                              true,
                                                              LifestyleType.Thread);
            else
                repository = new MongoRepositoryImpl(ConnectionString);

            repository.CollectionName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath.Replace("/", "_");
            return repository;
        }

        /// <summary>
        /// 출력 캐시에서 지정된 항목에 대한 참조를 반환합니다.
        /// </summary>
        /// <returns>
        /// 캐시에서 지정된 항목을 식별하는 <paramref name="key"/> 값이거나 캐시에 지정된 항목이 없는 경우 null입니다.
        /// </returns>
        /// <param name="key">출력 캐시에서 캐시된 항목에 대한 고유 식별자입니다. </param>
        public override object Get(string key) {
            if(key.IsWhiteSpace())
                return null;

            RemoveExpires();

            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 로드합니다. key=[{0}]", key);
            using(var repository = GetRepository()) {
                var document = repository.FindOneByIdAs<MongoAspOutputCacheEntry>(key);
                return (document != null) ? document.GetEntry() : null;
            }
        }

        /// <summary>
        /// 지정된 항목을 출력 캐시에 삽입합니다. 
        /// </summary>
        /// <returns>
        /// 지정된 공급자에 대한 참조입니다. 
        /// </returns>
        /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
        /// <param name="utcExpiry">캐시된 항목이 만료되는 날짜와 시간입니다.</param>
        public override object Add(string key, object entry, DateTime utcExpiry) {
            if(key.IsWhiteSpace())
                return null;

            RemoveExpires();

            var result = Get(key);

            if(result != null)
                return result;

            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 추가합니다. key=[{0}], utcExpiry=[{1}]", key, utcExpiry);

            using(var repository = GetRepository()) {
                repository.Save(new MongoAspOutputCacheEntry(key, entry, utcExpiry));
            }
            return entry;
        }

        /// <summary>
        /// 지정된 항목을 출력 캐시에 삽입하고 이미 캐시되어 있는 경우 해당 항목을 덮어씁니다.
        /// </summary>
        /// <param name="key"><paramref name="entry"/>에 대한 고유 식별자입니다.</param><param name="entry">출력 캐시에 추가할 내용입니다.</param>
        /// <param name="utcExpiry">캐시된 <paramref name="entry"/>가 만료되는 날짜와 시간입니다.</param>
        public override void Set(string key, object entry, DateTime utcExpiry) {
            if(key.IsWhiteSpace())
                return;

            RemoveExpires();

            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 저장합니다. 기존에 존재하면 갱신합니다... key=[{0}], utcExpiry=[{1}]", key, utcExpiry);

            using(var repository = GetRepository())
                repository.Save(new MongoAspOutputCacheEntry(key, entry, utcExpiry));
        }

        /// <summary>
        /// 출력 캐시에서 지정된 항목을 제거합니다.
        /// </summary>
        /// <param name="key">출력 캐시에서 제거할 항목에 대한 고유 식별자입니다. </param>
        public override void Remove(string key) {
            if(key.IsWhiteSpace())
                return;

            RemoveExpires();

            if(IsDebugEnabled)
                log.Debug("ASP.NET Page OutputCache를 제거합니다... key=[{0}]", key);

            using(var repository = GetRepository()) {
                var result = repository.RemoveByIdAs<MongoAspOutputCacheEntry>(key);

                if(IsDebugEnabled)
                    log.Debug("ASP.NET Page OutputCache를 제거했습니다. key=[{0}], Remove result=[{1}]", key, result.Ok);
            }
        }

        private void RemoveExpires() {
            Task.Factory.StartNew(() => {
                                      var query = Query.LTE("UtcExpiry", DateTime.UtcNow.ToMongoDateTime());
                                      With.TryAction(() => {
                                                         using(var repository = GetRepository())
                                                             repository.Remove(query);
                                                     });
                                  });
        }
    }
}