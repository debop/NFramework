using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Loquacious {
    [Serializable]
    public class CoreUser : DataEntityBase<Int32>, IUpdateTimestampedEntity {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public bool? Locked { get; set; }

        private ICollection<CoreTask> _tasks;

        public ICollection<CoreTask> Tasks {
            get { return _tasks ?? (_tasks = new List<CoreTask>()); }
            protected set { _tasks = value; }
        }

        public DateTime? UpdateTimestamp { get; set; }

        public override int GetHashCode() {
            return (IsSaved) ? base.GetHashCode() : HashTool.Compute(Name, Email);
        }
    }
}