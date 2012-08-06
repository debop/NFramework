namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// TrendLines Element ( <trendlines /> )
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TrendLinesElement<T> : CollectionElement<T> where T : LineElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public TrendLinesElement() : base("trendlines") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="elementName"></param>
        public TrendLinesElement(string elementName) : base(elementName) {}
    }

    /*
		/// <summary>
		/// Trendline Collection
		/// </summary>
		[Serializable]
		public class TrendLinesElement : ChartElementBase
		{
			public TrendLinesElement() : base("trendlines") { }
			public TrendLinesElement(string elementName) : base(elementName) { }

			private IList<LineElementBase> _lineElements;
			public virtual IList<LineElementBase> TrendlineElements
			{
				get { return _lineElements ?? (_lineElements = new List<LineElementBase>()); }
			}

			public override void GenerateXmlAttributes(System.Xml.XmlWriter writer)
			{
				base.GenerateXmlAttributes(writer);
			}
			protected override void GenerateXmlElements(System.Xml.XmlWriter writer)
			{
				base.GenerateXmlElements(writer);

				if (_lineElements != null)
				{
					foreach (var line in _lineElements)
						line.WriteXmlElement(writer);
				}
			}
		}
	*/
}