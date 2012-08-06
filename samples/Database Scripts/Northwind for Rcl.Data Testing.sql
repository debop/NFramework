use Northwind
go

if object_id('SaveOrUpdateCategory') <> 0
	drop proc [dbo].[SaveOrUpdateCategory]
GO	
		 
CREATE  PROCEDURE dbo.SaveOrUpdateCategory
	 @CATEGORY_ID int = -1
	,@CATEGORY_NAME nvarchar(15)
	,@DESCRIPTION ntext

AS
SET NOCOUNT ON

	if @CATEGORY_ID <= 0
	begin
		insert into Categories (CategoryName, Description)
		values(@CATEGORY_NAME, @DESCRIPTION)
		
		return scope_identity()
	end
	else begin
		update Categories
		   set  CategoryName = @CATEGORY_NAME
			   ,Description = @DESCRIPTION
		 where CategoryId = @CATEGORY_ID
		 
		return @CATEGORY_ID
	end

GO

if object_id('CustOrderHist2') <> 0
	drop proc [dbo].[CustOrderHist2] 
GO

CREATE PROCEDURE [dbo].[CustOrderHist2] 
@CustomerId nchar(5)
AS
	-- exec CustOrderHist2 'ANATR'
SELECT ProductName as PRODUCT_NAME, TOTAL=SUM(Quantity)
FROM Products P, [Order Details] OD, Orders O, Customers C
WHERE C.CustomerID = @CustomerId
AND C.CustomerID = O.CustomerID AND O.OrderID = OD.OrderID AND OD.ProductID = P.ProductID
GROUP BY ProductName

GO

if object_id('GetProductsByCategoryId') <> 0
	drop proc GetProductsByCategoryId
go

CREATE PROCEDURE GetProductsByCategoryId
@CategoryId int = 1
AS
BEGIN
		-- exec dbo.GetProductsByCategoryId 1

select  P.ProductId 
	   ,P.ProductName 
	   ,P.SupplierId
	   ,P.CategoryId
	   ,P.QuantityPerUnit
	   ,P.UnitPrice
	   ,P.UnitsInStock
	   ,P.UnitsOnOrder
	   ,P.ReorderLevel
	   ,P.Discontinued
	   ,C.CategoryName 
	   ,C.Description
  from Products P inner join Categories C on P.CategoryID = C.CategoryID
 where P.CategoryID = @CategoryID 
END

GO

IF OBJECT_ID('OrderAndOrderDetails') IS NOT NULL
	DROP PROCEDURE OrderAndOrderDetails
GO

CREATE PROCEDURE OrderAndOrderDetails
AS

--
-- Multi-ResultSet을 반환하는 것을 DataSet이나 DataTable 컬렉션으로 받을 수 있는지 테스트하기 위한 Procedure입니다.
--
-- EXEC OrderAndOrderDetails 

SELECT * FROM dbo.Orders;

SELECT * FROM dbo.[Order Details];


RETURN 


IF object_id('OrderByYear') IS NOT NULL
	DROP VIEW OrderByYear
go

CREATE VIEW dbo.OrderByYear
AS  SELECT  O.OrderID AS OrderID
		  , O.EmployeeID AS EmployeeID
		  , YEAR(OrderDate) AS OrderYear
		  , OD.Quantity*OD.UnitPrice AS Price
	FROM    dbo.Orders AS O
			INNER JOIN dbo.[Order Details] AS OD ON (O.OrderID = OD.OrderID)
 
GO