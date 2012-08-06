using System;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 메시지를 구성하는 요소의 기본 클래스
    /// </summary>
    [Serializable]
    public abstract class MessageObjectBase : IEquatable<IMessageObject> {
        public const string MessageFormatVersion = "1.0.0";

        protected MessageObjectBase() {
            FormatVersion = MessageFormatVersion;
        }

        /// <summary>
        /// Message Format Version
        /// </summary>
        public string FormatVersion { get; set; }

        public bool Equals(IMessageObject other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override bool Equals(object obj) {
            return (obj != null) && (obj is IMessageObject) && Equals((IMessageObject)obj);
        }

        public override int GetHashCode() {
            return 0;
        }
    }
}