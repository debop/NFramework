using System;
using System.Drawing;
using System.Linq;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// &lt;tasks/&gt; 을 표현하는 클래스입니다.
    /// </summary>
    [Serializable]
    public class TasksElement : CollectionElement<TaskElement> {
        public TasksElement() : base("tasks") {}

        public bool? ShowLabels { get; set; }
        public bool? ShowPercentLabel { get; set; }
        public bool? ShowStartDate { get; set; }
        public bool? ShowEndDate { get; set; }

        public Color? Color { get; set; }
        public int? Alpha { get; set; }

        private WidgetFontAttribute _fontAttr;

        public WidgetFontAttribute FontAttr {
            get { return _fontAttr ?? (_fontAttr = new WidgetFontAttribute()); }
            set { _fontAttr = value; }
        }

        private BorderAttribute _border;

        public BorderAttribute Border {
            get { return _border ?? (_border = new BorderAttribute()); }
            set { _border = value; }
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(ShowLabels.HasValue)
                writer.WriteAttributeString("ShowLabels", ShowLabels.Value.GetHashCode().ToString());
            if(ShowPercentLabel.HasValue)
                writer.WriteAttributeString("ShowPercentLabel", ShowPercentLabel.Value.GetHashCode().ToString());
            if(ShowStartDate.HasValue)
                writer.WriteAttributeString("ShowStartDate", ShowStartDate.Value.GetHashCode().ToString());
            if(ShowEndDate.HasValue)
                writer.WriteAttributeString("ShowEndDate", ShowEndDate.Value.GetHashCode().ToString());

            if(Color.HasValue)
                writer.WriteAttributeString("Color", Color.Value.ToHexString());
            if(Alpha.HasValue)
                writer.WriteAttributeString("Alpha", Alpha.Value.ToString());

            if(_fontAttr != null)
                _fontAttr.GenerateXmlAttributes(writer);
            if(_border != null)
                _border.GenerateXmlAttributes(writer);
        }

        /// <summary>
        /// 새로운 <see cref="TaskElement"/>를 생성합니다.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="label"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TaskElement AddTask(DateTime start, DateTime end, string label, string id) {
            var task = new TaskElement(start, end)
                       {
                           Id = id,
                           Label = label
                       };
            Add(task);
            return task;
        }

        /// <summary>
        /// 지정된 Id를 가지는 <see cref="TaskElement"/> 를 찾습니다. 없으면 null을 반환합니다. LINQ를 사용하세요
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TaskElement FindTaskById(string id) {
            return this.FirstOrDefault(task => StringTool.EqualTo(task.Id, id));
        }
    }
}