using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.Data.Mappers {
    /// <summary>
    /// Column 명과 Property 명의 매핑 정보 (Key = Column Name, Value = Property Name)
    /// </summary>
    public class NameMap : Dictionary<string, string>, INameMap {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NameMap() {}

        /// <summary>
        /// Initialize new instance of NameMap with initial data
        /// </summary>
        /// <param name="dictionary"></param>
        public NameMap(IDictionary<string, string> dictionary) : base(dictionary) {}

        /// <summary>
        /// Collection of Column Name (Dictionary의 Key 값)
        /// </summary>
        public IList<string> ColumnNames {
            get { return Keys.ToList(); }
        }

        /// <summary>
        /// Collection of Property Name (Dictionary의 Value 값)
        /// </summary>
        public IList<string> PropertyNames {
            get { return Values.ToList(); }
        }

        /// <summary>
        /// 해당 컬럼명의 키값에 존재하는지 검사한다.
        /// </summary>
        /// <param name="columnName">검사할 컬럼명</param>
        /// <returns>컬럼명 존재 유무</returns>
        public bool ContainsColumn(string columnName) {
            return ContainsKey(columnName);
        }

        /// <summary>
        /// 해당 속성명이 Value 값에 존재하는지 검사한다.
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <returns>속성명 존재 유무</returns>
        public bool ContainsProperty(string propertyName) {
            return ContainsValue(propertyName);
        }

        /// <summary>
        /// 해당 컬럼명의 인덱스를 구한다. 없다면 -1을 반환한다.
        /// </summary>
        /// <param name="columnName">컬럼명</param>
        /// <returns>해당 컬럼명의 인덱스, 없으면 -1을 반환한다.</returns>
        public int IndexOfColumn(string columnName) {
            return ColumnNames.IndexOf(columnName);
        }

        /// <summary>
        /// 해당 속성명의 인덱스를 구한다. 없다면 -1을 반환한다.
        /// </summary>
        /// <param name="propertyName">속성명</param>
        /// <returns>해당 속성명의 인덱스, 없으면 -1을 반환한다.</returns>
        public int IndexOfProperty(string propertyName) {
            return PropertyNames.IndexOf(propertyName);
        }
    }
}