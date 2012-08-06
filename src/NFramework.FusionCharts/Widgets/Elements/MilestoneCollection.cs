using System;
using System.Collections.Generic;

namespace NSoft.NFramework.FusionCharts.Widgets {
    [Obsolete("CollectionElement{MilestoneElement}를 직접 사용하세요")]
    [Serializable]
    public class MilestoneCollection : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        public MilestoneCollection() : base("milestones") {}

        private IList<MilestoneElement> _milestoneElements;

        public virtual IList<MilestoneElement> MilestoneElements {
            get { return _milestoneElements ?? (_milestoneElements = new List<MilestoneElement>()); }
        }

        protected override void GenerateXmlElements(System.Xml.XmlWriter writer) {
            base.GenerateXmlElements(writer);

            if(_milestoneElements != null) {
                foreach(var milestone in _milestoneElements)
                    milestone.WriteXmlElement(writer);
            }
        }
    }
}