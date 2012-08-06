using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Background Attribute
    /// </summary>
    [Serializable]
    public class BackgroundAttribute : ChartAttributeBase {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public BackgroundAttribute() : this(string.Empty) {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix"></param>
        public BackgroundAttribute(string prefix) {
            Prefix = (prefix ?? string.Empty).Trim();
        }

        protected string Prefix { get; private set; }

        /// <summary>
        /// Background Color
        /// </summary>
        public Color? BgColor { get; set; }

        /// <summary>
        /// Background Alpha (0~100)
        /// </summary>
        public int? BgAlpha { get; set; }

        /// <summary>
        /// background에 gradient를 사용 시에 비율을 설정할 수 있다. 
        /// </summary>
        public string BgRatio { get; set; }

        /// <summary>
        ///  Background angle
        /// </summary>
        public int? BgAngle { get; set; }

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

            if(BgRatio.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "BgRatio", BgRatio);

            if(BgAngle.HasValue)
                writer.WriteAttributeString(Prefix + "BgAngle", BgAngle.Value.ToString());
        }
    }
}