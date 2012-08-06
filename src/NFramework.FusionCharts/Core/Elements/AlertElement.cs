using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// For any real-time chart, you can set up the alert manager as under:
    /// </summary>
    /// <example>
    /// <code>
    /// <chart>
    /// …
    /// …
    ///		<alerts>
    ///			<alert minValue='240' maxValue='300' action='callJS' param="alert('Value between 240 and 300');" />
    ///			<alert minValue='300' maxValue='360' action='showAnnotation' param='statusRed' occurOnce='0'/> 
    ///			<alert minValue='360' maxValue='400' action='playsound' param='YourSound.mp3'/>       
    ///		</alerts>
    /// </chart>
    /// </code>
    /// </example>
    public class AlertElement : ChartElementBase {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AlertElement() : base("alert") {}

        /// <summary>
        /// The minimum value for the alert range. For example, if you want to define an alert for the range 100-120, 100 will be minValue. When real-time values are matched against minValue, it’s taken as inclusive. For example, if you define 100 as minValue, and the value which is being checked is 100, it’ll fall in this defined alert range.
        /// </summary>
        public int? MinValue { get; set; }

        /// <summary>
        /// The maximum value for the alert range. For example, if you want to define an alert for the range 100-120, 120 will be maxValue. Like minValue, even maxValue is inclusive during value checks against alert ranges.
        /// </summary>
        public int? MaxValue { get; set; }

        /// <summary>
        /// This attribute defines the action to be taken, when the value on the chart matches an alert range
        /// </summary>
        public AlertActionKind? Action { get; set; }

        /// <summary>
        /// This attribute accepts the parameter for the action, depending on the action. For CALLJS action, it takes the name of JavaScript function. For PLAYSOUND action, it takes the relative path of MP3 file, and for SHOWANNOTATION action, it takes the ID of the annotation group to show.
        /// </summary>
        public string Param { get; set; }

        /// <summary>
        /// Often, you might need an alert threshold range to act only once, irrespective of how many times the value goes into the range. For example, if you want to be notified only the first time when the temperature goes into an alert range, set this attribute to 1. That will make sure that alert action for that range takes place only once, irrespective of how many times the value goes into the range later.
        /// </summary>
        public bool? OccurOnce { get; set; }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(MinValue.HasValue)
                writer.WriteAttributeString("MinValue", MinValue.ToString());
            if(MaxValue.HasValue)
                writer.WriteAttributeString("MaxValue", MaxValue.ToString());
            if(Action.HasValue)
                writer.WriteAttributeString("Action", Action.ToString());
            if(Param.IsNotWhiteSpace())
                writer.WriteAttributeString("Param", Param);
            if(OccurOnce.HasValue)
                writer.WriteAttributeString("OccurOnce", OccurOnce.ToString());
        }
    }
}