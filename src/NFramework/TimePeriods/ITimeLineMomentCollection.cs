using System;
using System.Collections.Generic;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// <see cref="ITimePeriod"/>를 시작시각,완료시각을 키로 가지고, <see cref="ITimePeriod"/>를 Value로 가지는 MultiMap{DateTime, ITimePeriod} 을 생성합니다. 단 시각으로 정렬됩니다.
    /// </summary>
    public interface ITimeLineMomentCollection : IEnumerable<ITimeLineMoment> {
        int Count { get; }

        bool IsEmpty { get; }

        ITimeLineMoment Min { get; }

        ITimeLineMoment Max { get; }

        ITimeLineMoment this[int index] { get; }

        void Add(ITimePeriod period);

        void AddAll(IEnumerable<ITimePeriod> periods);

        void Remove(ITimePeriod period);

        ITimeLineMoment Find(DateTime moment);

        bool Contains(DateTime moment);
    }
}