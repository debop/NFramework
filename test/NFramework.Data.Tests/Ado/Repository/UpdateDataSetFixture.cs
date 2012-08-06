using System.Data;
using System.Data.Common;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Repositories {
    [TestFixture]
    public class UpdateDataSetFixture : AdoFixtureBase {
        #region << SetUp >>

        [TestFixtureSetUp]
        public override void ClassSetUp() {
            base.ClassSetUp();
            DeleteStoredProcedures();
            CreateStoredProcedures();

            CreateCommands();
        }

        [SetUp]
        public void SetUp() {
            AdoAdoImpl.ExecuteNonQueryBySqlString(SQL_REGION_DELETE);
            AddTestData();

            _startingData = AdoAdoImpl.ExecuteDataSetBySqlString(SQL_REGION_SELECT);

            Assert.IsNotNull(_startingData);
            Assert.AreEqual(1, _startingData.Tables.Count);

            _startingData.Tables[0].TableName = @"Region";
        }

        [TestFixtureTearDown]
        public override void ClassCleanUp() {
            DeleteStoredProcedures();
            AdoAdoImpl.ExecuteNonQueryBySqlString(SQL_REGION_DELETE);

            base.ClassCleanUp();
        }

        #endregion

        private static readonly object _syncLock = new object();

        private static IAdoRepository _adoImpl;

        public static IAdoRepository AdoAdoImpl {
            get {
                if(_adoImpl == null)
                    lock(_syncLock)
                        if(_adoImpl == null) {
                            var adoImpl = AdoRepositoryFactory.Instance.CreateRepository();
                            System.Threading.Thread.MemoryBarrier();
                            _adoImpl = adoImpl;
                        }

                return _adoImpl;
            }
        }

        #region << Update DataSet >>

        private DbCommand _insertCommand;
        private DbCommand _updateCommand;
        private DbCommand _deleteCommand;
        private DataSet _startingData;

        public static void CreateCommandDynamically(ref DbCommand insertCommand,
                                                    ref DbCommand updateCommand,
                                                    ref DbCommand deleteCommand) {
            insertCommand = AdoAdoImpl.GetProcedureCommandWithSourceColumn("RegionInsert", "RegionID", "RegionDescription");
            updateCommand = AdoAdoImpl.GetProcedureCommandWithSourceColumn("RegionUpdate", "RegionID", "RegionDescription");
            deleteCommand = AdoAdoImpl.GetProcedureCommandWithSourceColumn("RegionDelete", "RegionID");
        }

        public static void CreateCommands(ref DbCommand insertCommand, ref DbCommand updateCommand,
                                          ref DbCommand deleteCommand) {
            insertCommand = AdoAdoImpl.GetProcedureCommand("RegionInsert",
                                                           false,
                                                           new AdoParameter("RegionID", DbType.Int32),
                                                           new AdoParameter("RegionDescription", DbType.String));

            updateCommand = AdoAdoImpl.GetProcedureCommand("RegionUpdate",
                                                           false,
                                                           new AdoParameter("RegionID", DbType.Int32, "RegionID"),
                                                           new AdoParameter("RegionDescription", DbType.String, "RegionDescription"));
            deleteCommand = AdoAdoImpl.GetProcedureCommand("RegionDelete",
                                                           false,
                                                           new AdoParameter("RegionID", DbType.Int32, "RegionID"));
        }

        public static void AddTestData() {
            var queryBuilder = new StringBuilder()
                .Append("insert into Region values (200, 'Midwest');")
                .Append("insert into Region values (201, 'Central Europe');")
                .Append("insert into Region values (202, 'Middle East');")
                .Append("insert into Region values (203, 'Australia')");

            AdoAdoImpl.ExecuteNonQueryBySqlString(queryBuilder.ToString());
        }

        public void CreateCommands() {
            // CreateCommands(ref _insertCommand, ref _updateCommand, ref _deleteCommand);
            CreateCommandDynamically(ref _insertCommand, ref _updateCommand, ref _deleteCommand);
        }

        [Test]
        public void DeleteRow_ContinueBehavior() {
            var startingCount = _startingData.Tables[0].Rows.Count;

            _startingData.Tables[0].Rows[5].Delete();
            _startingData.Tables[0].Rows[6][1] = "바보야";

            var updated = AdoAdoImpl.UpdateDataSet(_startingData, null, _insertCommand, _updateCommand, _deleteCommand,
                                                   UpdateBehavior.Continue);

            Assert.AreEqual(2, updated);
            var resultTable = AdoAdoImpl.ExecuteDataTableBySqlString(SQL_REGION_SELECT);
            Assert.AreEqual(startingCount - 1, resultTable.Rows.Count);
            Assert.AreEqual(startingCount - 1, TotalCount());
        }

        [Test]
        public void UpdateRow_With_TransactionBehavior() {
            var startingCount = _startingData.Tables[0].Rows.Count;

            _startingData.Tables[0].Rows[5].Delete();
            _startingData.Tables[0].Rows[6][1] = "바보야";


            var updated = AdoAdoImpl.UpdateDataSet(_startingData, null, _insertCommand, _updateCommand, _deleteCommand,
                                                   UpdateBehavior.Transactional);

            Assert.AreEqual(2, updated);

            var resultTable = AdoAdoImpl.ExecuteDataTableBySqlString(SQL_REGION_SELECT);
            Assert.AreEqual(startingCount - 1, resultTable.Rows.Count);
            Assert.AreEqual(startingCount - 1, TotalCount());
        }

        #endregion
    }
}