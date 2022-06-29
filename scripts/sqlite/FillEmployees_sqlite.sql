--uuid generation
--select lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6)))  

--select * from employees
--delete from employees where Name!='admin'

WITH RECURSIVE
  cnt(x) AS (
     SELECT 1
     UNION ALL
     SELECT x+1 FROM cnt
      LIMIT 80
  )
--SELECT x FROM cnt;
insert into employees    
        (Id
		,Name
		,Email
		,Password
		,Role
		,BirthDate
		,Salary
		,CreatedDate
		,LastModifiedDate)           
select 
		lower(hex(randomblob(4))) || '-' || lower(hex(randomblob(2))) || '-4' || substr(lower(hex(randomblob(2))),2) || '-' || substr('89ab',abs(random()) % 4 + 1, 1) || substr(lower(hex(randomblob(2))),2) || '-' || lower(hex(randomblob(6)))
		--lower(hex(randomblob(16)))
		,'user' || cnt.x
		,'user'|| cnt.x || '@fake.mail.com'		
		,'EmGFU2dJLfQibtERTovwJA==.sJd4w0gbSHnD4zPiq9cKP6xH8ZvfCfklv9U96HYGk4s='
		,1--user         
		,DATE()
		,abs(random()) % (3500 - 500) + 1
		,DATE()
		,DATE()
from cnt 
