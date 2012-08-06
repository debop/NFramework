using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    ///  <see cref="XdsParameter"/> 에 대응되는 하나의 Argument 값이다.
    /// </summary>
    [Serializable]
    public class XdsArgument {
        /// <summary>
        /// default constructor
        /// </summary>
        public XdsArgument() {}

        /// <summary>
        /// Initialize a new instance of <see cref="XdsArgument"/> with the specified argument
        /// </summary>
        /// <param name="argument">argument value</param>
        public XdsArgument(object argument) {
            Argument = (argument != null) ? argument.ToString() : string.Empty;
        }

        /// <summary>
        /// 인자 값
        /// </summary>
        [XmlText]
        public string Argument { get; set; }
    }

    /// <summary>
    /// Collection of <see cref="XdsArgument"/>
    /// </summary>
    [Serializable]
    public class XdsArgumentCollection : List<XdsArgument> {
        /// <summary>
        /// Create a new argument with the specified argument and add to current collection.
        /// </summary>
        /// <param name="arg">value of <see cref="XdsArgument"/></param>
        /// <returns>index of collection</returns>
        public int AddArgument(object arg) {
            Add(new XdsArgument(arg));
            return Count - 1;
        }

        /// <summary>
        /// Add array of <see cref="XdsArgument"/> which has the specified args to current collection.
        /// </summary>
        /// <param name="args">values of <see cref="XdsArgument"/></param>
        public void AddArgumentArray(params object[] args) {
            if(args != null && args.Length > 0)
                for(int i = 0; i < args.Length; i++)
                    AddArgument(args[i]);
        }
    }
}