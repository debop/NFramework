using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt 차트
    /// </summary>
    [Serializable]
    public class GanttChart : WidgetBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public GanttChart() {
            DateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
        }

        /// <summary>
        /// Gantt 날짜 형식 (yyyy-mm-dd or mm/dd/yyyy)
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Gannt 화면에 보여줄 날짜의 Format (CurrentCulture.DateFormat 을 사용한다)
        /// </summary>
        public string OutputDateFormat { get; set; }

        public int? GanttPaneDuration { get; set; }

        /// <summary>
        /// Gant Pane을 scroll할 때의 단위가 된다 (예: y, m, d, h, mn, or s)
        /// </summary>
        public string GanttPaneDurationUnit { get; set; }

        /// <summary>
        /// 모든 Data를 표시할 것인가?
        /// </summary>
        public bool? ShowFullDataTable { get; set; }

        private int? _ganttWidthPercent;

        /// <summary>
        /// Gantt View pane의 폭의 비율 (10 ~100)
        /// </summary>
        public int? GanttWidthPercent {
            get { return _ganttWidthPercent; }
            set { _ganttWidthPercent = Math.Min(100, Math.Max(10, value ?? 100)); }
        }

        /// <summary>
        /// <see cref="GanttWidthPercent"/> 값을 강제로 적용할 것인가?
        /// </summary>
        public bool? ForceGanttWitdthPercent { get; set; }

        /// <summary>
        /// Task의 시작일을 표시할 것인가?
        /// </summary>
        public bool? ShowTaskStartDate { get; set; }

        /// <summary>
        /// Task의 종료일을 표시할 것인가?
        /// </summary>
        public bool? ShowTaskEndDate { get; set; }

        /// <summary>
        /// Task의 Label을 표시할 것인가?
        /// </summary>
        public bool? ShowTaskLabels { get; set; }

        /// <summary>
        /// Percentage Task를 사용하였다면 (진행률 표현), 그 Percentage 값을 표시할 것인가?
        /// </summary>
        public bool? ShowPercentLabel { get; set; }

        /// <summary>
        /// 진행률 정보를 표시하는 Percentage Task에서, 아직 진행하지 않은 빈 영역을 채울 것인가?
        /// </summary>
        public bool? ShowSlackAsFill { get; set; }

        /// <summary>
        /// Percentage Task의 빈 영역을 채울 때, 배경색
        /// </summary>
        public Color? SlackFillColor { get; set; }

        /// <summary>
        /// Gantt 종료일 이후의 빈 공간을 마지막 Category의 속성을 사용하여 채운다.
        /// </summary>
        public bool? ExtendCategoryBg { get; set; }

        public virtual Color? GanttLineColor { get; set; }
        public virtual int? GanttLineAlpha { get; set; }
        public virtual Color? GridBorderColor { get; set; }
        public virtual int? GridBorderAlpha { get; set; }
        public virtual Color? GridResizeBarColor { get; set; }
        public virtual int? GridResizeBarAlpha { get; set; }

        public virtual int? TaskBarRoundRadius { get; set; }

        /// <summary>
        /// String gradient formula
        /// </summary>
        public virtual string TaskBarFillMix { get; set; }

        /// <summary>
        /// String ratios separated by comma
        /// </summary>
        public virtual string TaskBarFillRatio { get; set; }

        public virtual int? TaskDatePadding { get; set; }

        private IList<CategoriesElement> _categoriesList;

        public virtual IList<CategoriesElement> CategoriesList {
            get { return _categoriesList ?? (_categoriesList = new List<CategoriesElement>()); }
            set { _categoriesList = value; }
        }

        private ProcessesElement _processes;

        public virtual ProcessesElement Processes {
            get { return _processes ?? (_processes = new ProcessesElement()); }
            set { _processes = value; }
        }

        private DataTableElement _dataTable;

        public virtual DataTableElement DataTable {
            get { return _dataTable ?? (_dataTable = new DataTableElement()); }
            set { _dataTable = value; }
        }

        private TasksElement _tasks;

        public virtual TasksElement Tasks {
            get { return _tasks ?? (_tasks = new TasksElement()); }
            set { _tasks = value; }
        }

        private CollectionElement<MilestoneElement> _milestones;

        public virtual CollectionElement<MilestoneElement> Milestones {
            get { return _milestones ?? (_milestones = new CollectionElement<MilestoneElement>("milestones")); }
            set { _milestones = value; }
        }

        private ConnectorsElement _connectors;

        public virtual ConnectorsElement Connectors {
            get { return _connectors ?? (_connectors = new ConnectorsElement()); }
            set { _connectors = value; }
        }

        private CollectionElement<DateTimeLineElement> _trendlines;

        public virtual CollectionElement<DateTimeLineElement> Trendlines {
            get { return _trendlines ?? (_trendlines = new CollectionElement<DateTimeLineElement>("trendlines")); }
            set { _trendlines = value; }
        }

        private CollectionElement<LegendItemElement> _legend;

        public virtual CollectionElement<LegendItemElement> Legend {
            get { return _legend ?? (_legend = new CollectionElement<LegendItemElement>("legend")); }
            set { _legend = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            writer.ShouldNotBeNull("writer");

            // Chart의 기본 Attribute 를 쓴다.
            base.GenerateXmlAttributes(writer);

            if(DateFormat.IsNotWhiteSpace())
                writer.WriteAttributeString("DateFormat", DateFormat);
            if(OutputDateFormat.IsNotWhiteSpace())
                writer.WriteAttributeString("OutputDateFormat", OutputDateFormat);
            if(GanttPaneDuration.HasValue)
                writer.WriteAttributeString("ganttPaneDuration", GanttPaneDuration.Value.ToString());
            if(GanttPaneDurationUnit.IsNotWhiteSpace())
                writer.WriteAttributeString("ganttPaneDurationUnit", GanttPaneDurationUnit);
            if(GanttWidthPercent.HasValue)
                writer.WriteAttributeString("GanttWidthPercent", GanttWidthPercent.Value.ToString());
            if(ForceGanttWitdthPercent.HasValue)
                writer.WriteAttributeString("ForceGanttWitdthPercent", ForceGanttWitdthPercent.Value.GetHashCode().ToString());

            if(ShowTaskStartDate.HasValue)
                writer.WriteAttributeString("ShowTaskStartDate", ShowTaskStartDate.Value.GetHashCode().ToString());
            if(ShowTaskEndDate.HasValue)
                writer.WriteAttributeString("ShowTaskEndDate", ShowTaskEndDate.Value.GetHashCode().ToString());
            if(ShowTaskLabels.HasValue)
                writer.WriteAttributeString("ShowTaskLabels", ShowTaskLabels.Value.GetHashCode().ToString());
            if(ShowPercentLabel.HasValue)
                writer.WriteAttributeString("ShowPercentLabel", ShowPercentLabel.Value.GetHashCode().ToString());
            if(ShowSlackAsFill.HasValue)
                writer.WriteAttributeString("ShowSlackAsFill", ShowSlackAsFill.Value.GetHashCode().ToString());

            if(SlackFillColor.HasValue)
                writer.WriteAttributeString("SlackFillColor", SlackFillColor.Value.ToHexString());

            if(ExtendCategoryBg.HasValue)
                writer.WriteAttributeString("ExtendCategoryBg", ExtendCategoryBg.Value.GetHashCode().ToString());

            //if (_cosmeticAttr != null)
            //    _cosmeticAttr.GenerateXmlAttributes(writer);

            if(GanttLineColor.HasValue)
                writer.WriteAttributeString("GanttLineColor", GanttLineColor.Value.ToHexString());
            if(GanttLineAlpha.HasValue)
                writer.WriteAttributeString("GanttLineAlpha", GanttLineAlpha.Value.ToString());
            if(GridBorderColor.HasValue)
                writer.WriteAttributeString("GridBorderColor", GridBorderColor.Value.ToHexString());
            if(GridBorderAlpha.HasValue)
                writer.WriteAttributeString("GridBorderAlpha", GridBorderAlpha.Value.ToString());
            if(GridResizeBarColor.HasValue)
                writer.WriteAttributeString("GridResizeBarColor", GridResizeBarColor.Value.ToHexString());
            if(GridResizeBarAlpha.HasValue)
                writer.WriteAttributeString("GridResizeBarAlpha", GridResizeBarAlpha.Value.ToString());

            if(TaskBarRoundRadius.HasValue)
                writer.WriteAttributeString("TaskBarRoundRadius", TaskBarRoundRadius.Value.ToString());
            if(TaskBarFillMix.IsNotWhiteSpace())
                writer.WriteAttributeString("TaskBarFillMix", TaskBarFillMix);
            if(TaskBarFillRatio.IsNotWhiteSpace())
                writer.WriteAttributeString("TaskBarFillRatio", TaskBarFillRatio);

            if(TaskDatePadding.HasValue)
                writer.WriteAttributeString("TaskDatePadding", TaskDatePadding.Value.ToString());
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            //if (_categoryCollections != null)
            //    foreach (var categoryCollection in _categoryCollections)
            //        categoryCollection.WriteXmlElement(writer);
            if(_categoriesList != null)
                foreach(var categories in _categoriesList)
                    categories.WriteXmlElement(writer);

            //if (_processes != null)
            //{
            //    _processes.WriteXmlElement(writer);

            //    // NOTE : Processes 아래에 Task를 넣을 수 있다. 이렇게 하면, Task의 ProcessID 속성이 자연스럽게 매핑되므로, 조작하기 더 쉽다.
            //    foreach (var process in _processes.ProcessElements)
            //        foreach (var task in process.Tasks.TaskElements)
            //            Tasks.TaskElements.Add(task);
            //}
            if(_processes != null) {
                _processes.WriteXmlElement(writer);
                foreach(var proc in _processes)
                    foreach(var task in proc.Tasks)
                        Tasks.Add(task);
            }

            if(_dataTable != null)
                _dataTable.WriteXmlElement(writer);

            if(_tasks != null)
                _tasks.WriteXmlElement(writer);

            if(_milestones != null)
                _milestones.WriteXmlElement(writer);
            if(_connectors != null)
                _connectors.WriteXmlElement(writer);
            if(_trendlines != null)
                _trendlines.WriteXmlElement(writer);
            if(_legend != null)
                _legend.WriteXmlElement(writer);
        }
    }
}