namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// 색으로 구간을 나타내는 Element입니다.
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;chart lowerLimit='0' upperLimit='100' lowerLimitDisplay='Bad' upperLimitDisplay='Good' gaugeStartAngle='180' gaugeEndAngle='0' palette='1' numberSuffix='%' tickValueDistance='20' showValue='1'&gt;
    ///		&lt;colorRange&gt;
    ///			&lt;color minValue='0' maxValue='75' code='FF654F'/&gt;
    ///			&lt;color minValue='75' maxValue='90' code='F6BD0F'/&gt;
    ///			&lt;color minValue='90' maxValue='100' code='8BBA00'/&gt;
    ///		&lt;/colorRange&gt;
    ///		&lt;dials&gt;
    ///			&lt;dial value='92' rearExtension='10'/&gt;
    ///		&lt;/dials&gt;
    /// &lt;/chart&gt;
    /// </code>
    /// </example>
    public class ColorRangeElement : CollectionElement<ColorElement> {
        public ColorRangeElement() : base("colorRange") {}
    }
}