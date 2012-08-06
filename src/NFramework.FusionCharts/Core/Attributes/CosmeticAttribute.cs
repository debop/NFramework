using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Cosmetic Attributes for Chart
    /// </summary>
    [Serializable]
    public class CosmeticAttribute : ChartAttributeBase {
        /// <summary>
        /// 배경으로 쓰일 SWF 의 전체 경로
        /// </summary>
        public string BgSWF { get; set; }

        /// <summary>
        /// 배경으로 쓰일 SWF의 투명도
        /// </summary>
        public int? BgSWFAlpha { get; set; }

        private BackgroundAttribute _canvasBackgroundAttr;

        public BackgroundAttribute CanvasBackgroundAttr {
            get { return _canvasBackgroundAttr ?? (_canvasBackgroundAttr = new BackgroundAttribute("canvas")); }
            set { _canvasBackgroundAttr = value; }
        }

        private BorderAttribute _canvasBorderAttr;

        public BorderAttribute CanvasBorderAttr {
            get { return _canvasBorderAttr ?? (_canvasBorderAttr = new BorderAttribute("canvas")); }
            set { _canvasBorderAttr = value; }
        }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(_canvasBackgroundAttr != null)
                _canvasBorderAttr.GenerateXmlAttributes(writer);

            if(BgSWF.IsNotWhiteSpace())
                writer.WriteAttributeString("BgSWF", BgSWF);

            if(BgSWFAlpha.HasValue)
                writer.WriteAttributeString("BgSWFAlpha", BgSWFAlpha.Value.ToString());

            if(_canvasBackgroundAttr != null)
                _canvasBackgroundAttr.GenerateXmlAttributes(writer);

            if(_canvasBorderAttr != null)
                _canvasBorderAttr.GenerateXmlAttributes(writer);
        }
    }
}