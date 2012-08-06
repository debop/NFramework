using System;
using NHibernate;
using NSoft.NFramework.Json;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// 특정 객체를 JSON 직렬화를 통해 DB에 저장하고, 읽어올 때에는 역직렬화를 통해 객체로 반환합니다.
    /// 겍체의 수형 정보와 직렬화 정보가 저장됩니다.
    /// </summary>
    /// <example>
    /// <code>
    ///		// Hbm
    /// 
    ///		<class name="className">
    ///			...
    /// 
    ///		    <property name="JsonData" type="NSoft.NFramework.Data.Domain.JsonCompressedSerializableUserType, NSoft.NFramework.Data">
    ///				<column nmae="Json_Compressed_Serialized_Data" length="9999" />
    ///			</property>
    /// 
    ///		</class>
    /// 
    ///		// Fluent
    /// 	Map(x=>x.JsonData)
    /// 		.CustomType{JsonCompressedSerializableUserType}()
    /// 		.Columns.Clear()
    /// 		.Columns.Add(new ColumnMapping {Name = "Json_Compressed_Serialized_Data", Length = 9999});
    /// 
    /// </code>
    /// </example>
    /// <seealso cref="JsonTool"/>
    public class JsonCompressedSerializableUserType : IoCCompressStringUserType {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        /// <summary>
        /// 객체를 직렬화합니다.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public byte[] Serialize(object graph) {
            if(graph == null)
                return null;

            // NOTE: TypeName에 AssebmlyName도 같이 써줘야 제대로 Type을 구분합니다.
            //
            var typeName = graph.GetType().ToStringWithAssemblyName();
            var jsonText = JsonTool.SerializeAsText(graph);

            if(IsDebugEnabled)
                log.Debug("Json Serialized type=[{0}], content=[{1}]", typeName, jsonText.EllipsisChar(255));

            var text = string.Concat(typeName, JsonSerializableUserType.Delimiter, jsonText);

            return CompressValue(text);
        }

        /// <summary>
        /// 저장된 문자열을 역직렬화하여 객체로 반환한다.
        /// </summary>
        /// <param name="compressedBytes">압축 저장된 Data</param>
        /// <returns></returns>
        public object Deserialize(byte[] compressedBytes) {
            if(compressedBytes == null || compressedBytes.Length == 0)
                return null;

            try {
                var savedText = DecompressValue(compressedBytes);

                if(savedText.IsWhiteSpace())
                    return null;

                var texts = StringTool.Split(savedText, JsonSerializableUserType.Delimiter);

                if(texts.Length != 2) {
                    if(log.IsWarnEnabled)
                        log.Warn("savedText가 구분자로 구분할 때 [수형] || [JsonText] 형태로 만들어지지 않았습니다. savedText=[{0}]", savedText);

                    return null;
                }

                var typeText = texts[0];
                var jsonText = texts[1];

                if(jsonText.IsWhiteSpace())
                    return null;

                var type = Type.GetType(typeText, true, true);

                if(IsDebugEnabled)
                    log.Debug("Json DeserializeSessionState type=[{0}], content=[{1}]", type.FullName, jsonText.EllipsisChar(255));

                return JsonTool.DeserializeFromText(jsonText, type);
            }
            catch(Exception ex) {
                if(log.IsErrorEnabled) {
                    log.Error("Json 역직렬화에 실패했습니다. null을 반환합니다.");
                    log.Error(ex);
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a JDBC resultset.
        /// Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="rs">a IDataReader</param><param name="names">column names</param><param name="owner">the containing entity</param>
        /// <returns/>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public override object NullSafeGet(System.Data.IDataReader rs, string[] names, object owner) {
            var value = NHibernateUtil.BinaryBlob.NullSafeGet(rs, names[0]);
            return Deserialize(value as byte[]);
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// </summary>
        /// <param name="cmd">a IDbCommand</param><param name="value">the object to write</param><param name="index">command parameter index</param><exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public override void NullSafeSet(System.Data.IDbCommand cmd, object value, int index) {
            NHibernateUtil.BinaryBlob.NullSafeSet(cmd, Serialize(value), index);
        }
    }
}