using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CFileBase : DataEntityBase<Guid> {
        public CFileBase() {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// 파일명
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// 파일 크기
        /// </summary>
        public virtual long? Size { get; set; }

        public virtual byte[] Content { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(GetType(), Id, Name);
        }
    }
}