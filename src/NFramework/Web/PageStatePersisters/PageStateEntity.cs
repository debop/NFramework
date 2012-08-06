using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Web.PageStatePersisters {
    /// <summary>
    /// Web Page 상태 정보를 Persister가 저장하고 로드하는 엔티티의 원형입니다.
    /// </summary>
    [Serializable]
    public class PageStateEntity : ValueObjectBase, IPageStateEntity {
        public PageStateEntity() : this(string.Empty, null, false) {}

        public PageStateEntity(string id, byte[] value, bool isCompressed) {
            Id = id.IsNotWhiteSpace() ? id : Guid.NewGuid().ToString();
            Value = value;
            IsCompressed = isCompressed;
            CreatedDate = DateTime.Now;
        }

        /// <summary>
        /// 엔티티의 Identifier
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 압축 여부
        /// </summary>
        public bool IsCompressed { get; set; }

        /// <summary>
        /// Page 상태 정보의 Snapshot
        /// </summary>
        public byte[] Value { get; set; }

        /// <summary>
        /// 생성 일자
        /// </summary>
        public DateTime CreatedDate { get; set; }

        public bool Equals(IPageStateEntity other) {
            return (other != null) && GetHashCode().Equals(other.GetHashCode());
        }

        public override int GetHashCode() {
            return HashTool.Compute(Id, IsCompressed);
        }

        public override string ToString() {
            return string.Format("PageStateEntity# Id=[{0}], IsCompressed=[{1}], Value=[{2}], CreatedDate=[{3}]", Id, IsCompressed,
                                 Value, CreatedDate);
        }
    }
}