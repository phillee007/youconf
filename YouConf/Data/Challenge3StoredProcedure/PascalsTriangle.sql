--Original source code from http://pascaltriangle.ayestaran.co.uk/- modified by Phil Lee for Azure Developer Spot Challenge 3
-- ╔═════════════════════════════════════════════════════════════════════════════════════════════════════════════╗
-- ║      Author: ◄ Franz Ayestaran ►                                                                            ║
-- ║        Date: ◄ Created 17/06/11, Revision 19/06/11 ►                                                        ║
-- ║ Description: ◄ Generate Pascal's triangular array of binomial coefficients to a maximum of 67 Lines ►       ║
-- ║                                                                                                             ║
-- ║ Max Single SQL Big Int Value 9,223,372,036,854,775,807                                                      ║
-- ║                                                                                                             ║
-- ║ nine quintillion, two hundred twenty three quadrillion, three hundred seventy two trillion,                 ║
-- ║ thirty six billion, eight hundred fifty four million, seven hundred seventy six thousand and seven          ║
-- ║                                                                                                             ║
-- ║ Max Single SQL Pascal Triangle Generated Value 7,219,428,434,016,265,740 within a complete horizontal sum   ║
-- ║                                                                                                             ║
-- ║ seven quintillion, two hundred nineteen quadrillion, four hundred twenty eight trillion,                    ║
-- ║ four hundred thirty four billion, sixteen million, two hundred sixty five thousand, seven hundred and forty ║
-- ╚═════════════════════════════════════════════════════════════════════════════════════════════════════════════╝

--  								                     1
--                                                    1     1
--                                                 1     2     1
--                                              1     3     3     1
--                                           1     4     6     4     1
--                                        1     5     10    10    5     1
--                                     1     6     15    20    15    6     1
--                                  1     7     21    35    35    21    7     1
--                               1     8     28    56    70    56    28    8     1
--                            1     9     36    84    126   126   84    36    9     1
--                         1     10    45    120   210   252   210   120   45    10    1
--                      1     11    55    165   330   462   462   330   165   55    11    1
--                   1     12    66    220   495   792   924   792   495   220   66    12    1
--                1     13    78    286   715   1287  1716  1716  1287  715   286   78    13    1
--             1    14     91   364   1001  2002  3003  3432  3003  2002  1001   364   91    14    1

-- When all the odd numbers (numbers not divisible by 2) in Pascal's Triangle are filled in (1) and the rest 
-- (the evens) are left blank (0), the recursive Sierpinski Triangle fractal is revealed, showing yet another 
-- pattern in Pascal's Triangle. Other interesting patterns are formed if the elements not divisible by other 
-- numbers are filled, especially those indivisible by prime numbers. 

-- Toggle between, 'Actual data' and 'Sierpinski Pattern', by setting @SIERPINSKI_PATTERN to '0' or '1'  
-- Phil Lee - Remove Sierpinski pattern option

