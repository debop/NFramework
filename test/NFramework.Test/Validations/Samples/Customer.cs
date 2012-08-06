using System;

namespace NSoft.NFramework.Validations {
    [Serializable]
    public class Customer : ValueObjectBase {
        public string Name { get; set; }

        public string Company { get; set; }

        public string ZipCode { get; set; }

        public Decimal? Discount { get; set; }

        public bool? HasDiscount { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(Name, Company);
        }
    }
}