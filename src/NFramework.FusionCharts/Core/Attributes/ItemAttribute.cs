using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Fusion Chart에서 정의하는 기본 Element
    /// </summary>
    [Serializable]
    public class ItemAttribute : ChartAttributeBase {
        /// <summary>
        /// Default constructor
        /// </summary>
        public ItemAttribute() : this(string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix"></param>
        public ItemAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        protected string Prefix { get; private set; }

        public Color? BgColor { get; set; }
        public int? BgAlpha { get; set; }
        public string Label { get; set; }
        public FusionTextAlign? Align { get; set; }
        public FusionVerticalAlign? VAlign { get; set; }

        private FontAttribute _fontAttr;

        public virtual FontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new FontAttribute(Prefix)); }
            set { _fontAttr = value; }
        }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(BgColor.HasValue)
                writer.WriteAttributeString(Prefix + "BgColor", BgColor.Value.ToHexString());
            if(BgAlpha.HasValue)
                writer.WriteAttributeString(Prefix + "BgAlpha", BgAlpha.Value.ToString());
            if(Label.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "Label", Label);
            if(Align.HasValue)
                writer.WriteAttributeString(Prefix + "Align", Align.Value.ToString().ToLower());
            if(VAlign.HasValue)
                writer.WriteAttributeString(Prefix + "VAlign", VAlign.Value.ToString().ToLower());

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
        }
    }
}