/*

using NUnit.Framework;

namespace NSoft.NFramework.Data.Ado.Repository
{
	[TestFixture]
	public class PMSPagingTestFixture
	{
		private static IAdoRepository _pmsRepository;

		public static IAdoRepository PmsRepository
		{
			get { return _pmsRepository ?? (_pmsRepository = AdoRepositoryFactory.Instance.CreateRepository("RealPMS_DEV02")); }
		}

		
		[TestCase(0, 4)]
		[TestCase(1, 4)]
		[TestCase(2, 4)]
		[TestCase(3, 4)]
		//[TestCase(4, 4)]
		//[TestCase(5, 4)]
		//[TestCase(6, 4)]
		//[TestCase(7, 4)]
		//[TestCase(8, 4)]
		//[TestCase(9, 4)]
		//[TestCase(10, 4)]
		//[TestCase(11, 4)]
		//[TestCase(12, 4)]
		//[TestCase(13, 4)]
		public void MyWorkListPagingTest(int pageIndex, int pageSize)
		{
			var parameters = new AdoParameter[] {new AdoParameter("Language", "ko"), new AdoParameter("UserId", "00921")};

			const string sql =
				@"
DECLARE @LANG VARCHAR(50)
SET @LANG = RealAdmin101.dbo.GetLanguage(@Language)
SET LANGUAGE @LANG
SELECT  up.ProjectID
	  , up.ProjectName
	  , up.ProjectCategory
	  , up.ProjectTypeName
	  , TaskID
	  , DataID
	  , TaskOutlineNumber
	  , TaskName
	  , TaskIsDelay
	  , TaskStartDate
	  , TaskFinishDate
	  , TaskEstStartDate
	  , TaskEstFinishDate
	  , TaskActStartDate
	  , TaskActFinishDate
	  , TaskStatus
	  , dbo.uf_ConversionStringToLocale(20202, TaskStatus) AS TaskStatusName
	  , TaskIsDelay
	  , TaskPlanRate
	  , TaskActRate
	  , UserId
	  , UserName
	  , TaskViewType
	  , AttrValue
FROM    dbo.uv_Project_HMX AS up
		INNER JOIN dbo.uv_MyWorkHandler_Locale AS utl ON (utl.ProjectID = up.ProjectID)
WHERE   up.projectStatus IN (1, 2)
		AND TaskIsSummary = 0
		AND UserId = @UserId
		AND (
			 utl.TaskStatus = 2
			 OR EXISTS ( SELECT *
						 FROM   DBO.ufn_date_ranges(GETDATE()) T
						 WHERE  period = 'Same Week'
								AND utl.TaskStartDate BETWEEN T.START_DATE AND T.END_DATE
								AND utl.TaskStatus = 1 )
			)
ORDER BY TaskActStartDate DESC";

			var pagingTable = PmsRepository.ExecutePagingDataTableBySqlString(sql, pageIndex, pageSize, parameters);

			Assert.IsNotNull(pagingTable);
			Assert.IsNotNull(pagingTable.Table);
			Assert.IsFalse(pagingTable.Table.HasErrors);

			Assert.AreEqual(pageIndex, pagingTable.PageIndex);
			Assert.AreEqual(pageSize, pagingTable.PageSize);

			Assert.IsTrue(pagingTable.TotalPageCount > 0);
			Assert.IsTrue(pagingTable.TotalItemCount > 0);

			Assert.IsTrue(pagingTable.Table.Rows.Count > 0);
			Assert.IsTrue(pagingTable.Table.Rows.Count <= pageSize);
		}
	}
}

*/

