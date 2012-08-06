using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Number Format Attribute
    /// </summary>
    [Serializable]
    public class NumberFormatAttribute : ChartAttributeBase {
        /// <summary>
        /// This configuration determines whether the numbers displayed on the chart will be formatted using commas, e.g., 40,000 if formatNumber='1' and 40000 if formatNumber='0 ' 
        /// </summary>
        public bool? FormatNumber { get; set; }

        /// <summary>
        /// Using this attribute, you could add prefix to all the numbers visible on the graph. For example, to represent all dollars figure on the chart, you could specify this attribute to ' $' to show like $40000, $50000. For more details, please see Advanced Number Formatting section. 
        /// </summary>
        public string NumberPrefix { get; set; }

        /// <summary>
        /// Using this attribute, you could add suffix to all the numbers visible on the graph. For example, to represent all figure quantified as per annum on the chart, you could specify this attribute to ' /a' to show like 40000/a, 50000/a. For more details, please see Advanced Number Formatting section. 
        /// </summary>
        public string NumberSuffix { get; set; }

        /// <summary>
        /// Number of decimal places to which all numbers on the chart would be rounded to. 
        /// </summary>
        public int? Decimals { get; set; }

        /// <summary>
        /// Whether to add 0 padding at the end of decimal numbers? For example, if you set decimals as 2 and a number is 23.4. If forceDecimals is set to 1, FusionCharts will convert the number to 23.40 (note the extra 0 at the end) 
        /// </summary>
        public bool? ForceDecimals { get; set; }

        /// <summary>
        /// Configuration whether to add K (thousands) and M (millions) to a number after truncating and rounding it - e.g., if formatNumberScale is set to 1, 1043 would become 1.04K (with decimals set to 2 places). Same with numbers in millions - a M will added at the end. For more details, please see Advanced Number Formatting section. 
        /// </summary>
        public bool? FormatNumberScale { get; set; }

        public string DefaultNumberScale { get; set; }
        public string NumberScaleUnit { get; set; }
        public string NumberScaleValue { get; set; }
        public bool? ScaleRecursively { get; set; }

        /// <summary>
        /// How many recursions to complete during recursive scaling? -1 completes the entire set of recursion. 
        /// </summary>
        public int? MaxScaleRecursion { get; set; }

        /// <summary>
        /// What character to use to separte the scales that generated after recursion? 
        /// </summary>
        public string ScaleSeparator { get; set; }

        public string DecimalSeparator { get; set; }
        public string ThousandSeparator { get; set; }
        public string InDecimalSeparator { get; set; }
        public string InThousandSeparator { get; set; }
        public int? YAxisValueDecimals { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(FormatNumber.HasValue)
                writer.WriteAttributeString("FormatNumber", FormatNumber.Value.GetHashCode().ToString());
            if(NumberPrefix.IsNotWhiteSpace())
                writer.WriteAttributeString("NumberPrefix", NumberPrefix);
            if(NumberSuffix.IsNotWhiteSpace())
                writer.WriteAttributeString("NumberSuffix", NumberSuffix);
            if(Decimals.HasValue)
                writer.WriteAttributeString("Decimals", Decimals.Value.ToString());
            if(ForceDecimals.HasValue)
                writer.WriteAttributeString("ForceDecimals", ForceDecimals.Value.GetHashCode().ToString());
            if(FormatNumberScale.HasValue)
                writer.WriteAttributeString("FormatNumberScale", FormatNumberScale.Value.GetHashCode().ToString());

            if(DefaultNumberScale.IsNotWhiteSpace())
                writer.WriteAttributeString("DefaultNumberScale", DefaultNumberScale);
            if(NumberScaleUnit.IsNotWhiteSpace())
                writer.WriteAttributeString("NumberScaleUnit", NumberScaleUnit);
            if(NumberScaleValue.IsNotWhiteSpace())
                writer.WriteAttributeString("NumberScaleValue", NumberScaleValue);
            if(ScaleRecursively.HasValue)
                writer.WriteAttributeString("ScaleRecursively", ScaleRecursively.Value.GetHashCode().ToString());
            if(MaxScaleRecursion.HasValue)
                writer.WriteAttributeString("MaxScaleRecursion", MaxScaleRecursion.Value.ToString());
            if(ScaleSeparator.IsNotWhiteSpace())
                writer.WriteAttributeString("ScaleSeparator", ScaleSeparator);
            if(DecimalSeparator.IsNotWhiteSpace())
                writer.WriteAttributeString("DecimalSeparator", DecimalSeparator);
            if(ThousandSeparator.IsNotWhiteSpace())
                writer.WriteAttributeString("ThousandSeparator", ThousandSeparator);

            if(InDecimalSeparator.IsNotWhiteSpace())
                writer.WriteAttributeString("InDecimalSeparator", InDecimalSeparator);
            if(InThousandSeparator.IsNotWhiteSpace())
                writer.WriteAttributeString("InThousandSeparator", InThousandSeparator);

            if(YAxisValueDecimals.HasValue)
                writer.WriteAttributeString("yAxisValueDecimals", YAxisValueDecimals.Value.ToString());
        }
    }
}