using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CRiskFile : CFileBase {
        public virtual string RiskCode { get; set; }
    }
}