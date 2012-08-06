using FluentNHibernate.Mapping;
using NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    public class TimephasedRuleBaseMap : ClassMap<TimephasedRuleBase> {
        public TimephasedRuleBaseMap() {
            Table("TimephasedRule");
            DynamicInsert();
            DynamicUpdate();
            Proxy<ITimephasedRule>();

            Id(x => x.Id).Column("RuleId").GeneratedBy.Assigned();

            DiscriminateSubClassesOnColumn("TimephasedKind").Length(64).Not.Nullable();

            Map(x => x.TimeRange)
                .CustomType<TimeRangeUserType>()
                .Index("IX_TimephasedRule_TimeRange")
                .Columns.Clear()
                .Columns.Add("StartTime")
                .Columns.Add("EndTime");

            Map(x => x.IsException);
            Map(x => x.WeightValue);

            Map(x => x.WorkValue);
            Map(x => x.ProgressValue);
            Map(x => x.CostValue);
            Map(x => x.Ext1Value);

            Map(x => x.Creator).Length(50);
            Map(x => x.CreateDate);

            Map(x => x.UpdateTimestamp);
        }
    }
}