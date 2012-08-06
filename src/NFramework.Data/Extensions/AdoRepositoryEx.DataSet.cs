using System;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NSoft.NFramework.Reflections;
using NSoft.NFramework.Tools;

namespace NSoft.NFramework.Data {
    public static partial class AdoRepositoryEx {
        /// <summary>
        /// Command를 실행시켜, 결과 셋을 DataSet에 저장한다.<br/>
        /// 여러 ResultSet을 반환하는 경우 각각의 ResultSet에 대응하는 TableName을 제공해야 합니다.
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="cmd">실행할 Command</param>
        /// <param name="targetDataSet"></param>
        /// <param name="tableNames"></param>
        /// <example>
        /// <code>
        /// // Region 전체 정보 및 갯수를 가져옵니다.
        /// string query = "select * from Region; select count(*) from Region";
        /// 
        /// DataSet ds = new DataSet();
        /// using (DbCommand cmd = Impl.GetSqlStringCommand(query))
        /// {
        /// 	Impl.LoadDataSet(cmd, ds, new string[] { "Region", "CountOfRegion" });
        /// 	Assert.AreEqual(2, ds.Tables.Count);
        /// 	Assert.AreEqual(4, ds.Tables[1].Rows[0][0]);
        /// }
        /// </code>
        /// </example>
        public static void LoadDataSet(this IAdoRepository repository, DbCommand cmd, DataSet targetDataSet, string[] tableNames) {
            cmd.ShouldNotBeNull("cmd");
            tableNames.ShouldNotBeEmpty("tableNames");

            if(IsDebugEnabled)
                log.Debug("DbCommand를 실행하여 DataSet에 로등합니다... cmd.CommandText=[{0}], tableNames=[{1}]",
                          cmd.CommandText, tableNames.CollectionToString());

            for(int i = 0; i < tableNames.Length; i++)
                tableNames[i].ShouldNotBeWhiteSpace("index=" + i);

            if(repository.IsActiveTransaction)
                AdoTool.EnlistCommandToActiveTransaction(repository, cmd);

            var newConnectionCreated = false;

            if(cmd.Connection == null)
                cmd.Connection = AdoTool.CreateTransactionScopeConnection(repository.Db, ref newConnectionCreated);

            using(var adapter = repository.GetDataAdapter()) {
                for(var j = 0; j < tableNames.Length; j++) {
                    var sourceTable = (j == 0) ? AdoTool.DefaultTableName : (AdoTool.DefaultTableName + j);
                    adapter.TableMappings.Add(sourceTable, tableNames[j]);
                }

                adapter.SelectCommand = cmd;
                adapter.Fill(targetDataSet);

                if(newConnectionCreated)
                    AdoTool.ForceCloseConnection(adapter.SelectCommand);
            }
        }

        /// <summary>
        /// Update DataSet with commands
        /// </summary>
        /// <param name="repository">IAdoRepository 인스턴스</param>
        /// <param name="dataSet">DataSet to update</param>
        /// <param name="tableName">table name to update</param>
        /// <param name="insertCommand">command to insert</param>
        /// <param name="updateCommand">command to update</param>
        /// <param name="deleteCommand">command to delete</param>
        /// <param name="updateBehavior">Behavior to update</param>
        /// <param name="updateBatchSize">batch size to update</param>
        /// <returns>affected row count</returns>
        public static int UpdateDataSet(this IAdoRepository repository,
                                        DataSet dataSet,
                                        string tableName,
                                        DbCommand insertCommand = null,
                                        DbCommand updateCommand = null,
                                        DbCommand deleteCommand = null,
                                        UpdateBehavior updateBehavior = UpdateBehavior.Transactional,
                                        int updateBatchSize = Int32.MaxValue) {
            dataSet.ShouldNotBeNull("dataSet");
            Guard.Assert(dataSet.Tables.Count > 0, "지정된 DataSet에 DataTable이 하나라도 있어야합니다.");

            if(tableName.IsWhiteSpace())
                tableName = dataSet.Tables[0].TableName;

            if(tableName.IsWhiteSpace())
                tableName = dataSet.Tables[0].TableName = Guid.NewGuid().ToString();

            if(repository.IsActiveTransaction) {
                if(IsDebugEnabled)
                    log.Debug("DataSet에 대해 Transaction 하에서 배치작업을 실행합니다... " +
                              "dataSet=[{0}], tableName=[{1}], insertCommand=[{2}], updateCommand=[{3}], deleteCommand=[{4}], ActiveTransaction=[{5}], updateBatchSize=[{6}]",
                              dataSet.DataSetName,
                              tableName,
                              insertCommand != null ? insertCommand.CommandText : "NULL",
                              updateCommand != null ? updateCommand.CommandText : "NULL",
                              deleteCommand != null ? deleteCommand.CommandText : "NULL",
                              repository.ActiveTransaction,
                              updateBatchSize);

                return repository.Db.UpdateDataSet(dataSet,
                                                   tableName,
                                                   insertCommand,
                                                   updateCommand,
                                                   deleteCommand,
                                                   repository.ActiveTransaction,
                                                   updateBatchSize);
            }

            if(IsDebugEnabled)
                log.Debug("DataSet에 대해 배치작업을 실행합니다... " +
                          "dataSet=[{0}], tableName=[{1}], insertCommand=[{2}], updateCommand=[{3}], deleteCommand=[{4}], ActiveTransaction=[{5}], updateBatchSize=[{6}]",
                          dataSet.DataSetName,
                          tableName,
                          insertCommand != null ? insertCommand.CommandText : "NULL",
                          updateCommand != null ? updateCommand.CommandText : "NULL",
                          deleteCommand != null ? deleteCommand.CommandText : "NULL",
                          repository.ActiveTransaction,
                          updateBatchSize);

            return repository.Db.UpdateDataSet(dataSet,
                                               tableName,
                                               insertCommand,
                                               updateCommand,
                                               deleteCommand,
                                               updateBehavior,
                                               updateBatchSize);
        }
    }
}