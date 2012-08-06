using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    public class FSms : DataEntityBase<Int32>, IUpdateTimestampedEntity {
        public virtual string Message { get; set; }

        /// <summary>
        /// Json 직렬화로 저장할 정보
        /// </summary>
        public virtual object JsonData { get; set; }

        public virtual object JsonCompressedData { get; set; }

        public virtual DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Message);
        }
    }
}