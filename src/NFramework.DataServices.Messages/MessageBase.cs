using System;
using System.Collections.Generic;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 요청/응답 문서의 기본 문서입니다.
    /// </summary>
    [Serializable]
    public abstract class MessageBase : MessageObjectBase {
        private readonly object _syncLock = new object();

        protected MessageBase() : this(MessageDirection.Request) {}

        protected MessageBase(MessageDirection direction) {
            MessageId = Guid.NewGuid();
            Direction = direction;
        }

        /// <summary>
        /// 요청 메시지 고유 Id
        /// </summary>
        public Guid MessageId { get; set; }

        /// <summary>
        /// 메시지 방향 (Request|Response)
        /// </summary>
        public MessageDirection Direction { get; set; }

        private IList<ErrorMessage> _errors;

        /// <summary>
        /// 예외 정보
        /// </summary>
        public IList<ErrorMessage> Errors {
            get { return _errors ?? (_errors = new List<ErrorMessage>()); }
            set { _errors = value; }
        }

        /// <summary>
        /// 요청 작업 처리에 예외가 있는지 표시합니다.
        /// </summary>
        public virtual bool HasError {
            get { return (_errors != null && _errors.Count > 0); }
        }

        /// <summary>
        /// 메시지에 예외정보 추가
        /// </summary>
        /// <param name="ex"></param>
        public void AddError(Exception ex) {
            if(ex != null)
                Errors.Add(new ErrorMessage(ex));
        }

        private IDictionary<string, string> _properties;

        /// <summary>
        /// 부가 정보
        /// </summary>
        public IDictionary<string, string> Properties {
            get { return _properties ?? (_properties = new Dictionary<string, string>()); }
            set { _properties = value; }
        }

        public void AddProperty(string key, string value) {
            lock(_syncLock) {
                if(Properties.ContainsKey(key))
                    Properties[key] = value;
                else
                    Properties.Add(key, value);
            }
        }

        public override int GetHashCode() {
            return Hasher.Compute(MessageId, Direction);
        }

        /// <summary>
        /// JSON 형식에서 DataTime 을 내부적으로 double이 아닌 long을 변경해서 저장하므로, .NET DateTime과 오차가 생길 수 있다.
        /// 직렬화된 정보 중 DateTime에 대한 비교는 꼭 ToUniversalJsonDateTime() 이용해서 DateTime을 변경한 후 비교해야 합니다.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ToUniversalJsonDateTime(DateTime? dateTime) {
            if(dateTime.HasValue) {
                var utcTime = dateTime.Value.ToUniversalTime();
                return utcTime.AddTicks(-(utcTime.Ticks % 10000));
            }

            return (DateTime?)null;
        }
    }
}