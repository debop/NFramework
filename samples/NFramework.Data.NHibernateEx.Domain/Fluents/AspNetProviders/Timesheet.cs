using System;
using System.Collections.Generic;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Mappings {
    [Serializable]
    public class Timesheet : DataEntityBase<Int32> {
        public virtual DateTime SubmittedDate { get; set; }
        public virtual bool Submitted { get; set; }

        private IList<User> _users;

        public virtual IEnumerable<User> Users {
            get { return _users ?? (_users = new List<User>()); }
        }

        private IList<TimesheetEntry> _entries;

        public virtual IList<TimesheetEntry> Entries {
            get { return _entries ?? (_entries = new List<TimesheetEntry>()); }
        }

        public override int GetHashCode() {
            if(IsSaved)
                return base.GetHashCode();

            return HashTool.Compute(SubmittedDate, Submitted);
        }
    }

    public class TimesheetEntry : DataEntityBase<Int32> {
        public virtual DateTime EntryDate { get; set; }
        public virtual int NumberOfHours { get; set; }
        public virtual string Comments { get; set; }

        public override int GetHashCode() {
            return IsSaved ? base.GetHashCode() : HashTool.Compute(EntryDate, NumberOfHours);
        }
    }
}