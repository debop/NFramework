using System.Data;
using System.Data.Common;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// Represents named parameter used <see cref="DbCommand"/>
    /// </summary>
    public sealed class AdoParameter : NamedParameterBase, IAdoParameter {
        #region << Constructors >>

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        public AdoParameter(string name, object value)
            : this(name, value, DbFunc.GetDbType((value != null) ? value.GetType() : typeof(object))) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="dbType">parameter type</param>
        public AdoParameter(string name, object value, DbType dbType)
            : base(name, value) {
            SourceVersion = DataRowVersion.Default;
            Direction = ParameterDirection.Input;
            ValueType = dbType;
            SourceColumn = name.RemoveParameterPrefix();

            if(dbType == DbType.AnsiString || dbType == DbType.AnsiStringFixedLength)
                _size = 8000;

            if(dbType == DbType.String || dbType == DbType.StringFixedLength)
                _size = 4000;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="dbType">parameter type</param>
        public AdoParameter(string name, DbType dbType) : this(name, null, dbType) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="dbType">parameter type</param>
        /// <param name="sourceColumn">source column</param>
        public AdoParameter(string name, DbType dbType, string sourceColumn)
            : this(name, null, dbType) {
            SourceColumn = sourceColumn;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="dbType">parameter type</param>
        /// <param name="sourceColumn">source column</param>
        /// <param name="sourceVersion">source version</param>
        public AdoParameter(string name, DbType dbType, string sourceColumn, DataRowVersion sourceVersion)
            : this(name, null, dbType) {
            SourceColumn = sourceColumn;
            SourceVersion = sourceVersion;
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="dbType">parameter type</param>
        /// <param name="size">size of parameter</param>
        public AdoParameter(string name, object value, DbType dbType, int size)
            : this(name, value, dbType) {
            _size = size;
            Direction = ParameterDirection.Input;
            SourceColumn = name.RemoveParameterPrefix();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="dbType">parameter type</param>
        /// <param name="direction">parameter direction</param>
        public AdoParameter(string name, object value, DbType dbType, ParameterDirection direction)
            : this(name, value, dbType, 0, direction) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="dbType">parameter type</param>
        /// <param name="size">size of parameter value</param>
        /// <param name="direction">parameter direction</param>
        public AdoParameter(string name, object value, DbType dbType, int size, ParameterDirection direction)
            : this(name, value, dbType, size) {
            Direction = direction;
            SourceColumn = name.RemoveParameterPrefix();
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="dbType">parameter type</param>
        /// <param name="direction">parameter direction</param>
        /// <param name="sourceColumn">source column name</param>
        /// <param name="sourceVersion">source version</param>
        public AdoParameter(string name, object value, DbType dbType, ParameterDirection direction, string sourceColumn,
                            DataRowVersion sourceVersion)
            : this(name, value, dbType, 0, direction, sourceColumn, sourceVersion) {}

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="name">parameter name</param>
        /// <param name="value">parameter value</param>
        /// <param name="dbType">parameter type</param>
        /// <param name="size">size of parameter value</param>
        /// <param name="direction">parameter direction</param>
        /// <param name="sourceColumn">source column name</param>
        /// <param name="sourceVersion">source version</param>
        public AdoParameter(string name, object value, DbType dbType, int size, ParameterDirection direction, string sourceColumn,
                            DataRowVersion sourceVersion)
            : this(name, value, dbType, size, direction) {
            SourceColumn = sourceColumn;
            SourceVersion = sourceVersion;
        }

        #endregion

        private int? _size;

        /// <summary>
        ///  Parameter 값의 Database 수형을 나타냅니다.
        /// </summary>
        public DbType ValueType { get; set; }

        /// <summary>
        /// DataSet, DataTable에 매핑되어 Parameter 값을 반환하거나 로드하기위한 컬럼의 명 
        /// DbDataAdapter를 이용하여 Update/Insert/Delete 시에 DataTable column 명과 Parameter를 매핑시키기 위해 필요하다.
        /// </summary>
        public string SourceColumn { get; set; }

        /// <summary>
        /// DataRow의 버전을 표현합니다.
        /// </summary>
        public DataRowVersion SourceVersion { get; set; }

        /// <summary>
        /// Parameter Data Size
        /// </summary>
        public int? Size {
            get { return _size ?? (_size = 0); }
            set { _size = value; }
        }

        /// <summary>
        /// Parameter Direction (Input, Output, InputOutput, ReturnValue)
        /// </summary>
        public ParameterDirection Direction { get; set; }

        public override int GetHashCode() {
            return HashTool.Compute(base.GetHashCode(), Direction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return string.Format("{0}, ValueType=[{1}], Size=[{2}], Direction=[{3}]", base.ToString(), ValueType, Size, Direction);
        }
    }
}