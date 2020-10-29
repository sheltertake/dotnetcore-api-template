# Docker file SqlServer + Restore db

## Links

 - [Restore SQL Server backups using Docker](https://helibertoarias.com/blog/sql-server/restore-sql-server-backups-using-docker/)

## HowTo

 - docker build -t custom-mssql:latest .  
 - docker run -d -p 1433:1433 --name sql1 custom-mssql   

## YAML

```yaml

steps:
- bash: |
   docker build -t custom-mssql:latest .  
   docker run -d -p 1433:1433 --name sql1 custom-mssql   
   docker exec sql1 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "yourStrong1234!Password" -Q "USE friends; select * from friends"
  workingDirectory: database
  displayName: 'Bash Script'

```

## Dockerfile

```dockerfile
FROM mcr.microsoft.com/mssql/server:2017-latest

RUN mkdir /work
COPY /friends.tar.gz /work
COPY /restore.sql /work
COPY /restore.sh /work

WORKDIR /work
RUN tar -xzvf ./friends.tar.gz -C ./
RUN chmod 666 friends.bak
RUN chmod 755 restore.sh

EXPOSE 1433
RUN ./restore.sh restore.sql

```

## Restore sql script

```sql
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
```

## Restore bash script

```bash
echo "Setting Environment variables."
export ACCEPT_EULA=Y
export SA_PASSWORD=yourStrong1234!Password
echo "Environment variables set."
echo "Starting SqlServr"
/opt/mssql/bin/sqlservr &
sleep 60 | echo "Waiting for 60s to start Sql Server"
# echo "Setting RAM to 2GB usage."
# /opt/mssql/bin/mssql-conf set memory.memorylimitmb 2048
# echo "Restarting to apply the changes."
# systemctl restart mssql-server.service
echo "Restoring DB."
/opt/mssql-tools/bin/sqlcmd -S127.0.0.1 -U sa -P $SA_PASSWORD -i $1
echo "DB restored."
echo "Deleting backup files."
rm -rf /work/*.bak
```

## Issues found

This dockerfile doesn't work

```dockerfile

FROM mcr.microsoft.com/mssql/server:2017-latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD yourStrong1234!Password

WORKDIR /
COPY ./friends.tar.gz ./tmp
RUN tar -xzvf ./tmp/friends.tar.gz -C ./tmp/

EXPOSE 1433

RUN (/opt/mssql/bin/sqlservr --accept-eula & ) | grep -q "Service Broker manager has started" &&  /opt/mssql-tools/bin/sqlcmd -S127.0.0.1 -U SA -P "yourStrong1234!Password"  -Q "RESTORE DATABASE friends FROM DISK='/tmp/friends.bkp';"

```

 - [Restore database in docker container](https://stackoverflow.com/questions/53623048/restore-database-in-docker-container)
 - [Trouble restoring sql server DB on docker container](https://stackoverflow.com/questions/50914578/trouble-restoring-sql-server-db-on-docker-container)
 - [provider: TCP Provider, error: 0 - An existing connection was forcibly closed by the remote host.](https://github.com/microsoft/mssql-docker/issues/563)
 - [Backup SQL Server 2019 in a Docker Linux container fails](https://stackoverflow.com/questions/61028002/backup-sql-server-2019-in-a-docker-linux-container-fails)
 - [SQL Server on Ubuntu 18.04 Operating system error 2(The system cannot find the file specified.)](https://stackoverflow.com/questions/52145458/sql-server-on-ubuntu-18-04-operating-system-error-2the-system-cannot-find-the-f)
 - [Creating your own SQL Server docker image](https://www.sqlshack.com/creating-your-own-sql-server-docker-image/)
 - [A short recipe for setting up the Microsoft SQL Server docker image](https://augustl.com/blog/2019/sql_server_docker_image_recipe/)