/****** Object:  StoredProcedure [dbo].[GetPascalTriangleNthRow]    Script Date: 05/23/2013 11:59:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPascalTriangleNthRow]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPascalTriangleNthRow]
GO

CREATE PROCEDURE dbo.GetPascalTriangleNthRow (
@RowNumber int
)
AS


CREATE TABLE [dbo].[#PASCAL_TRIANGLE](
	 [ROW] [int] IDENTITY(1,1) NOT NULL,
	 [HORIZONTAL_SUM] [varchar](max) NULL
) ON [PRIMARY]

--DBCC CHECKIDENT (#PASCAL_TRIANGLE, RESEED, 1)

DECLARE @HORIZ_POS AS INT
DECLARE @CurrentRowNum AS INT
DECLARE @HORIZONTAL_SUM_LENGTH AS INT
DECLARE @HORIZONTAL_SUM AS VARCHAR(MAX)
DECLARE @HORIZONTAL_NEXT_SUM AS VARCHAR(MAX)
DECLARE @HORIZONTAL_SIERPINSKI_SUM AS VARCHAR(MAX)
DECLARE @HORIZONTAL_EXTRACT_NUMBER AS VARCHAR(MAX)
DECLARE @HORIZONTAL_NUMBER AS BIGINT
DECLARE @HORIZONTAL_NUMBER1 AS BIGINT
DECLARE @HORIZONTAL_NUMBER2 AS BIGINT
DECLARE @LAST_NUMBER_LENGTH AS SMALLINT
DECLARE @LARGEST_GENERATED_NUMBER AS BIGINT
DECLARE @HORIZONTAL_NUMBER_COUNT AS INT
DECLARE @HORIZONTAL_CHAR AS VARCHAR(1)
DECLARE @ARITHMETIC_OVERFLOW AS BIT

INSERT INTO #PASCAL_TRIANGLE (HORIZONTAL_SUM) VALUES('1')
INSERT INTO #PASCAL_TRIANGLE (HORIZONTAL_SUM) VALUES('1 1')

SET @CurrentRowNum = 1
SET @HORIZ_POS = 0
SET @LAST_NUMBER_LENGTH = 0
SET @LARGEST_GENERATED_NUMBER = 0
SET @ARITHMETIC_OVERFLOW = 0

PRINT '1'

  PRINT '1 1'

WHILE @CurrentRowNum <= @RowNumber AND @ARITHMETIC_OVERFLOW = 0
  BEGIN
  Print @CurrentRowNum
    SELECT @HORIZONTAL_SUM = HORIZONTAL_SUM FROM #PASCAL_TRIANGLE WHERE ROW = @@IDENTITY 
    SET @HORIZONTAL_SUM_LENGTH = LEN(@HORIZONTAL_SUM)
    SET @HORIZ_POS = 1
    SET @HORIZONTAL_NEXT_SUM = ''
    SET @HORIZONTAL_NUMBER_COUNT = 0
    
    SET @HORIZONTAL_EXTRACT_NUMBER = ''

    WHILE @HORIZ_POS <= @HORIZONTAL_SUM_LENGTH + 1
      BEGIN
        
        SET @HORIZONTAL_CHAR = SUBSTRING(@HORIZONTAL_SUM,@HORIZ_POS,1)

        IF ISNUMERIC(@HORIZONTAL_CHAR) = 1 
          SET @HORIZONTAL_EXTRACT_NUMBER = @HORIZONTAL_EXTRACT_NUMBER +  @HORIZONTAL_CHAR
        
        IF ISNUMERIC(@HORIZONTAL_CHAR) = 0
          BEGIN
            SET @HORIZONTAL_NUMBER = CAST(@HORIZONTAL_EXTRACT_NUMBER AS BIGINT)
            SET @LAST_NUMBER_LENGTH = LEN(@HORIZONTAL_EXTRACT_NUMBER)
            SET @HORIZONTAL_EXTRACT_NUMBER = ''
            SET @HORIZONTAL_NUMBER_COUNT = @HORIZONTAL_NUMBER_COUNT + 1
          END   
          
        IF @HORIZONTAL_NUMBER_COUNT = 1 
          BEGIN
            SET @HORIZONTAL_NUMBER1 = @HORIZONTAL_NUMBER
          END
          
        IF @HORIZONTAL_NUMBER_COUNT = 2 
          BEGIN
            SET @HORIZONTAL_NUMBER2 = @HORIZONTAL_NUMBER
            
			BEGIN TRY
			  SET @HORIZONTAL_NEXT_SUM = @HORIZONTAL_NEXT_SUM 
                   + CAST(@HORIZONTAL_NUMBER1 + @HORIZONTAL_NUMBER2 AS VARCHAR(19))
              
              SET @HORIZONTAL_SIERPINSKI_SUM = @HORIZONTAL_SIERPINSKI_SUM 
                   + CAST((@HORIZONTAL_NUMBER1 + @HORIZONTAL_NUMBER2) % 2 AS VARCHAR(19))
                   
			  IF @LARGEST_GENERATED_NUMBER < @HORIZONTAL_NUMBER1 + @HORIZONTAL_NUMBER2 
              SET @LARGEST_GENERATED_NUMBER = @HORIZONTAL_NUMBER1 + @HORIZONTAL_NUMBER2

			  IF @HORIZ_POS < @HORIZONTAL_SUM_LENGTH 
			  SET @HORIZONTAL_NEXT_SUM = @HORIZONTAL_NEXT_SUM + ' '
	 
			  SET @HORIZONTAL_NUMBER_COUNT = 0
			  SET @HORIZ_POS = @HORIZ_POS - @LAST_NUMBER_LENGTH - 1        
            END TRY

            BEGIN CATCH
			  SET @ARITHMETIC_OVERFLOW = 1
		    END CATCH   
          END
        
        SET @HORIZ_POS = @HORIZ_POS + 1
      END
        
    SET @HORIZONTAL_NEXT_SUM = '1 ' + @HORIZONTAL_NEXT_SUM + ' 1'
    PRINT @HORIZONTAL_NEXT_SUM

	SET @CurrentRowNum = @CurrentRowNum + 1
    IF @ARITHMETIC_OVERFLOW = 0 INSERT INTO #PASCAL_TRIANGLE (HORIZONTAL_SUM) VALUES(@HORIZONTAL_NEXT_SUM)
  END

SELECT HORIZONTAL_SUM FROM #PASCAL_TRIANGLE WHERE [ROW] = @rowNumber

DROP TABLE #PASCAL_TRIANGLE

GO
