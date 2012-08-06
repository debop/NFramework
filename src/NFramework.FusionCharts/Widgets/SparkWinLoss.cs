using System.Drawing;

namespace NSoft.NFramework.FusionCharts.Widgets {
    public class SparkWinLoss : WidgetBase {
        public Color? WinColor { get; set; }
        public Color? LossColor { get; set; }
        public Color? DrawColor { get; set; }
        public Color? ScorelessColor { get; set; }

        public int? PeriodLength { get; set; }
        public Color? PeriodColor { get; set; }
        public int? PeriodAlpha { get; set; }

        #region << DataSet >>

        public virtual void AddValue(WinLossKind winLossKind) {
            AddValue(winLossKind, null);
        }

        public virtual void AddValue(WinLossKind winLossKind, bool? scoreless) {
            Dataset.Add(new SetElement
                        {
                            Value = winLossKind,
                            Scoreless = scoreless
                        });
        }

        private CollectionElement<SetElement> _dataset;

        public CollectionElement<SetElement> Dataset {
            get { return _dataset ?? (_dataset = new CollectionElement<SetElement>("dataset")); }
            set { _dataset = value; }
        }

        public class SetElement : ChartElementBase {
            public SetElement() : base("set") {}

            public WinLossKind? Value { get; set; }
            public bool? Scoreless { get; set; }

            public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
                base.GenerateXmlAttributes(writer);

                if(Value.HasValue)
                    writer.WriteAttributeString("value", Value.ToString().Substring(0, 1));
                if(Scoreless.HasValue)
                    writer.WriteAttributeString("scoreless", Scoreless.GetHashCode().ToString());
            }
        }

        #endregion

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(PeriodLength.HasValue)
                writer.WriteAttributeString("PeriodLength", PeriodLength.ToString());
            if(PeriodColor.HasValue)
                writer.WriteAttributeString("PeriodColor", PeriodColor.Value.ToHexString());
            if(PeriodAlpha.HasValue)
                writer.WriteAttributeString("PeriodAlpha", PeriodAlpha.ToString());
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_dataset != null)
                _dataset.WriteXmlElement(writer);
        }
    }
}