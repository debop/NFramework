namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// <alert/> 의 묶음을 나타내는 element 입니다.
    /// </summary>
    public class AlertsElement : CollectionElement<AlertElement> {
        /// <summary>
        /// Default constructor
        /// </summary>
        public AlertsElement() : base("alerts") {}
    }

    //public class AlertsElement : ChartElementBase
    //{
    //    /// <summary>
    //    /// Default constructor
    //    /// </summary>
    //    public AlertsElement() : base("alerts") { }

    //    private IList<AlertElement> _alertElements;

    //    /// <summary>
    //    /// AlertElement 컬렉션
    //    /// </summary>
    //    public virtual IList<AlertElement> AlertElements
    //    {
    //        get { return _alertElements ?? (_alertElements = new List<AlertElement>()); }
    //        set { _alertElements = value; }
    //    }

    //    /// <summary>
    //    /// Xml Element Node 속성을 XmlElement로 저장합니다. 꼭 <see cref="ChartElementBase.GenerateXmlAttributes"/> 수행 후에 해야 한다.
    //    /// </summary>
    //    /// <param name="writer"></param>
    //    protected override void GenerateXmlElements(System.Xml.XmlWriter writer)
    //    {
    //        base.GenerateXmlElements(writer);

    //        if (_alertElements != null)
    //            foreach (var alert in _alertElements)
    //                alert.WriteXmlElement(writer);

    //        // LINQ 사용시
    //        // _alertElements.ForEach(alert => alert.WriteXmlElement(writer));
    //    }
    //}
}