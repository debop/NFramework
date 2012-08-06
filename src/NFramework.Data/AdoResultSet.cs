using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// <see cref="IDataReader"/>로부터 정보를 받아두는 저장소 역할을 하는 클래스입니다. 
    /// IDataReader 정보를 병렬 방식으로 엔티티로 변환하기 위해서, 임시 버퍼 저장소로 쓰입니다.
    /// </summary>
    [Serializable]
    public class AdoResultSet : Dictionary<int, AdoResultRow> {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public AdoResultSet() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="reader">읽어들일 Reader</param>
        /// <param name="firstResult">첫번째 레코드의 인덱스</param>
        /// <param name="maxResults">최대 레코드 인덱스</param>
        public AdoResultSet(IDataReader reader, int firstResult = 0, int maxResults = 0) {
            reader.ShouldNotBeNull("reader");

            if(reader.IsClosed) {
                if(log.IsWarnEnabled)
                    log.Warn("DataReader[{0}]가 닫혀있어서 작업을 수행하지 못했습니다.", reader.GetType().FullName);

                return;
            }

            LoadAll(reader, firstResult, maxResults);
        }

        private void LoadAll(IDataReader reader, int firstResult, int maxResults) {
            if(IsDebugEnabled)
                log.Debug("IDataReader로부터 정보를 읽어 AdoResultRow를 만들어 AdoResultSet 으로 빌드합니다...");

            while(firstResult-- > 0) {
                if(reader.Read() == false)
                    return;
            }

            if(maxResults <= 0)
                maxResults = int.MaxValue;

            FieldNames = reader.GetFieldNames();

            var rowIndex = 0;

            while(reader.Read() && rowIndex < maxResults) {
                Add(rowIndex++, new AdoResultRow(reader, FieldNames));
            }

            if(IsDebugEnabled)
                log.Debug("IDataReader로부터 정보를 읽어 AdoResultRow 들을 만들어 AdoResultSet을 빌드했습니다!!! Row Count=[{0}]", Count);
        }

        /// <summary>
        /// 필드(컬럼) 명들
        /// </summary>
        public IList<string> FieldNames { get; private set; }

        /// <summary>
        /// AdoResultSet의 Value인 <see cref="AdoResultRow"/>의 정보를 바탕으로 <paramref name="targetType"/>로 매핑합니다.
        /// </summary>
        /// <param name="targetType">매핑할 수형</param>
        /// <returns>AdoResultRow의 값으로 매핑된 <see cref="targetType"/> 수형의 인스턴스의 컬렉션.</returns>
        public virtual IEnumerable<object> GetMappedObjects(Type targetType) {
            targetType.ShouldNotBeNull("targetType");

            if(IsDebugEnabled)
                log.Debug("Row 값들을 이용하여, ResultRow를 [{0}] 수형으로 Mapping 합니다.", targetType.FullName);

            return Values.Select(r => ObjectMapper.Map(r, () => ActivatorTool.CreateInstance(targetType, true)));
        }
    }
}