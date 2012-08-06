using System;

namespace NSoft.NFramework.FusionCharts.WebHost.Domain.Model {
    [Serializable]
    public class FactoryOutput {
        public virtual int FactoryId { get; set; }
        public virtual DateTime? DatePro { get; set; }
        public virtual int? Quantity { get; set; }

        public override string ToString() {
            return string.Format(@"FactoryOutput# FactoryId={0}, DatePro={1}, Quantity={2}",
                                 FactoryId, DatePro, Quantity);
        }
    }
}