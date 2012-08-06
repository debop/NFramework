using System.Data;
using System.Data.SqlClient;
using NUnit.Framework;
using SharpTestsEx;

namespace NSoft.NFramework.Data.Ado.SqlServer {
    [TestFixture]
    public class SqlCommandSetFixture : AdoFixtureBase {
        [Test]
        public void InstancingTest() {
            var commandSet = new NSoft.NFramework.Data.SqlServer.SqlCommandSet();
            SharpTestsEx.Extensions.Should((object)commandSet).Not.Be.Null();
        }

        [Test]
        public void Batch_ExecuteNonQuery() {
            const int BatchSize = 30;
            const string commandText = @"DELETE FROM dbo.Region where RegionID > @RegionId";

            var commandSet = new NSoft.NFramework.Data.SqlServer.SqlCommandSet
                             {
                                 Connection = (SqlConnection)AdoTool.CreateTransactionScopeConnection(AdoRepository.Db)
                             };

            commandSet.Append((SqlCommand)AdoRepository.GetSqlStringCommand(SQL_REGION_INSERT));
            commandSet.Append((SqlCommand)AdoRepository.GetSqlStringCommand(SQL_REGION_INSERT2));

            for(int i = 0; i < BatchSize; i++) {
                var command = AdoRepository.GetSqlStringCommand(commandText, new AdoParameter("RegionId", i + 100, DbType.Int32));
                commandSet.Append((SqlCommand)command);
            }

            var executedCount = commandSet.ExecuteNonQuery();

            // 새로 추가한 위의 2개가 영향을 받습니다.
            executedCount.Should().Be(4);
        }
    }
}