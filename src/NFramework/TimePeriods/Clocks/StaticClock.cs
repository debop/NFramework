using System;

namespace NSoft.NFramework.TimePeriods {
    /// <summary>
    /// 생성자에 의해 지정된 시간을 현재 시간으로 제공합니다. (단위 테스트 실행 시마다 현재 시각이 변경되는 것을 방지하기 위해서 사용합니다)
    /// </summary>
    [Serializable]
    public class StaticClock : AbstractClock {
        public StaticClock(DateTime now) : base(now) {}
    }
}