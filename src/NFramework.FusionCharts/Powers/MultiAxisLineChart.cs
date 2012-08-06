using System.Collections.Generic;
using NSoft.NFramework.FusionCharts.Charts;

namespace NSoft.NFramework.FusionCharts.Powers {
    /// <summary>
    /// The multi-axis line chart from PowerCharts suite is an interactive line chart that allows for the following features:
    /// <list>
    ///		<item>Multiple axis on the same chart.</item>
    ///		<item>Interactive axis, that allow the end user to show/hide data-sets (lines) plotting against that axis.</item>
    ///		<item>Options to make the axis visible or imaginary.</item>
    ///		<item>Option to draw the axis on left or right side of chart.</item>
    ///		<item>Interactive options like dynamic sliding of axis from one end to other, upon clicking.</item>
    ///		<item>Ability to plot multiple data-sets against the same axis.</item>
    /// </list>
    /// Axis specific properties like:
    /// <list>
    ///		<item>Upper and lower limits</item>
    ///		<item>Cosmetic Properties of axis</item>
    ///		<item>Divisional Lines</item>
    ///		<item>Cosmetic properties of datasets plotting against the particular axis</item>
    ///		<item>Number Formatting</item>
    /// </list>
    /// </summary>
    public class MultiAxisLineChart : FusionChartBase {
        /// <summary>
        /// Whether to show the checkboxes for each axis, thereby allowing to user to show/hide datasets belonging to that particular axis.
        /// </summary>
        public bool? AllowSelection { get; set; }

        /// <summary>
        /// Whether to allow shifting of axis from one end to other, upon clicking the same.
        /// </summary>
        public bool? AllowAxisShift { get; set; }

        /// <summary>
        /// This attributes lets you control whether empty data sets in your data will be connected to each other OR would they appear as broken data sets? Please see Discontinuous data section for more details on this.
        /// </summary>
        public bool? ConnectNullData { get; set; }

        private AnchorAttribute _anchorAttr;

        /// <summary>
        /// Anchor Attribute
        /// </summary>
        public AnchorAttribute AnchorAttr {
            get { return _anchorAttr ?? (_anchorAttr = new AnchorAttribute()); }
            set { _anchorAttr = value; }
        }

        /// <summary>
        /// 속성 중 Attribute Node로 표현해야 한다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(System.Xml.XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(AllowSelection.HasValue)
                writer.WriteAttributeString("AllowSelection", AllowSelection.GetHashCode().ToString());
            if(AllowAxisShift.HasValue)
                writer.WriteAttributeString("AllowAxisShift", AllowAxisShift.GetHashCode().ToString());
            if(ConnectNullData.HasValue)
                writer.WriteAttributeString("ConnectNullData", ConnectNullData.GetHashCode().ToString());

            if(_anchorAttr != null)
                _anchorAttr.GenerateXmlAttributes(writer);
        }

        private IList<MultiAxisElement> _axises;

        /// <summary>
        /// Axis collection
        /// </summary>
        public IList<MultiAxisElement> Axises {
            get { return _axises ?? (_axises = new List<MultiAxisElement>()); }
            set { _axises = value; }
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_axises != null && _axises.Count > 0)
                foreach(var axis in _axises)
                    axis.WriteXmlElement(writer);
        }
    }
}