namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// This attribute defines the action to be taken, when the value on the chart matches an alert range.
    /// </summary>
    public enum AlertActionKind {
        /// <summary>
        /// Calls a JavaScript function (specified in param attribute – explained next) when a value is matched against an alert range.
        /// If you need to pass parameters to JavaScript function, specify the exact function name and parameters in param attribute – 
        /// e.g., param="alert('Value between 240 and 300');"
        /// </summary>
        CallJS,

        /// <summary>
        /// Plays a MP3 sound (located relative to the chart), when an alert range is matched. 
        /// The relative URL of MP3 sound should be declared in param attribute.
        /// </summary>
        PlaySound,

        /// <summary>
        /// FusionWidgets allows you to create your custom annotations and annotation groups (with named IDs).
        /// This action lets you show a pre-defined annotation group in the chart. 
        /// For example, you may define 3 status indicators as 3 circles having green, yellow and red color, and assign an annotation group ID for each one of them. 
        /// By default, you may hide all status indicators. 
        /// Later, based on the chart’s value, you may show an annotation.
        /// The group Id of the annotation to be shown is defined in param attribute.
        /// When the value again falls out of alert range, FusionWidgets hides that annotation automatically.
        /// </summary>
        ShowAnnotation
    }
}