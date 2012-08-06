using System;
using System.Collections.Generic;
using System.Xml;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// FusionChart 3.0 부터 제공되는 스타일 정보입니다. HTML의 CSS와 같은 역할을 수행합니다.
    /// </summary>
    [Serializable]
    public class StylesElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public StylesElement() : base("styles") {}

        private IList<ChartStyleElementBase> _definition;

        /// <summary>
        /// Definition 
        /// </summary>
        public virtual IList<ChartStyleElementBase> Definition {
            get { return _definition ?? (_definition = new List<ChartStyleElementBase>()); }
            set { _definition = value; }
        }

        private IList<ApplyElement> _application;

        /// <summary>
        /// Application
        /// </summary>
        public virtual IList<ApplyElement> Application {
            get { return _application ?? (_application = new List<ApplyElement>()); }
            set { _application = value; }
        }

        public virtual void AddStyle(ChartStyleElementBase styleElement) {
            Definition.Add(styleElement);
        }

        public virtual void AddApply(string toObject, string styles) {
            Application.Add(new ApplyElement
                            {
                                ToObject = toObject,
                                Styles = styles
                            });
        }

        /// <summary>
        /// <see cref="ChartElementBase"/> 형식의 Element 객체들을 XML Element Node로 생성합니다.
        /// </summary>
        /// <param name="writer">Element를 쓸 Writer</param>
        protected override void GenerateXmlElements(XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_definition != null && _definition.Count > 0) {
                writer.WriteStartElement("definition");

                foreach(var style in _definition)
                    style.WriteXmlElement(writer);

                writer.WriteEndElement();
            }

            if(_application == null || _application.Count <= 0) return;

            writer.WriteStartElement("application");

            foreach(var apply in _application)
                apply.WriteXmlElement(writer);

            writer.WriteEndElement();
        }
    }
}