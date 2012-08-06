using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerConcrete {
    [Serializable]
    public abstract class FileBase : DataEntityBase<Guid> {
        protected FileBase() {
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

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Id, Name, GetType().FullName);
        }
    }
}