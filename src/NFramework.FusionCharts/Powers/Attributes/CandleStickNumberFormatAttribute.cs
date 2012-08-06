using System;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Powers {
    [Serializable]
    public class CandleStickNumberFormatAttribute : NumberFormatAttribute {
        /// <summary>
        /// This configuration determines whether the numbers belonging to volume chart will be formatted using commas, e.g., 40,000 if vFormatNumber='1' and 40000 if vFormatNumber='0 '
        /// </summary>
        public bool? VFormatNumber { get; set; }

        /// <summary>
        /// Configuration whether to add K (thousands) and M (millions) to a number belonging to volume chart after truncating and rounding it - e.g., if vFormatNumberScale is set to 1, 10434 would become 1.04K (with vDecimals set to 2 places). Same with numbers in millions - a M will added at the end. For more details, please see Advanced Number Formatting section.
        /// </summary>
        public bool? VFormatNumberScale { get; set; }

        /// <summary>
        ///  The default unit of the numbers belonging to volume chart. For more details, please see Advanced Number Formatting section.
        /// </summary>
        public string VDefaultNumberScale { get; set; }

        /// <summary>
        /// Unit of each block of the scale of volume chart. For more details, please see Advanced Number Formatting section.
        /// </summary>
        public string VNumberScaleUnit { get; set; }

        /// <summary>
        /// Range of the various blocks that constitute the scale for volume chart. For more details, please see Advanced Number Formatting section.
        /// </summary>
        public string VNumberScaleValue { get; set; }

        /// <summary>
        /// Using this attribute, you could add prefix to all the numbers belonging to volume chart. For more details, please see Advanced Number Formatting section.
        /// </summary>
        public string VNumberPrefix { get; set; }

        /// <summary>
        /// Using this attribute, you could add prefix to all the numbers belonging to volume chart. For more details, please see Advanced Number Formatting section.
        /// </summary>
        public string VNumberSuffix { get; set; }

        /// <summary>
        /// 0-10
        /// Number of decimal places to which all numbers belonging to volume chart would be rounded to.
        /// </summary>
        public int? VDecimals { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(VFormatNumber.HasValue)
                writer.WriteAttributeString("vFormatNumber", VFormatNumber.GetHashCode().ToString());
            if(VFormatNumberScale.HasValue)
                writer.WriteAttributeString("vFormatNumberScale", VFormatNumberScale.GetHashCode().ToString());
            if(VDefaultNumberScale.IsNotWhiteSpace())
                writer.WriteAttributeString("vDefaultNumberScale", VDefaultNumberScale);
            if(VNumberScaleUnit.IsNotWhiteSpace())
                writer.WriteAttributeString("vNumberScaleUnit", VNumberScaleUnit);
            if(VNumberScaleValue.IsNotWhiteSpace())
                writer.WriteAttributeString("vNumberScaleValue", VNumberScaleValue);
            if(VNumberPrefix.IsNotWhiteSpace())
                writer.WriteAttributeString("vNumberPrefix", VNumberPrefix);
            if(VNumberSuffix.IsNotWhiteSpace())
                writer.WriteAttributeString("vNumberSuffix", VNumberSuffix);
        }
    }
}