using System;
using System.Data;

namespace NSoft.NFramework.Data {
    /// <summary>
    /// ADO.NET 의 <see cref="IDbCommand"/>의 Parameter 에 사용되는 Named parameter의 interface
    /// </summary>
    public interface IAdoParameter : INamedParameter {
        /// <summary>
        ///  Parameter 값의 수형을 나타냅니다.
        /// </summary>
        DbType ValueType { get; set; }

        /// <summary>
        /// DataSet, DataTable에 매핑되어 Parameter 값을 반환하거나 로드하기위한 컬럼의 명 
        /// DbDataAdapter를 이용하여 Update/Insert/Delete 시에 DataTable column 명과 Parameter를 매핑시키기 위해 필요하다.
        /// </summary>
        String SourceColumn { get; set; }

        /// <summary>
        /// DataRow의 버전을 표현합니다.
        /// </summary>
        DataRowVersion SourceVersion { get; set; }

        /// <summary>
        /// Parameter Data Size
        /// </summary>
        int? Size { get; set; }

        /// <summary>
        /// Parameter Direction (Input | Output | InputOutput | ReturnValue)
        /// </summary>
        ParameterDirection Direction { get; set; }
    }
}