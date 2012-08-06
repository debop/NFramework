using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// FusionWidgets v3 introduces a new concept of streaming and showing real-time messages in the chart using Message Logger. The Message logger can be effectively used to show necessary real-time information or live error logs.
    /// Essentially, the message logger is a text based scrollable window that can listen to messages streamed from server and then do one of the following:
    /// Display the message in the message logger window
    /// Pass it to custom JavaScript functions (defined by you) for further actions
    /// </summary>
    public class MessageLogAttribute : ChartAttributeBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public MessageLogAttribute() : this("MessageLog") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="attrName"></param>
        public MessageLogAttribute(string attrName) {
            AttrName = (attrName ?? string.Empty).Trim();
        }

        /// <summary>
        /// 속성명
        /// </summary>
        public string AttrName { get; private set; }

        /// <summary>
        /// Whether to use message logger for the chart?
        /// </summary>
        public bool? Use { get; set; }

        /// <summary>
        /// Percent.
        /// This attribute lets you set the width percent of the message logger window w.r.t entire chart width. So, if you set it as 80, the message logger window will take up 80% of chart width.
        /// </summary>
        public int? WPercent { get; set; }

        /// <summary>
        /// Percent.
        /// This attribute lets you set the height percent of the message logger window w.r.t entire chart height.
        /// </summary>
        public int? HPercent { get; set; }

        /// <summary>
        /// Whether to show the title for message logger?
        /// </summary>
        public bool? ShowTitle { get; set; }

        /// <summary>
        /// If you've opted to show the title for message logger, you can define your custom title here. Examples are "Error log", "Server History" etc. The title displays at the top left corner of chart.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// You can customize the color of the entire message log window by setting a hex value for this attribute.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// The messages streamed to the chart can either be displayed in the message log window or can be passed to JavaScript (which we'll see next). This attribute lets you control whether the messages should be logged in the in-built log window.
        /// </summary>
        public bool? MessageGoesToLog { get; set; }

        /// <summary>
        /// This attribute lets you configure whether each message streamed from the server should be passed to a local JavaScript function.
        /// </summary>
        public bool? MessageGoesToJS { get; set; }

        /// <summary>
        /// If you've opted to pass each message to JavaScript function, this attribute lets you define the name of the function. This helps you create your custom functions to react to messages streamed from server.
        /// </summary>
        public string MessageJSHandler { get; set; }

        /// <summary>
        /// Whether all parameters passed as part of message envelope be passed to the custom JavaScript function.
        /// </summary>
        public bool? MessagePassAllJS { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML 속성으로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Use.HasValue)
                writer.WriteAttributeString("Use" + AttrName, Use.GetHashCode().ToString());
            if(WPercent.HasValue)
                writer.WriteAttributeString(AttrName + "WPercent", WPercent.ToString());
            if(HPercent.HasValue)
                writer.WriteAttributeString(AttrName + "HPercent", HPercent.ToString());
            if(ShowTitle.HasValue)
                writer.WriteAttributeString(AttrName + "ShowTitle", ShowTitle.GetHashCode().ToString());
            if(Title.IsNotWhiteSpace())
                writer.WriteAttributeString(AttrName + "Title", Title);
            if(Color.HasValue)
                writer.WriteAttributeString(AttrName + "Color", Color.Value.ToHexString());

            if(MessageGoesToLog.HasValue)
                writer.WriteAttributeString("MessageGoesToLog", MessageGoesToLog.GetHashCode().ToString());
            if(MessageGoesToJS.HasValue)
                writer.WriteAttributeString("MessageGoesToJS", MessageGoesToJS.GetHashCode().ToString());
            if(MessageJSHandler.IsNotWhiteSpace())
                writer.WriteAttributeString("MessageJSHandler", MessageJSHandler);
            if(MessagePassAllJS.HasValue)
                writer.WriteAttributeString("MessagePassAllJS", MessagePassAllJS.GetHashCode().ToString());
        }
    }
}