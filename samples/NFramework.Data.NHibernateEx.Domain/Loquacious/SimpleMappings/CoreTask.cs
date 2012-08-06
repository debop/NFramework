using System;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CoreTask : DataEntityBase<Int32> {
        public string Name { get; set; }
        public bool? Planned { get; set; }
        public string Notes { get; set; }
        public string Estimation { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? DueAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool? Archieved { get; set; }

        public CoreUser User { get; set; }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(Name, User);
        }
    }
}