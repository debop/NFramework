using System;
using System.Drawing;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [Serializable]
    public class GaugePivotAttribute : BorderAttribute {
        /// <summary>
        /// 생성자
        /// </summary>
        public GaugePivotAttribute() : base("pivot") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="prefix"></param>
        protected GaugePivotAttribute(string prefix) : base(prefix) {}

        public int? Radius { get; set; }
        public Color? FillColor { get; set; }
        public int? FillAlpha { get; set; }
        public int? FillAngle { get; set; }

        public FillMethod? FillType { get; set; }

        // TODO : Gradient fill fomula 를 class화 해야 한다.
        /// <summary>
        /// Gradient fill formula for the scale.
        /// </summary>
        /// <example>
        /// <code>
        /// &lt;chart showPivotBorder='1' pivotBorderThickness='2' pivotBorderColor='333333' pivotBorderAlpha='100' pivotFillColor='CCCCCC' pivotFillAlpha='100' pivotFillMix='' ...&gt;
        /// </code>
        /// </example>
        public string FillMix { get; set; }

        /// <summary>
        /// Ratio fill mix for the scale.
        /// Ratio of each color separated by comma
        /// </summary>
        public string FillRatio { get; set; }

        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Radius.HasValue)
                writer.WriteAttributeString(Prefix + "Radius", Radius.ToString());
            if(FillColor.HasValue)
                writer.WriteAttributeString(Prefix + "FillColor", FillColor.Value.ToHexString());
            if(FillAlpha.HasValue)
                writer.WriteAttributeString(Prefix + "FillAlpha", FillAlpha.ToString());
            if(FillAngle.HasValue)
                writer.WriteAttributeString(Prefix + "FillAngle", FillAngle.ToString());

            if(FillType.HasValue)
                writer.WriteAttributeString(Prefix + "FillType", FillType.GetHashCode().ToString());

            if(FillMix.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "FillMix", FillMix);
            if(FillRatio.IsNotWhiteSpace())
                writer.WriteAttributeString(Prefix + "FillRatio", FillRatio);
        }
    }
}