using System;
using System.Collections.Generic;
using System.Text;

namespace NSoft.NFramework.XmlData.Messages {
    /// <summary>
    /// 예외정보를 나타내는 <see cref="XdsError"/>의 컬렉션
    /// </summary>
    [Serializable]
    public class XdsErrorCollection : List<XdsError> {
        /// <summary>
        /// Add <see cref="XdsError"/> 
        /// </summary>
        /// <param name="item"></param>
        public void AddError(XdsError item) {
            base.Add(item);
        }

        /// <summary>
        /// Add <see cref="XdsError"/> with the specified exception.
        /// </summary>
        /// <param name="ex">exception</param>
        public void AddError(Exception ex) {
            if(ex != null)
                AddError(new XdsError(ex));
        }

        /// <summary>
        /// Add <see cref="XdsError"/> with the specified code, message, stacktrace
        /// </summary>
        /// <param name="code">error code number</param>
        /// <param name="msg">error message</param>
        /// <param name="stackTrace">stack trace</param>
        public void AddError(int code, string msg, string stackTrace) {
            AddError(new XdsError(code, msg, stackTrace, string.Empty));
        }

        /// <summary>
        /// Return string that represent current object.
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var sb = new StringBuilder();

            foreach(XdsError error in this)
                sb.AppendLine(error.ToString());

            return sb.ToString();
        }
    }
}