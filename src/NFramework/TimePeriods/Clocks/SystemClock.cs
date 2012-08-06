using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 시스템에서 제공하는 현재 시각을 제공합니다.
    /// </summary>
    [Serializable]
    public class SystemClock : AbstractClock {
        internal SystemClock() {}

        public override DateTime Now {
            get { return DateTime.Now; }
        }
    }
}