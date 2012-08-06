USE [Northwind]
GO

IF OBJECT_ID('usp_pivot_sql') IS NOT NULL 
	DROP PROCEDURE dbo.usp_pivot_sql;
GO	

/****** 개체:  StoredProcedure [dbo].[usp_pivot_sql]    스크립트 날짜: 09/11/2009 17:49:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROC [dbo].[usp_pivot_sql]
  @query    AS NVARCHAR(MAX),
  @on_rows  AS NVARCHAR(MAX),
  @on_cols  AS NVARCHAR(MAX),
  @agg_func AS NVARCHAR(MAX) = N'MAX',
  @agg_col  AS NVARCHAR(MAX)
AS

DECLARE
  @sql     AS NVARCHAR(MAX),
  @cols    AS NVARCHAR(MAX),
  @newline AS NVARCHAR(2);

SET @newline = NCHAR(13) + NCHAR(10);

-- If input is a valid table or view
-- construct a SELECT statement against it
IF COALESCE(OBJECT_ID(@query, N'U'),
            OBJECT_ID(@query, N'V')) IS NOT NULL
  SET @query = N'SELECT * FROM ' + @query;

-- Make the query a derived table
SET @query = N'(' + @query + @newline + N'       ) AS Query';

-- Handle * input in @agg_col
IF @agg_col = N'*'
  SET @agg_col = N'1';

-- Construct column list
SET @sql =
  N'SET @result = '                                    + @newline +
  N'  STUFF('                                          + @newline +
  N'    (SELECT N'','' + '
           + N'QUOTENAME(pivot_col) AS [text()]'       + @newline +
  N'     FROM (SELECT DISTINCT('
           + @on_cols + N') AS pivot_col'              + @newline +
  N'           FROM' + @query + N') AS DistinctCols'   + @newline +
  N'     ORDER BY pivot_col'                           + @newline +
  N'     FOR XML PATH('''')),'                         + @newline +
  N'    1, 1, N'''');'

EXEC sp_executesql
  @stmt   = @sql,
  @params = N'@result AS NVARCHAR(MAX) OUTPUT',
  @result = @cols OUTPUT;

-- Create the PIVOT query
SET @sql =
  N'SELECT *'                                           + @newline +
  N'FROM'                                               + @newline +
  N'  ( SELECT '                                        + @newline +
  N'      ' + @on_rows + N','                           + @newline +
  N'      ' + @on_cols + N' AS pivot_col,'              + @newline +
  N'      ' + @agg_col + N' AS agg_col'                 + @newline +
  N'    FROM '                                          + @newline +
  N'      ' + @query                                    + @newline +
  N'  ) AS PivotInput'                                  + @newline +
  N'  PIVOT'                                            + @newline +
  N'    ( ' + @agg_func + N'(agg_col)'                  + @newline +
  N'      FOR pivot_col'                                + @newline +
  N'        IN(' + @cols + N')'                         + @newline +
  N'    ) AS PivotOutput;'

EXEC sp_executesql @sql;
