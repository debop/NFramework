using System;
using System.Xml;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.FusionCharts {
    /// <summary>
    /// DateTime을 나타내는 LineElement입니다.
    /// </summary>
    [Serializable]
    public class DateTimeLineElement : LineElement {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DateTimeLineElement() {}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="start">시작 일자</param>
        /// <param name="end">완료 일자</param>
        public DateTimeLineElement(DateTime start, DateTime end) {
            Start = start;
            End = end;
        }

        /// <summary>
        /// 시작 일자
        /// </summary>
        public DateTime? Start { get; set; }

        /// <summary>
        /// 완료 일자
        /// </summary>
        public DateTime? End { get; set; }

        /// <summary>
        /// Chart 설정 또는 변량에 대해 XML로 생성합니다.
        /// </summary>
        /// <param name="writer"></param>
        public override void GenerateXmlAttributes(XmlWriter writer) {
            base.GenerateXmlAttributes(writer);

            if(Start.HasValue)
                writer.WriteAttributeString("Start", Start.Value.ToSortableString(true));

            if(End.HasValue)
                writer.WriteAttributeString("End", End.Value.ToSortableString(true));
        }
    }
}