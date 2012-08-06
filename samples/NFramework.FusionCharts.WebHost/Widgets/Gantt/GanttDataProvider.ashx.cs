using System;
using System.Drawing;
using System.Web.Services;
using NSoft.NFramework.FusionCharts.Web;
using NSoft.NFramework.FusionCharts.Widgets;
using NSoft.NFramework.Threading;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.WebHost.Widgets.Gantt {
    [WebService(Namespace = "http://www.realweb21.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class GanttDataProvider : DataXmlHandlerBase {
        private static readonly Random rnd = new ThreadSafeRandom();

        public override IChart BuildFusionChart() {
            var option = Request["Option"].AsText("MultiTask");

            GanttChart chart;

            switch(option) {
                case "SimpleChart":
                    chart = (GanttChart)CreateSampleChart("Simple Chart");
                    break;
                case "MultipleCategory":
                    chart = (GanttChart)MultipleCategories();
                    break;
                case "Scrollable":
                    chart = (GanttChart)ChartWithScrolling();
                    break;
                case "DataTable":
                    chart = (GanttChart)ChartHasDataTable();
                    break;
                case "MultiTask":
                    chart = (GanttChart)MultiTaskedProcesses();
                    break;
                case "Milestone":
                    chart = (GanttChart)MilestoneChart();
                    break;
                case "Connector":
                    chart = (GanttChart)ConnectorChart();
                    break;
                default:
                    chart = (GanttChart)ConnectorChart();
                    break;
            }

            chart.SubCaption = "Generated at " + DateTime.Now.ToSortableString();

            return chart;
        }

        #region << Sample Chart >>

        #region << 기본 Data >>

        public static readonly DateTime StartDate = new DateTime(2009, 9, 1);
        public static readonly ITimePeriod ProjectPeriod = StartDate.GetRelativeMonthPeriod(9); //StartDate.GetRelativeMonthRange(9);

        public static readonly string[] ProcessLabels = {
                                                            "고객 파악",
                                                            "Survey 50 Customers",
                                                            "Interpret Requirements",
                                                            "Study Competition",
                                                            "Documentation of features",
                                                            "Brainstorm concepts",
                                                            "설계 & 구현",
                                                            "Testing / QA",
                                                            "Documentation of product",
                                                            "Global Release"
                                                        };

        public static readonly ITimePeriod[] TaskPeriods = {
                                                               ProjectPeriod.Start.AddDays(3).GetRelativeDayPeriod(6),
                                                               ProjectPeriod.Start.AddDays(8).GetRelativeDayPeriod(11),
                                                               ProjectPeriod.Start.AddDays(19).GetRelativeDayPeriod(12),
                                                               ProjectPeriod.Start.AddMonths(1).AddDays(1).GetRelativeDayPeriod(20),
                                                               ProjectPeriod.Start.AddMonths(1).AddDays(21).GetRelativeDayPeriod(15),
                                                               ProjectPeriod.Start.AddMonths(2).AddDays(5).GetRelativeDayPeriod(60),
                                                               ProjectPeriod.Start.AddMonths(5).AddDays(20).GetRelativeDayPeriod(30),
                                                               ProjectPeriod.Start.AddMonths(6).AddDays(-3).GetRelativeDayPeriod(26),
                                                               ProjectPeriod.Start.AddMonths(6).AddDays(24).GetRelativeDayPeriod(10),
                                                               ProjectPeriod.Start.AddMonths(7).GetRelativeDayPeriod(30)
                                                           };

        public static readonly string[] RoleNames = {
                                                        "홍길동",
                                                        "David",
                                                        "Mary",
                                                        "Andrew",
                                                        "Tiger",
                                                        "Sharon",
                                                        "Neil",
                                                        "Harry",
                                                        "Chris",
                                                        "Rechard"
                                                    };

        #endregion

        private static IChart CreateSampleChart(string caption) {
            var chart = new GanttChart
                        {
                            Caption = caption,
                            SubCaption = "소제목입니다.",
                            Palette = 2,
                            BaseFontAttr =
                            {
                                Font = "맑은 고딕",
                                FontColor = "#0372AB".FromHtml(),
                                FontSize = "8"
                            },
                        };

            GanttUtil.GenerateCategories(chart.CategoriesList, ProjectPeriod, PeriodFlags.YearWeek);

            chart.Processes.FontAttr.Font = "맑은 고딕";
            chart.Processes.FontAttr.FontSize = 12.ToString();
            chart.Processes.FontAttr.IsBold = true;
            chart.Processes.FontAttr.IsItalic = false;
            chart.Processes.Align = FusionTextAlign.Right;
            chart.Processes.HeaderText = "What to do?";
            chart.Processes.HeaderAttr.FontAttr.FontSize = 18.ToString();
            chart.Processes.HeaderAttr.Align = FusionTextAlign.Left;
            chart.Processes.HeaderAttr.VAlign = FusionVerticalAlign.Middle;

            // task의 진행률을 Percentage로 나타낼 때, 완료하지 않은 부분
            chart.SlackFillColor = Color.DarkGray;
            // Task의 진행률을 나타냅니다.
            chart.ShowPercentLabel = true;

            var procId = 0;
            foreach(string label in ProcessLabels) {
                var process = new ProcessElement("Process" + procId++)
                              {
                                  ItemAttr =
                                  {
                                      Label = label,
                                      FontAttr =
                                      {
                                          Font = "맑은 고딕",
                                          FontColor = Color.FromArgb(0, 0x03, 0x72, 0xAB),
                                          FontSize = 13.ToString(),
                                          IsBold = true
                                      }
                                  }
                              };

                // process.ItemAttr.FontAttr.IsItalic = true;
                // process.ItemAttr.FontAttr.IsUnderline = true;
                // process.ItemAttr.BgColor = Color.White;
                process.ItemAttr.Align = FusionTextAlign.Left;
                process.ItemAttr.VAlign = FusionVerticalAlign.Middle;

                // chart.Processes.ProcessElements.Add(process);
                chart.Processes.Add(process);
            }

            var taskId = 0;
            foreach(var taskPeriod in TaskPeriods) {
                var task = new TaskElement
                           {
                               Start = taskPeriod.Start,
                               End = taskPeriod.End,
                               Id = taskId++.ToString(),
                               Color = Color.SteelBlue,
                               ShowLabel = true,
                               Animation = true,
                               BorderThickness = 1,
                               PercentComplete = rnd.Next(0, 101)
                           };

                // Task의 진행률

                // chart.Tasks.TaskElements.Add(task);
                chart.Tasks.Add(task);
            }

            return chart;
        }

        private static IChart MultipleCategories() {
            var chart = (GanttChart)CreateSampleChart("Multiple Categories Gantt Chart");

            GanttUtil.GenerateCategories(chart.CategoriesList, ProjectPeriod, PeriodFlags.YearWeek);

            return chart;
        }

        private static IChart ChartWithScrolling() {
            var chart = (GanttChart)MultipleCategories();
            chart.Caption = "Gantt chart with Scrolling";

            // 3 개월 단위로 Scrolling을 수행한다.
            chart.GanttPaneDuration = 6;
            chart.GanttPaneDurationUnit = "m";

            //  Scrollable Chart
            chart.ScrollBarAttr.ScrollColor = Color.LimeGreen;
            chart.ScrollBarAttr.ScrollPadding = 3;
            chart.ScrollBarAttr.ScrollHeight = 16;
            chart.ScrollBarAttr.ScrollBtnWidth = 16;
            chart.ScrollBarAttr.ScrollBtnPadding = 3;

            return chart;
        }

        private static IChart ChartHasDataTable() {
            var chart = (GanttChart)ChartWithScrolling();

            chart.Caption = "Chart with DataTable";
            chart.DataTable.HeaderAttr.VAlign = FusionVerticalAlign.Bottom;

            var dataColumn = new DataColumnElement
                             {
                                 HeaderText = "Worker"
                             };
            dataColumn.HeaderAttr.FontAttr.Font = "맑은 고딕";
            dataColumn.HeaderAttr.FontAttr.FontColor = Color.DarkSlateGray;
            dataColumn.HeaderAttr.FontAttr.FontSize = 18.ToString();
            dataColumn.HeaderAttr.VAlign = FusionVerticalAlign.Bottom;
            dataColumn.HeaderAttr.Align = FusionTextAlign.Right;
            dataColumn.ItemAttr.Align = FusionTextAlign.Left;
            dataColumn.ItemAttr.FontAttr.FontSize = 12.ToString();

            foreach(var worker in RoleNames) {
                var text = new DataColumnTextElement { Label = worker };
                text.ItemAttr.FontAttr.Font = "맑은 고딕";
                text.ItemAttr.FontAttr.FontSize = "12";
                text.ItemAttr.Align = FusionTextAlign.Center;
                // mailto 는 javascript로 처리할 수 없다...
                // text.Link.Javascript = "mailto:rcl@realweb21.com";
                // dataColumn.TextElements.Add(text);
                dataColumn.Add(text);
            }

            // chart.DataTable.DataColumnElements.Add(dataColumn);
            chart.DataTable.Add(dataColumn);

            return chart;
        }

        private static IChart MultiTaskedProcesses() {
            var chart = (GanttChart)ChartHasDataTable();

            chart.Tasks.ShowLabels = true;

            // NOTE : Process 별로 다중의 Task가 존재할 때를 테스트한다.
            // chart.Tasks.TaskElements.Clear();
            chart.Tasks.Clear();

            for(int i = 0; i < TaskPeriods.Length; i++) {
                var taskPeriod = TaskPeriods[i];

                // Planned:
                var planTask = new TaskElement
                               {
                                   Start = taskPeriod.Start,
                                   End = taskPeriod.End,
                                   Id = "Planed Task" + i,
                                   ProcessId = "Process" + i,
                                   Label = "Planed",
                                   Color = Color.LightSlateGray,
                                   ShowLabel = true,
                                   Animation = true,
                                   BorderThickness = 1,
                                   Height = "25%",
                                   TopPadding = "22%"
                               };

                // chart.Processes.ProcessElements[i].Tasks.TaskElements.Add(planTask);
                chart.Processes[i].Tasks.Add(planTask);

                // Planned:
                var actualTask = new TaskElement
                                 {
                                     Start = taskPeriod.Start.AddDays(5),
                                     End = taskPeriod.End.AddDays(10),
                                     Id = "Actual Task" + i,
                                     ProcessId = "Process" + i,
                                     Label = "Actual",
                                     Color = Color.YellowGreen,
                                     ShowLabel = true,
                                     Animation = true,
                                     BorderThickness = 1,
                                     Height = "25%",
                                     TopPadding = "70%"
                                 };

                // chart.Processes.ProcessElements[i].Tasks.TaskElements.Add(actualTask);
                chart.Processes[i].Tasks.Add(actualTask);
            }

            chart.LegendAttr.Show = true;
            // chart.Legend.Items.Add(new LegendItemElement("Planned", Color.LightSlateGray));
            // chart.Legend.Items.Add(new LegendItemElement("Actual", Color.YellowGreen));
            chart.Legend.Add(new LegendItemElement("Planned", Color.LightSlateGray));
            chart.Legend.Add(new LegendItemElement("Actual", Color.Green));

            return chart;
        }

        private static IChart MilestoneChart() {
            var chart = (GanttChart)ChartHasDataTable();

            // var task = chart.Tasks.TaskElements[4];
            var task = chart.Tasks[4];

            var milestone = new MilestoneElement
                            {
                                TaskId = task.Id,
                                Date = task.End.Value,
                                ToolText = "Development Complete",
                                Color = Color.Red
                            };
            // chart.Milestones.MilestoneElements.Add(milestone);
            chart.Milestones.Add(milestone);

            // task = chart.Tasks.TaskElements[6];
            task = chart.Tasks[6];
            milestone = new MilestoneElement
                        {
                            TaskId = task.Id,
                            Date = task.End.Value,
                            ToolText = "Successful Global Release",
                            Color = Color.DarkBlue
                        };
            // chart.Milestones.MilestoneElements.Add(milestone);
            chart.Milestones.Add(milestone);

            return chart;
        }

        private static IChart ConnectorChart() {
            //
            // 		chart.Connectors.AddConnector() 를 사용하세요
            //

            var chart = (GanttChart)MilestoneChart();

            var connector = new ConnectorElement
                            {
                                // FromTaskId = chart.Tasks.TaskElements[1].Id,
                                // ToTaskId = chart.Tasks.TaskElements[3].Id,
                                FromTaskId = chart.Tasks[1].Id,
                                ToTaskId = chart.Tasks[3].Id,
                                FromTaskConnectStart = false,
                                ToTaskConnectStart = true,
                                LineAttr =
                                {
                                    Thickness = 2,
                                    IsDashed = true,
                                    Color = Color.Blue
                                },
                            };

            // chart.Connectors.ConnectorElements.Add(connector);
            chart.Connectors.Add(connector);

            connector = new ConnectorElement
                        {
                            //FromTaskId = chart.Tasks.TaskElements[4].Id,
                            //ToTaskId = chart.Tasks.TaskElements[6].Id,
                            FromTaskId = chart.Tasks[4].Id,
                            ToTaskId = chart.Tasks[6].Id,
                            ToTaskConnectStart = true,
                            LineAttr =
                            {
                                Thickness = 2,
                                IsDashed = true,
                                Color = Color.Red
                            },
                        };

            // chart.Connectors.ConnectorElements.Add(connector);
            chart.Connectors.Add(connector);

            connector = new ConnectorElement
                        {
                            //FromTaskId = chart.Tasks.TaskElements[6].Id,
                            //ToTaskId = chart.Tasks.TaskElements[7].Id,
                            FromTaskId = chart.Tasks[6].Id,
                            ToTaskId = chart.Tasks[7].Id,
                            ToTaskConnectStart = true,
                            LineAttr =
                            {
                                Thickness = 2,
                                IsDashed = false,
                                Color = Color.DarkTurquoise
                            },
                        };

            // chart.Connectors.ConnectorElements.Add(connector);
            chart.Connectors.Add(connector);

            var trendline = new DateTimeLineElement
                            {
                                // Start = chart.Tasks.TaskElements[3].Start.Value,
                                Start = chart.Tasks[3].Start.Value,
                                DisplayValue = "Today",
                                Color = "#33333".FromHtml(),
                                Thickness = 2,
                                IsDashed = false
                            };

            // chart.Trendlines.TrendlineElements.Add(trendline);
            chart.Trendlines.Add(trendline);

            // TrendZone (범위)
            trendline = new DateTimeLineElement
                        {
                            //Start = chart.Tasks.TaskElements[5].Start.Value.AddDays(10),
                            //End = chart.Tasks.TaskElements[5].Start.Value.AddDays(15),
                            Start = chart.Tasks[5].Start.Value.AddDays(10),
                            End = chart.Tasks[5].Start.Value.AddDays(15),
                            IsTrendZone = true,
                            //  범위를 나타냄
                            DisplayValue = "Vacation",
                            Color = "#FF5904".FromHtml(),
                            Alpha = 20,
                            Thickness = 2,
                            IsDashed = false
                        };

            // chart.Trendlines.TrendlineElements.Add(trendline);
            chart.Trendlines.Add(trendline);

            return chart;
        }

        #endregion
    }
}