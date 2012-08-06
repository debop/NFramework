using System;
using System.Globalization;

namespace NSoft.NFramework.Data.NHibernateEx.Domain {
    [Serializable]
    public class Locale : AssignedIdEntityBase<CultureInfo> {
        protected Locale() {
            //Id = CultureInfo.InvariantCulture;
            //Name = Id.Name;
        }

        public Locale(CultureInfo culture) {
            Id = culture ?? CultureInfo.InvariantCulture;
            Name = culture.Name;
        }

        public virtual string Name { get; protected set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : Name.CalcHash();
        }

        public override string ToString() {
            return string.Format("Locale# Id=[{0}], Name=[{1}]", Id, Name);
        }
    }
}