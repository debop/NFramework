using System;
using NUnit.Framework;

namespace NSoft.NFramework.Data.Repositories {
    [TestFixture]
    public class PagingDataTableFixture : AdoFixtureBase {
        #region << logger >>

        private static readonly NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        private static readonly bool IsDebugEnabled = log.IsDebugEnabled;

        #endregion

        private static readonly object _syncLock = new object();

        private static IAdoRepository _ado;

        public static IAdoRepository Ado {
            get {
                if(_ado == null)
                    lock(_syncLock)
                        if(_ado == null) {
                            var ado = AdoRepositoryFactory.Instance.CreateRepository();
                            System.Threading.Thread.MemoryBarrier();
                            _ado = ado;
                        }
                return _ado;
            }
        }

        #region << 테스트용 SQL 문 >>

        /// <summary>
        /// 조회용 SQL 문장
        /// </summary>
        private static readonly string[] _selectSqls = new[]
                                                       {
                                                           @"SELECT * FROM Customers Where CustomerID=@CustomerID",
                                                           @"SET NOCOUNT OFF SELECT FROM_NAME, SELECT_NAME FROM CUSTOMER ORDER BY SELECT_NAME"
                                                           ,
                                                           @"SELECT * FROM Customers C WHERE EXISTS (SELECT * FROM Orders O WHERE O.CustomerID=C.CustomerID)"
                                                           ,
                                                           @"SELECT  O.OrderID AS OrderID
		  , O.EmployeeID AS EmployeeID
		  , YEAR(OrderDate) AS OrderYear
		  , OD.Quantity*OD.UnitPrice AS Price
	FROM    dbo.Orders AS O
			INNER JOIN dbo.[Order Details] AS OD ON (O.OrderID = OD.OrderID)"
                                                           ,
                                                           @"SELECT  *
FROM    (
		 SELECT City
			  , CompanyName
			  , ContactName
			  , 'Customers' AS Relationship
		 FROM   Customers
		 UNION
		 SELECT City
			  , CompanyName
			  , ContactName
			  , 'Suppliers'
		 FROM   Suppliers
		) AS Addr"
                                                           ,
                                                           @"SELECT Name, (SELECT CodeName FROM Code C Where C.CodeID=E.ClassID) AS ClassName from Employee E ORDER BY E.EmpID"
                                                           ,
                                                           @"SELECT (SELECT CodeName FROM Code C Where C.CodeID=E.ClassID) AS ClassName, (SELECT CodeName FROM Code C Where C.CodeID=E.ClassID) AS ClassName2 from Employee E ORDER BY E.EmpID"
                                                       };

        private static readonly string[] _exptectedCountSqls = new string[]
                                                               {
                                                                   @"SELECT COUNT(*) FROM Customers Where CustomerID=@CustomerID",
                                                                   @"SET NOCOUNT OFF SELECT COUNT(*) FROM CUSTOMER",
                                                                   @"SELECT COUNT(*) FROM Customers C WHERE EXISTS (SELECT * FROM Orders O WHERE O.CustomerID=C.CustomerID)"
                                                                   ,
                                                                   @"SELECT COUNT(*) FROM    dbo.Orders AS O
			INNER JOIN dbo.[Order Details] AS OD ON (O.OrderID = OD.OrderID)"
                                                                   ,
                                                                   @"SELECT COUNT(*) FROM    (
		 SELECT City
			  , CompanyName
			  , ContactName
			  , 'Customers' AS Relationship
		 FROM   Customers
		 UNION
		 SELECT City
			  , CompanyName
			  , ContactName
			  , 'Suppliers'
		 FROM   Suppliers
		) AS Addr"
                                                                   ,
                                                                   @"SELECT COUNT(*) from Employee E",
                                                                   @"SELECT COUNT(*) from Employee E"
                                                               };

        #endregion

        [Test]
        public void CanConvertSelectSqlToCountSql() {
            // NOTE : DISTINCT 가 있는 SELECT 문은 변환이 불가
            // NOTE : 마지막 ORDER BY 절은 Counting SQL 문에서는 제거된다.
            //
            for(int i = 0; i < _selectSqls.Length; i++) {
                Assert.AreEqual(_exptectedCountSqls[i], AdoTool.GetCountingSqlString(_selectSqls[i]));
            }
        }

        [TestCase("SELECT distinct CustomerID from Customers")]
        public void ShouldBeExceptionRaise(string selectSql) {
            Assert.Throws<NotSupportedException>(() => AdoTool.GetCountingSqlString(selectSql));
        }

        [Test]
        public void CanConvertToCountingSQL() {
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

            const string sql2 =
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

            var convertable = AdoTool.CanConvertableSqlStringToCounting(sql);
            Assert.IsTrue(convertable);

            var convertedSql = AdoTool.GetCountingSqlString(sql);

            if(IsDebugEnabled)
                log.Debug("Counting SQL: " + convertedSql);

            convertable = AdoTool.CanConvertableSqlStringToCounting(sql2);
            Assert.IsTrue(convertable);

            convertedSql = AdoTool.GetCountingSqlString(sql2);

            if(IsDebugEnabled)
                log.Debug("Counting SQL: " + convertedSql);
        }
    }
}