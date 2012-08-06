using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.Mappers {
    /// <summary>
    /// 공백을 제거하고 컬럼명과 속성명이 대소문자가 같게 매핑한다.
    /// </summary>
    public sealed class TrimNameMapper : NameMapperBase {
        /// <summary>
        /// 공백 문자
        /// </summary>
        public static readonly char WhiteSpaceChar = ' ';

        /// <summary>
        /// 컬럼명을 속성명으로 맵핑시킨다. 공백을 제거하고 컬럼명과 속성명이 대소문자가 같게 매핑한다.
        /// </summary>
        /// <param name="columnName">컬럼명</param>
        /// <returns>컬럼명에 매칭되는 속성명</returns>
        public override string MapToPropertyName(string columnName) {
            columnName.ShouldNotBeWhiteSpace("columnName");

            return columnName.DeleteChar(WhiteSpaceChar);
        }
    }
}