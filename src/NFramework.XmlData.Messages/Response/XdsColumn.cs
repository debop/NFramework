using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Database Query 수행 결과를 나타내는 ResultSet의 하나의 컬럼 값을 나타낸다.
    /// </summary>
    [Serializable]
    public class XdsColumn {
        /// <summary>
        /// Constructor
        /// </summary>
        public XdsColumn()
            : this(null) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column"></param>
        public XdsColumn(object column) {
            Value = column;
        }

        /// <summary>
        /// Value of Column
        /// </summary>
        [XmlIgnore]
        public object Value { get; set; }

        /// <summary>
        /// Return string that represent value of column
        /// </summary>
        [XmlText(typeof(string))]
        public string ValueToString {
            get { return (Value == null) ? string.Empty : Value.ToString(); }
            set {
                Value = (value.IsNotWhiteSpace())
                            ? value.DeleteCharAny(new[] { '\0', (char)0x10 })
                            : null;
            }
        }
    }

    /// <summary>
    /// Collection of <see cref="XdsColumn"/>
    /// </summary>
    [Serializable]
    public class XdsColumnCollection : List<XdsColumn> {
        #region << AddColumn >>

        /// <summary>
        /// Null 값을 가지는 <see cref="XdsColumn"/>을 추가한다.
        /// </summary>
        /// <returns></returns>
        public virtual XdsColumn AddColumn() {
            return AddColumn(null);
        }

        /// <summary>
        /// 지정된 <paramref name="c"/>값을 가지는 <see cref="XdsColumn"/>을 추가한다.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual XdsColumn AddColumn(object c) {
            var column = new XdsColumn(c);
            base.Add(column);

            return column;
        }

        /// <summary>
        /// 지정된 값을 가지는 <see cref="XdsColumn"/>의 컬렉션을 추가한다.
        /// </summary>
        /// <param name="columns"></param>
        public virtual void AddColumnArray(params object[] columns) {
            if(columns != null && columns.Length > 0)
                foreach(object c in columns)
                    AddColumn(c);
        }

        /// <summary>
        /// 지정된 <see cref="DataRow"/>의 모든 컬럼 값을 이용하여 <see cref="XdsColumn"/>을 빌드하고, 컬렉션에 추가한다.
        /// </summary>
        /// <param name="row"></param>
        public virtual void AddColumnArray(DataRow row) {
            var items = row.ItemArray;
            AddColumnArray(items);
        }

        /// <summary>
        /// <see cref="IDataReader"/>의 현재 레코드 정보 (<see cref="IDataRecord"/>)로부터 값을 읽어 <see cref="XdsColumn"/>을 빌드하고 컬렉션에 추가한다.
        /// </summary>
        /// <param name="dr"></param>
        public virtual void AddColumnArray(IDataReader dr) {
            // IDataReader의 현재 record 의 값들을 추출한다.
            var items = new object[dr.FieldCount];
            dr.GetValues(items);

            AddColumnArray(items);
        }

        #endregion

        #region << Column To Value Array >>

        /// <summary>
        /// 컬렉션의 모든 <see cref="XdsColumn.Value"/>를 1차원 배열로 만들어 반환한다.
        /// </summary>
        /// <returns></returns>
        public object[] GetValues() {
            var array = new object[Count];

            for(int i = 0; i < array.Length; i++)
                array[i] = this[i].Value;

            return array;
        }

        #endregion
    }
}