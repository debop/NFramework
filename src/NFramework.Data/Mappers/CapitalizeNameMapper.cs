using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.Mappers {
    /// <summary>
    /// 컬럼명에서 '_', White Space 를 제외하고, Pascal 명명법에 따라 단어를 대문자로 사용한다. (예: PROJECT_NAME => ProjectName)
    /// </summary>
    /// <example>
    ///	<code>
    ///		CapitalizeNameMapper cMapper = new CapitalizeNameMapper();
    ///		string propertyName = cMapper.MapToPropertyName("PROJECT_NAME");	// property name is ProjectName
    ///		propertyName = cMapper.MapToPropertyName("PROJECT_ID");				// property name is ProjectId
    /// </code>
    /// </example>
    public sealed class CapitalizeNameMapper : NameMapperBase {
        /// <summary>
        /// DB 테이블 컬럼명에서 제거할 Char 들 ('_', ' ')
        /// </summary>
        public static readonly char[] CharsToRemove = new[] { '_', ' ' };

        /// <summary>
        /// 컬럼명에서 '_', Space 를 제외하고, Pascal 명명법에 따라 단어를 대문자로 사용한다.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        /// <example>
        ///	<code>
        ///		CapitalizeNameMapper cMapper = new CapitalizeNameMapper();
        ///		string propertyName = cMapper.MapToPropertyName("PROJECT_NAME");	// property name is ProjectName
        ///		propertyName = cMapper.MapToPropertyName("PROJECT_ID");				// property name is ProjectId
        /// </code>
        /// </example>
        public override string MapToPropertyName(string columnName) {
            columnName.ShouldNotBeWhiteSpace("columnName");

            return
                columnName
                    .Capitalize()
                    .DeleteCharAny(CharsToRemove);
        }
    }
}