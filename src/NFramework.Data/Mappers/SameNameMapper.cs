namespace NSoft.NFramework.Data.Mappers {
    /// <summary>
    /// 컬럼명과 속성명이 같은 Mapper
    /// </summary>
    public sealed class SameNameMapper : NameMapperBase {
        /// <summary>
        /// 컬럼명을 속성명으로 맵핑시킨다. 컬럼명과 속성명이 같다.
        /// </summary>
        /// <param name="columnName">컬럼명</param>
        /// <returns>컬럼명에 매칭되는 속성명</returns>
        public override string MapToPropertyName(string columnName) {
            columnName.ShouldNotBeWhiteSpace("columnName");

            return columnName;
        }
    }
}