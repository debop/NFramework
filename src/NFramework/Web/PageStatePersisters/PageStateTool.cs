using System;
using NSoft.NFramework.Compressions;
using NSoft.NFramework.Serializations.Serializers;

namespace NSoft.NFramework.Web.PageStatePersisters {
    /// <summary>
    /// 웹 Page 상태 정보를 관리하는 Utility 클래스입니다.
    /// </summary>
    public static class PageStateTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static ISerializer Serializer {
            get { return BinarySerializer.Instance; }
        }

        /// <summary>
        /// <paramref name="pageState"/> 정보를 직렬화해서, <see cref="IPageStateEntity"/> 객체로 빌드합니다.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pageState"></param>
        /// <param name="compressor"></param>
        /// <param name="compressThreshold"></param>
        /// <returns></returns>
        public static IPageStateEntity CreateStateEntity(string id, object pageState, ICompressor compressor,
                                                         int compressThreshold = 40960) {
            compressor.ShouldNotBeNull("compressor");
            compressThreshold.ShouldBePositive("compressThreshold");

            if(IsDebugEnabled)
                log.Debug("상태 정보를 가지는 인스턴스를 생성합니다. pageState=[{0}], compressor=[{1}], compressThreshold=[{2}]",
                          pageState, compressor, compressThreshold);

            if(pageState == null)
                return new PageStateEntity(id, null, false);

            var serializedValue = Serializer.Serialize(pageState);

            if(serializedValue.Length > compressThreshold)
                return new PageStateEntity(id, compressor.Compress(serializedValue), true);

            return new PageStateEntity(id, serializedValue, false);
        }

        /// <summary>
        /// <paramref name="stateEntity"/> 정보를 파싱하여 원본 Page 상태정보를 빌드합니다.
        /// </summary>
        /// <param name="stateEntity"></param>
        /// <param name="compressor"></param>
        /// <param name="pageState"></param>
        /// <returns></returns>
        public static bool TryParseStateEntity(IPageStateEntity stateEntity, ICompressor compressor, out object pageState) {
            if(IsDebugEnabled)
                log.Debug("상태 정보를 파싱합니다...");

            pageState = null;
            if(stateEntity == null)
                return false;

            bool result;
            try {
                if(stateEntity.Value != null) {
                    if(IsDebugEnabled)
                        log.Debug("저장된 상태 정보(stateEntity)의 값이 존재합니다. 값을 로드합니다... entity Id=[{0}]", stateEntity.Id);

                    var bytes = (stateEntity.IsCompressed)
                                    ? compressor.Decompress(stateEntity.Value)
                                    : stateEntity.Value;

                    pageState = Serializer.Deserialize(bytes);
                }
                result = (pageState != null);
            }
            catch(Exception ex) {
                if(log.IsWarnEnabled) {
                    log.Warn("State 정보를 파싱하는데 실패했습니다. stateEntity=[{0}]", stateEntity);
                    log.Warn(ex);
                }

                result = false;
            }

            if(IsDebugEnabled)
                log.Debug("저장된 상태 정보를 로드했습니다. 결과=[{0}], StateEntity=[{1}]", result, stateEntity);

            return result;
        }
    }
}