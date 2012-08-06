using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt Chart의 맵상의 Element를 나타낸다.
    /// </summary>
    [Obsolete("TasksElement를 사용하세요")]
    [Serializable]
    public class TaskCollection : ChartElementBase {
        public TaskCollection()
            : base("tasks") {}

        public virtual bool? ShowLabels { get; set; }
        public virtual bool? ShowPercentLabel { get; set; }
        public virtual bool? ShowStartDate { get; set; }
        public virtual bool? ShowEndDate { get; set; }

        public virtual Color? Color { get; set; }
        public virtual int? Alpha { get; set; }

        private WidgetFontAttribute _fontAttr;

        public virtual WidgetFontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new WidgetFontAttribute()); }
        }

        private BorderAttribute _borderAttr;

        public virtual BorderAttribute BorderAttr {
            get { return _borderAttr ?? (_borderAttr = new BorderAttribute()); }
        }

        private IList<TaskElement> _taskElements;

        public virtual IList<TaskElement> TaskElements {
            get { return _taskElements ?? (_taskElements = new List<TaskElement>()); }
        }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowLabels.HasValue)
                writer.WriteAttributeString("ShowLabels", ShowLabels.Value.GetHashCode().ToString());
            if(ShowPercentLabel.HasValue)
                writer.WriteAttributeString("ShowPercentLabel", ShowPercentLabel.Value.GetHashCode().ToString());
            if(ShowStartDate.HasValue)
                writer.WriteAttributeString("ShowStartDate", ShowStartDate.Value.GetHashCode().ToString());
            if(ShowEndDate.HasValue)
                writer.WriteAttributeString("ShowEndDate", ShowEndDate.Value.GetHashCode().ToString());

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.Value.ToString());

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
            if(_borderAttr != null)
                _borderAttr.GenerateXmlAttributes(writer);
        }

        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_taskElements != null)
                foreach(var task in _taskElements)
                    task.WriteXmlElement(writer);
        }
    }
}