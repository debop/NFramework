using System;
using System.Collections.Generic;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// Gantt의 Y축을 표현한다.
    /// </summary>
    [Obsolete("ProcessesElement를 사용하세요.")]
    public class ProcessCollection : ChartElementBase {
        public ProcessCollection()
            : base("processes") {}

        public virtual string HeaderText { get; set; }
        public virtual HorizontalPosition? PositionInGrid { get; set; }
        public virtual string Width { get; set; }

        private ItemAttribute _headerAttr;

        public virtual ItemAttribute HeaderAttr {
            get { return _headerAttr ?? (_headerAttr = new ItemAttribute("header")); }
        }

        private ItemAttribute _itemAttr;

        public virtual ItemAttribute ItemAttr {
            get { return _itemAttr ?? (_itemAttr = new ItemAttribute()); }
        }

        private IList<ProcessElement> _processElements;

        public virtual IList<ProcessElement> ProcessElements {
            get { return _processElements ?? (_processElements = new List<ProcessElement>()); }
        }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(HeaderText.IsNotWhiteSpace())
                writer.WriteAttributeString("HeaderText", HeaderText);
            if(PositionInGrid.HasValue)
                writer.WriteAttributeString("PositionInGrid", PositionInGrid.Value.ToString());
            if(Width.IsNotWhiteSpace())
                writer.WriteAttributeString("Width", Width);

            if(_headerAttr != null)
                _headerAttr.GenerateXmlAttributes(writer);

            if(_itemAttr != null)
                _itemAttr.GenerateXmlAttributes(writer);
        }

        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_processElements != null)
                foreach(var processElement in _processElements)
                    processElement.WriteXmlElement(writer);
        }
    }
}