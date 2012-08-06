using System;

namespace NSoft.NFramework.FusionCharts.Charts {
    /// <summary>
    /// Padding 속성
    /// </summary>
    [Serializable]
    public class PaddingAttribute : ChartAttributeBase {
        internal const string PaddingSuffix = @"Padding";

        public int? Canvas { get; set; }
        public int? Caption { get; set; }
        public int? XAxisName { get; set; }
        public int? YAxisName { get; set; }
        public int? YAxisValues { get; set; }
        public int? Label { get; set; }
        public int? Value { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Canvas.HasValue)
                writer.WriteAttributeString("canvas" + PaddingSuffix, Canvas.ToString());
            if(Caption.HasValue)
                writer.WriteAttributeString("Caption" + PaddingSuffix, Caption.ToString());
            if(XAxisName.HasValue)
                writer.WriteAttributeString("XAxisName" + PaddingSuffix, XAxisName.ToString());
            if(YAxisName.HasValue)
                writer.WriteAttributeString("YAxisName" + PaddingSuffix, YAxisName.ToString());
            if(YAxisValues.HasValue)
                writer.WriteAttributeString("YAxisValues" + PaddingSuffix, YAxisValues.ToString());
            if(Label.HasValue)
                writer.WriteAttributeString("Label" + PaddingSuffix, Label.ToString());
            if(Value.HasValue)
                writer.WriteAttributeString("Value" + PaddingSuffix, Value.ToString());
        }
    }
}