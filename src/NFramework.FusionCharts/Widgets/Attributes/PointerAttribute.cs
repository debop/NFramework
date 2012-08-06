using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// <see cref="LinearGauge"/>에서 사용하는 Point 속성들
    /// </summary>
    public class PointerAttribute : ChartAttributeBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public PointerAttribute() : this("pointer") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="attrName"></param>
        public PointerAttribute(string attrName) {
            AttrName = (attrName ?? string.Empty).Trim();
        }

        public string AttrName { get; private set; }

        public int? Radius { get; set; }
        public Color? BgColor { get; set; }
        public int? BgAlpha { get; set; }
        public int? Sides { get; set; }

        public Color? BorderColor {
            get { return BorderAttr.Color; }
            set { BorderAttr.Color = value; }
        }

        public int? BorderAlpha {
            get { return BorderAttr.Alpha; }
            set { BorderAttr.Alpha = value; }
        }

        public int? BorderThickness {
            get { return BorderAttr.Thickness; }
            set { BorderAttr.Thickness = value; }
        }

        private BorderAttribute _borderAttr;

        protected BorderAttribute BorderAttr {
            get { return _borderAttr ?? (_borderAttr = new BorderAttribute(AttrName)); }
            set { _borderAttr = value; }
        }

        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Radius.HasValue)
                writer.WriteAttributeString(AttrName + "Radius", Radius.ToString());
            if(BgColor.HasValue)
                writer.WriteAttributeString(AttrName + "BgColor", BgColor.GetHashCode().ToString());
            if(BgAlpha.HasValue)
                writer.WriteAttributeString(AttrName + "BgAlpha", BgAlpha.ToString());
            if(Sides.HasValue)
                writer.WriteAttributeString(AttrName + "Sides", Sides.ToString());

            if(_borderAttr != null)
                _borderAttr.GenerateXmlAttributes(writer);
        }
    }
}