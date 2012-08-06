using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.InheritanceMappings.TablePerConcrete {
    [Serializable]
    public class RiskFile : FileBase {
        /// <summary>
        /// 관련 Risk Code
        /// </summary>
        public virtual string RiskCode { get; set; }
    }
}