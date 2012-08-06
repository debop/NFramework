using System;
using System.Drawing;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// 디버깅 등을 위한 메시지 로깅 정보
    /// </summary>
    [Serializable]
    public class MessageLoggerAttribute : ChartAttributeBase {
        public bool? UseMessageLog { get; set; }
        public double? WPercent { get; set; }
        public double? HPercent { get; set; }

        public bool? ShowTitle { get; set; }
        public string Title { get; set; }
        public Color? Color { get; set; }

        public bool? MessageGoesToLog { get; set; }
        public bool? MessageGoesToJS { get; set; }
        public string MessageJSHandler { get; set; }
        public bool? MessagePassAllToJS { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer">xml writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(UseMessageLog.HasValue)
                writer.WriteAttributeString("UseMessageLog", UseMessageLog.Value.GetHashCode().ToString());
            if(WPercent.HasValue)
                writer.WriteAttributeString("WPercent", WPercent.Value.ToString());
            if(HPercent.HasValue)
                writer.WriteAttributeString("HPercent", HPercent.Value.ToString());

            if(ShowTitle.HasValue)
                writer.WriteAttributeString("ShowTitle", ShowTitle.Value.GetHashCode().ToString());
            if(Title.IsNotWhiteSpace())
                writer.WriteAttributeString("Title", Title);
            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());

            if(MessageGoesToLog.HasValue)
                writer.WriteAttributeString("MessageGoesToLog", MessageGoesToLog.Value.GetHashCode().ToString());
            if(MessageGoesToJS.HasValue)
                writer.WriteAttributeString("MessageGoesToJS", MessageGoesToJS.Value.GetHashCode().ToString());
            if(MessageJSHandler.IsNotWhiteSpace())
                writer.WriteAttributeString("MessageJSHandler", MessageJSHandler);
            if(MessagePassAllToJS.HasValue)
                writer.WriteAttributeString("MessagePassAllToJS", MessagePassAllToJS.Value.GetHashCode().ToString());
        }
    }
}