using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CIssueFile : CFileBase {
        public virtual string IssueCode { get; set; }
    }
}