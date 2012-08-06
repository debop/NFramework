using System.Collections.Generic;

namespace NSoft.NFramework.Data.Mappers {
    /// <summary>
    /// Column 명과 Property 명의 매핑 정보 (Key = Column Name, Value = Property Name)
    /// </summary>
    public interface INameMap : IDictionary<string, string> {
        /// <summary>
        /// Collection of Column Name (Dictionary의 Key 값)
        /// </summary>
        IList<string> ColumnNames { get; }

        /// <summary>
        /// Collection of Property Name (Dictionary의 Value 값)
        /// </summary>
        IList<string> PropertyNames { get; }

        /// <summary>
        /// 해당 컬럼명의 키값에 존재하는지 검사한다.
        /// </summary>
        /// <param name="columnName">검사할 컬럼명</param>
        /// <returns>컬럼명 존재 유무</returns>
        bool ContainsColumn(string columnName);

        /// <summary>
        /// 해당 속성명이 Value 값에 존재하는지 검사한다.
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <returns>속성명 존재 유무</returns>
        bool ContainsProperty(string propertyName);

        /// <summary>
        /// 해당 컬럼명의 인덱스를 구한다. 없다면 -1을 반환한다.
        /// </summary>
        /// <param name="columnName">컬럼명</param>
        /// <returns>해당 컬럼명의 인덱스, 없으면 -1을 반환한다.</returns>
        int IndexOfColumn(string columnName);

        /// <summary>
        /// 해당 속성명의 인덱스를 구한다. 없다면 -1을 반환한다.
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <returns>해당 속성명의 인덱스, 없으면 -1을 반환한다.</returns>
        int IndexOfProperty(string propertyName);
    }
}