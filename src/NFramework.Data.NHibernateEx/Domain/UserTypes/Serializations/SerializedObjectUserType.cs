using System;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NSoft.NFramework.Serializations;
using NSoft.NFramework.Serializations.SerializedObjects;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// 객체를 직렬화하여 저장할 때, 직렬화 방법, 원본 객체 수형, 직렬화 결과를 하나의 수형으로 제공하도록 합니다.<br/>
    /// <see cref="ISerializedObject"/>를 구현한 Binary, Json, Bson, Xml, Soap 등의 직렬화 방법을 제공합니다.\
    /// 직렬화 방법을 같이 저장하기 때문에, 역직렬화를 그 방법으로 수행하면 됩니다.
    /// 
    /// 실제 원본 객체를 얻으로고 한다면, <seealso cref="ISerializedObject.GetDeserializedObject"/>를 수행하면 됩니다.
    /// </summary>
    /// <example>
    /// <code>
    ///		// Hbm
    /// 
    ///		<class name="className">
    ///			...
    /// 
    ///		    <property name="SerializedObject" type="NSoft.NFramework.Data.Domain.SerializedObjectUserType, NSoft.NFramework.Data">
    ///				<column name="SERIALIZATION_METHOD"/>
    ///				<column nmae="OBJECT_TYPE_NAME" />
    ///				<column name="SERIALIZED_VALUE" length="9999" />
    ///			</property>
    /// 
    ///		</class>
    /// 
    ///		// Fluent 
    /// 	Map(x => x.SerializedObject)
    /// 		.CustomType{SerializedObjectUserType}()
    /// 		.Columns.Clear()
    /// 		.Columns.Add("SERIALIZATION_METHOD")
    /// 		.Columns.Add("OBJECT_TYPE_NAME")
    /// 		.Columns.Add(new ColumnMapping {Name = "SERIALIZED_VALUE", Length = 9999});
    /// 
    /// </code>
    /// </example>
    /// <seealso cref="SerializationMethod"/>
    /// <seealso cref="BinarySerializedObject"/>
    /// <seealso cref="BsonSerializedObject"/>
    /// <seealso cref="JsonSerializedObject"/>
    /// <seealso cref="XmlSerializedObject"/>
    /// <seealso cref="SoapSerializedObject"/>
    public class SerializedObjectUserType : ICompositeUserType {
        public static ISerializedObject AsSerializedObject(object value) {
            var obj = value as ISerializedObject;
            Guard.Assert(obj != null, "Expected Type=[{0}], but Value Type = [{1}]", typeof(ISerializedObject), value.GetType());
            return obj;
        }

        #region << ICompositeUserType >>

        /// <summary>
        /// Get the value of a property
        /// </summary>
        /// <param name="component">an instance of class mapped by this "type"</param><param name="property"/>
        /// <returns>
        /// the property value
        /// </returns>
        public object GetPropertyValue(object component, int property) {
            if(component == null)
                return null;

            var serializedObject = AsSerializedObject(component);

            switch(property) {
                case 0:
                    return serializedObject.Method;
                case 1:
                    return serializedObject.ObjectTypeName;
                case 2:
                    return serializedObject.SerializedValue;
                default:
                    throw new InvalidOperationException("복합 수형의 속성 인덱스가 범위를 벗어났습니다. 0, 1, 2만 가능합니다. index of property=" + property);
            }
        }

        /// <summary>
        /// Set the value of a property
        /// </summary>
        /// <param name="component">an instance of class mapped by this "type"</param><param name="property"/><param name="value">the value to set</param>
        public void SetPropertyValue(object component, int property, object value) {
            component.ShouldNotBeNull("component");

            var serializedObject = AsSerializedObject(component);

            switch(property) {
                case 0:
                    serializedObject.Method = value.AsEnum(SerializationMethod.None);
                    break;
                case 1:
                    serializedObject.ObjectTypeName = value.AsText("System.Object");
                    break;
                case 2:
                    serializedObject.SerializedValue = (byte[])value;
                    break;

                default:
                    throw new InvalidOperationException("복합 수형의 속성 인덱스가 범위를 벗어났습니다. 0, 1, 2만 가능합니다. index of property=" + property);
            }
        }

        /// <summary>
        /// Compare two instances of the class mapped by this type for persistence
        ///             "equality", ie. equality of persistent state.
        /// </summary>
        /// <param name="x"/><param name="y"/>
        /// <returns/>
        public new bool Equals(object x, object y) {
            if(ReferenceEquals(x, y))
                return true;

            if(x == null || y == null)
                return false;

            //! 이 코드를 Equals(x, y)로 변경하면 무한루프에 빠집니다!!!
            return x.Equals(y);
        }

        /// <summary>
        /// Get a hashcode for the instance, consistent with persistence "equality"
        /// </summary>
        public int GetHashCode(object x) {
            return (x == null) ? typeof(ISerializedObject).GetHashCode() : x.GetHashCode();
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a IDataReader. Implementors
        ///             should handle possibility of null values.
        /// </summary>
        /// <param name="dr">IDataReader</param><param name="names">the column names</param><param name="session"/><param name="owner">the containing entity</param>
        /// <returns/>
        public virtual object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner) {
            var method = NHibernateUtil.String.NullSafeGet(dr, names[0]).AsEnum(SerializationMethod.Binary);
            var typename = NHibernateUtil.String.NullSafeGet(dr, names[1]).AsText("System.Object");
            var value = NHibernateUtil.Binary.NullSafeGet(dr, names[2]);

            return SerializedObjectTool.Create(method, typename, (byte[])value);
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        ///             Implementors should handle possibility of null values.
        ///             A multi-column type should be written to parameters starting from index.
        ///             If a property is not settable, skip it and don't increment the index.
        /// </summary>
        /// <param name="cmd"/><param name="value"/><param name="index"/><param name="settable"/><param name="session"/>
        public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session) {
            NullSafeSet(cmd, value, index, session);
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// If a property is not settable, skip it and don't increment the index.
        /// </summary>
        public void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) {
            if(value == null) {
                NHibernateUtil.String.NullSafeSet(cmd, null, index);
                NHibernateUtil.String.NullSafeSet(cmd, null, index + 1);
                NHibernateUtil.Binary.NullSafeSet(cmd, null, index + 2);
            }
            else {
                var serializedObject = AsSerializedObject(value);
                NHibernateUtil.String.NullSafeSet(cmd, serializedObject.Method.ToString(), index);
                NHibernateUtil.String.NullSafeSet(cmd, serializedObject.ObjectTypeName, index + 1);
                NHibernateUtil.Binary.NullSafeSet(cmd, serializedObject.SerializedValue, index + 2);
            }
        }

        /// <summary>
        /// Return a deep copy of the persistent state, stopping at entities and at collections.
        /// </summary>
        /// <param name="value">generally a collection element or entity field</param>
        /// <returns/>
        public object DeepCopy(object value) {
            if(value == null)
                return null;

            var original = AsSerializedObject(value);
            return original.DeepCopy();
        }

        /// <summary>
        /// Transform the object into its cacheable representation.
        /// At the very least this method should perform a deep copy.
        /// That may not be enough for some implementations, method should perform a deep copy. That may not be enough for some implementations, however; for example, associations must be cached as identifier values. (optional operation)
        /// </summary>
        /// <param name="value">the object to be cached</param><param name="session"/>
        /// <returns/>
        public object Disassemble(object value, ISessionImplementor session) {
            return DeepCopy(value);
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation.
        /// At the very least this method should perform a deep copy. (optional operation)
        /// </summary>
        /// <param name="cached">the object to be cached</param><param name="session"/><param name="owner"/>
        /// <returns/>
        public object Assemble(object cached, ISessionImplementor session, object owner) {
            return DeepCopy(cached);
        }

        /// <summary>
        /// During merge, replace the existing (target) value in the entity we are merging to
        /// with a new (original) value from the detached entity we are merging. For immutable
        /// objects, or null values, it is safe to simply return the first parameter. For
        /// mutable objects, it is safe to return a copy of the first parameter. However, since
        /// composite user types often define component values, it might make sense to recursively
        /// replace component values in the target object.
        /// </summary>
        public object Replace(object original, object target, ISessionImplementor session, object owner) {
            return DeepCopy(original);
        }

        /// <summary>
        /// Get the "property names" that may be used in a query.
        /// </summary>
        public string[] PropertyNames {
            get { return new[] { "Method", "ObjectTypeName", "SerializedValue" }; }
        }

        /// <summary>
        /// Get the corresponding "property types"
        /// </summary>
        public IType[] PropertyTypes {
            get { return new IType[] { NHibernateUtil.String, NHibernateUtil.String, NHibernateUtil.Binary }; }
        }

        /// <summary>
        /// The class returned by NullSafeGet().
        /// </summary>
        public Type ReturnedClass {
            get { return typeof(ISerializedObject); }
        }

        /// <summary>
        /// Are objects of this type mutable?
        /// </summary>
        public bool IsMutable {
            get { return true; }
        }

        #endregion
    }
}