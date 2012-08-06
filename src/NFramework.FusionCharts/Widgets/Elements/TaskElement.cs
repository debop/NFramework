using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// GANTT 에서 하나의 TASK를 나타낸다.
    /// </summary>
    [Serializable]
    public class TaskElement : ChartElementBase {
        public TaskElement() : base("task") {}

        public TaskElement(DateTime start, DateTime end)
            : this() {
            Start = start;
            End = end;
        }

        public TaskElement(string id)
            : this() {
            Id = id;
        }

        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public string Id { get; set; }
        public string ProcessId { get; set; }
        public string Label { get; set; }

        private FusionLink _link;

        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        public int? PercentComplete { get; set; }
        public bool? ShowAsGroup { get; set; }
        public bool? Animation { get; set; }

        private WidgetFontAttribute _fontAttr;

        public WidgetFontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new WidgetFontAttribute()); }
            set { _fontAttr = value; }
        }

        public Color? Color { get; set; }
        public int? Alpha { get; set; }

        public bool? ShowBorder { get; set; }
        public Color? BorderColor { get; set; }
        public int? BorderThickness { get; set; }
        public int? BorderAlpha { get; set; }

        public string Height { get; set; }
        public string TopPadding { get; set; }

        public bool? ShowLabel { get; set; }
        public bool? ShowPercentLabel { get; set; }
        public bool? ShowStartDate { get; set; }
        public bool? ShowEndDate { get; set; }

        public string ToolText { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Start.HasValue)
                writer.WriteAttributeString("Start", Start.Value.ToSortableString(true));
            if(End.HasValue)
                writer.WriteAttributeString("End", End.Value.ToSortableString(true));

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("Id", Id);
            if(ProcessId.IsNotWhiteSpace())
                writer.WriteAttributeString("ProcessId", ProcessId);
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString("Label", Label);

            if(_link != null)
                _link.GenerateXmlAttributes(writer);

            if(PercentComplete.HasValue)
                writer.WriteAttributeString("PercentComplete", PercentComplete.Value.ToString());
            if(ShowAsGroup.HasValue)
                writer.WriteAttributeString("ShowAsGroup", ShowAsGroup.Value.GetHashCode().ToString());
            if(Animation.HasValue)
                writer.WriteAttributeString("Animation", Animation.Value.GetHashCode().ToString());

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.Value.ToString());

            if(ShowBorder.HasValue)
                writer.WriteAttributeString("ShowBorder", ShowBorder.Value.GetHashCode().ToString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("BorderColor", BorderColor.Value.ToHexString());
            if(BorderThickness.HasValue)
                writer.WriteAttributeString("BorderThickness", BorderThickness.Value.ToString());
            if(BorderAlpha.HasValue)
                writer.WriteAttributeString("BorderAlpha", BorderAlpha.Value.ToString());

            if(Height.IsNotWhiteSpace())
                writer.WriteAttributeString("Height", Height);
            if(TopPadding.IsNotWhiteSpace())
                writer.WriteAttributeString("TopPadding", TopPadding);

            if(ShowLabel.HasValue)
                writer.WriteAttributeString("ShowLabel", ShowLabel.Value.GetHashCode().ToString());
            if(ShowPercentLabel.HasValue)
                writer.WriteAttributeString("ShowPercentLabel", ShowPercentLabel.Value.GetHashCode().ToString());
            if(ShowStartDate.HasValue)
                writer.WriteAttributeString("ShowStartDate", ShowStartDate.Value.GetHashCode().ToString());
            if(ShowEndDate.HasValue)
                writer.WriteAttributeString("ShowEndDate", ShowEndDate.Value.GetHashCode().ToString());

            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("ToolText", ToolText);
        }
    }
}