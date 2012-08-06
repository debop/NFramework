namespace NSoft.NFramework.TimePeriods {
    [Microsoft.Silverlight.Testing.Tag("DynamicProxy")]
    public abstract class TimePeriodFixtureBase : AbstractFixture {
        #region << logger >>

        protected static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        protected static readonly bool IsDebugEnabled = log.IsDebugEnabled;
        protected static readonly bool IsInfoEnabled = log.IsInfoEnabled;

        #endregion

        public const int January = 1;
        public const int Feburary = 2;
        public const int March = 3;
        public const int April = 4;
        public const int May = 5;
        public const int June = 6;
        public const int July = 7;
        public const int August = 8;
        public const int September = 9;
        public const int October = 10;
        public const int November = 11;
        public const int December = 12;

        protected static readonly WeekOfYearRuleKind[] WeekOfYearRules = new WeekOfYearRuleKind[]
                                                                         { WeekOfYearRuleKind.Calendar, WeekOfYearRuleKind.Iso8601 };
    }
}