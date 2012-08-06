using System;
using System.Collections.Generic;
using System.Linq;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.Fluents.Timesheets {
    /// <summary>
    /// 일단위 Timesheet 를 기간별 Rule로 상호 변환할 수 있는 메소드를 제공합니다.
    /// </summary>
    public static class TimesheetTool {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public static IEnumerable<ProjectTaskTimeSheet> GenerateTimesheets(this ProjectTaskTimephasedRule rule) {
            var days = (decimal)rule.TimeRange.Duration.TotalDays;

            if(log.IsDebugEnabled)
                log.Debug("rule.WeightValue=[{0}], Days=[{1}], TimeRange=[{2}]", rule.WeightValue, days, rule.TimeRange);

            var weightValue = rule.WeightValue / days;

            return
                rule.TimeRange
                    .ForEachDays()
                    .Select(day => {
                                return new ProjectTaskTimeSheet(rule.ProjectId, rule.TaskId, rule.Id)
                                       {
                                           StartTime = day.Start,
                                           EndTime = day.End,
                                           PlanWeightValue = weightValue
                                       };
                            });
        }

        public static IEnumerable<ProjectTaskTimeSheet> GenerateTimesheets(this IEnumerable<ProjectTaskTimephasedRule> rules) {
            return
                rules
                    .OrderBy(r => r.TaskId)
                    .ThenBy(r => r.TimeRange.Start)
                    .SelectMany(rule => rule.GenerateTimesheets());
        }

        public static IEnumerable<ProjectTaskTimeSheet> GenerateTimesheetsAsParallel(this IEnumerable<ProjectTaskTimephasedRule> rules) {
#if !SILVERLIGHT
            return
                rules
                    .OrderBy(r => r.TaskId)
                    .ThenBy(r => r.TimeRange.Start)
                    .AsParallel()
                    .AsOrdered()
                    .SelectMany(rule => rule.GenerateTimesheets());
#else
			var _syncLock = new object();
			var timesheets = new List<ProjectTaskTimeSheet>();

			Parallel.ForEach(rules,
							 (rule, loop) =>
							 {
								 lock(_syncLock)
									 timesheets.AddRange(rule.GenerateTimesheets());
							 });

			return timesheets.OrderBy(r => r.TaskId).ThenBy(r => r.StartTime);

#endif
        }

        public static IEnumerable<ProjectTaskTimephasedRule> GenerateTimephasedRules(this IEnumerable<ProjectTaskTimeSheet> timesheets) {
            ProjectTaskTimeSheet prevTimesheet = null;
            ProjectTaskTimephasedRule rule = null;

            foreach(var timesheet in timesheets.OrderBy(ts => ts.TaskId).ThenBy(ts => ts.StartTime)) {
                var populateRule = (prevTimesheet == null) || (prevTimesheet.TaskId != timesheet.TaskId) ||
                                   (prevTimesheet.PlanWeightValue != timesheet.PlanWeightValue);

                if(populateRule) {
                    if(rule != null) {
                        yield return rule;
                    }

                    rule = new ProjectTaskTimephasedRule(timesheet.ProjectId, timesheet.TaskId)
                           {
                               TimeRange = new TimeRange(timesheet.StartTime, TimeSpan.FromDays(1)),
                               WeightValue = timesheet.PlanWeightValue
                           };
                }
                else {
                    rule.TimeRange.End = timesheet.EndTime;
                    rule.WeightValue += timesheet.PlanWeightValue;
                }

                prevTimesheet = timesheet;
            }

            if(rule != null)
                yield return rule;
        }

        public static IEnumerable<ProjectTaskTimephasedRule> GenerateTimephasedRulesAsParallel(
            this IEnumerable<ProjectTaskTimeSheet> timesheets) {
            var timesheetList = timesheets.ToList();

#if !SILVERLIGHT

            return
                timesheets
                    .Select(x => x.TaskId)
                    .Distinct()
                    .AsParallel()
                    .AsOrdered()
                    .SelectMany(taskId => {
                                    ProjectTaskTimeSheet prevTimesheet = null;
                                    ProjectTaskTimephasedRule rule = null;

                                    var rules = new List<ProjectTaskTimephasedRule>();

                                    foreach(var timesheet in timesheetList.Where(x => x.TaskId == taskId)) {
                                        var populateRule = (prevTimesheet == null) ||
                                                           (prevTimesheet.PlanWeightValue != timesheet.PlanWeightValue);

                                        if(populateRule) {
                                            rule = new ProjectTaskTimephasedRule(timesheet.ProjectId, timesheet.TaskId)
                                                   {
                                                       TimeRange = new TimeRange(timesheet.StartTime, TimeSpan.FromDays(1)),
                                                       WeightValue = timesheet.PlanWeightValue
                                                   };
                                            rules.Add(rule);
                                        }
                                        else {
                                            rule.TimeRange.End = timesheet.EndTime;
                                            rule.WeightValue += timesheet.PlanWeightValue;
                                        }

                                        prevTimesheet = timesheet;
                                    }
                                    return rules;
                                });
#else
			var rules = new List<ProjectTaskTimephasedRule>();

			object _syncLock = new object();

			Parallel.ForEach(timesheetList.Select(x => x.TaskId).Distinct(),
							 () => new List<ProjectTaskTimephasedRule>(),
							 (taskId, loop, localRules) =>
							 {
								 ProjectTaskTimeSheet prevTimesheet = null;
								 ProjectTaskTimephasedRule rule = null;

								 foreach(var timesheet in timesheetList.Where(x => x.TaskId == taskId))
								 {
									 var populateRule = (prevTimesheet == null) || (prevTimesheet.PlanWeightValue != timesheet.PlanWeightValue);

									 if(populateRule)
									 {
										 rule = new ProjectTaskTimephasedRule(timesheet.ProjectId, timesheet.TaskId)
												{
													TimeRange = new TimeRange(timesheet.StartTime, TimeSpan.FromDays(1)),
													WeightValue = timesheet.PlanWeightValue
												};
										 localRules.Add(rule);
									 }
									 else
									 {
										 rule.TimeRange.End = timesheet.EndTime;
										 rule.WeightValue += timesheet.PlanWeightValue;
									 }

									 prevTimesheet = timesheet;
								 }
								 return localRules;
							 },
							 localRules =>
							 {
								 lock(_syncLock)
									 rules.AddRange(localRules);
							 });

			return rules.OrderBy(r => r.TaskId).ThenBy(r => r.TimeRange.Start);
#endif
        }
    }
}