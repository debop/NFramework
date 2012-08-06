using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Database 수행 결과를 나타내는 ResultSet의 하나의 레코드를 표현한다.
    /// </summary>
    [Serializable]
    public class XdsRecord {
        /// <summary>
        /// 생성자
        /// </summary>
        public XdsRecord() {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="columns">컬럼들의 값</param>
        public XdsRecord(params object[] columns) {
            Columns.AddColumnArray(columns);
        }

        /// <summary>
        /// <see cref="DataRow"/>의 모든 컬럼 값으로 <see cref="XdsRecord"/>를 생성한다.
        /// </summary>
        /// <param name="row"><see cref="DataTable"/>의 하나의 Record</param>
        public XdsRecord(DataRow row) {
            Columns.AddColumnArray(row);
        }

        /// <summary>
        /// <see cref="IDataReader"/>의 현재 <see cref="IDataRecord"/>의 모든 컬럼 값으로 <see cref="XdsRecord"/>를 생성한다.
        /// </summary>
        /// <param name="dr"></param>
        public XdsRecord(IDataReader dr) {
            Columns.AddColumnArray(dr);
        }

        private XdsColumnCollection _columns = new XdsColumnCollection();

        /// <summary>
        /// 컬럼들
        /// </summary>
        [XmlElement("C")]
        public XdsColumnCollection Columns {
            get { return _columns ?? (_columns = new XdsColumnCollection()); }
            set { _columns = value; }
        }

        /// <summary>
        /// 결과 값들을 반환한다.
        /// </summary>
        /// <returns></returns>
        public virtual object[] GetValues() {
            lock(Columns) {
                return Columns.GetValues();
            }
        }
    }

    /// <summary>
    /// Collection of <see cref="XdsRecord"/>
    /// </summary>
    public class XdsRecordCollection : List<XdsRecord> {
        /// <summary>
        /// 결과 Record의 각각의 Column 값을 가져온다.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public object this[int row, int column] {
            get { return this[row].Columns[column].Value; }
        }

        /// <summary>
        /// 지정된 컬럼 값을 가지는 <see cref="XdsRecord"/>를 생성해서 컬렉션에 추가한다.
        /// </summary>
        /// <param name="columns">컬럼 값</param>
        /// <returns>추가된 <see cref="XdsRecord"/></returns>
        public virtual XdsRecord AddColumnArray(params object[] columns) {
            var r = new XdsRecord(columns);
            Add(r);
            return r;
        }

        /// <summary>
        /// <see cref="DataRow"/>의 값으로부터 <see cref="XdsRecord"/>를 생성해서 컬렉션에 추가한다.
        /// </summary>
        /// <param name="row"><see cref="DataRow"/> to add</param>
        /// <returns>추가된 <see cref="XdsRecord"/></returns>
        public XdsRecord AddRecord(DataRow row) {
            row.ShouldNotBeNull("row");

            var r = new XdsRecord(row);
            Add(r);
            return r;
        }

        /// <summary>
        /// <see cref="IDataReader"/>의 값으로부터 <see cref="XdsRecord"/>를 생성해서 컬렉션에 추가한다.
        /// </summary>
        /// <param name="dr"><see cref="IDataReader"/> to add</param>
        /// <returns>추가된 <see cref="XdsRecord"/></returns>
        public XdsRecord AddRecord(IDataReader dr) {
            dr.ShouldNotBeNull("dr");

            var r = new XdsRecord(dr);
            Add(r);
            return r;
        }
    }
}