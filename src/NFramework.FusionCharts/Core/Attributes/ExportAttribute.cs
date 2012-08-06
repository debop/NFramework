using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// XML attributes of <chart> element are used to configure all aspects of export chart.
    /// </summary>
    public class ExportAttribute : ChartAttributeBase {
        internal const string EXPORT = @"Export";

        /// <summary>
        /// Whether the chart will allow exporting to images/PDFs?
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Whether the menu items related to export (e.g., Save as JPEG etc.) will appear in the context menu of chart.
        /// </summary>
        public bool? ShowMenuItem { get; set; }

        /// <summary>
        /// List of formats that the chart will show in context menu, along with label for each one.
        /// The attribute value should be a delimiter separated key-value pair. The delimiter character to be used is '|' (pipe character). The syntax for the attribute value is as follows:
        /// KEY=Value[|KEY=Value]*
        /// 
        /// Example: The code required to enable PNG, JPG and PDF type of export with custom context-menu message for PNG and PDF.
        /// exportType=”PNG=Export as High Quality Image;JPG;PDF=Export as PDF File”
        /// </summary>
        public string Formats { get; set; }

        /// <summary>
        /// Whether to use client side export handlers, or server side export handlers
        /// </summary>
        public bool? AtClient { get; set; }

        /// <summary>
        /// In case of server side exporting, this refers to the path of the server-side export handler (the ready-to-use scripts that we provide).
        /// In case of client-side exporting, this refers to the DOM-Id of FusionCharts Export component that is embedded in your web page, along with the chart.
        /// </summary>
        public string Handler { get; set; }

        /// <summary>
        /// Save|Download
        /// In case of server-side exporting, the action specifies whether the exported image will be sent back to client as download, or whether it'll be saved on the server.
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// _self|_blank
        /// In case of server-side exporting and when using download as action, this lefts you configure whether the return image/PDF would open in same window (as an attachment for download), or whether it will open in a new window.
        /// </summary>
        public string TargetWindow { get; set; }

        /// <summary>
        /// Name of JavaScript function that'll be called back when export process has finished in case of:
        /// 
        ///		Client side export
        ///		Batch export
        ///		Server-side export using 'save' as action
        /// </summary>
        public string Callback { get; set; }

        /// <summary>
        /// Using this attribute you can specify the name (excluding the extension) of the output (export) file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Whether to show the export dialog during capture phase. If not, the chart starts capturing process without the dialog visible.
        /// </summary>
        public bool? ShowDialog { get; set; }

        /// <summary>
        /// The message to be shown in the dialog box. The default is "Capturing Data : "
        /// </summary>
        public string DialogMessage { get; set; }

        /// <summary>
        /// Background color of dialog box.
        /// </summary>
        public Color? DialogColor { get; set; }

        /// <summary>
        /// Border color of dialog box.
        /// </summary>
        public Color? DialogBorderColor { get; set; }

        /// <summary>
        /// Font color to be used for text in dialog.
        /// </summary>
        public Color? DialogFontColor { get; set; }

        /// <summary>
        /// Color of progress bar in dialog.
        /// </summary>
        public Color? DialogPBColor { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Enabled.HasValue)
                writer.WriteAttributeString(EXPORT + "Enabled", Enabled.GetHashCode().ToString());
            if(ShowMenuItem.HasValue)
                writer.WriteAttributeString(EXPORT + "ShowMenuItem", ShowMenuItem.GetHashCode().ToString());
            if(Formats.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT + "Formats", Formats);
            if(AtClient.HasValue)
                writer.WriteAttributeString(EXPORT + "AtClient", AtClient.GetHashCode().ToString());
            if(Handler.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT + "Handler", Handler);
            if(Action.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT + "Action", Action);
            if(TargetWindow.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT + "TargetWindow", TargetWindow);
            if(Callback.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT + "Callback", Callback);
            if(FileName.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT + "FileName", FileName);

            if(ShowDialog.HasValue)
                writer.WriteAttributeString("Show" + EXPORT + "Dialog", ShowDialog.GetHashCode().ToString());
            if(DialogMessage.IsNotWhiteSpace())
                writer.WriteAttributeString(EXPORT + "DialogMessage", DialogMessage);

            if(DialogColor.HasValue)
                writer.WriteAttributeString(EXPORT + "DialogColor", DialogColor.Value.ToHexString());
            if(DialogBorderColor.HasValue)
                writer.WriteAttributeString(EXPORT + "DialogBorderColor", DialogBorderColor.Value.ToHexString());
            if(DialogFontColor.HasValue)
                writer.WriteAttributeString(EXPORT + "DialogFontColor", DialogFontColor.Value.ToHexString());
            if(DialogPBColor.HasValue)
                writer.WriteAttributeString(EXPORT + "DialogPBColor", DialogPBColor.Value.ToHexString());
        }
    }
}