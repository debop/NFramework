using System.Drawing;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Scroll 관련 속성 정보
    /// </summary>
    public class ScrollBarAttribute : ChartAttributeBase {
        /// <summary>
        ///	 Color for scroll bar.
        /// </summary>
        public Color? ScrollColor { get; set; }

        /// <summary>
        /// In Pixels. 
        /// Distance between the end of canvas (bottom part) and start of scroll bar.
        /// </summary>
        public int? ScrollPadding { get; set; }

        /// <summary>
        /// In Pixels. 
        /// Required height for scroll bar.
        /// </summary>
        public int? ScrollHeight { get; set; }

        /// <summary>
        /// In Pixels. 
        /// Width of both scroll bar buttons (left & right).
        /// </summary>
        public int? ScrollBtnWidth { get; set; }

        /// <summary>
        /// In Pixels. 
        /// Padding between the scroll buttons and the scroll track (background).
        /// </summary>
        public int? ScrollBtnPadding { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ScrollColor.HasValue)
                writer.WriteAttributeString("scrollColor", ScrollColor.Value.ToHexString());
            if(ScrollPadding.HasValue)
                writer.WriteAttributeString("scrollPadding", ScrollPadding.Value.ToString());
            if(ScrollHeight.HasValue)
                writer.WriteAttributeString("scrollHeight", ScrollHeight.Value.ToString());
            if(ScrollBtnWidth.HasValue)
                writer.WriteAttributeString("scrollBtnWidth", ScrollBtnWidth.Value.ToString());
            if(ScrollBtnPadding.HasValue)
                writer.WriteAttributeString("scrollBtnPadding", ScrollBtnPadding.Value.ToString());
        }
    }
}