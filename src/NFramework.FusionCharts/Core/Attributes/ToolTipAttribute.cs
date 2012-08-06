using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart의 ToolTip 관련 Attribute
    /// </summary>
    [Serializable]
    public class ToolTipAttribute : ChartAttributeBase {
        private const string ToolTipPrexfix = @"ToolTip";

        public bool? Show { get; set; }
        public Color? BgColor { get; set; }
        public Color? BorderColor { get; set; }
        public string SepChar { get; set; }
        public bool? DateIn { get; set; }
        public bool? ShowShadow { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Show.HasValue)
                writer.WriteAttributeString("show" + ToolTipPrexfix, Show.Value.GetHashCode().ToString());
            if(BgColor.HasValue)
                writer.WriteAttributeString(ToolTipPrexfix + "BgColor", BgColor.Value.ToHexString());
            if(BorderColor.HasValue)
                writer.WriteAttributeString(ToolTipPrexfix + "BorderColor", BorderColor.Value.ToHexString());

            if(SepChar.IsNotWhiteSpace())
                writer.WriteAttributeString(ToolTipPrexfix + "SepChar", SepChar);

            if(DateIn.HasValue)
                writer.WriteAttributeString("dateIn" + ToolTipPrexfix, DateIn.Value.GetHashCode().ToString());
            if(ShowShadow.HasValue)
                writer.WriteAttributeString("show" + ToolTipPrexfix + "Shadow", ShowShadow.Value.GetHashCode().ToString());
        }
    }
}