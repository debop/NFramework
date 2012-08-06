using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Pointer 요소
    /// </summary>
    [Serializable]
    public class PointerElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public PointerElement() : base("pointer") {}

        public string Id { get; set; }
        public double? Value { get; set; }
        public bool? ShowValue { get; set; }
        public bool? EditMode { get; set; }

        public Color? Color { get; set; }
        public int? Alpha { get; set; }

        private BorderAttribute _border;

        public BorderAttribute Border {
            get { return _border ?? (_border = new BorderAttribute()); }
            set { _border = value; }
        }

        public int? Radius { get; set; }
        public int? Sides { get; set; }

        private FusionLink _link;

        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        public string ToolText { get; set; }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("Id", Id);
            if(Value.HasValue)
                writer.WriteAttributeString("Value", Value.ToString());
            if(ShowValue.HasValue)
                writer.WriteAttributeString("ShowValue", ShowValue.GetHashCode().ToString());
            if(EditMode.HasValue)
                writer.WriteAttributeString("EditMode", EditMode.GetHashCode().ToString());
            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.ToString());

            if(_border != null)
                _border.GenerateXmlAttributes(writer);

            if(Radius.HasValue)
                writer.WriteAttributeString("Radius", Radius.ToString());
            if(Sides.HasValue)
                writer.WriteAttributeString("Sides", Sides.ToString());

            if(_link != null)
                _link.GenerateXmlAttributes(writer);

            if(ToolText.IsNotWhiteSpace())
                writer.WriteAttributeString("ToolText", ToolText);
        }
    }
}