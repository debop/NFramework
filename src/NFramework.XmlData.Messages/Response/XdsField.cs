using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Database 수행 결과를 나타내는 ResultSet의 컬럼 스키마 정보를 표현한다.
    /// </summary>
    [Serializable]
    public class XdsField {
        #region << Constructors >>

        /// <summary>
        /// Constructor
        /// </summary>
        public XdsField() : this(string.Empty, string.Empty, 0) {}

        /// <summary>
        /// Initialize a new instance of <see cref="XdsField"/> with the specified field name, type, size
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="fieldType">field type</param>
        /// <param name="fieldSize">field size</param>
        public XdsField(string fieldName, string fieldType, int fieldSize) {
            Name = fieldName;
            TypeName = fieldType;
            Size = fieldSize;
        }

        #endregion

        #region << Properties >>

        /// <summary>
        /// field name
        /// </summary>
        [XmlText]
        public string Name { get; set; }

        /// <summary>
        /// field type
        /// </summary>
        [XmlAttribute("paramType")]
        public string TypeName { get; set; }

        /// <summary>
        /// field size
        /// </summary>
        [XmlAttribute("size")]
        public int Size { get; set; }

        #endregion
    }

    /// <summary>
    /// Collectio of <see cref="XdsFieldCollection"/>
    /// </summary>
    [Serializable]
    public class XdsFieldCollection : List<XdsField> {
        #region << AddField >>

        /// <summary>
        /// Add a new instance of <see cref="XdsField"/> that has the specified field name.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <returns>added new instance of <see cref="XdsField"/></returns>
        public virtual XdsField AddField(string fieldName) {
            var field = new XdsField { Name = fieldName };
            Add(field);
            return field;
        }

        /// <summary>
        /// Add a new instance of <see cref="XdsField"/> that has the specified field name, type, size.
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <param name="fieldType">field type</param>
        /// <param name="size">field size</param>
        /// <returns>added new instance of <see cref="XdsField"/></returns>
        public virtual XdsField AddField(string fieldName, string fieldType, int size) {
            var field = new XdsField(fieldName, fieldType, size);
            Add(field);
            return field;
        }

        /// <summary>
        /// DataTable의 Column 정보를 나타내는 <see cref="DataColumn"/> 파싱해서 <see cref="XdsField"/>를 빌드해서 컬렉션에 추가한다.
        /// </summary>
        /// <param name="dataColumn">data column</param>
        /// <returns></returns>
        public virtual XdsField AddField(DataColumn dataColumn) {
            dataColumn.ShouldNotBeNull("dataColumn");

            string fieldName = dataColumn.ColumnName;
            string fieldType = dataColumn.DataType.FullName;
            int fieldSize = MsgConsts.INVALID_ID;

            if(dataColumn.DataType.Equals(typeof(string)))
                fieldSize = dataColumn.MaxLength;

            return AddField(fieldName, fieldType, fieldSize);
        }

        /// <summary>
        /// 주어진 필드명을 가지는 <see cref="XdsField"/> 들을 생성하여 컬렉션에 추가한다.
        /// </summary>
        /// <param name="fieldNames"></param>
        public virtual void AddFieldArray(params string[] fieldNames) {
            foreach(string fieldName in fieldNames)
                AddField(fieldName);
        }

        /// <summary>
        /// Adds the elements of the specified collection of <see cref="XdsField"/> to the end of the <see cref="XdsFieldCollection"/>
        /// </summary>
        /// <param name="fields">collection of <see cref="XdsField"/></param>
        public virtual void AddFieldArray(params XdsField[] fields) {
            if(fields != null && fields.Length > 0)
                base.AddRange(fields);
        }

        #endregion
    }
}