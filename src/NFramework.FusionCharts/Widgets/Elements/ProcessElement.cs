using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts.Widgets {
    /// <summary>
    /// GANTT 의 Y축의 값을 뜻합니다. (WBS 의 한 항목입니다 - 여기에는 여러개의 TaskElement를 가질 수 있습니다.)
    /// </summary>
    [Serializable]
    public class ProcessElement : ChartElementBase {
        /// <summary>
        /// 생성자
        /// </summary>
        protected ProcessElement() : base("process") {}

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="id"></param>
        public ProcessElement(string id)
            : this() {
            Guard.Assert(id.IsNotWhiteSpace(), @"id is null or empty string.");
            Id = id;
        }

        /// <summary>
        /// 프로세스 Id
        /// </summary>
        public string Id { get; set; }

        private FusionLink _link;

        public FusionLink Link {
            get { return _link ?? (_link = new FusionLink()); }
            set { _link = value; }
        }

        private ItemAttribute _itemAttr;

        public ItemAttribute ItemAttr {
            get { return _itemAttr ?? (_itemAttr = new ItemAttribute()); }
            set { _itemAttr = value; }
        }

        //private TaskCollection _tasks;
        ///// <summary>
        ///// WBS 한 항목에 해당하는 여러개의 Task. 
        ///// </summary>
        //public virtual TaskCollection Tasks
        //{
        //    get { return _tasks ?? (_tasks = new TaskCollection()); }
        //}
        private TasksElement _tasks;

        public TasksElement Tasks {
            get { return _tasks ?? (_tasks = new TasksElement()); }
            set { _tasks = value; }
        }

        public virtual void AddTask(TaskElement task) {
            task.ShouldNotBeNull("task");

            task.ProcessId = Id;
            Tasks.Add(task);
        }

        /// <summary>
        /// 속성들을 Xml Attribute로 생성합니다.
        /// </summary>
        /// <param name="writer">Attribute를 쓸 Writer</param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Id.IsNotWhiteSpace())
                writer.WriteAttributeString("Id", Id);

            if(_link != null)
                _link.GenerateXmlAttributes(writer);

            if(_itemAttr != null)
                _itemAttr.GenerateXmlAttributes(writer);
        }
    }
}