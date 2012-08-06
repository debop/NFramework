using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// Stored Procedure의 Parameter를 표현하는 Class (<P paramType="string">파라미터 이름</P>)
    /// </summary>
    [Serializable]
    public class XdsParameter {
        /// <summary>
        /// Constructor
        /// </summary>
        public XdsParameter() : this("", "") {}

        /// <summary>
        /// Initialize a new instance of XdsParameter with parameter name and type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public XdsParameter(string name, string type) {
            ParamName = name;
            ParamType = type;
        }

        #region << Properties >>

        /// <summary>
        /// Parameter Name
        /// </summary>
        [XmlText]
        public string ParamName { get; set; }

        /// <summary>
        /// Parameter Type Name
        /// </summary>
        [XmlAttribute("paramType")]
        public string ParamType { get; set; }

        #endregion
    }

    /// <summary>
    /// Collection of <see cref="XdsParameter"/>
    /// </summary>
    [Serializable]
    public class XdsParameterCollection : List<XdsParameter> {
        /// <summary>
        /// Add Parameter with parameter name
        /// </summary>
        /// <param name="paramName">parameter name</param>
        /// <returns>new instance of <see cref="XdsParameter"/></returns>
        public XdsParameter AddParameter(string paramName) {
            return AddParameter(paramName, string.Empty);
        }

        /// <summary>
        /// Add new instance of <see cref="XdsParameter"/> with parameter name and type.
        /// </summary>
        /// <param name="paramName">parameter name</param>
        /// <param name="paramType">parameter type name</param>
        /// <returns>new instance of <see cref="XdsParameter"/></returns>
        public XdsParameter AddParameter(string paramName, string paramType) {
            var p = new XdsParameter(paramName, paramType);
            Add(p);
            return p;
        }
    }
}