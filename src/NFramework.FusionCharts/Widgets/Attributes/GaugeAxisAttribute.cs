using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Guage 의 Axis Attributes
    /// </summary>
    [Serializable]
    public class GaugeAxisAttribute : ChartAttributeBase {
        /// <summary>
        /// This attribute lets you set whether the lower limit would be 0 (in case of all positive values on chart) or should the lower limit adapt itself to a different figure based on values provided to the chart.
        /// </summary>
        public bool? SetAdaptiveMin { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the lower limit of the chart. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you.
        /// </summary>
        public int? UpperLimit { get; set; }

        /// <summary>
        /// This attribute helps you explicitly set the upper limit of the chart. If you don't specify this value, it is automatically calculated by FusionCharts based on the data provided by you.
        /// </summary>
        public int? LowerLimit { get; set; }

        /// <summary>
        /// This attribute allows you to display a label instead of the lower limit. For example, in a chart displaying Literacy Rate on a scale of 0-100%, you may need to show the label Low at the starting point of the chart. Upon using this attribute to specify the label, the value 0 would be replaced by Low.
        /// Default value: If you do not specify the lowerLimitDisplay attribute, the lower limit value would be shown.
        /// </summary>
        public string UpperLimitDisplay { get; set; }

        /// <summary>
        /// This attribute allows you to display a label instead of the upper limit. Upon using this attribute, the upper limit of the chart gets replaced by the label specified.
        /// Default value: If you do not specify the upperLimitDisplay attribute, the upper limit value would be shown.
        /// </summary>
        public string LowerLimitDisplay { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(SetAdaptiveMin.HasValue)
                writer.WriteAttributeString("SetAdaptiveMin", SetAdaptiveMin.GetHashCode().ToString());
            if(UpperLimit.HasValue)
                writer.WriteAttributeString("UpperLimit", UpperLimit.ToString());
            if(LowerLimit.HasValue)
                writer.WriteAttributeString("LowerLimit", LowerLimit.ToString());
            if(UpperLimitDisplay.IsNotWhiteSpace())
                writer.WriteAttributeString("UpperLimitDisplay", UpperLimitDisplay);
            if(LowerLimitDisplay.IsNotWhiteSpace())
                writer.WriteAttributeString("LowerLimitDisplay", LowerLimitDisplay);
        }
    }
}