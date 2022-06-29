--select * from Employees order by CreatedDate 

--delete from Employees where Role!='admin'    

DECLARE @RowCount INT
DECLARE @RowString VARCHAR(10)
DECLARE @Random INT
DECLARE @RandomYear INT
DECLARE @Upper INT
DECLARE @Lower INT
DECLARE @InsertDate DATETIME

SET @Lower = 730
SET @Upper = 3000
SET @RowCount = 0

WHILE @RowCount < 80
BEGIN
    SET @RowString = CAST(@RowCount AS VARCHAR(10))	
    SELECT @Random = ROUND(
	(RAND()*(@Upper-@Lower+1)+@Lower),
	0), 
	@RandomYear = ROUND(
	(RAND()*(65-18+1)+18),
	0)
    SET @InsertDate = DATEADD(yyyy, -@RandomYear, GETDATE())

    INSERT INTO Employees
        (Id
		,Name
		,Email
		,Password
		,Role
		,BirthDate
		,Salary
		,CreatedDate
		,LastModifiedDate)
    VALUES
        (NEWID()
		,'user'+@RowString
		,'user'+@RowString+'@fake.mail.com'		
		,'EmGFU2dJLfQibtERTovwJA==.sJd4w0gbSHnD4zPiq9cKP6xH8ZvfCfklv9U96HYGk4s='
		,1--user         
		,@InsertDate
		,@Random
		,@InsertDate
		,@InsertDate)		

    SET @RowCount = @RowCount + 1
END
