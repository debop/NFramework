using System;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NSoft.NFramework.TimePeriods;

namespace NSoft.NFramework.Data.NHibernateEx.Domain.UserTypes {
    /// <summary>
    /// <see cref="TimeBlock"/>에 대한 NHibernate용 UserType입니다. 기간을 나타냅니다.
    /// </summary>
    /// <example>
    /// <code>
    ///		// Hbm
    /// 
    ///		<class name="className">
    ///			...
    /// 
    ///		    <property name="ActiveBlock" type="NSoft.NFramework.Data.Domain.TimeBlockUserType, NSoft.NFramework.Data">
    ///				<column name="ACTIVE_START_TIME"/>
    ///				<column nmae="ACTIVE_DURATION" />
    ///			</property>
    /// 
    ///		</class>
    /// 
    /// 	// Fluent 방식
    /// 	Map(x => x.ActivePeriod)
    /// 		.CustomType{TimeBlockUserType}()
    /// 		.Columns.Clear()
    /// 		.Columns.Add("ACTIVE_FROM_DATE")
    /// 		.Columns.Add("ACTIVE_TO_DATE");
    /// </code>
    /// </example>
    /// <seealso cref="TimeBlock"/>
    [Serializable]
    public sealed class TimeBlockUserType : ICompositeUserType {
        /// <summary>
        /// 지정한 객체를 <see cref="TimeRange"/>로 캐스팅합니다.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TimeBlock AsTimeBlock(object value) {
            var block = value as TimeBlock;
            Guard.Assert(block != null, "Expected=[{0}] type, but input=[{1}]", typeof(TimeRange), value.GetType());
            return block;
        }

        #region Implementation of ICompositeUserType

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

            var block = AsTimeBlock(component);

            switch(property) {
                case 0:
                    return block.Start;
                case 1:
                    return block.Duration;

                default:
                    throw new InvalidOperationException("복합 수형의 속성 인덱스가 범위를 벗어났습니다. 0, 1만 가능합니다. index of property=" + property);
            }
        }

        /// <summary>
        /// Set the value of a property
        /// </summary>
        /// <param name="component">an instance of class mapped by this "type"</param><param name="property"/><param name="value">the value to set</param>
        public void SetPropertyValue(object component, int property, object value) {
            component.ShouldNotBeNull("component");

            var block = AsTimeBlock(component);

            switch(property) {
                case 0:
                    block.Start = value.AsDateTime(TimeSpec.MinPeriodTime);
                    break;
                case 1:
                    block.Duration = value.AsTimeSpan(TimeSpec.MinPeriodDuration);
                    break;

                default:
                    throw new InvalidOperationException("복합 수형의 속성 인덱스가 범위를 벗어났습니다. 0, 1만 가능합니다. index of property=" + property);
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
            return (x == null) ? typeof(TimeBlock).GetHashCode() : x.GetHashCode();
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a IDataReader. Implementors should handle possibility of null values.
        /// </summary>
        /// <param name="dr">IDataReader</param><param name="names">the column names</param><param name="session"/><param name="owner">the containing entity</param>
        /// <returns/>
        public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner) {
            var startTime = (DateTime?)NHibernateUtil.Timestamp.NullSafeGet(dr, names[0]);
            var duration = (TimeSpan?)NHibernateUtil.TimeSpan.NullSafeGet(dr, names[1]);

            return new TimeBlock(startTime.GetValueOrDefault(TimeSpec.MinPeriodTime),
                                 duration.GetValueOrDefault(TimeSpec.MinPeriodDuration));
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement.
        /// Implementors should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// If a property is not settable, skip it and don't increment the index.
        /// </summary>
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
                NHibernateUtil.Timestamp.NullSafeSet(cmd, null, index);
                NHibernateUtil.TimeSpan.NullSafeSet(cmd, null, index + 1);
            }
            else {
                var block = AsTimeBlock(value);
                NHibernateUtil.Timestamp.NullSafeSet(cmd, block.StartAsNullable, index);
                NHibernateUtil.TimeSpan.NullSafeSet(cmd, block.Duration, index + 1);
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

            var original = AsTimeBlock(value);
            return new TimeRange(original, false);
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
            get { return new string[] { "Start", "Duration" }; }
        }

        /// <summary>
        /// Get the corresponding "property types"
        /// </summary>
        public IType[] PropertyTypes {
            get { return new IType[] { NHibernateUtil.Timestamp, NHibernateUtil.TimeSpan }; }
        }

        /// <summary>
        /// The class returned by NullSafeGet().
        /// </summary>
        public Type ReturnedClass {
            get { return typeof(TimeBlock); }
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