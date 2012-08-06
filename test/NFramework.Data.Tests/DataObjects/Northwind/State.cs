using System;

namespace NSoft.NFramework.Data.DataObjects.Northwind {
    [Serializable]
    public sealed class State : DataObjectBase {
        public string St { get; set; }

        public string StateName { get; set; }

        public override int GetHashCode() {
            return St.CalcHash();
        }
    }
}