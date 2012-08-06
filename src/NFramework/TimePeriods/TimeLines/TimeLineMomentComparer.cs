using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods.TimeLines {
    /// <summary>
    /// <see cref="ITimeLineMoment"/> 비교자
    /// </summary>
    public class TimeLineMomentComparer : IComparer<ITimeLineMoment> {
        public int Compare(ITimeLineMoment x, ITimeLineMoment y) {
            return x.Moment.CompareTo(y.Moment);
        }
    }
}