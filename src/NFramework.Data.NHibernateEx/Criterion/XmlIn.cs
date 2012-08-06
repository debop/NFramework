using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.UserTypes;
using NHibernate.Util;

#pragma warning disable 1591

namespace NSoft.NFramework.Data.NHibernateEx.Criterion {
    /// <summary>
    /// XmlIn
    /// </summary>
    public sealed class XmlIn : AbstractCriterion {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private readonly AbstractCriterion _expr;
        private readonly string _propertyName;
        private readonly object[] _values;
        private readonly int _maxParametersToNotUseXml = 100;

        #region << Constructors >>

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        public XmlIn(string propertyName, IEnumerable values) {
            _propertyName = propertyName;

            var list = new ArrayList();
            foreach(object val in values)
                list.Add(val);
            _values = list.ToArray();

            _expr = Restrictions.In(propertyName, list);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="values"></param>
        /// <param name="maxParametersToNotUseXml"></param>
        public XmlIn(string propertyName, IEnumerable values, int maxParametersToNotUseXml)
            : this(propertyName, values) {
            _maxParametersToNotUseXml = maxParametersToNotUseXml;
        }

        #endregion

        #region << Static Methods >>

        /// <summary>
        /// Create criterion
        /// </summary>
        public static AbstractCriterion Create(string property, IEnumerable values) {
            return new XmlIn(property, values);
        }

        /// <summary>
        /// Create criterion
        /// </summary>
        public static AbstractCriterion Create(string property, IEnumerable values, int maxParametersToNotUseXml) {
            return new XmlIn(property, values, maxParametersToNotUseXml);
        }

        #endregion

        #region << Overrides >>

        public override IProjection[] GetProjections() {
            return null;
        }

        /// <summary>
        /// Return array of TypedValue 
        /// </summary>
        public override TypedValue[] GetTypedValues(ICriteria criteria, ICriteriaQuery criteriaQuery) {
            //we only need this for SQL Server, and or large amount of values
            if((criteriaQuery.Factory.Dialect is MsSql2005Dialect) == false || _values.Length < _maxParametersToNotUseXml) {
                return _expr.GetTypedValues(criteria, criteriaQuery);
            }

            IEntityPersister persister = null;
            var type = criteriaQuery.GetTypeUsingProjection(criteria, _propertyName);

            if(type.IsEntityType) {
                persister = criteriaQuery.Factory.GetEntityPersister(type.ReturnedClass.FullName);
            }
            var sw = new StringWriter();
            var writer = XmlWriter.Create(sw);
            writer.WriteStartElement("items");
            foreach(object value in _values) {
                if(value == null)
                    continue;
                object valToWrite;
                if(persister != null)
                    valToWrite = persister.GetIdentifier(value, EntityMode.Poco);
                else
                    valToWrite = value;
                writer.WriteElementString("val", valToWrite.ToString());
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            string xmlString = sw.GetStringBuilder().ToString();

            return new[]
                   {
                       new TypedValue(new CustomType(typeof(XmlType),
                                                     new Dictionary<string, string>()), xmlString, EntityMode.Poco),
                   };
        }

        /// <summary>
        /// Return sql string
        /// </summary>
        public override SqlString ToSqlString(ICriteria criteria,
                                              ICriteriaQuery criteriaQuery, IDictionary<string, IFilter> enabledFilters) {
            // we only need this for SQL Server, and or large amount of values
            if((criteriaQuery.Factory.Dialect is MsSql2005Dialect) == false || _values.Length < _maxParametersToNotUseXml)
                return _expr.ToSqlString(criteria, criteriaQuery, enabledFilters);

            var type = criteriaQuery.GetTypeUsingProjection(criteria, _propertyName);
            if(type.IsCollectionType)
                throw new QueryException("Cannot use collections with InExpression");

            if(_values.Length == 0)
                return new SqlString("1=0"); // " somthing in ()" is always false

            var result = new SqlStringBuilder();
            var columnNames = criteriaQuery.GetColumnsUsingProjection(criteria, _propertyName);

            // Generate SqlString of the form:
            // columnName1 in (xml query) and columnName2 in (xml query) and ...

            for(int columnIndex = 0; columnIndex < columnNames.Length; columnIndex++) {
                string columnName = columnNames[columnIndex];

                if(columnIndex > 0) {
                    result.Add(" and ");
                }
                var sqlType = type.SqlTypes(criteriaQuery.Factory)[columnIndex];
                result
                    .Add(columnName)
                    .Add(" in (")
                    .Add("SELECT ParamValues.Val.value('.','")
                    .Add(criteriaQuery.Factory.Dialect.GetTypeName(sqlType))
                    .Add("') FROM ")
                    .AddParameter()
                    .Add(".nodes('/items/val') as ParamValues(Val)")
                    .Add(")");
            }

            return result.ToSqlString();
        }

        ///<summary>
        /// Return sql string with current values.
        ///</summary>
        ///<returns></returns>
        public override string ToString() {
            return _propertyName + " big in (" + StringHelper.ToString(_values) + ')';
        }

        #endregion

        #region << XmlType >>

        private class XmlType : IUserType {
            public SqlType[] SqlTypes {
                get { return new[] { new SqlType(DbType.Xml) }; }
            }

            public Type ReturnedType {
                get { return typeof(string); }
            }

            public new bool Equals(object x, object y) {
                return Object.Equals(x, y);
            }

            public int GetHashCode(object x) {
                return x.GetHashCode();
            }

            public object NullSafeGet(IDataReader rs, string[] names, object owner) {
                return null;
            }

            public void NullSafeSet(IDbCommand cmd, object value, int index) {
                var parameter = (IDataParameter)cmd.Parameters[index];
                parameter.Value = value;
            }

            public object DeepCopy(object value) {
                return value;
            }

            public bool IsMutable {
                get { return true; }
            }

            public object Replace(object original, object target, object owner) {
                return original;
            }

            public object Assemble(object cached, object owner) {
                return cached;
            }

            public object Disassemble(object value) {
                return value;
            }
        }

        #endregion
    }
}

#pragma warning restore 1591