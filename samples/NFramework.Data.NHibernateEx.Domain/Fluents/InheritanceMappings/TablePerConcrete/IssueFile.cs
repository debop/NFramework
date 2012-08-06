using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerConcrete {
    [Serializable]
    public class IssueFile : FileBase {
        /// <summary>
        /// 관련 Issue 코드
        /// </summary>
        public virtual string IssueCode { get; set; }
    }
}