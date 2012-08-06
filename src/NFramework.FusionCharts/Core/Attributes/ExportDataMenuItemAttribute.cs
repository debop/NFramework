using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// Chart 의 Data를 CSV 형태로 Clipboard로 복사하기 위한 Attribute
    /// </summary>
    public class ExportDataMenuItemAttribute : MenuItemAttributeBase {
        internal const string EXPORT_DATA = @"ExportData";

        /// <summary>
        /// Constructor
        /// </summary>
        public ExportDataMenuItemAttribute() : base(EXPORT_DATA) {}

        /// <summary>
        /// Lets you set the separator for CSV data. For ease of use, this attribute accepts the following pseudo codes for characters: 
        ///		{tab} - To specify tab character
        ///		{quot} - To specify double quotes
        ///		{apos} - To specify single quotes
        /// You can also specify any other character apart from these pseudo codes.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Lets you set the qualifier character for CSV data. For ease of use, this attribute accepts the following pseudo codes for characters:
        ///		{tab} - To specify tab character
        ///		{quot} - To specify double quotes
        ///		{apos} - To specify single quotes
        /// You can also specify any other character apart from these pseudo codes.
        /// </summary>
        public string Qualifier { get; set; }

        /// <summary>
        /// 포맷된 값
        /// </summary>
        public string FormattedVal { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Separator.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT_DATA + "Separator", Separator);

            if(Qualifier.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT_DATA + "Qualifier", Qualifier);

            if(FormattedVal.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT_DATA + "FormattedVal", FormattedVal);
        }
    }
}