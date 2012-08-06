USE Northwind
go

IF OBJECT_ID('OrderByYear') IS NOT NULL 
    DROP VIEW dbo.OrderByYear
GO

CREATE VIEW dbo.OrderByYear
AS  SELECT  O.OrderID AS OrderID
          , O.EmployeeID AS EmployeeID
          , YEAR(OrderDate) AS OrderYear
          , OD.Quantity*OD.UnitPrice AS Price
    FROM    dbo.Orders AS O
            INNER JOIN dbo.[Order Details] AS OD ON (O.OrderID = OD.OrderID)
 
GO

IF OBJECT_ID('fn_OrderByYear') IS NOT NULL 
	DROP FUNCTION dbo.fn_OrderByYear
GO

CREATE FUNCTION dbo.fn_OrderByYear() RETURNS TABLE 
AS 
   RETURN
   SELECT  O.OrderID AS OrderID
          , O.EmployeeID AS EmployeeID
          , YEAR(OrderDate) AS OrderYear
          , OD.Quantity*OD.UnitPrice AS Price
    FROM    dbo.Orders AS O
            INNER JOIN dbo.[Order Details] AS OD ON (O.OrderID = OD.OrderID)	
       

GO
 
EXEC dbo.usp_pivot_sql @query=N'dbo.OrderByYear', @on_rows='EmployeeID', @on_cols='OrderYear', @agg_func='SUM', @agg_col='Price'
EXEC dbo.usp_pivot_sql @query='select * from dbo.fn_OrderByYear()', @on_rows='EmployeeID', @on_cols='OrderYear', @agg_func=N'SUM', @agg_col='Price'

