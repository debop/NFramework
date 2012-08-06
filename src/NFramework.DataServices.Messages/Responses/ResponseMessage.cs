using System;
using System.Collections.Generic;
using System.Linq;

namespace NSoft.NFramework.DataServices.Messages {
    /// <summary>
    /// 응답 메시지
    /// </summary>
    [Serializable]
    public class ResponseMessage : MessageBase {
        public ResponseMessage() : base(MessageDirection.Response) {
            CreatedUtcTime = DateTime.Now;
        }

        private IList<ResponseItem> _items;

        /// <summary>
        /// 결과 항목들
        /// </summary>
        public IList<ResponseItem> Items {
            get { return _items ?? (_items = new List<ResponseItem>()); }
            set { _items = value; }
        }

        public override bool HasError {
            get {
                if(base.HasError)
                    return true;
                return Items.Any(item => item.HasError);
            }
        }

        public IEnumerable<ErrorMessage> AggregateErrors {
            get { return Enumerable.Union(base.Errors, Items.SelectMany(x => x.Errors)); }
        }

        private DateTime? _createdUtcTime;

        public DateTime? CreatedUtcTime {
            get { return _createdUtcTime; }
            set { _createdUtcTime = ToUniversalJsonDateTime(value); }
        }
    }
}