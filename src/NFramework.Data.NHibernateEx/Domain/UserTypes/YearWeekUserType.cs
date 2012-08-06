using System;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// NHibernate용 <see cref="YearWeek"/>의 User Type입니다.
    /// </summary>
    /// <example>
    /// <code>
    ///		// Hbm
    ///		<class name="xxx">
    ///			...
    /// 
    ///			<property name="YearWeek" type="NSoft.NFramework.Data.Domain.UserTypes.YearWeekUserType, NSoft.NFramework.Data.NHibernateEx">
    ///				<column name="YEAR_NO" />
    ///				<column name="WEEK_NO" />
    ///			</property>
    /// 
    ///         ...
    ///		</class>
    /// </code>
    /// </example>
    /// <seealso cref="YearWeek"/>
    [Obsolete("Use NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes.YearAndWeekUserType instead.")]
    [Serializable]
    public class YearWeekUserType : ICompositeUserType {
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

            var yearWeek = AsYearWeek(component);

            switch(property) {
                case 0:
                    return yearWeek.Year;
                case 1:
                    return yearWeek.Week;
                default:
                    throw new InvalidOperationException("Not support property index of " + property);
            }
        }

        /// <summary>
        /// Set the value of a property
        /// </summary>
        /// <param name="component">an instance of class mapped by this "type"</param><param name="property"/><param name="value">the value to set</param>
        public void SetPropertyValue(object component, int property, object value) {
            component.ShouldNotBeNull("component");

            var yearWeek = AsYearWeek(component);
            switch(property) {
                case 0:
                    yearWeek.Year = (int?)value;
                    break;
                case 1:
                    yearWeek.Week = (int?)value;
                    break;
                default:
                    throw new InvalidOperationException("Not support property index of " + property);
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
            return (x == null) ? typeof(YearWeek).GetHashCode() : x.GetHashCode();
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a IDataReader. Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="dr">IDataReader</param><param name="names">the column names</param><param name="session"/><param name="owner">the containing entity</param>
        /// <returns/>
        public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner) {
            return new YearWeek((int?)NHibernateUtil.Int32.NullSafeGet(dr, names[0]),
                                (int?)NHibernateUtil.Int32.NullSafeGet(dr, names[1]));
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// If a property is not settable, skip it and don't increment the index.
        /// </summary>
        /// <param name="cmd"/><param name="value"/><param name="index"/><param name="settable"/><param name="session"/>
        public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session) {
            NullSafeSet(cmd, value, index, session);
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// </summary>
        /// <param name="cmd"/><param name="value"/><param name="index"/><param name="session"/>
        public void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session) {
            if(value == null) {
                NHibernateUtil.Int32.NullSafeSet(cmd, null, index);
                NHibernateUtil.Int32.NullSafeSet(cmd, null, index + 1);
            }
            else {
                var yearWeek = AsYearWeek(value);
                NHibernateUtil.Int32.NullSafeSet(cmd, yearWeek.Year, index);
                NHibernateUtil.Int32.NullSafeSet(cmd, yearWeek.Week, index + 1);
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

            var original = AsYearWeek(value);
            return new YearWeek(original.Year, original.Week);
        }

        /// <summary>
        /// Transform the object into its cacheable representation.
        ///             At the very least this method should perform a deep copy.
        ///             That may not be enough for some implementations, method should perform a deep copy. That may not be enough for some implementations, however; for example, associations must be cached as identifier values. (optional operation)
        /// </summary>
        /// <param name="value">the object to be cached</param><param name="session"/>
        /// <returns/>
        public object Disassemble(object value, ISessionImplementor session) {
            return DeepCopy(value);
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation.
        ///             At the very least this method should perform a deep copy. (optional operation)
        /// </summary>
        /// <param name="cached">the object to be cached</param><param name="session"/><param name="owner"/>
        /// <returns/>
        public object Assemble(object cached, ISessionImplementor session, object owner) {
            return DeepCopy(cached);
        }

        /// <summary>
        /// During merge, replace the existing (target) value in the entity we are merging to
        ///             with a new (original) value from the detached entity we are merging. For immutable
        ///             objects, or null values, it is safe to simply return the first parameter. For
        ///             mutable objects, it is safe to return a copy of the first parameter. However, since
        ///             composite user types often define component values, it might make sense to recursively 
        ///             replace component values in the target object.
        /// </summary>
        public object Replace(object original, object target, ISessionImplementor session, object owner) {
            return DeepCopy(original);
        }

        /// <summary>
        /// Get the "property names" that may be used in a query. 
        /// </summary>
        public string[] PropertyNames {
            get { return new[] { "Year", "Week" }; }
        }

        /// <summary>
        /// Get the corresponding "property types"
        /// </summary>
        public IType[] PropertyTypes {
            get { return new IType[] { NHibernateUtil.Int32, NHibernateUtil.Int32 }; }
        }

        /// <summary>
        /// The class returned by NullSafeGet().
        /// </summary>
        public Type ReturnedClass {
            get { return typeof(YearWeek); }
        }

        /// <summary>
        /// Are objects of this type mutable?
        /// </summary>
        public bool IsMutable {
            get { return true; }
        }

        #endregion

        private static YearWeek AsYearWeek(object value) {
            if(object.Equals(value, null))
                return new YearWeek();

            return (YearWeek)value;
        }
    }
}