using System;
using System.Threading.Tasks;
using System.Web.UI;
using MongoDB.Driver.Builders;
using NSoft.NFramework.Tools;
using NSoft.NFramework.Web.PageStatePersisters;

namespace NSoft.NFramework.Data.MongoDB.Web {
    /// <summary>
    /// ViewState 정보를 MongoDB에 저장합니다.
    /// </summary>
    public class MongoPageStatePersister : AbstractServerPageStatePersister {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 상태정보를 저장할 DB의 기본 ConnectionString
        /// </summary>
        public const string DefaultStateConnectionString = @"server=localhost;database=AspPageState;safe=true;";

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="page"></param>
        public MongoPageStatePersister(Page page) : base(page) {}

        /// <summary>
        /// 상태정보 DB의 ConnectionString
        /// </summary>
        public string ConnectionString { get; set; }

        public IMongoRepository CreateRepository() {
            return MongoTool.CreateRepository(ConnectionString.AsText(DefaultStateConnectionString));
        }

        /// <summary>
        /// ViewState 저장소로부터 저장된 ViewState 정보를 가져옵니다.
        /// </summary>
        protected override void LoadFromRepository() {
            if(IsDebugEnabled)
                log.Debug("저장소에서 키에 해당하는 상태정보 로드를 시작합니다... StateValue=[{0}]", StateValue);

            if(StateValue.IsWhiteSpace())
                return;

            RemoveExpires(Expiration);

            using(var repository = CreateRepository()) {
                var stateEntity = repository.FindOneByIdAs<PageStateEntity>(StateValue);

                if(stateEntity != null) {
                    object pageState;

                    if(PageStateTool.TryParseStateEntity(stateEntity, Compressor, out pageState)) {
                        ViewState = ((Pair)pageState).First;
                        ControlState = ((Pair)pageState).Second;

                        if(IsDebugEnabled)
                            log.Debug("저장된 상태정보를 로드하여, Page의 ViewState, ControlState에 설정했습니다!!!");
                    }
                    else
                        throw new InvalidOperationException("저장된 정보를 로드하였지만, ViewState로 Parsing하는데 실패했습니다.");
                }
                else {
                    if(IsDebugEnabled)
                        log.Debug("상태정보를 조회하지 못했습니다. StateValue=[{0}]", StateValue);
                }
            }
        }

        /// <summary>
        /// Page의 ViewState 정보를 특정 저장소에 저장하고, 저장 토큰 값을 <see cref="AbstractServerPageStatePersister.StateValue"/>에 저장합니다.
        /// </summary>
        protected override void SaveToRepository() {
            if(IsDebugEnabled)
                log.Debug("Page 상태정보를 저장합니다...");

            RemoveExpires(Expiration);

            var pageState = new Pair(ViewState, ControlState);

            if(StateValue.IsWhiteSpace())
                StateValue = Guid.NewGuid().ToString();

            var stateEntity = (PageStateEntity)PageStateTool.CreateStateEntity(StateValue, pageState, Compressor, CompressThreshold);

            using(var repository = CreateRepository()) {
                var query = Query.EQ(MongoTool.IdString, stateEntity.Id);
                var updated = Update.Set("Value", stateEntity.Value).Set("IsCompressed", stateEntity.IsCompressed).Set("CreatedDate",
                                                                                                                       stateEntity.
                                                                                                                           CreatedDate);

                // UPSERT
                var result = repository.FindAndModify(query, SortBy.Null, updated, true, true);

                if(IsDebugEnabled)
                    log.Debug("캐시에 Page 상태정보를 저장했습니다!!! StateValue=[{0}], Expiration=[{1}] (min), Result=[{2}]", StateValue, Expiration,
                              result.Ok);
            }
        }

        private void RemoveExpires(TimeSpan expiration) {
            try {
                Task.Factory.StartNew(() => {
                                          var query = Query.LTE("CreatedDate", DateTime.Now.Add(-expiration).ToMongoDateTime());

                                          using(var repository = CreateRepository()) {
                                              var result = repository.Remove(query);

                                              if(result.Ok && result.DocumentsAffected > 0)
                                                  if(log.IsDebugEnabled)
                                                      log.Debug("유효기간이 지난 ViewState정보 [{0}]개를 삭제했습니다.", result.DocumentsAffected);
                                          }
                                      });
            }
            catch {}
        }
    }
}