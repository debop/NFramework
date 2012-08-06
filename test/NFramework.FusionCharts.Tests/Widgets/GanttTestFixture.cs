using System;
using System.Drawing;
using System.Globalization;
using NSoft.NFramework.TimePeriods;
using NSoft.NFramework.Tools;
using NUnit.Framework;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [TestFixture]
    public class GanttTestFixture : ChartTestFixtureBase {
        public static readonly DateTime StartDate = new DateTime(2007, 2, 1);
        public static readonly ITimePeriod ProjectPeriod = StartDate.GetRelativeMonthPeriod(8);

        public static readonly string[] ProcessLabels = {
                                                            "Identify Customers",
                                                            "Survey 50 Customers",
                                                            "Interpret Requirements",
                                                            "Study Competition",
                                                            "Documentation of features",
                                                            "Brainstorm concepts",
                                                            "Design & Code",
                                                            "테스트 / QA",
                                                            "Documentation of product",
                                                            "배포"
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
                                                        "김갑동",
                                                        "Sharon",
                                                        "Neil",
                                                        "Harry",
                                                        "Chris",
                                                        "Rechard"
                                                    };

        [Test]
        public void XmlEncodingTest() {
            var chart = new GanttChart
                        {
                            Caption = "Gantt Chart 'Sample' 입니다.",
                            SubCaption = "<< Sub \"LegendCaption\" & 특수문자 입니다 >>",
                            DateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
                        };

            var categories = new CategoriesElement
                             {
                                 new CategoryElement
                                 {
                                     Start = DateTime.Today.AddYears(-1),
                                     End = DateTime.Today.AddYears(1),
                                 }
                             };
            categories.BgColor = Color.WhiteSmoke;

            chart.CategoriesList.Add(categories);
            chart.ClickURL.Url = "http://localhost/Gantt.aspx";
            chart.DefaultAnimation = false;

            Console.WriteLine("Gantt Chart with categories");

            ValidateChartXml(chart);
        }

        [Test]
        public void FirstGanttChart() {
            var chart = CreateSampleChart("First Gantt Chart");

            ValidateChartXml(chart);
        }

        [Test]
        public void MultipleCategories() {
            var chart = CreateSampleChart("Multiple Categories Gantt Chart");

            var quaterCategories = new CategoriesElement();

            // 분기별 Category를 만든다.
            foreach(var quarterRange in ProjectPeriod.ForEachQuarters()) {
                var category = new CategoryElement
                               {
                                   Start = quarterRange.Start,
                                   End = quarterRange.End,
                                   ItemAttr = { Label = quarterRange.Start.GetQuarter().ToString() }
                               };

                quaterCategories.Add(category);
            }

            chart.CategoriesList.Insert(0, quaterCategories);

            ValidateChartXml(chart);
        }

        [Test]
        public void ChartWithScrolling() {
            var chart = CreateSampleChart("Scrolling");
            chart.Caption = "Gantt chart with Scrolling";

            // 3 개월 단위로 Scrolling을 수행한다.
            chart.GanttPaneDuration = 6;
            chart.GanttPaneDurationUnit = "m";

            //  Scrollable Chart
            chart.ScrollBarAttr.ScrollColor = Color.LimeGreen;
            chart.ScrollBarAttr.ScrollPadding = 3;
            chart.ScrollBarAttr.ScrollHeight = 20;
            chart.ScrollBarAttr.ScrollBtnWidth = 28;
            chart.ScrollBarAttr.ScrollBtnPadding = 3;

            ValidateChartXml(chart);
        }

        [Test]
        public void ChartHasDataTable() {
            var chart = CreateSampleChart("Chart with DataTable");

            chart.DataTable.HeaderAttr.VAlign = FusionVerticalAlign.Bottom;

            var dataColumn = new DataColumnElement
                             {
                                 HeaderText = "작업자"
                             };
            dataColumn.HeaderAttr.FontAttr.FontSize = 18.ToString();
            dataColumn.HeaderAttr.VAlign = FusionVerticalAlign.Bottom;
            dataColumn.HeaderAttr.Align = FusionTextAlign.Right;
            dataColumn.ItemAttr.Align = FusionTextAlign.Left;
            dataColumn.ItemAttr.FontAttr.FontSize = 12.ToString();

            foreach(var worker in RoleNames) {
                var text = new DataColumnTextElement { Label = worker };
                text.Link.Url = "mailto:rcl@realweb21.com";
                // dataColumn.TextElements.Add(text);
                dataColumn.Add(text);
            }

            // chart.DataTable.DataColumnElements.Add(dataColumn);
            chart.DataTable.Add(dataColumn);


            ValidateChartXml(chart);
        }

        [Test]
        public void MultiTaskedProcess() {
            var chart = CreateSampleChart("MultiTaskedProcess");

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
                                   Id = "Planed Task" + i.ToString(),
                                   ProcessId = "Process" + i.ToString(),
                                   Label = "Planed",
                                   Color = Color.BlueViolet,
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
                                     Id = "Actual Task" + i.ToString(),
                                     ProcessId = "Process" + i.ToString(),
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

            ValidateChartXml(chart);
        }

        private static GanttChart CreateSampleChart(string caption) {
            var chart = new GanttChart
                        {
                            Caption = caption,
                            SubCaption = "소제목입니다.",
                            Palette = 2,
                        };

            var categories = new CategoriesElement
                             {
                                 FontAttr =
                                 {
                                     Font = "Tahoma",
                                     FontColor = Color.FromArgb(0, 0x03, 0x72, 0xAB),
                                     FontSize = 13.ToString(),
                                     IsBold = true,
                                     IsItalic = true,
                                     IsUnderline = true
                                 },
                                 BgColor = Color.White,
                                 Align = FusionTextAlign.Center,
                                 VAlign = FusionVerticalAlign.Middle
                             };


            foreach(var month in ProjectPeriod.ForEachMonths()) {
                var category = new CategoryElement
                               {
                                   Start = month.Start,
                                   End = month.End,
                               };
                category.ItemAttr.Label = month.Start.Month.ToString();
                category.ItemAttr.FontAttr.Font = "Tahoma";
                category.ItemAttr.FontAttr.FontColor = Color.FromArgb(0, 0x03, 0x72, 0xAB);
                category.ItemAttr.FontAttr.FontSize = 13.ToString();
                category.ItemAttr.FontAttr.IsBold = true;
                category.ItemAttr.FontAttr.IsItalic = true;
                category.ItemAttr.FontAttr.IsUnderline = true;
                category.ItemAttr.BgColor = Color.White;
                category.ItemAttr.Align = FusionTextAlign.Center;
                category.ItemAttr.VAlign = FusionVerticalAlign.Middle;

                categories.Add(category);
            }

            chart.CategoriesList.Add(categories);

            chart.Processes.FontAttr.FontSize = 12.ToString();
            chart.Processes.FontAttr.IsBold = true;
            chart.Processes.Align = FusionTextAlign.Right;
            chart.Processes.HeaderText = "What to do?";
            chart.Processes.HeaderAttr.FontAttr.FontSize = 18.ToString();
            chart.Processes.HeaderAttr.Align = FusionTextAlign.Right;
            chart.Processes.HeaderAttr.VAlign = FusionVerticalAlign.Bottom;

            var procId = 0;
            foreach(string label in ProcessLabels) {
                var process = new ProcessElement("Process" + procId++.ToString())
                              {
                                  ItemAttr =
                                  {
                                      Label = label,
                                      FontAttr =
                                      {
                                          Font = "Tahoma",
                                          FontColor = Color.FromArgb(0, 0x03, 0x72, 0xAB),
                                          FontSize = 13.ToString(),
                                          IsBold = true,
                                          IsItalic = true,
                                          IsUnderline = true
                                      },
                                      BgColor = Color.White,
                                      Align = FusionTextAlign.Center,
                                      VAlign = FusionVerticalAlign.Middle
                                  }
                              };

                chart.Processes.Add(process);
            }

            procId = 0;
            var taskId = 0;
            foreach(var taskPeriod in TaskPeriods) {
                var task = new TaskElement
                           {
                               Start = taskPeriod.Start,
                               End = taskPeriod.End,
                               Id = "Task" + taskId++,
                               ProcessId = "Process" + procId++,
                               Color = Color.BlueViolet,
                               ShowLabel = true,
                               Animation = true,
                               BorderThickness = 1
                           };

                // chart.Tasks.TaskElements.Add(task);
                chart.Tasks.Add(task);
            }

            return chart;
        }
    }
}