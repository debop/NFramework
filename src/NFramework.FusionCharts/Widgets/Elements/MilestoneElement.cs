using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Milestone (중요지점)을 나타내는 정보 (완료일, 중간발표일)
    /// </summary>
    [Serializable]
    public class MilestoneElement : ChartElementBase {
        private const string DefaultMilestoneShape = @"star";

        /// <summary>
        /// 생성자
        /// </summary>
        public MilestoneElement()
            : base("milestone") {
            Shape = DefaultMilestoneShape;
        }

        public MilestoneElement(DateTime date)
            : this() {
            Date = date;
        }

        /// <summary>
        /// Milestone
        /// </summary>
        public DateTime? Date { get; set; }

        /// <summary>
        /// 관련 <see cref="TaskElement"/>의 Id
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// 모양 ( star | polygon )
        /// </summary>
        public string Shape { get; set; }

        /// <summary>
        /// Number of sides that the milestone should have. ( value = 3 ~15 )
        /// </summary>
        public int? NumSides { get; set; }

        /// <summary>
        /// Starting angle for the shape (star | polygon) (value = 0 ~ 360)
        /// </summary>
        public int? StartAngle { get; set; }

        /// <summary>
        /// Milestone의 반지름(크기 in Pixels) (value = 2 ~ 15)
        /// </summary>
        public int? Radius { get; set; }

        public Color? Color { get; set; }

        /// <summary>
        /// Alpha (value= 0 ~ 100)
        /// </summary>
        public int? Alpha { get; set; }

        public Color? BorderColor { get; set; }
        public int? BorderThickness { get; set; }

        private FusionLink _link;

        public virtual FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        public virtual string ToolText { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Date.HasValue)
                writer.WriteAttributeString("Date", Date.Value.ToSortableString(true));
            if(TaskId.IsNotWhiteSpace())
                writer.WriteAttributeString("TaskId", TaskId);
            if(Shape.IsNotWhiteSpace())
                writer.WriteAttributeString("Shape", Shape);
            if(NumSides.HasValue)
                writer.WriteAttributeString("NumSides", NumSides.Value.ToString());
            if(StartAngle.HasValue)
                writer.WriteAttributeString("StartAngle", StartAngle.Value.ToString());
            if(Radius.HasValue)
                writer.WriteAttributeString("Radius", Radius.Value.ToString());

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.Value.ToString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString("BorderColor", BorderColor.Value.ToHexString());
            if(BorderThickness.HasValue)
                writer.WriteAttributeString("BorderThickness", BorderThickness.Value.ToString());

            if(_link != null)
                _link.GenerateXmlAttributes(writer);

            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("ToolText", ToolText);
        }
    }
}