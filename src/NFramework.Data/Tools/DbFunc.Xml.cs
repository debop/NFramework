using System.Data;
using System.IO;
using System.Text;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class DbFunc {
        /// <summary>
        /// DataTable 내용을 Xml Text로 변경한다.
        /// </summary>
        /// <param name="table">Instance of DataTable</param>
        /// <param name="writeMode">Xml Text 생성 모드</param>
        /// <returns>생성된 Xml 문자열</returns>
        public static string ToXmlText(DataTable table, XmlWriteMode writeMode = XmlWriteMode.WriteSchema) {
            table.ShouldNotBeNull("table");

            if(IsDebugEnabled)
                log.Debug("Convert DataTable to Xml Text. table=[{0}], writeMode=[{1}]", table.TableName, writeMode);

            var sb = new StringBuilder();

            using(var writer = new StringWriter(sb))
                table.WriteXml(writer, writeMode);

            return sb.ToString();
        }

        /// <summary>
        /// 지정된 XML 문자열을 읽어, DataSet을 빌드한다.
        /// </summary>
        /// <param name="xmlText">Xml Text</param>
        /// <returns>Instance of DataSet</returns>
        public static DataSet ToDataSet(string xmlText) {
            xmlText.ShouldNotBeWhiteSpace("xmlText");

            if(IsDebugEnabled)
                log.Debug("Xml 문자열로부터 DataSet 인스턴스를 빌드합니다. xmlText=[{0}]", xmlText.EllipsisChar(80));

            var ds = new DataSet();

            if(xmlText.IsNotWhiteSpace()) {
                using(var reader = new StringReader(xmlText))
                    ds.ReadXml(reader);
            }

            return ds;
        }

        /// <summary>
        /// 지정된 XML 문자열을 읽어, DataTable을 빌드한다.
        /// </summary>
        /// <param name="xmlText">Xml Text</param>
        /// <returns>Instance of DataTable</returns>
        public static DataTable ToDataTable(string xmlText) {
            xmlText.ShouldNotBeWhiteSpace("xmlText");

            if(IsDebugEnabled)
                log.Debug("Xml 문자열로부터 DataTable 인스턴스를 빌드합니다. xmlText=[{0}]", xmlText.EllipsisChar(80));

            var dt = new DataTable();

            if(xmlText.IsNotWhiteSpace()) {
                using(var reader = new StringReader(xmlText))
                    dt.ReadXml(reader);
            }

            return dt;
        }
    }
}