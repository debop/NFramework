using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// <see cref="XdsArgument"/>의 SET을 가진 것이다.
    /// </summary>
    [Serializable]
    public class XdsValue {
        /// <summary>
        /// Constructor
        /// </summary>
        public XdsValue() {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="args">인자값</param>
        public XdsValue(params object[] args) {
            _arguments.AddArgumentArray(args);
        }

        private readonly XdsArgumentCollection _arguments = new XdsArgumentCollection();

        /// <summary>
        /// 인자 정보
        /// </summary>
        [XmlElement("P")]
        public XdsArgumentCollection Arguments {
            get { return _arguments; }
        }
    }

    /// <summary>
    /// Collection of <see cref="XdsValue"/>
    /// </summary>
    [Serializable]
    public class XdsValueCollection : List<XdsValue> {
        /// <summary>
        /// <see cref="XdsArgument"/>의 배열을 하나의 Value Set으로 추가한다.
        /// </summary>
        /// <param name="args"><see cref="XdsValue"/>의 <see cref="XdsArgument"/> 컬렉션</param>
        /// <returns></returns>
        public XdsValue AddValue(params object[] args) {
            var v = new XdsValue(args);
            Add(v);
            return v;
        }
    }
}