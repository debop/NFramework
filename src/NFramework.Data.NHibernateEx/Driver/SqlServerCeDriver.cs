using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.SqlCommand;
using NHibernate.SqlTypes;

namespace NSoft.NFramework.Data.NHibernateEx {
    /// <summary>
    /// SQL Server CE가 Unicode만 지원하여, AnsiString을 지원하지 않는다. 이 때문에, SQL Server와 혼용하는 HBM 작성이 힘들다.
    /// 그래서 SQL Server CE에서 AnsiString을 NVarChar로 매핑하도록 해주는 Driver를 작성하였습니다.
    /// 
    /// 참고 : http://netfrustrations.blogspot.com/2009/10/nhibernate-mapping-typeansistring.html
    /// </summary>
    [Obsolete("AnsiString을 자동으로 String을 매핑하지 못한다.")]
    public class SqlServerCeDriver : NHibernate.Driver.SqlServerCeDriver {
        /// <summary>
        /// Generates an IDbCommand from the SqlString according to the requirements of the DataProvider.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Data.CommandType"/> of the command to generate.</param><param name="sqlString">The SqlString that contains the SQL.</param><param name="parameterTypes">The types of the parameters to generate for the command.</param>
        /// <returns>
        /// An IDbCommand with the CommandText and Parameters fully set.
        /// </returns>
        public override IDbCommand GenerateCommand(CommandType type, SqlString sqlString, SqlType[] parameterTypes) {
            var filteredParameterTypes = GetFilteredParameterTypes(parameterTypes);

            return base.GenerateCommand(type, sqlString, filteredParameterTypes);
        }

        /// <summary>
        /// HBM에서 정의된 AnsiString을 String 수형으로 변환하여 제공합니다.
        /// </summary>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        private static SqlType[] GetFilteredParameterTypes(IEnumerable<SqlType> parameterTypes) {
            var sqlTypes = new List<SqlType>();

            foreach(var type in parameterTypes) {
                // AnsiString 타입을 String 타입으로 변환합니다. (Sql CE는 Unicode만 지원하므로)
                if(type is AnsiStringSqlType)
                    sqlTypes.Add(new StringSqlType(type.Length));
                else
                    sqlTypes.Add(type);
            }

            return sqlTypes.ToArray();
        }
    }
}