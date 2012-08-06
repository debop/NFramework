using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
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
    ///		    <property name="JsonData" type="NSoft.NFramework.Data.Domain.JsonSerializableUserType, NSoft.NFramework.Data">
    ///				<column nmae="Json_Serialized_Data" />
    ///			</property>
    /// 
    ///		</class>
    /// 
    ///		// Fluent
    ///		Map(x=>x.JsonData)
    ///			.CustomType{JsonSerializableUserType}()
    ///			.Columns.Clear()
    ///			.Columns.Add(new ColumnMapping {Name = "Json_Serialized_Data", Length = 9999});
    /// 
    /// </code>
    /// </example>
    /// <seealso cref="JsonUtil"/>
    [Serializable]
    public class JsonSerializableUserType : IUserType {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        public const string Delimiter = "@@";

        /// <summary>
        /// 객체를 직렬화합니다.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        public static string Serialize(object graph) {
            if(graph == null)
                return null;

            // NOTE: TypeName에 AssebmlyName도 같이 써줘야 제대로 Type을 구분합니다.
            //
            var typeName = graph.GetType().ToStringWithAssemblyName();
            var jsonText = JsonTool.SerializeAsText(graph);

            if(IsDebugEnabled)
                log.Debug("Json Serialized type=[{0}], content=[{1}]", typeName, jsonText.EllipsisChar(255));

            return string.Concat(typeName, Delimiter, jsonText);
        }

        /// <summary>
        /// 저장된 문자열을 역직렬화하여 객체로 반환한다.
        /// </summary>
        /// <param name="savedText"></param>
        /// <returns></returns>
        public static object Deserialize(string savedText) {
            if(savedText.IsWhiteSpace())
                return null;

            var texts = StringTool.Split(savedText, Delimiter);

            if(texts.Length != 2) {
                if(log.IsWarnEnabled)
                    log.Warn("savedText가 구분자로 구분할 때 [수형] || [JsonText] 형태로 만들어지지 않았습니다. savedText=[{0}]", savedText);

                return null;
            }

            try {
                var typeText = texts[0];
                var jsonText = texts[1];

                if(jsonText.IsWhiteSpace())
                    return null;

                var type = Type.GetType(typeText, true, true);

                if(IsDebugEnabled)
                    log.Debug("Json DeserializeSessionState type=[{0}], content=[{1}]", type.AssemblyQualifiedName,
                              jsonText.EllipsisChar(255));

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

        #region Implementation of IUserType

        /// <summary>
        /// Compare two instances of the class mapped by this type for persistent "equality" ie. equality of persistent state
        /// </summary>
        /// <param name="x"/><param name="y"/>
        /// <returns/>
        public new bool Equals(object x, object y) {
            if(ReferenceEquals(x, y))
                return true;

            if(x == null || y == null)
                return false;

            return x.Equals(y);
        }

        /// <summary>
        /// Get a hashcode for the instance, consistent with persistence "equality"
        /// </summary>
        public virtual int GetHashCode(object x) {
            return (x == null) ? GetType().FullName.GetHashCode() : x.GetHashCode();
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a JDBC resultset.
        /// Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="rs">a IDataReader</param><param name="names">column names</param><param name="owner">the containing entity</param>
        /// <returns/>
        /// <exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public object NullSafeGet(IDataReader rs, string[] names, object owner) {
            var value = NHibernateUtil.String.NullSafeGet(rs, names[0]);

            return Deserialize(value as string);
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// </summary>
        /// <param name="cmd">a IDbCommand</param><param name="value">the object to write</param><param name="index">command parameter index</param><exception cref="T:NHibernate.HibernateException">HibernateException</exception>
        public void NullSafeSet(IDbCommand cmd, object value, int index) {
            NHibernateUtil.String.NullSafeSet(cmd, Serialize(value), index);
        }

        /// <summary>
        /// Return a deep copy of the persistent state, stopping at entities and at collections.
        /// </summary>
        /// <param name="value">generally a collection element or entity field</param>
        /// <returns>
        /// a copy
        /// </returns>
        public object DeepCopy(object value) {
            return value;
        }

        /// <summary>
        /// During merge, replace the existing (<paramref name="target"/>) value in the entity
        /// we are merging to with a new (<paramref name="original"/>) value from the detached
        /// entity we are merging. For immutable objects, or null values, it is safe to simply
        /// return the first parameter. For mutable objects, it is safe to return a copy of the
        /// first parameter. For objects with component values, it might make sense to
        /// recursively replace component values.
        /// </summary>
        /// <param name="original">the value from the detached entity being merged</param><param name="target">the value in the managed entity</param><param name="owner">the managed entity</param>
        /// <returns>
        /// the value to be merged
        /// </returns>
        public object Replace(object original, object target, object owner) {
            return DeepCopy(original);
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation. At the very least this
        /// method should perform a deep copy if the type is mutable. (optional operation)
        /// </summary>
        /// <param name="cached">the object to be cached</param><param name="owner">the owner of the cached object</param>
        /// <returns>
        /// a reconstructed object from the cachable representation
        /// </returns>
        public object Assemble(object cached, object owner) {
            return DeepCopy(cached);
        }

        /// <summary>
        /// Transform the object into its cacheable representation. At the very least this
        /// method should perform a deep copy if the type is mutable. That may not be enough
        /// for some implementations, however; for example, associations must be cached as
        /// identifier values. (optional operation)
        /// </summary>
        /// <param name="value">the object to be cached</param>
        /// <returns>
        /// a cacheable representation of the object
        /// </returns>
        public object Disassemble(object value) {
            return DeepCopy(value);
        }

        /// <summary>
        /// The SQL types for the columns mapped by this type. 
        /// </summary>
        public SqlType[] SqlTypes {
            get { return new[] { NHibernateUtil.String.SqlType }; }
        }

        /// <summary>
        /// The type returned by <c>NullSafeGet()</c>
        /// </summary>
        public Type ReturnedType {
            get { return typeof(object); }
        }

        /// <summary>
        /// Are objects of this type mutable?
        /// </summary>
        public bool IsMutable {
            get { return NHibernateUtil.String.IsMutable; }
        }

        #endregion
    }
}