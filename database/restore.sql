USE master
GO
PRINT 'Restoring db'
 -------------------------------------------------
--> Restoring friends 
-------------------------------------------------
RESTORE DATABASE friends
FROM DISK =  N'/work/friends.bak'
WITH FILE = 1,
     MOVE N'friends'
     TO  N'/var/opt/mssql/data/friends.mdf',
     MOVE N'friends_log'
     TO  N'/var/opt/mssql/data/friends_log.ldf',
     NOUNLOAD,
     STATS = 5;
GO

-- -------------------------------------------------
-- --> Adding user AdventureUser 
-- -------------------------------------------------
-- USE master;
-- GO
-- CREATE LOGIN AdventureUser
-- WITH PASSWORD = N'Adventure.@2018',
--      DEFAULT_DATABASE = AdventureWorks2017
-- GO
-- -------------------------------------------------
-- --> Adding permissions to AdventureUser
-- -------------------------------------------------
-- USE AdventureWorks2017
-- GO
-- CREATE USER AdventureUser FOR LOGIN AdventureUser
-- GO
-- USE AdventureWorks2017
-- GO
-- ALTER ROLE db_owner ADD MEMBER AdventureUser
-- GO