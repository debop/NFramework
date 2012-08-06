namespace NSoft.NFramework.Data.Mappers {
    /// <summary>
    /// 내부에 <see cref="INameMap"/>을 가지고, 컬럼명-속성명을 매핑시키는 래퍼 클래습니다.
    /// </summary>
    public class NameMapMapper : INameMapper {
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="nameMap"></param>
        public NameMapMapper(INameMap nameMap) {
            nameMap.ShouldNotBeNull("nameMap");
            NameMap = nameMap;
        }

        /// <summary>
        /// 매핑정보를 가진 INameMap
        /// </summary>
        public INameMap NameMap { get; private set; }

        /// <summary>
        /// 컬럼명을 속성명으로 맵핑시킨다.
        /// </summary>
        /// <param name="columnName">컬럼명</param>
        /// <returns>컬럼명에 매칭되는 속성명</returns>
        public string MapToPropertyName(string columnName) {
            string propertyName;
            return NameMap.TryGetValue(columnName, out propertyName) ? propertyName : string.Empty;
        }
    }
}